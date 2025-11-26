using TMPro;
using UnityEngine;

namespace Mode.Scripts.UI
{
    public class LoadingScreenView : MonoBehaviour
    {
        [SerializeField] private GameObject _connectionLostScreen;
        [SerializeField] private GameObject _loadingScreen;
        [SerializeField] private TextMeshProUGUI _connectionLostText;

        public GameObject ConnectionLostScreen => _connectionLostScreen;
        public GameObject LoadingScreen => _loadingScreen;

        public void SetLostText(string text)
        {
            _connectionLostText.text = text;
        }

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}