using System;
using System.Collections;
using System.Collections.Generic;
using Proyecto26;
using UnityEngine;
using UnityEngine.Networking;

public class Gemini : MonoBehaviour
{
    public string deployId; // Inspectorから設定可能に
    public string apiKey; // Inspectorから設定可能に
    public string questionText; // Inspectorから設定する質問テキスト
    
    private string url="https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent?key=";
    
    void Start()
    {
        //StartCoroutine(CallAIScriptbyGAS());
        
        // Debug.Log(JsonUtility.ToJson(requestPayload));
        // RestClient.Post<AnswerPayload>( url+ apiKey,requestPayload).Then(response =>
        // {
        //     Debug.Log("レスポンス: " + response.GetAnswer());
        // }).Catch(error =>
        // {
        //     Debug.LogError("エラー: " + error.Message);
        // });
        StartCoroutine(CallAIScriptbtRestAPI());
    }

    IEnumerator CallAIScriptbyGAS()
    {
        string url = $"https://script.google.com/macros/s/{deployId}/exec?question=" + UnityWebRequest.EscapeURL(questionText);
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("エラー: " + request.error);
        }
        else
        {
            Debug.Log("レスポンス: " + request.downloadHandler.text);
        }
    }

    IEnumerator CallAIScriptbtRestAPI()
    {
        AnswerPayload.Candidate.RequestPayload requestPayload = new ("123");
        string msg=JsonUtility.ToJson(requestPayload);
        Debug.Log(msg);
        UnityWebRequest www = UnityWebRequest.PostWwwForm(url+apiKey,msg);
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("エラー: " + www.error);
        }
        else
        {
            Debug.Log("レスポンス: " + www.downloadHandler.text);
        }
    }
}
[Serializable]
public class AnswerPayload
{
    public string GetAnswer()
    {
        return candidates[0].content.parts[0].text;
    }
    
    [Serializable]
    public class Candidate
    {
        [Serializable]
        public class Content
        {
            [Serializable]
            public class Part
            {
                public string text;
            }
            public Part[] parts;
        }

        public class RequestPayload
        {
            public RequestPayload(string questionText)
            {
                contents = new Content[1];
                contents[0] = new Content();
                contents[0].parts = new Content.Part[1];
                contents[0].parts[0] = new Content.Part();
                contents[0].parts[0].text = questionText;
            }
            public Content[] contents;
        }

        public Content content;
        public string finfishReason;
        public int index;
    }
    public Candidate[] candidates;
}