using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class GouguGameplay : MonoBehaviour
{
    public PlayableDirector director;
    IEnumerator Gameplay()
    {
        for(int i=0;i<GouguData.Instance.levels.Length;i++)
            yield return new GridGameManager.WaitForGameEnds(i);
        director.Play();
        //TODO Switch Scene
        
    }

    private void Start()
    {
        StartCoroutine(Gameplay());
    }
}
