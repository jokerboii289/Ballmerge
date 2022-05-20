using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextAnimation : MonoBehaviour
{

    Transform playerPos;
    float modifier;
    float textSpeed;
    float alpha;
    int count = 0;
    
    private void OnEnable()
    {
        count++;
        if (count > 1)
        {
            alpha = 255;
            playerPos = GameObject.FindGameObjectWithTag("Player").transform;
            modifier = GeneralVariables.instance.textModifierValue;
            textSpeed = GeneralVariables.instance.textUpSpeed;
        }
    }


    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * textSpeed * Time.deltaTime;
        var offsetY =Mathf.Abs(playerPos.position.y - transform.position.y);

        //for change aplha of text
        alpha -=offsetY *modifier * Time.deltaTime;
        gameObject.GetComponent<TextMeshPro>().color = new Color(255,255,255,alpha);

        StartCoroutine(DestroyText());

    }
    IEnumerator DestroyText()
    {
        yield return new WaitForSeconds(3f);
        ObjectPooling.instance.AddToPool(gameObject);
    }
}
