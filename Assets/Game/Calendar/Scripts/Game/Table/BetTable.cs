// BetTable.cs - Add initialization for TableFields
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using Calendar.Scripts.Data.Enums;
using Game.Calendar.Scripts.Data;
using Game.Calendar.Scripts.Game.Chip;
using Game.Calendar.Scripts.Services.Sound;

namespace Game.Calendar.Scripts.Game.Table
{
    public class BetTable : MonoBehaviour
    {
        public event Action<List<BetData>> OnBetsChanged;
        
        [SerializeField] private List<TableField> _tableFields = new List<TableField>();
        
        private List<BetData> _currentBets = new List<BetData>();
        private int _totalBetAmount = 0;

        private ISoundService _soundService;
        private ChipsController _chipsController;


        public void Initialize(ISoundService soundService, ChipsController chipsController)
        {
            _soundService = soundService;
            _chipsController = chipsController;
            
            // Initialize all table fields with reference to ChipsController
            foreach (var tableField in _tableFields)
            {
                tableField.Initialize(_chipsController);
            }
        }
        
        public void SubscribeTable()
        {
            foreach (var tableField in _tableFields)
            {
                tableField.OnChipPlaced += HandleChipPlaced;
            }
        }

        public void UnsubscribeTable()
        {
            foreach (var tableField in _tableFields)
            {
                tableField.OnChipPlaced -= HandleChipPlaced;
            }
        }

        private void HandleChipPlaced(TableCategory category, int chipValue)
        {
            BetData existingBet = _currentBets.Find(bet => bet.MatchesCategory(category, chipValue));

            if (existingBet != null)
            {
                existingBet.IncrementBet();
            }
            else
            {
                BetData newBet = new BetData(
                    category.Type,
                    category.Color,
                    category.Suit,
                    category.TableValue,
                    chipValue
                );
                _currentBets.Add(newBet);
            }

            _totalBetAmount += chipValue;
            _chipsController.DeselectChip();
            _soundService.PlayEffectSound(SoundId.Chip);
            
            OnBetsChanged?.Invoke(_currentBets);
        }

        public List<BetData> GetCurrentBets()
        {
            return new List<BetData>(_currentBets);
        }

        public int GetTotalBetAmount()
        {
            return _totalBetAmount;
        }

        public void ClearBets()
        {
            _currentBets.Clear();
            _totalBetAmount = 0;
            
            foreach (var field in _tableFields)
            {
                field.ClearChips();
            }
            
            OnBetsChanged?.Invoke(_currentBets);
        }
        
        public void ClearBetsWithAnimation()
        {
            StartCoroutine(ClearBetsCoroutine());
        }

        private IEnumerator ClearBetsCoroutine()
        {
            List<ChipObject> allChips = new List<ChipObject>();
            
            foreach (var field in _tableFields)
            {
                var chips = field.GetChipObjects(); 
                if (chips != null)
                {
                    allChips.AddRange(chips);
                }
            }
            
            if (allChips.Count > 0)
            {
                yield return StartCoroutine(AnimateChipsOut(allChips));
            }
            
            _currentBets.Clear();
            _totalBetAmount = 0;
            
            foreach (var field in _tableFields)
            {
                field.ClearChips();
            }
            
            OnBetsChanged?.Invoke(_currentBets);
        }

        private IEnumerator AnimateChipsOut(List<ChipObject> chips)
        {
            float duration = 0.7f;
            float elapsed = 0f;
    
            Vector3 dealerPosition = new Vector3(0, 6f, 0f); 
            Dictionary<ChipObject, (Vector3 startPos, float delay, bool soundPlayed)> chipData = new Dictionary<ChipObject, (Vector3, float, bool)>();
    
            float maxDelay = 0.3f;
            int chipIndex = 0;
    
            foreach (var chip in chips)
            {
                if (chip != null)
                {
                    float delay = (chipIndex / (float)chips.Count) * maxDelay;
                    chipData[chip] = (chip.transform.position, delay, false);
                    chipIndex++;
                }
            }
    
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
        
                foreach (var chip in chips)
                {
                    if (chip != null && chipData.TryGetValue(chip, out var value))
                    {
                        var (startPos, delay, soundPlayed) = value;
                
                        float chipElapsed = elapsed - delay;
                        if (chipElapsed > 0)
                        {
                            if (!soundPlayed)
                            {
                                _soundService.PlayEffectSound(SoundId.BetsClear);
                                chipData[chip] = (startPos, delay, true);
                            }
                    
                            float t = chipElapsed / (duration - maxDelay);
                            t = Mathf.Clamp01(t);
                            float easedT = Mathf.SmoothStep(0f, 1f, t);
                            chip.transform.position = Vector3.Lerp(startPos, dealerPosition, easedT);
                        }
                    }
                }
        
                yield return null;
            }
        }
    }
}