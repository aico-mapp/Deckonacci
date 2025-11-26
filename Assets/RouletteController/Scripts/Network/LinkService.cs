using System.Text;
using Mode.Scripts.Analytics;
using Mode.Scripts.Data;
using Mode.Scripts.Localization;
using UnityEngine;

namespace Mode.Scripts.Network
{
    public class LinkService
    {
        private readonly AnalyticsModel _analyticsModel;
        private readonly LocalizationService _localizationService;
        private readonly DataService _dataService;

        public LinkService(
            AnalyticsModel analyticsModel,
            LocalizationService localizationService,
            DataService dataService)
        {
            _analyticsModel = analyticsModel;
            _localizationService = localizationService;
            _dataService = dataService;
        }
        
        public string GetLinker()
        {
            var urlBuilder = new StringBuilder(GetResponseLinker(), 300);
            SetNotification(urlBuilder);
            SetConversionTag(urlBuilder);
            SetChain(urlBuilder);
            SetParams(urlBuilder);
            return urlBuilder.ToString();
        }

        private void SetNotification(StringBuilder urlBuilder)
        {
            var pushLink = _dataService.PushLink;
            if (string.IsNullOrWhiteSpace(pushLink)) return;
            pushLink = pushLink.Replace("var1", "r");
            AddParamSymbol(urlBuilder);
            urlBuilder.Append(pushLink);
        }

        private void SetChain(StringBuilder urlBuilder)
        {
            AddParamSymbol(urlBuilder);
            urlBuilder.Append("chain=" + _analyticsModel.Chain);
        }
        
        private void SetLang(StringBuilder urlBuilder)
        {
            AddParamSymbol(urlBuilder);
            var appLanguage = _localizationService.Word;
            urlBuilder.Append("lang=" + (appLanguage.AliasLang ?? PreciseLocale.GetLanguage()));
        }
        
        private void SetConversionTag(StringBuilder urlBuilder)
        {
            AddParamSymbol(urlBuilder);
            urlBuilder.Append("ctag=" + _dataService.ConversionTag);
        }

        private void SetParams(StringBuilder urlBuilder)
        {
            AddParamSymbol(urlBuilder);
            urlBuilder.Append(_dataService.Wrapper.additionalParametrs);
        }

        private void AddParamSymbol(StringBuilder urlBuilder) =>
            urlBuilder.Append(urlBuilder.ToString().IndexOf('?') != -1 ? '&' : '?');

        private string GetResponseLinker()
        {
            string locale = PreciseLocale.GetRegion();

            if (string.IsNullOrWhiteSpace(_dataService.PushLink) || _dataService.Wrapper.pushUrls == null ||
                !_dataService.Wrapper.pushUrls.ContainsKey("def")) return GetLinkWrapperUrl(locale);

            string pushUrl = GetLinkWrapperPushUrl(locale);
            return string.IsNullOrWhiteSpace(pushUrl) ? GetLinkWrapperUrl(locale) : pushUrl;
        }

        private string GetLinkWrapperUrl(string localeKey)
        {
            LinkWrapper linkWrapper = TryGetLocaleUrl(localeKey, out LinkWrapper configURL)
                ? configURL
                : _dataService.Wrapper.urls["def"];

            return linkWrapper.url + linkWrapper.hash;
        }
        
        private string GetLinkWrapperPushUrl(string localeKey)
        {
            LinkWrapper linkWrapper = TryGetLocalePushUrl(localeKey, out LinkWrapper configURL)
                ? configURL
                : _dataService.Wrapper.pushUrls["def"];

            return linkWrapper.url + linkWrapper.hash;
        }

        private bool TryGetLocaleUrl(string locale, out LinkWrapper configLink) =>
            _dataService.Wrapper.urls.TryGetValue(locale, out configLink);
        
        private bool TryGetLocalePushUrl(string locale, out LinkWrapper configLink) =>
            _dataService.Wrapper.pushUrls.TryGetValue(locale, out configLink);
    }
}