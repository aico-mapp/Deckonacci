using System;

namespace Mode.Scripts.Data
{
    [Serializable]
    public class DataModel
    {
        public int status;
        public string projectName;
        public string pushStatus;
        public string configuration;
        public string conversionTag;
        public int loadingCounter;
        public string token;
        public string tokenEndpoint;
        public int tokenRefreshPeriod;
    }
}