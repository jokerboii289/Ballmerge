using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateBall : MonoBehaviour
{
    [SerializeField]
    GameObject ballsPrefab;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(0f);
        var obj = ObjectPooling.instance.GetFromPool(ballsPrefab);
        if(obj!=null)
        {
            obj.transform.position = transform.position;
        }
    }
}
