using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public enum DayOfWeek
    {
        Sunday,
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
    }

    [System.Serializable]
    public class DayData
    {
        public DayOfWeek day;
        public float value;

        public DayData(DayOfWeek day, float value)
        {
            this.day = day;
            this.value = value;
        }
    }

    public class BarChart : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private RectTransform _chartArea;
        [SerializeField] private GameObject _barPrefab;
    
        [Header("Chart Settings")]
        [SerializeField] private float _maxValue = 20f;
        [SerializeField] private float _barWidth = 23f;
        public float spacing = 93f;
        public float chartHeight = 371f;

        private List<DayData> _weekData;
        private Dictionary<DayOfWeek, GameObject> _bars;

        private void Start()
        {
            InitializeData();
        }
        private void InitializeData()
        {
            ResetWeekData();

            _bars = new Dictionary<DayOfWeek, GameObject>();
            
            CreateChart();
        }

        private void CreateChart()
        {
            CreateBars();
        }

        private void CreateBars()
        {
            for (int i = 0; i < _weekData.Count; i++)
            {
                DayData data = _weekData[i];

                GameObject bar = Instantiate(_barPrefab, _chartArea);
                _bars[data.day] = bar;

                RectTransform rectTransform = bar.GetComponent<RectTransform>();
                float xPosition = i * (_barWidth + spacing);
                float height = (data.value / _maxValue) * chartHeight;

                rectTransform.anchoredPosition = new Vector2(xPosition, (-240 + height) / 2);
                rectTransform.sizeDelta = new Vector2(_barWidth, height);
            }
        }

        public void ResetWeekData()
        {
            _weekData = new List<DayData>
            {
                new (DayOfWeek.Sunday, 0),
                new (DayOfWeek.Monday, 0),
                new (DayOfWeek.Tuesday, 0),
                new (DayOfWeek.Wednesday, 0),
                new (DayOfWeek.Thursday, 0),
                new (DayOfWeek.Friday, 0),
                new (DayOfWeek.Saturday, 0)
            };
        }
        
        public void UpdateValue(DayOfWeek day)
        {
            DayData dayData = _weekData.Find(d => d.day == day);
            if (dayData != null)
            {
                if (_bars.ContainsKey(day))
                {
                    dayData.value++;
                    Debug.Log($"For Day {day} daydata.value: {dayData.value}");
                    RectTransform rectTransform = _bars[day].GetComponent<RectTransform>();
                    float height = (dayData.value / _maxValue) * chartHeight;
                    float xPosition = (int)day * (_barWidth + spacing);
                    
                    rectTransform.anchoredPosition = new Vector2(xPosition, (-240 + height) / 2);
                    rectTransform.sizeDelta = new Vector2(_barWidth, height);
                }
            }
        }
    }
}