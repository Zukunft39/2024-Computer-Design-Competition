using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 可序列化的字典
/// </summary>
/// <typeparam name="TKey">键类型</typeparam>
/// <typeparam name="TValue">值类型</typeparam>
[Serializable]
public class SerializeableDictionary<TKey,TValue> 
{
    // 键值对
    [Serializable]
    public struct Element
    {
        public TKey _keys;
        public TValue _values;
    }
    
    // 字典
    public List<Element> dict=new();
    
    // 索引器
    public (TKey,TValue) this[int index]
    {
        get
        {
            return (dict[index]._keys,dict[index]._values);
        }
        set
        {
            dict[index] = new Element
            {
                _keys = value.Item1,
                _values = value.Item2
            };
        }
    }
    
    // 索引器重载
    public TValue this[TKey key]
    {
        get
        {
            return GetValueByKey(key);
        }
        set
        {
            SetValueByKey(key,value);
        }
    }
    
    // 根据键获取值
    public TValue GetValueByKey(TKey key)
    {
        foreach (var element in dict)
        {
            if (element._keys.Equals(key))
            {
                return element._values;
            }
        }
        Debug.LogError("Key not found!");
        return default;
    }
    
    // 根据键设置值
    public void SetValueByKey(TKey key,TValue value)
    {
        for (int i = 0; i < dict.Count; i++)
        {
            if (dict[i]._keys.Equals(key))
            {
                dict[i] = new Element
                {
                    _keys = key,
                    _values = value
                };
                return;
            }
        }
        dict.Add(new Element
        {
            _keys = key,
            _values = value
        });
    }
}
