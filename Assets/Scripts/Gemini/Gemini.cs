using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Gemini : MonoBehaviour
{
    public string deployId; // Inspectorから設定可能に
    public string questionText; // Inspectorから設定する質問テキスト


    void Start()
    {
        StartCoroutine(CallGoogleAppsScript());
    }

    IEnumerator CallGoogleAppsScript()
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
}
