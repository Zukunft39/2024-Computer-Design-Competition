using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializeableDictionary<TKey,TValue> 
{
    [Serializable]
    public struct Element
    {
        public TKey _keys;
        public TValue _values;
    }

    public List<Element> dict=new();
    
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
