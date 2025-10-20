using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.InteropServices;
using Customs_Scanner;


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
        Debug.Log("bridge çalıştı");
        SendToUnity(1);



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
        //    string MessageToSend = TextInput.text;
        //   print(MessageToSend);
        // DisplayText.text = MessageToSend;

#if UNITY_WEBGL && !UNITY_EDITOR
        
#endif
    }


    public void SendToJSJson(string jsonToSend)
    {

        Debug.Log("Sending JSON to WebGL: " + jsonToSend);
        //  string MessageToSend = TextInput.text;
        //print(MessageToSend);
        //DisplayText.text = MessageToSend;


        // string degiskenAdi = JsonUtility.ToJson(myObject);



#if UNITY_WEBGL && !UNITY_EDITOR
        SendResultToWebGL(jsonToSend);
        
#endif
    }


    public void SendToUnity(int levelId)
    {

        // gm.level = levelId;
        gm.StartFromWebGL(levelId);

        // JSONObject json = new JSONObject(message);


        //  StartCoroutine(gm.getRequest(json));

        //  string json = message.ToString();
        //     json = json.Replace("\\", "");
        //     json = json.Replace("[]", "");


        // // DisplayText.text = message;
        //  JsonDecode(json);
    }
}