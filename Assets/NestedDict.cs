using System;
using System.Collections;
using System.Collections.Generic;

// generic abuusse
[Serializable]
public class NestedDict<T1, T2>
{
    public List<DictItem<T1, T2>> items;
    public NestedDict()
    {

    }
    public T2 Get(T1 key)
    {
        foreach (DictItem<T1, T2> item in items)
        {
            if (item.key.Equals(key))
            {
                return item.value;
            }
        }
        return default(T2);
    }
    public void Add(T1 key, T2 value)
    {
        items.Add(new DictItem<T1, T2>(key, value));
    }
}
[Serializable]
public class DictItem<T1, T2>
{
    public T1 key;
    public T2 value;
    public DictItem(T1 _key, T2 _value)
    {
        key = _key;
        value = _value;
    }
}