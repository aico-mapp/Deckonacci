using UnityEngine;
using DG.Tweening;
using Game.Calendar.Scripts.Game.Chip;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;

namespace Game.Calendar.Scripts.Game.Table
{
    public class TableField : MonoBehaviour, IPointerClickHandler
    {
        public event Action<TableCategory, int> OnChipPlaced;

        [Header("Chip Offset Settings")]
        [SerializeField] private float _chipStackOffset = 0.15f;
        [SerializeField] private float _chipRotationVariance = 5f;
        [SerializeField] private float _chipMoveDuration = 0.25f;
        
        private TableCategory _tableCategory;
        private Transform _chipsParent;
        private List<ChipObject> _chipsOnField = new List<ChipObject>();
        private ChipsController _chipsController;
        
        private void Awake()
        {
            _chipsParent = transform;
            _tableCategory = GetComponent<TableCategory>();
        }

        public void Initialize(ChipsController chipsController)
        {
            _chipsController = chipsController;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_chipsController == null) return;
            
            ChipObject selectedChip = _chipsController.GetSelectedChip();
            if (selectedChip != null)
            {
                PlaceSelectedChip(selectedChip);
            }
        }

        private void PlaceSelectedChip(ChipObject selectedChip)
        {
            selectedChip.TryPlaceBet(
                () => {
                    
                    MoveChipToTable(selectedChip);
                    selectedChip.SpawnNewChipAtSlot();
                },
                () => {
                    
                    Debug.Log("Failed to place bet - limit exceeded");
                }
            );
        }

        private void MoveChipToTable(ChipObject chip)
        {
            ChipData chipData = chip.GetChipData();
            int orderId = chipData.OrderId;
            
            Vector3 targetPosition = CalculateChipPosition(_chipsOnField.Count);
            int finalSortingOrder = orderId + _chipsOnField.Count;
            float targetRotation = CalculateChipRotation();
            
            chip.transform.SetParent(_chipsParent);
            
            var sortingGroup = chip.GetComponent<SortingGroup>();
            var collider = chip.GetComponent<Collider2D>();
            
            if (collider != null)
            {
                collider.enabled = false;
            }
            
            _chipsOnField.Add(chip);
            
            Sequence moveSequence = DOTween.Sequence();
            moveSequence.Append(chip.transform.DOScale(1.15f, _chipMoveDuration * 0.3f).SetEase(Ease.OutQuad));
            moveSequence.Join(chip.transform.DOMove(targetPosition, _chipMoveDuration).SetEase(Ease.OutBack));
            moveSequence.Join(chip.transform.DORotate(
                new Vector3(chip.transform.rotation.eulerAngles.x, chip.transform.rotation.eulerAngles.y, targetRotation), 
                _chipMoveDuration
            ).SetEase(Ease.OutQuad));
            moveSequence.Append(chip.transform.DOScale(1f, _chipMoveDuration * 0.3f).SetEase(Ease.InQuad));
            moveSequence.OnComplete(() => 
            {
                sortingGroup.sortingOrder = finalSortingOrder;
                chip.canResize = false;
                chip.SetTableStyle();
                chip.SetSelected(false);
            });
            
            OnChipPlaced?.Invoke(_tableCategory, chip.Value);
        }

        public void SpawnChip(ChipObject chip, int orderId)
        {
            chip.transform.SetParent(_chipsParent);
            
            var sortingGroup = chip.GetComponent<SortingGroup>();
            chip.GetComponent<Collider2D>().enabled = false;
            
            Vector3 targetPosition = CalculateChipPosition(_chipsOnField.Count);
            int finalSortingOrder = orderId + _chipsOnField.Count;
            
            _chipsOnField.Add(chip);
            
            chip.transform.rotation = Quaternion.Euler(chip.transform.rotation.x, chip.transform.rotation.y, CalculateChipRotation());
            chip.transform.DOMove(targetPosition, 0.25f).SetEase(Ease.OutBack).OnComplete(() => 
            {
                sortingGroup.sortingOrder = finalSortingOrder;
            });
            
            OnChipPlaced?.Invoke(_tableCategory, chip.Value);
        }
        
        public List<ChipObject> GetChipObjects()
        {
            List<ChipObject> chipObjects = new List<ChipObject>();

            foreach (var chip in _chipsOnField)
            {
                if (chip != null)
                {
                    chipObjects.Add(chip);
                }
            }

            return chipObjects;
        }

        private Vector3 CalculateChipPosition(int chipIndex)
        {
            Vector3 basePosition = transform.position;
            Vector3 offset = new Vector3(0, chipIndex * _chipStackOffset, 0);
            return basePosition + offset;
        }

        private float CalculateChipRotation()
        {
            return UnityEngine.Random.Range(-_chipRotationVariance, _chipRotationVariance);
        }

        public void ClearChips()
        {
            foreach (var chip in _chipsOnField)
            {
                if (chip != null)
                    chip.gameObject.SetActive(false);
            }
            _chipsOnField.Clear();
        }
    }
}