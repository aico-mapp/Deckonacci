using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Mode.Scripts.Network
{
    public class NetworkService
    {
        private readonly string[] _httpTargets =
        {
            "https://www.google.com/generate_204",
            "https://icanhazip.com",
            "https://one.one.one.one",
            "https://cloudflare.com",
            "https://wikipedia.org"
        };

        private const float ICMP_TIMEOUT_SECONDS = 1f;
        private const int ICMP_RETRIES = 1;
        private const int HTTP_TIMEOUT_SECONDS = 1;
        private const int DELAY = 1;

        private readonly string[] _pingTargets =
        {
            "1.1.1.1",
            "8.8.8.8",
            "208.67.222.222",
            "1.0.0.1",
            "208.67.220.220"
        };

        private CancellationTokenSource _cts;
        private bool _isLost;
        private ConnectionState _state = ConnectionState.Online;
        
        public bool IsConnectionStatus => Application.internetReachability is not NetworkReachability.NotReachable &&
                                          _state != ConnectionState.Offline;
        
        public event Action OnInternetConnectionLost;
        public event Action OnInternetConnectionReturned;

        public async UniTaskVoid StartMonitoringAsync()
        {
            _cts = new CancellationTokenSource();
            while (!_cts.IsCancellationRequested)
            {
                await CheckConnectionAsync(_cts.Token);
                await UniTask.Delay(TimeSpan.FromSeconds(DELAY), cancellationToken: _cts.Token);
            }
        }

        public void StopMonitoring()
        {
            _cts.Cancel();
        }

        private async UniTask CheckConnectionAsync(CancellationToken token)
        {
            try
            {
                var icmpAlive = await CheckTargetsAsync(true, token);
                if (icmpAlive)
                {
                    UpdateState(ConnectionState.Online);
                    return;
                }

                var httpAlive = await CheckTargetsAsync(false, token);
                if (httpAlive)
                {
                    UpdateState(ConnectionState.VPN_ICMP_Blocked);
                    return;
                }

                UpdateState(ConnectionState.Offline);
            }
            catch (Exception)
            {
                UpdateState(ConnectionState.Offline);
            }
        }

        private async UniTask<bool> CheckTargetsAsync(bool withPing, CancellationToken token)
        {
            var targets = withPing ? _pingTargets : _httpTargets;
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token);
            var localToken = linkedCts.Token;
            var tasks = new UniTask<bool>[targets.Length];
            for (var i = 0; i < tasks.Length; i++)
            {
                tasks[i] = withPing
                    ? PingAsync(targets[i], localToken)
                    : HttpAsync(targets[i], localToken);
            }
            
            try
            {
                while (tasks.Length > 0)
                {
                    var finished = await UniTask.WhenAny(tasks);
                    if (finished.result)
                    {
                        linkedCts.Cancel();
                        return true;
                    }
                    
                    tasks = tasks.Where(t => !t.Status.IsCompleted()).ToArray();
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }


        private async UniTask<bool> PingAsync(string target, CancellationToken token)
        {
            var attempts = 0;
            while (attempts < ICMP_RETRIES)
            {
                attempts++;
                var ping = new Ping(target);
                var startTime = Time.realtimeSinceStartup;

                while (!ping.isDone && Time.realtimeSinceStartup - startTime < ICMP_TIMEOUT_SECONDS)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                    if (token.IsCancellationRequested)
                        return false;
                }

                if (ping.isDone && ping.time >= 0) return true;

                await UniTask.Delay(100, cancellationToken: token);
            }

            return false;
        }

        private async UniTask<bool> HttpAsync(string target, CancellationToken token)
        {
            using var request = UnityWebRequest.Get(target);
            request.timeout = HTTP_TIMEOUT_SECONDS;
            try
            {
                await request.SendWebRequest().ToUniTask(cancellationToken: token);

                if (request.result == UnityWebRequest.Result.Success ||
                    request.responseCode == 204 || request.responseCode == 200)
                    return true;
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        private void UpdateState(ConnectionState state)
        {
            _state = state;
            if (state is ConnectionState.Online or ConnectionState.VPN_ICMP_Blocked)
                ConnectionReturned();
            else
                ConnectionLost();
        }

        private void ConnectionLost()
        {
            if (_isLost) return;
        
            _isLost = true;
            OnInternetConnectionLost?.Invoke();
        }

        private void ConnectionReturned()
        {
            if(!_isLost) return;
        
            _isLost = false;
            OnInternetConnectionReturned?.Invoke();
        }

    }
}