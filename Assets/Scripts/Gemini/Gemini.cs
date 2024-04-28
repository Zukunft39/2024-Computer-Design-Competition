using System;
using System.Collections;
using System.Collections.Generic;
using Proyecto26;
using UnityEngine;
using UnityEngine.Networking;

public class Gemini : MonoBehaviour
{
    public string deployId;
    public string apiKey;
    public string questionText;

    private string url = "";

    void Start()
    {
        StartCoroutine(CallAIScriptbtRestAPI());
    }

    IEnumerator CallAIScriptbyGAS()
    {
        yield break;
    }

    IEnumerator CallAIScriptbtRestAPI()
    {
        yield break;
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
}