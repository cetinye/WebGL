using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using Witmina_SweetMemory;


public class Bridge : MonoBehaviour
{
     public GameManager gm;

    #if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void SendResultToWebGL(string message);
#endif


    [Serializable]
    public class MyClass
    {
        public int id;
        public float time;
        public string name;
    }

    // public InputField TextInput;
    // public Text DisplayText;

    



    // Start is called before the first frame update
    void Start()
    {
        print("bridge çalıştı");

        string json = "{\"level\":2,\"locale\":\"en\"}";
        // SendToUnity(json);

#if !UNITY_EDITOR && UNITY_WEBGL
        WebGLInput.captureAllKeyboardInput = false;
#endif
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void SendToJS()
    {
        Debug.Log("click");
        //    string MessageToSend = TextInput.text;
        //   print(MessageToSend);
        // DisplayText.text = MessageToSend;

#if UNITY_WEBGL && !UNITY_EDITOR
        SendResultToWebGL("hacım naber");
        
#endif
    }


    public void SendToJSJson(string jsonToSend)
    {
        Debug.Log("sonuç json geldi     " + jsonToSend);
        //  string MessageToSend = TextInput.text;
        //print(MessageToSend);
        //DisplayText.text = MessageToSend;


        // string degiskenAdi = JsonUtility.ToJson(myObject);



#if UNITY_WEBGL && !UNITY_EDITOR
        SendResultToWebGL(jsonToSend);
        
#endif
    }


    [Serializable]
    struct StartPayload
    {
        public int level;
        public string locale;
    }

    public void SendToUnity(string json)
    {
        print("gelen");
        print(json);
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("Boş JSON.");
            return;
        }

        StartPayload p;
        try
        {
            p = JsonUtility.FromJson<StartPayload>(json);
        }
        catch (Exception e)
        {
            Debug.LogError("JSON parse hatası: " + e.Message);
            return;
        }

        var lvl = p.level;
        var loc = string.IsNullOrEmpty(p.locale) ? "en" : p.locale;

        Debug.Log("received locale: " + loc);
        gm.SetLanguage(loc);
        Debug.Log("received level: " + lvl);
        gm.StartFromWebGL(lvl);
    }
}