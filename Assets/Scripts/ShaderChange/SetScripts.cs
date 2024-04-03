using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetScripts : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() 
    {
        foreach (Transform child in transform){
            foreach(Transform grandChild in child){
                grandChild.gameObject.AddComponent<ShaderChange>();
            }     
        }
    }
}
