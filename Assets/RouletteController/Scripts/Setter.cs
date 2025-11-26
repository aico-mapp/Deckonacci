using Mode.Scripts.Services;
using Mode.Scripts.UI;

namespace Mode.Scripts
{
    public class Setter
    {
        private readonly OrientationChanger _orientationChanger;
        private readonly WebViewResizer _webViewResizer;
        private readonly StarterModel _starterModel;
        private readonly StarterService _starterService;

        public Setter(
            OrientationChanger orientationChanger,
            WebViewResizer webViewResizer,
            StarterModel starterModel,
            StarterService starterService)
        {
            _orientationChanger = orientationChanger;
            _webViewResizer = webViewResizer;
            _starterModel = starterModel;
            _starterService = starterService;
        }

        public void Set()
        {
            _starterModel.OrientationChanger = _orientationChanger;
            _starterModel.WebViewResizer = _webViewResizer;
            _starterService.Run();
        }
    }
}