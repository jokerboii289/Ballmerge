using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralVariables : MonoBehaviour
{
    public static GeneralVariables instance;
    public float modifierValue;//for ball text rotation


    //for bal text
    public GameObject ballText;

    //for ball Movement
    public float thresoldDis;//check radius for ball when new ball is spawned so it move towards the ball of same type
    public float ballSpeed;
    public float speedForParticle;  //speed above which the particle gets activate near the boundary
    public float force;//force for the ball to move
    
    //ball hit boundary particleeffect
    public GameObject particleEffect;
    public GameObject smokeParticleEffect;
    public GameObject bombParticleEffect;
    public GameObject colorSmoke;
  

    //for bomb
    public GameObject bomb;
    //for ColorBall
    public GameObject colorBall;
    //for shockwave particleEffect
    public GameObject purple;
    public GameObject red;
    public GameObject green;
    public GameObject blue;
    public GameObject orange;
    public GameObject pink;
    public GameObject violet;
    public GameObject yellow;
    public GameObject white;


    // for points -score
    public GameObject pointsText;
    public float textModifierValue;
    public float textUpSpeed;

    //for slider value
    public float sliderUpperLimit;

    //for comboText
    public float fadeOutSpeed;
    public float scaleUpBy;


    //for deathmark
    public GameObject aim;

   
    private void Start()
    {
        instance = this;
    }
}
