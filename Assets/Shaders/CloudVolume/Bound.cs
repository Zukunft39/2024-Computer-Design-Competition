using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bound : MonoBehaviour
{
    // Start is called before the first frame update
     Material material;
    void Start()
    {
        Renderer render = this.GetComponent<Renderer>();

        material = render.material;
    }

    // Update is called once per frame
    void Update()
    {
        material.SetVector("_BoundMin", transform.position - (transform.localScale / 2));
        material.SetVector("_BoundMax", transform.position + (transform.localScale / 2));
       
    }
}
