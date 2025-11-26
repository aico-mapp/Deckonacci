using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Mode.Scripts.Timer
{
    public class TimerService
    {
        public event Action OnTick;
        
        private CancellationTokenSource _cts;

        public void Start()
        {
            _cts = new CancellationTokenSource();
            TimerAsync().Forget();
        }

        public void Stop()
        {
            _cts?.Cancel();
        }

        private async UniTask TimerAsync()
        {
            while (!_cts.IsCancellationRequested)
            {
                OnTick?.Invoke();
                await UniTask.WaitForSeconds(1);
            }
        }
    }
}