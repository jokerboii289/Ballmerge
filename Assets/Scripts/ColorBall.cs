using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorBall : MonoBehaviour
{
    [SerializeField] GameObject[] balls;

    private void Update()
    {
        if (!transform.IsChildOf(GameObject.FindGameObjectWithTag("Player").transform))
        {
            StartCoroutine(DelayDeactivate());
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag != "platform" || collision.gameObject.tag != "noreflect" || collision.gameObject.tag != "boundary")
        {
            var obj = collision.gameObject;
            for(int i=0;i<balls.Length;i++)
            {
                if(obj.gameObject.tag==balls[i].gameObject.tag)
                {
                    SpawnBallsUponCollide.instance.Spawnball(i+1,collision.transform.position);
                    obj.GetComponent<Balls>().DeactivateText();
                    ObjectPooling.instance.AddToPool(obj);
                    gameObject.SetActive(false);
                    break;
                }
            }
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
        gameObject.SetActive(false);
    }
}
