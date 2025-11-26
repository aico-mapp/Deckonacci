using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Calendar.Scripts.Game.Table
{
    public enum CategoryColor
    {
        White,
        Orange
    }
    
    public enum CategoryType
    {
        Value,
        Joker,
        Fibonacci,
        Suit
    }
    
    public enum SuitType
    {
        Hearts,
        Clubs,
        Spades,
        Diamonds
    }
    
    public class TableCategory : MonoBehaviour
    {
        public int TableValue;
        public CategoryType Type;
        public CategoryColor Color;
        public SuitType Suit;
    }
}