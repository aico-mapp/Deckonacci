using UnityEngine;

namespace Game.Calendar.Scripts.Game.Fibonacci
{
    [System.Serializable]
    public class FibonacciCard
    {
        public int Value;
        public Sprite FrontSprite;
        public Sprite BackSprite;
        
        public FibonacciCard(int value)
        {
            Value = value;
        }
    }
}