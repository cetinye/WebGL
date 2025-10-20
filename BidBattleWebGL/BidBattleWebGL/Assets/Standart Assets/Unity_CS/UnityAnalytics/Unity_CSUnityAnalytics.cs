using System.Collections;
using System.Collections.Generic;
using Unity_CS.UnityAnalytics;
using UnityEngine;
using UnityEngine.Analytics;

namespace Unity_CS.UnityAnalytics
{
    public class Unity_CSUnityAnalytics : Unity_CSUnityAnalyticsAbstract
{
    private readonly int unityAnalyticsDictionaryMaxSize = 10;
    public override void CustomEvent(string eventName, string parameterName)
    {
        var parameter = parameterName;
       PushCustomEvents(eventName+parameter);
    }

    public override void CustomEvent(string eventName, int parameterValue)
    {
        var parameter = parameterValue.ToString("N");
        PushCustomEvents(eventName+parameter);
    }

    public override void CustomEvent(string eventName, float parameterValue)
    {
        var parameter = parameterValue.ToString("F2");
        PushCustomEvents(eventName+parameter);
    }

    public override void CustomEvent(string eventName, double parameterValue)
    {
        var parameter = parameterValue.ToString("F6");
        PushCustomEvents(eventName+parameter);
    }

    public override void CustomEvent(string eventName, bool isParameterDone)
    {
        var parameter = isParameterDone.ToString();
        PushCustomEvents(eventName+parameter);
    }

    public override void CustomEvent(string eventName, Dictionary<string, object> eventsDictionary)
    {
        if (eventsDictionary.Count <= unityAnalyticsDictionaryMaxSize)
        {
            PushCustomEvents(eventName, eventsDictionary);
        }
        else
        {
            Debug.LogError($"Check the dictionary size you are pushing, it could not be bigger than {unityAnalyticsDictionaryMaxSize}");
        }
            
    }

    public override void PushCustomEvents(string eventMessage, Dictionary<string, object> eventsDictionary = null)
    {
        if (eventsDictionary != null)
        {
            var analyticsResult = Analytics.CustomEvent(eventMessage, eventsDictionary);
            // Debug.Log($"Unity Analytic event Results : {analyticsResult}");
        }
        else
        {
            var analyticsResult = Analytics.CustomEvent(eventMessage);
            // Debug.Log($"Unity Analytic event Results : {analyticsResult}");
        }
    }
}
}

