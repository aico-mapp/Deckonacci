using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace Mode.Scripts.Http
{
    public class HttpService
    {
        private const string POST = "POST";
        private const string APPLICATION_JSON = "application/json";
        private const string CONTENT_TYPE = "Content-Type";

        public async UniTask Send(string url, string json)
        {
            await SendPostRequest(url, new System.Text.UTF8Encoding().GetBytes(json));
        }

        private async UniTask SendPostRequest(string url, byte[] postData)
        {
            UploadHandler uploadHandler = new UploadHandlerRaw(postData);
            DownloadHandler downloadHandler = new DownloadHandlerBuffer();
            using var request = new UnityWebRequest(url, POST);
            request.uploadHandler = uploadHandler;
            request.downloadHandler = downloadHandler;
            request.SetRequestHeader(CONTENT_TYPE, APPLICATION_JSON);
            await request.SendWebRequest();
        }
    }
}