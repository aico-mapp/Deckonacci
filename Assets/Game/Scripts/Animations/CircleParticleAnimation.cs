using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class CircularParticleAnimation : MonoBehaviour
{
    [Header("Circle Settings")]
    [SerializeField] private float baseRadius = 5f;
    [SerializeField] private float radiusVariation = 1f; // Random range for radius
    
    [Header("Particle Settings")]
    [SerializeField] private GameObject particlePrefab;
    [SerializeField] private int particleCount = 50;
    [SerializeField] private float baseRotationDuration = 5f; // Base time for one full rotation
    [SerializeField] private float minSpeed = 0.5f; // Minimum speed multiplier (slower)
    [SerializeField] private float maxSpeed = 2.0f; // Maximum speed multiplier (faster)
    [SerializeField] private Ease easeType = Ease.Linear;
    
    [Header("Visual Settings")]
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 1.5f;
    [SerializeField] private bool randomizeStartPosition = true;
    
    [Header("Gizmo Settings")]
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private bool showRadiusLabels = true;
    [SerializeField] private Color centerColor = Color.yellow;
    [SerializeField] private Color baseRadiusColor = Color.green;
    [SerializeField] private Color minRadiusColor = Color.cyan;
    [SerializeField] private Color maxRadiusColor = Color.magenta;
    [SerializeField] private float centerGizmoSize = 0.3f;
    [SerializeField] private int circleSegments = 64;
    
    private List<ParticleData> particles = new List<ParticleData>();
    
    private class ParticleData
    {
        public GameObject gameObject;
        public float radius;
        public float startAngle;
        public float speedMultiplier;
        public Sequence sequence;
    }
    
    void Start()
    {
        CreateParticles();
        StartAnimation();
    }
    
    void CreateParticles()
    {
        for (int i = 0; i < particleCount; i++)
        {
            GameObject particle = Instantiate(particlePrefab, transform);
            
            // Random radius within range
            float randomRadius = baseRadius + Random.Range(-radiusVariation, radiusVariation);
            
            // Random start angle or evenly distributed
            float startAngle = randomizeStartPosition 
                ? Random.Range(0f, 360f) 
                : (i / (float)particleCount) * 360f;
            
            // Random speed multiplier - higher value = faster rotation
            float speedMultiplier = Random.Range(minSpeed, maxSpeed);
            
            // Random scale
            float randomScale = Random.Range(minScale, maxScale);
            particle.transform.localScale = Vector3.one * randomScale;
            
            // Initial position
            Vector2 initialPos = GetPositionOnCircle(startAngle, randomRadius);
            particle.transform.position = initialPos;
            
            ParticleData data = new ParticleData
            {
                gameObject = particle,
                radius = randomRadius,
                startAngle = startAngle,
                speedMultiplier = speedMultiplier,
                sequence = null
            };
            
            particles.Add(data);
        }
    }
    
    void StartAnimation()
    {
        foreach (var particleData in particles)
        {
            AnimateParticle(particleData);
        }
    }
    
    void AnimateParticle(ParticleData particleData)
    {
        // Calculate duration based on speed multiplier
        // Higher multiplier = faster = shorter duration
        float duration = baseRotationDuration / particleData.speedMultiplier;
        
        // Create a sequence for smooth looping
        particleData.sequence = DOTween.Sequence();
        
        // Animate angle from current to current + 360 (one full rotation)
        float currentAngle = particleData.startAngle;
        
        particleData.sequence.Append(
            DOVirtual.Float(currentAngle, currentAngle - 360f, duration, (angle) =>
            {
                Vector2 newPos = GetPositionOnCircle(angle, particleData.radius);
                particleData.gameObject.transform.position = newPos;
            })
            .SetEase(easeType)
        );
        
        // Loop infinitely
        particleData.sequence.SetLoops(-1, LoopType.Restart);
    }
    
    Vector2 GetPositionOnCircle(float angleDegrees, float radius)
    {
        float angleRadians = angleDegrees * Mathf.Deg2Rad;
        Vector2 center = transform.position;
        float x = center.x + radius * Mathf.Cos(angleRadians);
        float y = center.y + radius * Mathf.Sin(angleRadians);
        return new Vector2(x, y);
    }
    
    public void StopAnimation()
    {
        foreach (var particleData in particles)
        {
            particleData.sequence?.Kill();
        }
    }
    
    public void UpdateRadius(float newRadius, float newVariation)
    {
        baseRadius = newRadius;
        radiusVariation = newVariation;
        
        // Update each particle's radius
        foreach (var particleData in particles)
        {
            particleData.radius = baseRadius + Random.Range(-radiusVariation, radiusVariation);
        }
    }
    
    public void UpdateSpeed(float newMinSpeed, float newMaxSpeed)
    {
        minSpeed = newMinSpeed;
        maxSpeed = newMaxSpeed;
        
        // Restart animation with new speeds
        StopAnimation();
        foreach (var particleData in particles)
        {
            particleData.speedMultiplier = Random.Range(minSpeed, maxSpeed);
        }
        StartAnimation();
    }
    
    void OnDestroy()
    {
        StopAnimation();
    }
    
    // Visualize in editor
    void OnDrawGizmos()
    {
        if (!showGizmos) return;
        
        Vector2 center = transform.position;
        
        // Draw center point with crosshair
        Gizmos.color = centerColor;
        Gizmos.DrawWireSphere(center, centerGizmoSize);
        float crossSize = centerGizmoSize * 1.5f;
        Gizmos.DrawLine(center + Vector2.left * crossSize, center + Vector2.right * crossSize);
        Gizmos.DrawLine(center + Vector2.up * crossSize, center + Vector2.down * crossSize);
        
        // Draw base radius circle
        Gizmos.color = baseRadiusColor;
        DrawCircle(center, baseRadius, circleSegments);
        
        // Draw radius variation range
        if (radiusVariation > 0)
        {
            float minRadius = Mathf.Max(0, baseRadius - radiusVariation);
            float maxRadius = baseRadius + radiusVariation;
            
            // Min radius circle
            Gizmos.color = minRadiusColor;
            DrawCircle(center, minRadius, circleSegments);
            
            // Max radius circle
            Gizmos.color = maxRadiusColor;
            DrawCircle(center, maxRadius, circleSegments);
            
            // Draw radius indicators if enabled
            if (showRadiusLabels)
            {
                DrawRadiusLine(center, minRadius, "Min", minRadiusColor, 0);
                DrawRadiusLine(center, baseRadius, "Base", baseRadiusColor, 45);
                DrawRadiusLine(center, maxRadius, "Max", maxRadiusColor, 90);
            }
        }
    }
    
    void DrawCircle(Vector2 center, float radius, int segments)
    {
        if (radius <= 0) return;
        
        float angleStep = 360f / segments;
        Vector3 prevPoint = GetPositionOnCircle(0, radius);
        
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep;
            Vector3 newPoint = GetPositionOnCircle(angle, radius);
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
    
    void DrawRadiusLine(Vector2 center, float radius, string label, Color color, float angleDegrees)
    {
        if (radius <= 0) return;
        
        Gizmos.color = color;
        Vector2 endPoint = GetPositionOnCircle(angleDegrees, radius);
        Gizmos.DrawLine(center, endPoint);
        
        // Draw perpendicular tick mark at the end
        float tickSize = 0.15f;
        float perpAngle = angleDegrees + 90f;
        float perpRad = perpAngle * Mathf.Deg2Rad;
        Vector2 perpDir = new Vector2(Mathf.Cos(perpRad), Mathf.Sin(perpRad));
        
        Gizmos.DrawLine(endPoint - perpDir * tickSize, endPoint + perpDir * tickSize);
    }
    
    void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;
        
        Vector2 center = transform.position;
        
        // Draw directional guides when selected
        Gizmos.color = new Color(1, 1, 1, 0.3f);
        float guideLength = baseRadius + radiusVariation + 1f;
        
        // Cardinal directions
        Gizmos.DrawLine(center, center + Vector2.up * guideLength);
        Gizmos.DrawLine(center, center + Vector2.right * guideLength);
        Gizmos.DrawLine(center, center + Vector2.down * guideLength);
        Gizmos.DrawLine(center, center + Vector2.left * guideLength);
    }
}