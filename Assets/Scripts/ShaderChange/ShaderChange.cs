using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderChange : MonoBehaviour
{ 
    
    private void Start() {
        ChangeShader();
    }
    /// <summary>
    /// 切换游戏内物体材质为Line
    /// </summary>
    private void ChangeShader(){
        Material material = GetComponent<Renderer>().material;
        if (material.HasProperty("_BaseMap"))
        {
            Texture originalBaseMap = material.GetTexture("_BaseMap");
            Shader lineShader = Shader.Find("Shader Graphs/Line");
            material.shader = lineShader;
            material.SetTexture("_Base_Map", originalBaseMap);
            material.SetColor("_Base_Map_Color", Color.white);
        }
    }
}
