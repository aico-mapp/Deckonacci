using System;
using System.Collections.Generic;
using System.Linq;
using Calendar.Scripts.Data.Enums;
using Calendar.Scripts.Game.UI.Base;
using Calendar.Scripts.Services.EntityContainer;
using Cysharp.Threading.Tasks;
using Game.Calendar.Scripts.Data;
using Game.Calendar.Scripts.Data.StaticData;
using Game.Calendar.Scripts.Game.Roulette;
using Game.Calendar.Scripts.Game.Table;
using Game.Calendar.Scripts.Services.Sound;
using Game.Scripts;
using Game.Scripts.Animations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace Game.Calendar.Scripts.Game.UI.RouletteScreen
{
    public class RouletteView : BaseView, IFactoryEntity
    {
        public UnityEvent OnBackClick => _backButton.onClick;

        public event Action OnRouletteResulted;
        public event Action<RouletteReward, int> OnPayoutCalculated;
        public UnityEvent OnGetClick => _getButton.onClick;

        [Header("Buttons")] 
        [SerializeField] private Button _getButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private SoundToggleButton _soundToggleButton;

        [Header("UI")]
        [SerializeField] private Image _getImage;
        [SerializeField] private Sprite _payoutSprite;
        [SerializeField] private Sprite _noPayoutSprite;
        
        [Header("Roulette")]
        [SerializeField] private RouletteController _rouletteController;
        
        [Header("Current Reward Display")]
        [SerializeField] private TextMeshProUGUI _currentRewardText;
        [SerializeField] private Image _currentRewardIcon;
        [SerializeField] private GameObject _currentRewardPanel;
        
        [Header("Popup Animation")]
        [SerializeField] private PopupAnimation _resultPopup;
        [SerializeField] private RouletteResultInfo _resultInfo;
        
        [Space(20)]
        [SerializeField] private List<BetData> _bets = new List<BetData>();
        
        private ISoundService _soundService;

        public void Construct(ISoundService soundService)
        {
            _soundService = soundService;
            _soundToggleButton.Construct(soundService);
        }
        
        public void SubscribeView()
        {
            if (_rouletteController != null)
            {
                _rouletteController.OnRewardChanged += OnRewardChanged;
                _rouletteController.OnSpinStart += OnSpinStarted;
                _rouletteController.OnSpinComplete += OnSpinCompleted;
            }
            
            OnGetClick.AddListener(AddResult);
        }

        public void UnsubscribeView()
        {
            if (_rouletteController != null)
            {
                _rouletteController.OnRewardChanged -= OnRewardChanged;
                _rouletteController.OnSpinStart -= OnSpinStarted;
                _rouletteController.OnSpinComplete -= OnSpinCompleted;
            }
            
            OnGetClick.RemoveAllListeners();
        }

        public void SetBets(List<BetData> bets)
        {
            _bets = new List<BetData>(bets);
        }
        
        public void TrySpin()
        {
            if (!_rouletteController.IsSpinning)
            {
                _rouletteController.StartSpin();
            }
        }
        
        private void OnSpinStarted()
        {
            _backButton.interactable = false;
            
            if (_currentRewardPanel != null)
                _currentRewardPanel.SetActive(true);
        }
        
        private void OnRewardChanged(RouletteReward reward)
        {
            if (reward == null) return;
            
            bool isImageReward = reward.RewardAmount == -1;
            
            if (isImageReward)
            {
                _currentRewardText.gameObject.SetActive(false);
                _currentRewardIcon.gameObject.SetActive(true);
                
                if (_currentRewardIcon != null && reward.RewardIcon != null)
                {
                    _currentRewardIcon.sprite = reward.RewardIcon;
                }
            }
            else
            {
                _currentRewardText.gameObject.SetActive(true);
                _currentRewardIcon.gameObject.SetActive(false);
                    
                if (_currentRewardText != null)
                {
                    _currentRewardText.text = reward.RewardAmount.ToString();

                    switch (reward.RewardColor)
                    {
                        case CategoryColor.White:
                            _currentRewardText.color = Color.white;
                            break;
                        case CategoryColor.Orange: 
                            ColorUtility.TryParseHtmlString("#FF9019", out var color);
                            _currentRewardText.color = color;
                            break;
                    }
                }
            }
            
            _soundService?.PlayEffectSound(SoundId.Roulette);
        }
        
        private void OnSpinCompleted(RouletteReward reward)
        {
            UniTask.Delay(500);
            _backButton.interactable = true;
            
            string rewardText = reward.RewardAmount == -1 
                ? reward.RewardDescription 
                : $"{reward.RewardAmount}";

            var payoutAmount = GetPayoutFromBet(reward);
            ShowResult(reward, payoutAmount);
            
            Debug.Log($"Won reward: {reward.Number} - {rewardText}");
        }

        private int GetPayoutFromBet(RouletteReward reward)
        {
            int totalPayout = 0;
            
            foreach (var betData in _bets)
            {
                if (reward.RewardType == betData.Type)
                {
                    int payout = CalculateExactMatchPayout(betData, reward);
                    if (payout > 0)
                    {
                        totalPayout += payout;
                    }
                }
            }
            
            // Handle Suit color matching - only if reward is NOT a Suit itself
            if (reward.RewardType != CategoryType.Suit)
            {
                totalPayout += CalculateSuitColorMatchPayout(reward);
            }
            
            return totalPayout;
        }

        private int CalculateExactMatchPayout(BetData betData, RouletteReward reward)
        {
            bool isWinningBet = false;
            string betType = "";
            
            switch (betData.Type)
            {
                case CategoryType.Value:
                    isWinningBet = reward.RewardAmount == betData.TableValue && reward.RewardColor == betData.Color;
                    betType = $"Value {betData.TableValue}";
                    break;
                    
                case CategoryType.Joker:
                    isWinningBet = reward.RewardColor == betData.Color;
                    betType = $"Joker {betData.Color}";
                    break;
                    
                case CategoryType.Fibonacci:
                    isWinningBet = true;
                    betType = "Fibonacci";
                    break;
                    
                case CategoryType.Suit:
                    isWinningBet = reward.RewardSuit == betData.Suit;
                    betType = $"Suit {betData.Suit} (Exact Match)";
                    break;
            }
            
            if (isWinningBet)
            {
                int payout = (betData.ChipValue * betData.BetCount) * reward.Odds;
                Debug.Log($"{betType} payout: {payout} (Odds: {reward.Odds})");
                return payout;
            }
            
            return 0;
        }

        private int CalculateSuitColorMatchPayout(RouletteReward reward)
        {
            // Group suit bets by color
            var suitBetsByColor = new Dictionary<CategoryColor, (int totalBetAmount, int maxOdds)>();
            
            foreach (var betData in _bets)
            {
                if (betData.Type == CategoryType.Suit && betData.Color == reward.RewardColor)
                {
                    int suitOdds = GetSuitOdds(betData.Suit);
                    int betAmount = betData.ChipValue * betData.BetCount;
                    
                    if (!suitBetsByColor.ContainsKey(betData.Color))
                    {
                        suitBetsByColor[betData.Color] = (0, 0);
                    }
                    
                    var current = suitBetsByColor[betData.Color];
                    suitBetsByColor[betData.Color] = (
                        current.totalBetAmount + betAmount,
                        Mathf.Max(current.maxOdds, suitOdds)
                    );
                }
            }
            
            int totalPayout = 0;
            foreach (var colorGroup in suitBetsByColor)
            {
                var color = colorGroup.Key;
                var (totalBetAmount, maxOdds) = colorGroup.Value;
                
                int payout = totalBetAmount * maxOdds;
                totalPayout += payout;
                Debug.Log($"Suit color match ({color}) payout: {payout} (Total bet: {totalBetAmount}, Odds: {maxOdds})");
            }
            
            return totalPayout;
        }

        private int GetSuitOdds(SuitType suit)
        {
            foreach (var reward in _rouletteController.GetRewards())
            {
                if (reward.RewardType == CategoryType.Suit && reward.RewardSuit == suit)
                {
                    return reward.Odds;
                }
            }
            
            Debug.LogWarning($"Could not find odds for suit {suit}, using default odds of 3");
            return 3;
        }
        
        public void UpdateSound(bool isMuted)
        {
            _soundToggleButton.UpdateImage(isMuted);
        }

        private void AddResult()
        {
            OnRouletteResulted?.Invoke();
            HideResult();
        }
        
        private void ShowResult(RouletteReward reward, int payout)
        {
            OnPayoutCalculated?.Invoke(reward, payout);
            
            _resultInfo.UpdateInfo(reward, payout);
            _resultPopup.Show();
            
            _soundService.PlayEffectSound(payout != 0 ? SoundId.PopupWin : SoundId.PopupLose);

            _getImage.sprite = payout != 0 ? _payoutSprite : _noPayoutSprite;
        }

        public void HideResult()
        {
            _resultPopup.Hide();
        }
    }
}