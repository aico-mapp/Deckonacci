using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Calendar.Scripts.Game.Wheel
{
    public class SpinWheelController : MonoBehaviour
    {
        [Header("Wheel Settings")]
        [SerializeField] private RectTransform _wheelTransform;
        [SerializeField] private int[] _rewardValues = { 100, 500, 200, 900, 300, 100, 600, 200, 500, 300, 600, 400 };
        [SerializeField] private TextMeshProUGUI[] _rewardTexts;
        
        [Header("Spin Settings")]
        [SerializeField] private float _spinDuration = 3f;
        [SerializeField] private float _minRotations = 3.2f;
        [SerializeField] private float _maxRotations = 5.2f;
        [SerializeField] private Ease _spinEase = Ease.OutQuart;
        
        [Header("Calibration")]
        [SerializeField] private float _segmentOffset = 0f; 
        
        [Header("Debug Gizmos")]
        [SerializeField] private bool _showGizmos = true;
        [SerializeField] private float _gizmoRadius = 200f;
        [SerializeField] private bool _showSegmentLines = true;
        [SerializeField] private bool _showSegmentNumbers = true;
        
        private bool _isSpinning = false;
        private int _segmentCount;
        private float _segmentAngle;
        private float _currentRotation = 0f;
        
        private const float POINTER_ANGLE = 0f; 

        private void Awake()
        {
            _segmentCount = _rewardValues.Length;
            
            if (_segmentCount == 0)
            {
                Debug.LogError("Reward values array is empty!");
                return;
            }
            
            _segmentAngle = 360f / _segmentCount;
        }

        public void InitializeWheel()
        {
            if (_rewardTexts != null && _rewardTexts.Length > 0)
            {
                for (int i = 0; i < _rewardValues.Length && i < _rewardTexts.Length; i++)
                {
                    if (_rewardTexts[i] != null)
                    {
                        _rewardTexts[i].text = _rewardValues[i].ToString();
                    }
                }
            }
        }

        public void Spin(Action<int> onSpinComplete)
        {
            if (_isSpinning || _segmentCount == 0) return;

            _isSpinning = true;
            
            float rotations = Random.Range(_minRotations, _maxRotations);
            int targetSegment = Random.Range(0, _segmentCount);
            int reward = _rewardValues[targetSegment];
            
            float targetSegmentCenter = (targetSegment * _segmentAngle) + (_segmentAngle / 2f);
            float targetAngle = targetSegmentCenter - POINTER_ANGLE - _segmentOffset;
            float spinAmount = (rotations * 360f) + targetAngle;
            float finalRotation = _currentRotation - spinAmount;
            
            _wheelTransform.DORotate(new Vector3(0, 0, finalRotation), _spinDuration, RotateMode.FastBeyond360)
                .SetEase(_spinEase)
                .OnComplete(() =>
                {
                    _isSpinning = false;
                    _currentRotation = finalRotation;
                    
                    Sequence bounceSequence = DOTween.Sequence();
                    bounceSequence.Append(_wheelTransform.DORotate(new Vector3(0, 0, finalRotation + 3f), 0.1f));
                    bounceSequence.Append(_wheelTransform.DORotate(new Vector3(0, 0, finalRotation), 0.1f));
                    
                    bounceSequence.OnComplete(() =>
                    {
                        int finalReward = GetRewardAtPointer();
                        onSpinComplete?.Invoke(finalReward);
                    });
                });
        }

        private int GetSegmentAtPointer()
        {
            if (_segmentCount == 0) return 0;
            
            float wheelAngle = _wheelTransform.eulerAngles.z;
            float adjustedAngle = NormalizeAngle(wheelAngle + POINTER_ANGLE + _segmentOffset);
            
            int segmentIndex = Mathf.FloorToInt(adjustedAngle / _segmentAngle) % _segmentCount;
            
            return segmentIndex;
        }

        private int GetRewardAtPointer()
        {
            int segmentIndex = GetSegmentAtPointer();
            return _rewardValues[segmentIndex];
        }

        private float NormalizeAngle(float angle)
        {
            angle = angle % 360f;
            if (angle < 0) angle += 360f;
            return angle;
        }

        public bool IsSpinning() => _isSpinning;

        public void ResetWheel()
        {
            _wheelTransform.DOKill();
            _wheelTransform.rotation = Quaternion.identity;
            _currentRotation = 0f;
            _isSpinning = false;
        }

        private void OnDestroy()
        {
            _wheelTransform.DOKill();
        }
        
        private void OnDrawGizmos()
        {
            if (!_showGizmos || _wheelTransform == null) return;

            Vector3 center = _wheelTransform.position;
            int segCount = _rewardValues?.Length ?? 12;
            float segAngle = 360f / segCount;
            
            if (_showSegmentLines)
            {
                for (int i = 0; i < segCount; i++)
                {
                    
                    float boundaryAngle = (i * segAngle) - _wheelTransform.eulerAngles.z - _segmentOffset;
                    float radians = boundaryAngle * Mathf.Deg2Rad;
                    
                    Vector3 direction = new Vector3(Mathf.Sin(radians), Mathf.Cos(radians), 0);
                    
                    Gizmos.color = i % 2 == 0 ? new Color(1f, 1f, 0f, 0.7f) : new Color(1f, 0.5f, 0f, 0.7f);
                    Gizmos.DrawLine(center, center + direction * _gizmoRadius);
                    
                    if (_showSegmentNumbers)
                    {
                        float centerAngle = ((i * segAngle) + (segAngle / 2f)) - _wheelTransform.eulerAngles.z - _segmentOffset;
                        float centerRad = centerAngle * Mathf.Deg2Rad;
                        Vector3 centerDir = new Vector3(Mathf.Sin(centerRad), Mathf.Cos(centerRad), 0);
                        Vector3 textPos = center + centerDir * (_gizmoRadius * 0.65f);
                        
                        #if UNITY_EDITOR
                        UnityEditor.Handles.Label(textPos, $"S{i}\n{(_rewardValues != null && i < _rewardValues.Length ? _rewardValues[i].ToString() : "?")}");
                        #endif
                    }
                }
            }
            
            float pointerRad = POINTER_ANGLE * Mathf.Deg2Rad;
            Vector3 pointerDir = new Vector3(Mathf.Sin(pointerRad), Mathf.Cos(pointerRad), 0);
            
            Gizmos.color = Color.red;
            Gizmos.DrawLine(center, center + pointerDir * (_gizmoRadius + 50f));
            
            #if UNITY_EDITOR
            UnityEditor.Handles.Label(center + pointerDir * (_gizmoRadius + 70f), "POINTER");
            #endif
            
            if (Application.isPlaying)
            {
                int currentSegment = GetSegmentAtPointer();
                int currentReward = GetRewardAtPointer();
                
                
                Gizmos.color = Color.green;
                float arcStart = POINTER_ANGLE - (segAngle / 2f);
                float arcEnd = POINTER_ANGLE + (segAngle / 2f);
                
                for (float a = arcStart; a < arcEnd; a += 3f)
                {
                    float rad1 = a * Mathf.Deg2Rad;
                    float rad2 = (a + 3f) * Mathf.Deg2Rad;
                    Vector3 p1 = center + new Vector3(Mathf.Sin(rad1), Mathf.Cos(rad1), 0) * (_gizmoRadius - 30f);
                    Vector3 p2 = center + new Vector3(Mathf.Sin(rad2), Mathf.Cos(rad2), 0) * (_gizmoRadius - 30f);
                    Gizmos.DrawLine(p1, p2);
                }
                
                #if UNITY_EDITOR
                
                UnityEditor.Handles.color = Color.green;
                UnityEditor.Handles.Label(center + pointerDir * (_gizmoRadius + 50f), 
                    $"S{currentSegment}: {currentReward}");
                
                UnityEditor.Handles.color = Color.white;
                UnityEditor.Handles.Label(center + new Vector3(0, _gizmoRadius + 120f, 0), 
                    $"Segment: {currentSegment} | Reward: {currentReward}\n" +
                    $"Wheel: {_wheelTransform.eulerAngles.z:F1}° | Offset: {_segmentOffset:F1}°");
                #endif
            }
        }
    }
}