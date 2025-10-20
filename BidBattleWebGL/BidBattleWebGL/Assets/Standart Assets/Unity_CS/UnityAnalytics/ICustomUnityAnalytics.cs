using System.Collections.Generic;

namespace Unity_CS.UnityAnalytics
{
    public interface ICustomUnityAnalytics
    {
        void CustomEvent(string eventName, string parameterName);
        void CustomEvent(string eventName, int parameterValue);
        void CustomEvent(string eventName, float parameterValue);
        void CustomEvent(string eventName, double parameterValue);
        void CustomEvent(string eventName, bool isParameterDone);
        void CustomEvent(string eventName, Dictionary<string, object> eventsDictionary);
        void PushCustomEvents(string eventName, Dictionary<string, object> eventsDictionary = null );
    }
}