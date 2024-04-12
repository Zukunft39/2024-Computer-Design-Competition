using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GouguData : MonoBehaviourSingletonPersistent<GouguData>
{
    public float gridSize;
    public bool isDebug;
    public GouguLevelSO[] levels;
    public SerializeableDictionary<string,AudioClip> audioDict;
}
