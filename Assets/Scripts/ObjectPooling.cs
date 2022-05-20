//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour     // for to use this object pool pass type of game Object to function "getfrompool"
{
    [SerializeField] GameObject[] prefab;
    
    private Dictionary<string, List<GameObject>> objectPool = new Dictionary<string, List<GameObject>>();

    [SerializeField] int limitOfPool;

    public static ObjectPooling instance;
    public static int noOfBallTypes;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }      
    }

    private void Start()
    {
        noOfBallTypes = prefab.Length;
        for (int i = 0; i < prefab.Length; i++)
        {
            CreateNewObject(prefab[i]);
        }
    }
    public GameObject GetFromPool(GameObject obj)
    {
        if (objectPool.TryGetValue(obj.tag, out List<GameObject> objectList))
        {
            
            if (objectList.Count==0)
            {
                CreateNewObject(obj);
            }
            else if(objectList.Count>0)
            {
                for (int i = 0; i < objectList.Count; i++)
                {
                    if (!objectList[i].activeInHierarchy)
                    {
                        objectList[i].SetActive(true);
                        return objectList[i];
                        break;
                    }
                }
            }
        }
        return null;
    }


    private void CreateNewObject(GameObject prefab)
    {
        for (int i = 0; i < limitOfPool; i++)
        {
            var obj= Instantiate(prefab);
            AddToPool(obj);
        }
    }

    public void AddToPool(GameObject obj)
    {
        if (objectPool.TryGetValue(obj.tag,out List<GameObject> objectList))
        {
            objectList.Add(obj);
        }
        else
        {
            List<GameObject> newobjectList = new List<GameObject>();
            newobjectList.Add(obj);
       
            objectPool.Add(obj.tag, newobjectList);  
        }
        obj.SetActive(false);
    }
}
