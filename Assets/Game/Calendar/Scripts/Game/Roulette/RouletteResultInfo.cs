using System;
using Game.Calendar.Scripts.Data.StaticData;
using Game.Calendar.Scripts.Game.Table;
using TMPro;
using UnityEngine;

namespace Game.Calendar.Scripts.Game.Roulette
{
    public class RouletteResultInfo: MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _resultText;
        [SerializeField] private TextMeshProUGUI _addedText;

        public void UpdateInfo(RouletteReward reward, int payout)
        {
            _resultText.text = reward.RewardType switch
            {
                CategoryType.Value => $"Number {reward.RewardAmount}",
                CategoryType.Joker => $"{reward.RewardColor} Joker",
                CategoryType.Fibonacci => "Fibonacci",
                CategoryType.Suit => $"{reward.RewardSuit} Suit",
                _ => throw new ArgumentOutOfRangeException()
            };

            _addedText.text = $"+{payout}";

            switch (reward.RewardColor)
            {
                case CategoryColor.White:
                    _resultText.color = Color.white;
                    break;
                case CategoryColor.Orange:
                    ColorUtility.TryParseHtmlString("#FF9019", out var color);
                    _resultText.color = color;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}