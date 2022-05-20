using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextComboHighlight : MonoBehaviour
{
    int count;
    float offset;
    RawImage image;
    float temp;
    private void OnEnable()
    {
        count++;
        temp = 0;
        offset = GeneralVariables.instance.scaleUpBy;
        transform.localScale = Vector3.zero;

        if(count>1)
            gameObject.GetComponent<RawImage>().color= image.color = new Color(255, 255, 255, 1);
    }

    private void Start()
    {
        image = gameObject.GetComponent<RawImage>();
    }
    // Update is called once per frame
    void Update()
    {
        LeanTween.scale(gameObject, new Vector3(offset, offset, offset), .5f);
        if(Mathf.Abs(transform.localScale.x-offset)<0.001f)
        {
            FadeOut();
        }
    
        StartCoroutine(Disable());
    }
    IEnumerator Disable()
    {
        yield return new WaitForSeconds(GeneralVariables.instance.fadeOutSpeed);
        gameObject.SetActive(false);
    }
    void FadeOut()
    {
        temp += Time.deltaTime;
        image.color = new Color(255,255,255, 1-temp);
    }
}
