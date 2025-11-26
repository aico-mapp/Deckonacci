using System;
using System.Collections;
using System.Collections.Generic;
using Game.Calendar.Scripts.Data;
using Game.Calendar.Scripts.Data.StaticData;
using Game.Calendar.Scripts.Game.Table;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Calendar.Scripts.Game.UI.RouletteScreen
{
    public class RouletteController : MonoBehaviour
    {
        [Header("Roulette Setup")]
        [SerializeField] private Transform _rouletteWheel;
        [SerializeField] private Transform _ball;
        [SerializeField] private RouletteReward[] _rewards;
        
        [Header("Spin Settings")]
        [SerializeField] private float _wheelSpinDuration = 3f;
        [SerializeField] private float _ballSpinDuration = 4f;
        [SerializeField] private AnimationCurve _wheelSpinCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private AnimationCurve _ballSpinCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        [Header("Ball Movement")]
        [SerializeField] private float _ballRadius = 150f;
        [SerializeField] private float _ballInnerRadius = 50f;
        [SerializeField] private float _startAngleOffset = 90f; // Start at top (90 degrees)
        [SerializeField] private int _minWheelRotations = 3;
        [SerializeField] private int _maxWheelRotations = 5;
        [SerializeField] private int _minBallRotations = 5;
        [SerializeField] private int _maxBallRotations = 8;
        
        [Header("Reward Layout")]
        [SerializeField] private float _firstRewardAngle = 90f; // Angle where first reward (index 0) is positioned
        [SerializeField] private bool _clockwiseRewards = false; // Direction rewards are arranged
        
        private bool _isSpinning;
        private int _currentRewardIndex;
        private float _segmentAngle;
        private Vector3 _ballStartPosition;
        
        public event Action<RouletteReward> OnRewardChanged;
        public event Action<RouletteReward> OnSpinComplete;
        public event Action OnSpinStart;
        
        public RouletteReward[] GetRewards()
        {
            return _rewards;
        }

        private void Awake()
        {
            if (_rewards.Length > 0)
            {
                _segmentAngle = 360f / _rewards.Length;
            }
            
            // Store initial ball position
            if (_ball != null)
            {
                _ballStartPosition = _ball.localPosition;
            }
            else
            {
                // Default to top position if not set
                _ballStartPosition = new Vector3(0, _ballRadius, 0);
            }
        }

        private void Start()
        {
            // Set ball to start position
            if (_ball != null)
            {
                _ball.localPosition = _ballStartPosition;
            }
        }

        public void StartSpin(int? targetRewardIndex = null)
        {
            if (_isSpinning) return;
            
            StartCoroutine(SpinRoutine(targetRewardIndex));
        }

        private IEnumerator SpinRoutine(int? targetRewardIndex)
        {
            _isSpinning = true;
            OnSpinStart?.Invoke();
            
            // Reset ball to start position
            _ball.localPosition = _ballStartPosition;
            
            // Determine target reward
            int targetIndex = targetRewardIndex ?? Random.Range(0, _rewards.Length);
            float targetAngle = CalculateTargetAngle(targetIndex);
            
            // Calculate rotations
            int wheelRotations = Random.Range(_minWheelRotations, _maxWheelRotations + 1);
            int ballRotations = Random.Range(_minBallRotations, _maxBallRotations + 1);
            
            float wheelTotalRotation = wheelRotations * 360f + Random.Range(-25f, 25f);
            
            // Ball rotates in opposite direction to wheel
            // Calculate final angle needed to land on target
            float ballTotalRotation = -(ballRotations * 360f + targetAngle - _startAngleOffset);
            
            // Start both animations
            Coroutine wheelSpin = StartCoroutine(SpinWheel(wheelTotalRotation));
            Coroutine ballSpin = StartCoroutine(SpinBall(ballTotalRotation, targetIndex));
            
            // Wait for ball to finish
            yield return ballSpin;
            yield return wheelSpin;
            
            _isSpinning = false;
            OnSpinComplete?.Invoke(_rewards[targetIndex]);
        }

        private IEnumerator SpinWheel(float totalRotation)
        {
            float elapsed = 0f;
            Quaternion startRotation = _rouletteWheel.localRotation;
            
            while (elapsed < _wheelSpinDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / _wheelSpinDuration;
                float curveValue = _wheelSpinCurve.Evaluate(progress);
                
                float currentRotation = curveValue * totalRotation;
                _rouletteWheel.localRotation = startRotation * Quaternion.Euler(0, 0, currentRotation);
                
                yield return null;
            }
        }

        private IEnumerator SpinBall(float totalRotation, int targetIndex)
        {
            float elapsed = 0f;
            float currentAngle = _startAngleOffset;
            int lastRewardIndex = -1;
            
            while (elapsed < _ballSpinDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / _ballSpinDuration;
                float curveValue = _ballSpinCurve.Evaluate(progress);
                
                // Calculate angle (starting from _startAngleOffset)
                currentAngle = _startAngleOffset + (curveValue * totalRotation);
                
                // Calculate radius (ball moves inward as it slows down)
                float currentRadius = Mathf.Lerp(_ballRadius, _ballInnerRadius, progress);
                
                // Position ball
                float angleRad = currentAngle * Mathf.Deg2Rad;
                Vector3 ballPosition = new Vector3(
                    Mathf.Cos(angleRad) * currentRadius,
                    Mathf.Sin(angleRad) * currentRadius,
                    0
                );
                _ball.localPosition = ballPosition;
                
                // Check current reward - ball position relative to static detection point
                int currentRewardIndex = GetRewardIndexFromBallAngle(currentAngle);
                
                if (currentRewardIndex != lastRewardIndex && currentRewardIndex >= 0)
                {
                    lastRewardIndex = currentRewardIndex;
                    _currentRewardIndex = currentRewardIndex;
                    OnRewardChanged?.Invoke(_rewards[currentRewardIndex]);
                }
                
                yield return null;
            }
            
            // Ensure final position is exact
            _currentRewardIndex = targetIndex;
            float finalAngle = _startAngleOffset + totalRotation;
            float angleRadFinal = finalAngle * Mathf.Deg2Rad;
            _ball.localPosition = new Vector3(
                Mathf.Cos(angleRadFinal) * _ballInnerRadius,
                Mathf.Sin(angleRadFinal) * _ballInnerRadius,
                0
            );
            
            OnRewardChanged?.Invoke(_rewards[targetIndex]);
        }

        private float CalculateTargetAngle(int rewardIndex)
        {
            // Calculate angle for the reward on the roulette
            // This is the angle from the wheel's perspective
            float angle = _firstRewardAngle + (rewardIndex * _segmentAngle);
            
            if (!_clockwiseRewards)
            {
                angle = _firstRewardAngle - (rewardIndex * _segmentAngle);
            }
            
            return NormalizeAngle(angle);
        }

        private int GetRewardIndexFromBallAngle(float ballAngle)
        {
            // Normalize the ball angle
            ballAngle = NormalizeAngle(ballAngle);
            
            // Get current wheel rotation
            float wheelRotation = _rouletteWheel.localEulerAngles.z;
            
            // Calculate relative angle between ball and wheel
            // The detection point is where the ball "lands" - typically at the top or a specific marker
            float detectionPointAngle = _startAngleOffset; // Usually top of wheel
            float relativeAngle = NormalizeAngle(ballAngle - wheelRotation);
            
            // Calculate which segment this corresponds to
            // Adjust the relative angle to match where rewards are on the wheel
            float adjustedAngle = NormalizeAngle(detectionPointAngle - relativeAngle);
            
            // Find which reward this angle corresponds to
            for (int i = 0; i < _rewards.Length; i++)
            {
                float rewardAngle = CalculateTargetAngle(i);
                float angleDiff = Mathf.Abs(Mathf.DeltaAngle(adjustedAngle, rewardAngle));
                
                if (angleDiff < _segmentAngle / 2f)
                {
                    return i;
                }
            }
            
            // Fallback: calculate by segment
            float segmentIndex = adjustedAngle / _segmentAngle;
            return Mathf.RoundToInt(segmentIndex) % _rewards.Length;
        }

        private float NormalizeAngle(float angle)
        {
            angle = angle % 360f;
            if (angle < 0) angle += 360f;
            return angle;
        }

        public RouletteReward GetCurrentReward()
        {
            return _currentRewardIndex >= 0 && _currentRewardIndex < _rewards.Length 
                ? _rewards[_currentRewardIndex] 
                : null;
        }

        public bool IsSpinning => _isSpinning;
    }
}