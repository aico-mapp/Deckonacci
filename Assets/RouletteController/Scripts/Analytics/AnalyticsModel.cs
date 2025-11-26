using System;
using System.Collections.Generic;

namespace Mode.Scripts.Analytics
{
    public class AnalyticsModel
    {
        public readonly List<string> DeferredSteps = new(3);
        public readonly Dictionary<string, AnalyticsStep> Steps = new();
        public readonly AnalyticsData AnalyticsData = new AnalyticsData 
            { analyticsEventProperties = new AnalyticsEventPropertiesData { standartData = new StandartData() } };
        public readonly Dictionary<AnalyticsStepType, string> StepNames = new()
        {
            [AnalyticsStepType.TotalLoading] = "Total Loading Time",
            [AnalyticsStepType.AppsFlyerInitialize] = "AppsFlyerData Loading Time",
            [AnalyticsStepType.ConfigurationLoading] = "Remote AppConfiguration Loading Time",
            [AnalyticsStepType.ProjectLoading] = "Site Loading Time",
            [AnalyticsStepType.ConnectionLost] = "Offline Period Time"
        };
        
        public string Chain { get; } = Guid.NewGuid().ToString();
        public string Location;
        public string Common;
        public int LoadingNumber;

        public void AddStep(string stepName)
        {
            Steps.Add(stepName, new AnalyticsStep(stepName, Chain, LoadingNumber));
        }
        
        public AnalyticsStep GetStepWithRemove(AnalyticsStepType stepType)
        {
            var stepKey = StepNames[stepType];
            var analyticsStep = Steps[stepKey];
            Steps.Remove(stepKey);
            return analyticsStep;
        }
    }
}