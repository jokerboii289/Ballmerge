using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BallSecond : MonoBehaviour
{
    [SerializeField]GameObject[] typesOfBalls;
    [SerializeField] Material[] typesOfMaterial;
  
    int previousValue;

    Renderer materials;
    int randValue;
    //for text on the ball
    GameObject objText;
    float modifier;

    Transform deadLine;//finish line
    float ballValue;                  //value of the ball
    [SerializeField] GameObject text;
    [SerializeField] Vector3 offset;
    [SerializeField]int materialIndex;



    private void OnEnable()
    {
        modifier = 0;

        randValue = RandomValues();
        materialIndex = randValue;
        var size= typesOfBalls[randValue].transform.localScale;
        transform.localScale = size;
        materials = GetComponent<Renderer>();
        materials.material = typesOfMaterial[randValue];

        ballValue = Mathf.Pow(2,randValue+1);

        deadLine = GameObject.FindGameObjectWithTag("failline").transform;

        objText = ObjectPool.instance.GetFromPool(text);
        if (objText != null)
        {
            objText.transform.SetPositionAndRotation(transform.position + offset, Quaternion.Euler(new Vector3(75, 0, 0)));
            objText.GetComponent<TextMeshPro>().text = ballValue.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        var distance = (deadLine.position - transform.position).magnitude;
        modifier = distance * GeneralVariables.instance.modifierValue;
        objText.transform.SetPositionAndRotation(transform.position + offset, Quaternion.Euler(new Vector3(75 - modifier, 0, 0)));

        //clamp y value so that the ball doesnt jump;
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -4f, 1.2f), transform.position.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(gameObject.tag))
        {
           /* if (materialIndex == collision.gameObject.GetComponent<BallSecond>().materialIndex)
            {
                ChangeBall();
                StartCoroutine(DelayDeactivate(collision.gameObject));
            }*/
            ChangeBall();
            StartCoroutine(DelayDeactivate(collision.gameObject));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("failline"))
        {
            transform.SetParent(null);
        }
    }

    int RandomValues()
    {
        var rand = Random.Range(0, 6); //max value of five so value of small ball can be spawn 
        while (rand == previousValue)
        {
            RandomValues();
        }
        previousValue = rand;
        return rand;
    }

    IEnumerator DelayDeactivate(GameObject obj)
    {
        yield return new WaitForSeconds(0f);

        var rBody = gameObject.GetComponent<Rigidbody>();
        rBody.drag = 100f;
        rBody.angularDrag = 100f;
        materialIndex++;
        if (transform.GetInstanceID() > obj.GetInstanceID())
        {
            ObjectPooling.instance.AddToPool(objText);
            ObjectPooling.instance.AddToPool(gameObject);
        }
    }

    void ChangeBall()  // change material color , size etc 
    {
        objText.GetComponent<TextMeshPro>().text = Mathf.Pow(2, randValue + 2).ToString();
        materials.material = typesOfMaterial[randValue+1];
    }


}
