using Game.Scripts;
using Game.Scripts.Game.DeviceAdaptation;
using UnityEngine;

namespace Game.Calendar.Scripts.Game.DeviceAdaptation
{
    public class ChartResolution : DeviceSelection, IDeviceAdjustable
    {
        private BarChart _barChart;

        [Header("IPhone")] 
        [SerializeField] private int _iPhoneChartHeight = 371;
        [SerializeField] private int _iPhoneSpacing = 93;
        [Header("IPad")] 
        [SerializeField] private int _iPadChartHeight = 290;
        [SerializeField] private int _iPadSpacing = 65;
    

        private void Start()
        {
            _barChart = GetComponent<BarChart>();
            SetGridLayoutSize(IsIpad());
        }

        public void SetGridLayoutSize(bool isIpad)
        {
            _barChart.spacing = isIpad ? _iPadSpacing : _iPhoneSpacing;
            _barChart.chartHeight = isIpad ? _iPadChartHeight : _iPhoneChartHeight;
        }

        public void AdjustResolution()
        {
        
        }
    }
}