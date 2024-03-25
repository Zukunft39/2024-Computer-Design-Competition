using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GouguData : MonoBehaviourSingleton<GouguData>
{
    public float gridSize;
    public bool isDebug;
    public GouguLevelSO[] levels;
}
