using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ObjectPooling.instance.GetFromPool(prefab);
        StartCoroutine(Delay());
    }
    IEnumerator Delay()
    {
        yield return new WaitForSeconds(4f);
        GameObject[] lol = GameObject.FindGameObjectsWithTag(prefab.tag);
        foreach(GameObject element in lol)
        {
            element.SetActive(false);
        }
    }
}
