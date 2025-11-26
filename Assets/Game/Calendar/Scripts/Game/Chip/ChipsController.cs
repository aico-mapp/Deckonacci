// ChipsController.cs
using System;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Animations;

namespace Game.Calendar.Scripts.Game.Chip
{
    public class ChipsController : MonoBehaviour
    {
        [SerializeField] private int _betLimit = 200;
        
        [Header("References")]
        [SerializeField] private RectTransform _spawnArea;
        [SerializeField] private ChipObject _chipPrefab;
        [SerializeField] private BetLimitAnimation _betLimitAnimation;

        [Header("Chips Setup")]
        [SerializeField] private int[] _chipValues = { 1, 5, 10, 25, 100 };
        [SerializeField] private Vector3 _chipNormalSize;
        [SerializeField] private Vector3 _chipSelectedSize;
        [SerializeField] private Color _chipNormalColor;
        [SerializeField] private Color _chipSelectedColor;

        [SerializeField] private List<RectTransform> _slots;
        private Camera _mainCamera;
        private List<ChipObject> _chips = new List<ChipObject>();

        private int _currentTotalBet = 0;
        private ChipObject _selectedChip = null;

        private void Awake()
        {
            _mainCamera = Camera.main;
            
            _betLimitAnimation.Initialize(_betLimit);
        }

        public void SubscribeChips()
        {
            if (_chips.Count == 0) return;
            
            foreach (var chip in _chips)
            {
                chip.OnChipSpawned += SpawnChipAtSlot;
                chip.OnBetAttempt += HandleBetAttempt;
                chip.OnChipClicked += HandleChipClicked;
            }
        }
        
        public void UnsubscribeChips()
        {
            foreach (var chip in _chips)
            {
                chip.OnChipSpawned -= SpawnChipAtSlot;
                chip.OnBetAttempt -= HandleBetAttempt;
                chip.OnChipClicked -= HandleChipClicked;
            }
        }
        
        public void SpawnAllChips()
        {
            for (int i = 0; i < _slots.Count && i < _chipValues.Length; i++)
                SpawnChipAtSlot(i);
        }

        public void ClearAllChips()
        {
            foreach (var chip in _chips.ToList())
            {
                Destroy(chip.gameObject);
                _chips.Remove(chip);
            }
            
            _currentTotalBet = 0;
            _selectedChip = null;
        }

        public void SpawnChipAtSlot(int index)
        {
            if (index < 0 || index >= _slots.Count || index >= _chipValues.Length)
                return;

            RectTransform slot = _slots[index];
            
            Vector3 worldPos = _mainCamera.ScreenToWorldPoint(
                RectTransformUtility.WorldToScreenPoint(_mainCamera, slot.position));
            worldPos.z = 0;

            ChipObject chip = Instantiate(_chipPrefab, slot.position, Quaternion.identity);
            
            chip.Construct(
                _chipValues[index], 
                _chipNormalSize, 
                _chipSelectedSize,
                _chipNormalColor, 
                _chipSelectedColor,
                index, 
                slot.position
            );
            
            _chips.Add(chip);
            SubscribeChip(chip);
        }

        private void SubscribeChip(ChipObject chip)
        {
            chip.OnChipSpawned += SpawnChipAtSlot;
            chip.OnBetAttempt += HandleBetAttempt;
            chip.OnChipClicked += HandleChipClicked;
        }

        private void HandleChipClicked(ChipObject chip)
        {
            if (_selectedChip == chip)
            {
                // Deselect if clicking the same chip
                _selectedChip.SetSelected(false);
                _selectedChip = null;
            }
            else
            {
                // Deselect previous chip
                if (_selectedChip != null)
                {
                    _selectedChip.SetSelected(false);
                }
                
                // Select new chip
                _selectedChip = chip;
                _selectedChip.SetSelected(true);
            }
        }

        public ChipObject GetSelectedChip()
        {
            return _selectedChip;
        }

        public void DeselectChip()
        {
            _selectedChip = null;
        }

        private void HandleBetAttempt(ChipObject chip, Action onSuccess, Action onFailed)
        {
            if ((_currentTotalBet + chip.Value) <= _betLimit)
            {
                _currentTotalBet += chip.Value;
                Debug.Log($"Chip placed. Current bet: {_currentTotalBet}/{_betLimit}");
                onSuccess?.Invoke();
            }
            else
            {
                int remaining = _betLimit - _currentTotalBet;
                Debug.Log($"Cannot place chip! Bet limit exceeded. Remaining: {remaining}");
                _betLimitAnimation.Show();
                onFailed?.Invoke();
            }
        }

        public void RemoveChipFromBet(int chipValue)
        {
            _currentTotalBet -= chipValue;
            _currentTotalBet = Mathf.Max(0, _currentTotalBet);
            Debug.Log($"Chip removed. Current bet: {_currentTotalBet}/{_betLimit}");
        }

        public void ResetBetTotal()
        {
            _currentTotalBet = 0;
        }
    }
}