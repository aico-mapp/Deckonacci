using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Calendar.Scripts.Data.Enums;
using DG.Tweening;
using Game.Calendar.Scripts.Services.Sound;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Calendar.Scripts.Game.Fibonacci
{
    public class FibonacciGameController : MonoBehaviour
    {
        [Header("Card Settings")]
        [SerializeField] private CardVisual _cardPrefab;
        
        [Header("Player Positions")]
        [SerializeField] private Transform _playerCardContainer;
        [SerializeField] private Transform _bot1CardContainer;
        [SerializeField] private Transform _bot2CardContainer;
        [SerializeField] private Transform _bot3CardContainer;
        
        [Header("Play Field")]
        [SerializeField] private Transform _playerPlayField;
        [SerializeField] private Transform _bot1PlayField;
        [SerializeField] private Transform _bot2PlayField;
        [SerializeField] private Transform _bot3PlayField;
        
        [Header("Deck Position")]
        [SerializeField] private Transform _deckPosition;
        
        [Header("Card Spacing")]
        [SerializeField] private float _playerCardSpacing = 200f;
        
        private Vector2[] _botCardPositions = new Vector2[]
        {
            new Vector2(-69f, 9f),
            new Vector2(-27f, 9f),
            new Vector2(13f, 18f),
            new Vector2(57f, 17f),
            new Vector2(97f, 5f)
        };
        
        private float[] _cardRotations = new float[] { 45f, 30f, 15f, 0f, -15f };

        private List<int> _fibonacciDeck = new List<int> 
            { 0, 0, 1, 1, 1, 1, 2, 2, 3, 3, 5, 5, 8, 8, 13, 13, 21, 21, 34, 34 };
        
        private FibonacciPlayer _player;
        private FibonacciPlayer _bot1;
        private FibonacciPlayer _bot2;
        private FibonacciPlayer _bot3;
        
        private List<CardVisual> _playerHandVisuals = new List<CardVisual>();
        private List<CardVisual> _bot1HandVisuals = new List<CardVisual>();
        private List<CardVisual> _bot2HandVisuals = new List<CardVisual>();
        private List<CardVisual> _bot3HandVisuals = new List<CardVisual>();
        
        private CardVisual _selectedPlayerCard;
        private List<CardVisual> _currentRoundCards = new List<CardVisual>();
        
        private int _currentRound = 0;
        private int _playerScore = 0;
        private int _bot1Score = 0;
        private int _bot2Score = 0;
        private int _bot3Score = 0;
        
        private event Action<int> OnGameEnd;
        private event Action<int, int, int, int> OnScoreUpdate;

        private ISoundService _soundService;
        

        public void Initialize(ISoundService soundService, Action<int> onGameEnd, Action<int, int, int, int> onScoreUpdate = null)
        {
            _soundService = soundService;
            OnGameEnd = onGameEnd;
            OnScoreUpdate = onScoreUpdate;
            
            _player = new FibonacciPlayer(_playerCardContainer, true);
            _bot1 = new FibonacciPlayer(_bot1CardContainer, false);
            _bot2 = new FibonacciPlayer(_bot2CardContainer, false);
            _bot3 = new FibonacciPlayer(_bot3CardContainer, false);
        }

        public void StartGame()
        {
            _currentRound = 0;
            _playerScore = 0;
            _bot1Score = 0;
            _bot2Score = 0;
            _bot3Score = 0;
            
            OnScoreUpdate?.Invoke(_playerScore, _bot1Score, _bot2Score, _bot3Score);
            
            DealCards();
        }

        private void DealCards()
        {
            List<int> shuffledDeck = _fibonacciDeck.OrderBy(x => Random.value).ToList();
            
            ClearAllCards();
            
            List<FibonacciPlayer> players = new List<FibonacciPlayer> { _player, _bot1, _bot2, _bot3 };
            List<List<CardVisual>> handVisuals = new List<List<CardVisual>> 
                { _playerHandVisuals, _bot1HandVisuals, _bot2HandVisuals, _bot3HandVisuals };
            
            int cardIndex = 0;
            float dealDelay = 0f;
            
            for (int i = 0; i < 5; i++)
            {
                for (int playerIndex = 0; playerIndex < players.Count; playerIndex++)
                {
                    var player = players[playerIndex];
                    var visuals = handVisuals[playerIndex];
                    
                    int cardValue = shuffledDeck[cardIndex++];
                    FibonacciCard card = new FibonacciCard(cardValue);
                    player.AddCard(card);
                    
                    CardVisual cardVisual = Instantiate(_cardPrefab, player.CardContainer);
                    RectTransform cardRect = cardVisual.GetComponent<RectTransform>();
                    if (cardRect != null)
                    {
                        cardRect.anchoredPosition = Vector2.zero;
                    }
                    
                    System.Action<CardVisual> onSelect = player.IsHumanPlayer ? OnPlayerCardSelected : null;
                    cardVisual.Initialize(card, onSelect, _soundService);
                    
                    visuals.Add(cardVisual);
                    
                    int cardInHandIndex = player.Hand.Count - 1;
                    Vector2 targetLocalPos = GetCardLocalPositionInHand(player, cardInHandIndex);
                    float targetRotation = GetCardRotation(player, cardInHandIndex);
                    
                    cardVisual.AnimateDeal(targetLocalPos, targetRotation, dealDelay, 0.3f);
                    
                    dealDelay += 0.1f;
                }
            }
            
            StartCoroutine(ShowPlayerCardsAfterDealing(dealDelay + 0.5f));
        }

        private void ClearAllCards()
        {
            ClearCardList(_playerHandVisuals);
            ClearCardList(_bot1HandVisuals);
            ClearCardList(_bot2HandVisuals);
            ClearCardList(_bot3HandVisuals);
            ClearCardList(_currentRoundCards);
        }

        private void ClearCardList(List<CardVisual> cards)
        {
            foreach (var card in cards)
            {
                if (card != null)
                    Destroy(card.gameObject);
            }
            cards.Clear();
        }

        private IEnumerator ShowPlayerCardsAfterDealing(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            foreach (var cardVisual in _playerHandVisuals)
            {
                if (cardVisual != null)
                {
                    cardVisual.ShowFront();
                    cardVisual.SetInteractable(true);
                }
            }
        }

        private Vector2 GetCardLocalPositionInHand(FibonacciPlayer player, int cardIndex)
        {
            if (player.IsHumanPlayer)
            {
                float totalWidth = 4 * _playerCardSpacing; 
                float startX = -totalWidth / 2f;
                return new Vector2(startX + (cardIndex * _playerCardSpacing), 0);
            }
            else
            {
                if (cardIndex >= 0 && cardIndex < _botCardPositions.Length)
                {
                    return _botCardPositions[cardIndex];
                }
                return Vector2.zero;
            }
        }

        private float GetCardRotation(FibonacciPlayer player, int cardIndex)
        {
            if (player.IsHumanPlayer)
                return 0f;
            
            if (cardIndex >= 0 && cardIndex < _cardRotations.Length)
                return _cardRotations[cardIndex];
            
            return 0f;
        }

        private void OnPlayerCardSelected(CardVisual cardVisual)
        {
            if (_selectedPlayerCard != null || _currentRound >= 5) return;
            
            _selectedPlayerCard = cardVisual;
            
            foreach (var card in _playerHandVisuals)
            {
                if (card != null)
                    card.SetInteractable(false);
            }
            
            _playerHandVisuals.Remove(cardVisual);
            _currentRoundCards.Add(cardVisual);
            
            cardVisual.transform.SetParent(transform);
            cardVisual.MoveToWorldPosition(_playerPlayField.position, 0f, 0.3f, OnPlayerCardPlayed);
            
            _soundService.PlayEffectSound(SoundId.FibonacciCard);
        }

        private void OnPlayerCardPlayed()
        {
            StartCoroutine(PlayBotCards());
        }

        private IEnumerator PlayBotCards()
        {
            yield return new WaitForSeconds(0.3f);
            
            List<FibonacciPlayer> bots = new List<FibonacciPlayer> { _bot1, _bot2, _bot3 };
            List<List<CardVisual>> botHands = new List<List<CardVisual>> 
                { _bot1HandVisuals, _bot2HandVisuals, _bot3HandVisuals };
            List<CardVisual> botPlayedCards = new List<CardVisual>();
            
            for (int i = 0; i < bots.Count; i++)
            {
                var bot = bots[i];
                var botHand = botHands[i];
                
                if (botHand.Count > 0)
                {
                    int randomIndex = Random.Range(0, botHand.Count);
                    CardVisual cardVisual = botHand[randomIndex];
                    
                    botHand.RemoveAt(randomIndex);
                    _currentRoundCards.Add(cardVisual);
                    botPlayedCards.Add(cardVisual);
                    
                    Transform playField = GetPlayFieldForBot(i);
                    
                    cardVisual.transform.SetParent(transform);
                    cardVisual.MoveToWorldPosition(playField.position, 0f, 0.3f);
                    
                    _soundService.PlayEffectSound(SoundId.FibonacciCard);
                    
                    yield return new WaitForSeconds(0.2f);
                }
            }
            
            yield return new WaitForSeconds(0.5f);
            
            foreach (var card in botPlayedCards)
            {
                card.ShowFront();
                yield return new WaitForSeconds(0.1f);
            }
            
            yield return new WaitForSeconds(0.5f);
            
            EvaluateRound(botPlayedCards);
        }

        private void EvaluateRound(List<CardVisual> botCards)
        {
            int playerValue = _selectedPlayerCard.Value;
            int bot1Value = botCards[0].Value;
            int bot2Value = botCards[1].Value;
            int bot3Value = botCards[2].Value;
            
            List<int> allValues = new List<int> { bot1Value, bot2Value, bot3Value };
            
            bool playerWon = CanFormSum(playerValue, allValues);
            
            if (playerWon)
            {
                _playerScore++;
                
                _selectedPlayerCard.HighlightWin();
                _soundService.PlayEffectSound(SoundId.PopupWin);
            }
            else
            {
                _selectedPlayerCard.HighlightLose();
                _soundService.PlayEffectSound(SoundId.PopupLose);
            }
            
            List<int> valuesForBot1 = new List<int> { playerValue, bot2Value, bot3Value };
            bool bot1Won = CanFormSum(bot1Value, valuesForBot1);
            if (bot1Won)
            {
                _bot1Score++;
                botCards[0].HighlightWin();
                Debug.Log($"Bot1 WON!");
            }
            else
            {
                botCards[0].HighlightLose();
            }
            
            List<int> valuesForBot2 = new List<int> { playerValue, bot1Value, bot3Value };
            bool bot2Won = CanFormSum(bot2Value, valuesForBot2);
            if (bot2Won)
            {
                _bot2Score++;
                botCards[1].HighlightWin();
                Debug.Log($"Bot2 WON!");
            }
            else
            {
                botCards[1].HighlightLose();
            }
            
            List<int> valuesForBot3 = new List<int> { playerValue, bot1Value, bot2Value };
            bool bot3Won = CanFormSum(bot3Value, valuesForBot3);
            if (bot3Won)
            {
                _bot3Score++;
                botCards[2].HighlightWin();
                Debug.Log($"Bot3 WON!");
            }
            else
            {
                botCards[2].HighlightLose();
            }
            
            OnScoreUpdate?.Invoke(_playerScore, _bot1Score, _bot2Score, _bot3Score);
            
            StartCoroutine(PrepareNextRound());
        }

        private bool CanFormSum(int target, List<int> values)
        {
            for (int i = 0; i < values.Count; i++)
            {
                for (int j = i + 1; j < values.Count; j++)
                {
                    if (values[i] + values[j] == target)
                    {
                        Debug.Log($"Found combination: {values[i]} + {values[j]} = {target}");
                        return true;
                    }
                }
            }
            return false;
        }

        private IEnumerator PrepareNextRound()
        {
            yield return new WaitForSeconds(1.5f);
            
            foreach (var card in _currentRoundCards)
            {
                if (card != null)
                {
                    card.transform.DOScale(0, 0.3f).OnComplete(() => Destroy(card.gameObject));
                }
            }
            _currentRoundCards.Clear();
            
            yield return new WaitForSeconds(0.5f);
            
            _currentRound++;
            _selectedPlayerCard = null;
            
            if (_currentRound >= 5)
            {
                EndGame();
            }
            else
            {
                ReorganizeHands();
                
                yield return new WaitForSeconds(0.3f);
                
                foreach (var card in _playerHandVisuals)
                {
                    if (card != null)
                    {
                        card.SetInteractable(true);
                    }
                }
            }
        }

        private void ReorganizeHands()
        {
            ReorganizePlayerHand(_player, _playerHandVisuals);
            ReorganizePlayerHand(_bot1, _bot1HandVisuals);
            ReorganizePlayerHand(_bot2, _bot2HandVisuals);
            ReorganizePlayerHand(_bot3, _bot3HandVisuals);
        }

        private void ReorganizePlayerHand(FibonacciPlayer player, List<CardVisual> handVisuals)
        {
            for (int i = 0; i < handVisuals.Count; i++)
            {
                if (handVisuals[i] != null)
                {
                    Vector2 targetLocalPos = GetCardLocalPositionInHand(player, i);
                    float targetRotation = GetCardRotation(player, i);
                    handVisuals[i].AnimateToLocalPosition(targetLocalPos, targetRotation, 0.3f);
                }
            }
        }

        private void EndGame()
        {
            Debug.Log($"Game Over! Final Scores - Player: {_playerScore}, Bot1: {_bot1Score}, Bot2: {_bot2Score}, Bot3: {_bot3Score}");
            OnGameEnd?.Invoke(_playerScore);
        }

        private Transform GetPlayFieldForBot(int botIndex)
        {
            switch (botIndex)
            {
                case 0: return _bot1PlayField;
                case 1: return _bot2PlayField;
                case 2: return _bot3PlayField;
                default: return null;
            }
        }

        private void OnDestroy()
        {
            ClearAllCards();
        }
    }
}