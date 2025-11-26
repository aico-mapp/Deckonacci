using TMPro;
using UnityEngine;

namespace Game.Calendar.Scripts.Game.Wheel
{
    public class BonusResultInfo: MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _bonusText;

        public void SetBonusText(int reward)
        {
            _bonusText.text = $"+{reward}";
        }
    }
}