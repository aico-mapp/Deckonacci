using System.Collections.Generic;
using UnityEngine;

namespace Game.Calendar.Scripts.Game.Fibonacci
{
    public class FibonacciPlayer
    {
        public List<FibonacciCard> Hand = new List<FibonacciCard>();
        public Transform CardContainer;
        public bool IsHumanPlayer;
        
        public FibonacciPlayer(Transform container, bool isHuman)
        {
            CardContainer = container;
            IsHumanPlayer = isHuman;
        }

        public void AddCard(FibonacciCard card)
        {
            Hand.Add(card);
        }

        public FibonacciCard PlayCard(int index)
        {
            if (index >= 0 && index < Hand.Count)
            {
                var card = Hand[index];
                Hand.RemoveAt(index);
                return card;
            }
            return null;
        }

        public FibonacciCard GetRandomCard()
        {
            if (Hand.Count == 0) return null;
            
            int randomIndex = Random.Range(0, Hand.Count);
            return PlayCard(randomIndex);
        }
    }
}