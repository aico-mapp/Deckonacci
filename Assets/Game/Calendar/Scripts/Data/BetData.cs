using System;
using Game.Calendar.Scripts.Game.Table;

namespace Game.Calendar.Scripts.Data
{
    [Serializable]
    public class BetData
    {
        public CategoryType Type;
        public CategoryColor Color;
        public SuitType Suit;
        public int TableValue;
        public int ChipValue;
        public int BetCount;

        public BetData(CategoryType type, CategoryColor color, SuitType suit, int tableValue, int chipValue)
        {
            Type = type;
            Color = color;
            Suit = suit;
            TableValue = tableValue;
            ChipValue = chipValue;
            BetCount = 1;
        }

        public bool MatchesCategory(TableCategory category, int chipValue)
        {
            return Type == category.Type &&
                   Color == category.Color &&
                   Suit == category.Suit &&
                   TableValue == category.TableValue &&
                   ChipValue == chipValue;
        }

        public void IncrementBet()
        {
            BetCount++;
        }
        
        public BetData Clone()
        {
            return new BetData(Type, Color, Suit, TableValue, ChipValue)
            {
                BetCount = this.BetCount
            };
        }
    }
}