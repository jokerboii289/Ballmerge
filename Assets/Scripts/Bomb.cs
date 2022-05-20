using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    Transform player;
    // Start is called before the first frame update
    void Start()
    {
        player= GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(!transform.IsChildOf(player))
        {
            StartCoroutine(DelayDeactivate());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //info about type of ball it hit
        if (collision.gameObject.tag !="platform" || collision.gameObject.tag != "noreflect" || collision.gameObject.tag != "boundary")
        {
            GameObject[] obj = GameObject.FindGameObjectsWithTag(collision.gameObject.tag);
            foreach(GameObject x in obj)
            {
                if (!x.transform.IsChildOf(player))
                {
                    var ele = x.GetComponent<Balls>();
                    ele.DeactivateText();
                    ele.DeactivateBall();
                }
            }
            AudioManager.instance.Play("bombexplosion");  //sound explosion
            ObjectPooling.instance.AddToPool(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("failline"))
        {
            transform.SetParent(null);
            PlayerController.instance.Reactivate();
        }
    }

    IEnumerator DelayDeactivate()
    {
        yield return new WaitForSeconds(2.5f);
        var effect = ObjectPooling.instance.GetFromPool(GeneralVariables.instance.bombParticleEffect);
        if (effect != null)
        {
            effect.transform.position = transform.position;
        }
        AudioManager.instance.Play("bombexplosion");  //sound explosion
        gameObject.SetActive(false);
    }

}
