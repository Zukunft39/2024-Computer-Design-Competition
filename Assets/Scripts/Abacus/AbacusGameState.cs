using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="AbacusData",menuName ="GamesData")]
public class AbacusGameState : ScriptableObject
{
    private int count = 0;
    public void Count(int count)
    {
        this.count = count;
    }
}
