using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour{

    // Use this for initialization

    public List<GameObject> list_pfObject;
    protected Dictionary<string, List<GameObject>> m_dicObject=new Dictionary<string, List<GameObject>>();
    //protected List<GameObject> m_listGameObject=new List<GameObject>();
    void Awake()
    {
        foreach(var key in list_pfObject)
        {
            m_dicObject.Add(key.name, new List<GameObject>());
        }
    }
    virtual public void InitObj(GameObject obj)
    {
            obj.transform.SetParent(transform);
            obj.SetActive(false);
    }
    virtual protected GameObject Find(string strObject)
    {
        foreach(var v in list_pfObject)
        {
            if (v.name== strObject)
                return v;
        }
        return Add(strObject);
    }
    public GameObject Add(string strObject)
    {
        GameObject obj = Resources.Load<GameObject>(strObject);
        if (obj != null)
        {
            list_pfObject.Add(obj);
            m_dicObject.Add(strObject, new List<GameObject>());
        }
            
        return obj;
    }
    public void Generate(string strObject, int num)
    {
        GameObject Key = Find(strObject);
        if (Key != null)
        {
            for (int i = 0; i < num; i++)
            {
                GameObject obj = GameObject.Instantiate<GameObject>(Key);
                InitObj(obj);
                m_dicObject[strObject].Add(obj);
            }
        }       
    }
    virtual public void SetAllActive(bool b)
    {
        foreach (var key in m_dicObject)
        {
            foreach(var v in key.Value)
            {
                v.gameObject.SetActive(b);
            }
        }
    }
    virtual public GameObject GetObject(string strObject)
    {
        if(m_dicObject.ContainsKey(strObject))
        {
            foreach (var v in m_dicObject[strObject])
            {
                if (v.activeSelf == false)
                    return v;
            }
        }
        return null;
    }

}
