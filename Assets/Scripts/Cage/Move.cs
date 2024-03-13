using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public bool isMoving = true;

    [SerializeField] private string currentTag;
    [Tooltip("最长距离")]
    [SerializeField] private float maxDis;
    [Tooltip("速度")]
    [SerializeField] private float speed;
    private Vector3 dir;
    [SerializeField] private Vector3 initialPos;
    private void Start()
    {
        currentTag = transform.tag;
#if UNITY_EDITOR
        if (initialPos == null) Debug.LogError(initialPos + "is not exit");
#endif
    }
    private void Update()
    {
        Moving();
    }
    private void Moving()
    {
        if (currentTag == "Chicken") dir = Vector3.right;
        else if (currentTag == "Rabbit") dir = Vector3.left;
#if UNITY_EDITOR
        else Debug.LogError("No Tag:" + currentTag);
#endif
        if (Mathf.Abs(transform.position.x - initialPos.x) <= maxDis)
        {
            if(isMoving) transform.position += speed * Time.deltaTime * dir;
        }
        else isMoving = false;
    }
}
