using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    #region 池初始工作
    [Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        [Tooltip("池子大小")] public int size;
        [Tooltip("父对象")] public GameObject parent;
    }
    public List<Pool> pools;
    Dictionary<string, Queue<GameObject>> poolDic;

    #region 单例
    public static ObjectPooler Instance;
    private void Awake()
    {
        Instance = this; 
    }
    #endregion

    #endregion
    //初始化
    private void Start()
    {
        Init();
    }
    public void Init()
    {
        poolDic = new Dictionary<string, Queue<GameObject>>();
        foreach (var pool in pools)
        {
            //用于给每个pool一个队列存储
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.SetParent(pool.parent.transform, false);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            //用于通过tag快速查找
            poolDic.Add(pool.tag, objectPool);
        }
    }
    #region 获取
    //只有tag
    public GameObject GetSpawnObj(string tag)
    {
#if UNITY_EDITOR
        if (!poolDic.ContainsKey(tag))
        {
            Debug.LogError("No Obj With the Tag(" + tag + ") in the Dictionary");
            return null;
        }
#endif
        GameObject gameObject = poolDic[tag].Dequeue();
        gameObject.transform.rotation = Quaternion.identity;
        if (!gameObject.activeSelf) gameObject.SetActive(true);
#if UNITY_EDITOR
        else Debug.LogWarning(gameObject + "should not be active");
        Debug.Log(gameObject.name);
#endif

        //直接入池
        //poolDic[tag].Enqueue(gameObject);
        return gameObject;
    }

    #endregion
    #region 销毁，用于特殊情况手动销毁
    public void Recover(GameObject gameObject,string tag) 
    {
        gameObject.SetActive(false);
        if (!gameObject.GetComponent<Collider2D>().enabled) gameObject.GetComponent<Collider2D>().enabled = true;
        if (poolDic.ContainsKey(tag))
        {
            poolDic[tag].Enqueue(gameObject);
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localScale = Vector3.one;
            //Debug.Log("Name:" + tag + "    " + poolDic[tag].Count);
        }
#if UNITY_EDITOR
        else
        {
            Debug.LogError("No Obj ("+gameObject+")With the Tag(" + tag + ") in the Dictionary");
        }
#endif
    }
    #endregion
}
