// ChipObject.cs - Add public method to trigger bet attempt
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Game.Calendar.Scripts.Game.Table;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Rendering;

namespace Game.Calendar.Scripts.Game.Chip
{
    [RequireComponent(typeof(Collider2D))]
    public class ChipObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        public event Action<int> OnChipSpawned;
        public event Action<ChipObject, Action, Action> OnBetAttempt;
        public event Action<ChipObject> OnChipClicked;
        
        [SerializeField] private int _orderId;
        [SerializeField] private TextMeshPro _valueText;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private Camera _mainCamera;
        private Vector3 _offset;
        private Vector3 _startPosition;
        private SortingGroup _sortingGroup;
        
        private Vector3 _normalScale;
        private Vector3 _selectedScale;
        private Color _normalColor;
        private Color _selectedColor;
        
        private int _slotIndex;
        private bool _isSelected = false;
        private bool _isDragging = false;

        public bool canResize;
        
        public int Value { get; private set; }
        public int SlotIndex => _slotIndex;
        public int OrderId => _orderId;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _sortingGroup = GetComponent<SortingGroup>();
        }

        public void Construct(int value, Vector3 normalScale, Vector3 selectedScale, Color normalColor, Color selectedColor, int slotIndex, Vector3 position)
        {
            Value = value;
            _normalScale = normalScale;
            _selectedScale = selectedScale;
            _normalColor = normalColor;
            _selectedColor = selectedColor;
            _slotIndex = slotIndex;
            _startPosition = position;
            
            _valueText.text = value.ToString();
            _valueText.color = _normalColor;
            
            _sortingGroup.sortingOrder = _orderId;
            
            transform.localScale = _normalScale;

            canResize = true;

            ShowChip();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_isDragging)
            {
                OnChipClicked?.Invoke(this);
            }
        }
        
        public void SetTableStyle()
        {
            // Update text color to white for table chips
            _valueText.DOColor(Color.white, 0.3f);
    
            // Reset scale to normal
            transform.localScale = Vector3.one;
    
            // Deselect the chip
            _isSelected = false;
        }

        public void SetSelected(bool selected)
        {
            if (!canResize) return;
            
            _isSelected = selected;
            
            if (_isSelected)
            {
                transform.DOScale(_selectedScale, 0.2f).SetEase(Ease.OutBack);
                _valueText.DOColor(_selectedColor, 0.2f);
                _sortingGroup.sortingOrder = 1000;
            }
            else
            {
                transform.DOScale(_normalScale, 0.2f).SetEase(Ease.OutBack);
                _valueText.DOColor(_normalColor, 0.2f);
                _sortingGroup.sortingOrder = _orderId;
            }
        }

        // Public method to trigger bet attempt from external code
        public void TryPlaceBet(Action onSuccess, Action onFailed)
        {
            OnBetAttempt?.Invoke(this, onSuccess, onFailed);
        }

        // Public method to spawn new chip at original slot
        public void SpawnNewChipAtSlot()
        {
            OnChipSpawned?.Invoke(_slotIndex);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _isDragging = true;
            
            Vector3 worldPoint = _mainCamera.ScreenToWorldPoint(eventData.position);
            worldPoint.z = 0;
            _offset = transform.position - worldPoint;

            transform.DOScale(1.15f, 0.15f);

            _sortingGroup.sortingOrder = 999;
            _valueText.DOColor(Color.white, 0.3f);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector3 worldPoint = _mainCamera.ScreenToWorldPoint(eventData.position);
            worldPoint.z = 0;
            transform.position = worldPoint + _offset;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _isDragging = false;
            
            transform.DOScale(1f, 0.15f);

            Vector3 worldPos = _mainCamera.ScreenToWorldPoint(eventData.position);
            worldPos.z = 0;

            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
            if (hit.collider != null && hit.collider.TryGetComponent(out TableField field))
            {
                TryPlaceBet(
                    () => {
                        field.SpawnChip(this, _orderId);
                        SpawnNewChipAtSlot();
                    },
                    ReturnToStartPosition
                );
                return;
            }
            
            ReturnToStartPosition();
        }

        private void ReturnToStartPosition()
        {
            if (!canResize) return;
            
            transform.DOMove(_startPosition, 0.3f).SetEase(Ease.OutBack);
            
            if (_isSelected)
            {
                transform.DOScale(_selectedScale, 0.3f).SetEase(Ease.OutBack);
                _valueText.DOColor(_selectedColor, 0.3f);
                _sortingGroup.sortingOrder = 1000;
            }
            else
            {
                transform.DOScale(_normalScale, 0.3f).SetEase(Ease.OutBack);
                _valueText.DOColor(_normalColor, 0.3f);
                _sortingGroup.sortingOrder = _orderId;
            }
        }

        private void ShowChip()
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(_normalScale, 0.3f).SetEase(Ease.OutBack);
        }

        // Method to get chip data for cloning
        public ChipData GetChipData()
        {
            return new ChipData
            {
                Value = Value,
                NormalScale = _normalScale,
                SelectedScale = _selectedScale,
                NormalColor = _normalColor,
                SelectedColor = _selectedColor,
                SlotIndex = _slotIndex,
                Position = _startPosition,
                OrderId = _orderId
            };
        }
    }

    // Data structure for chip information
    public struct ChipData
    {
        public int Value;
        public Vector3 NormalScale;
        public Vector3 SelectedScale;
        public Color NormalColor;
        public Color SelectedColor;
        public int SlotIndex;
        public Vector3 Position;
        public int OrderId;
    }
}