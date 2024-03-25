using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GouguGameplay : MonoBehaviour
{
    public Text isApprovedTxt;

    IEnumerator Gameplay()
    {
        yield return new GridGameManager.WaitForGameEnds(0);
        Debug.Log(1);
    }

    private void Start()
    {
        StartCoroutine(Gameplay());
    }
}
