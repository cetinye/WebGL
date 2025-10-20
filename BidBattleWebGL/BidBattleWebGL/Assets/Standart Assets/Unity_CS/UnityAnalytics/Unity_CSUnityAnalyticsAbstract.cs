using System.Collections.Generic;

namespace Unity_CS.UnityAnalytics
{
    public abstract class Unity_CSUnityAnalyticsAbstract: ICustomUnityAnalytics
    {
        public abstract void CustomEvent(string eventName, string parameterName);
        public abstract void CustomEvent(string eventName, int parameterValue);
        public abstract void CustomEvent(string eventName, float parameterValue);
        public abstract void CustomEvent(string eventName, double parameterValue);
        public abstract void CustomEvent(string eventName, bool isParameterDone);
        public abstract void CustomEvent(string eventName, Dictionary<string, object> eventsDictionary);
        public abstract void PushCustomEvents(string eventName, Dictionary<string, object> eventsDictionary = null);
    }
}