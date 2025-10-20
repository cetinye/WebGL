using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using Cashier;


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
        // SendToUnity(1);

    //   string asd = "{\"exam_data\":{\"examID\":1,\"gameID\":3,\"title\":\"ImagePuzzle\",\"totalQuestions\":9,\"questions\":{\"data\":[{\"id\":1,\"game_id\":3,\"image_path\":\"https://bilsemonline-v-3.fra1.cdn.digitaloceanspaces.com/photos/shares/game_questions/1/65ae3a5627fdc.png\",\"settings\":{\"part_count\":null},\"answers\":[]},{\"id\":2,\"game_id\":3,\"image_path\":\"https://bilsemonline-v-3.fra1.cdn.digitaloceanspaces.com/photos/shares/game_questions/2/65ae3a62aadfe.png\",\"settings\":{\"part_count\":null},\"answers\":[]},{\"id\":3,\"game_id\":3,\"image_path\":\"https://bilsemonline-v-3.fra1.cdn.digitaloceanspaces.com/photos/shares/game_questions/3/65ae3a6e95c04.png\",\"settings\":{\"part_count\":null},\"answers\":[]},{\"id\":4,\"game_id\":3,\"image_path\":\"https://bilsemonline-v-3.fra1.cdn.digitaloceanspaces.com/photos/shares/game_questions/4/65ae3aa655269.png\",\"settings\":{\"part_count\":null},\"answers\":[]},{\"id\":5,\"game_id\":3,\"image_path\":\"https://bilsemonline-v-3.fra1.cdn.digitaloceanspaces.com/photos/shares/game_questions/5/65ae3ab0c6a95.png\",\"settings\":{\"part_count\":null},\"answers\":[]},{\"id\":6,\"game_id\":3,\"image_path\":\"https://bilsemonline-v-3.fra1.cdn.digitaloceanspaces.com/photos/shares/game_questions/6/65ae3abb0748f.png\",\"settings\":{\"part_count\":null},\"answers\":[]},{\"id\":27,\"game_id\":3,\"image_path\":\"https://bilsemonline-v-3.fra1.cdn.digitaloceanspaces.com/photos/shares/game_questions/27/65ae3e6553621.png\",\"settings\":{\"part_count\":null},\"answers\":[]},{\"id\":28,\"game_id\":3,\"image_path\":\"https://bilsemonline-v-3.fra1.cdn.digitaloceanspaces.com/photos/shares/game_questions/28/65ae3e6fecca3.png\",\"settings\":{\"part_count\":null},\"answers\":[]},{\"id\":29,\"game_id\":3,\"image_path\":\"https://bilsemonline-v-3.fra1.cdn.digitaloceanspaces.com/photos/shares/game_questions/29/65ae3e7b88d19.png\",\"settings\":{\"part_count\":null},\"answers\":[]}]}},\"avatar\":\"https://bilsemonline-v-3.fra1.cdn.digitaloceanspaces.com/photos/shares/outfits/9/6512d2e796896.png\"}";
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


    public void SendToUnity(int levelId)
    {

        // gm.level = levelId;
         print("send geldi    " + levelId);
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