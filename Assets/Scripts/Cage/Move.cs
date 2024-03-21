using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public bool isMoving = true;
    public float jumpTimer = 0;

    [SerializeField] private string currentTag;
    [Tooltip("最长距离")]
    [SerializeField] private float maxDis;
    [Tooltip("速度")]
    [SerializeField] private float speed;
    private Vector3 dir;
    [SerializeField] private Transform initialPos;
    [SerializeField] private int type;//1-chicken，-1-rabbit

    ObjectPooler pooler;
    CageManager manager;
    private void Start()
    {
        Init();
#if UNITY_EDITOR
        Debug.Log(currentTag + "方向:" + dir.ToString());
#endif
    }
    private void Init()
    {
        pooler = ObjectPooler.Instance;
        manager = FindObjectOfType<CageManager>();
        currentTag = transform.tag;
        initialPos = GameObject.Find(currentTag).GetComponent<Transform>();
#if UNITY_EDITOR
        if (initialPos == null) Debug.LogError(initialPos + "is not exit");
        if (manager == null) Debug.LogError("No CageManager!");
#endif

        if (currentTag == "Chicken")
        {
            dir = Vector3.right;
            type = 1;
        }
        else if (currentTag == "Rabbit")
        {
            dir = Vector3.left;
            type = -1;
        }
#if UNITY_EDITOR
        else
        {
            Debug.LogError("No Tag:" + currentTag);
            type = 0;
        }
#endif
    }
    private void Update()
    {
        #region 计时器
        if (!manager.GetPause())
        {
            jumpTimer += Time.deltaTime;
        }
        #endregion

        if (type == 1) Moving();
        else if (type == -1) Jump();
#if UNITY_EDITOR
        else Debug.LogError("No This Type(" + type + ")!");
#endif
    }

    private void Jump()
    {
        #region 移动
        if (Mathf.Abs(transform.localPosition.x) <= maxDis)
        {
            if (jumpTimer <= 2.5f)
            {
                isMoving = false;
            }else if (jumpTimer > 2.5f && jumpTimer < 4.5f)
            {
                isMoving = true;
            }else if (jumpTimer >= 4.5f)
            {
                jumpTimer = 0;
            }

            if (isMoving && !manager.GetPause())
            {
                float horizontal = speed * Time.deltaTime * dir.x;
                float vertical = Time.deltaTime * (-jumpTimer + 3.5f);
                transform.position += new Vector3(horizontal, vertical, 0f);
            }
        }
        else
        {
            if (gameObject.activeSelf) pooler.Recover(gameObject, transform.tag);
        }
        #endregion
    }

    private void Moving()
    {
        
        if (Mathf.Abs(transform.localPosition.x) <= maxDis)
        {
            if (isMoving && !manager.GetPause()) transform.position += speed * Time.deltaTime * dir;
        }
        else
        {
            //isMoving = false;
            if (gameObject.activeSelf) pooler.Recover(gameObject, transform.tag);
        }
    }
    public Transform GetTransform()
    {
        return initialPos;
    }
}
