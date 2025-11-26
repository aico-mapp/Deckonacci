using Game.Calendar.Scripts.Game.Chip;
using UnityEngine;

namespace Game.Calendar.Scripts.Game.Table
{
    public class TableController: MonoBehaviour
    {
        [SerializeField] private GameObject _tablePrefab;
        [SerializeField] private Vector3 _tableSize;

        private GameObject _currentTable;
        private BetTable _betTable;
        

        public BetTable SetupTable()
        {
            _currentTable = Instantiate(_tablePrefab, Vector3.zero, Quaternion.identity);
            _currentTable.transform.localScale = _tableSize;
            _currentTable.transform.SetParent(null);
            
            return _currentTable.GetComponent<BetTable>();
        }

        public void SubscribeBetTable()
        {
            _betTable.SubscribeTable();
        }
        
        public void UnsubscribeBetTable()
        {
            _betTable.UnsubscribeTable();
        }

        public void ResetTable()
        {
            Destroy(_currentTable);
        }
        
        
    }
}