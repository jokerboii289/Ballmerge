using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Balls : MonoBehaviour
{
    //count for fall line enter
    bool failLine;
    

    [SerializeField]
    string type;

    //for text
    [SerializeField] int ballValue;
    GameObject objText;
    float modifier;
    public static GameObject tempText;
    [SerializeField] Vector3 offset;
    int count = 0;

    Rigidbody rBody;

    Transform deadLine;//finish line

    //move ball towards same ball tag 
    GameObject[] ballOfSimilarColor;
    float previousDistance;
    List<GameObject> tempObjs;
    Vector3 directionHolder;

    //for ball to pop (player dead)
    float timerForBallpop;
    bool startDestroy;

    //for ball bulgeEffect
    float timer;
    float size;
    Vector3 originalTextSize;

    Transform player;

    // ball explosion marker
    int enterdestroyZoneCount;
    GameObject aimHolder;


    private void OnEnable()
    {
        aimHolder = null;
        failLine = false;
        // enterdestroyZoneCount = 0;
        count++;
        player = GameObject.FindGameObjectWithTag("Player").transform;

        size = transform.localScale.x;
        previousDistance = 100;
        timerForBallpop = 0;
        startDestroy = false;

        //   tempText = text;
        modifier = 0;
        deadLine = GameObject.FindGameObjectWithTag("failline").transform;
        if (count > 1)
        {
            objText = ObjectPooling.instance.GetFromPool(GeneralVariables.instance.ballText);
            if (objText != null)
            {
                objText.transform.SetPositionAndRotation(transform.position + offset, Quaternion.Euler(new Vector3(80, 0, 0)));
                objText.GetComponent<TextMeshPro>().text = ballValue.ToString();
                originalTextSize = objText.transform.localScale;
            }
        }
       

        rBody = gameObject.GetComponent<Rigidbody>();
        rBody.drag = 1f;
        rBody.angularDrag = 1f;

        StartCoroutine(StartMoving());

        //make the Ball Pop out
        StartCoroutine(PopOut());
    }

    private void Start()
    {
        if (gameObject.activeInHierarchy && !transform.IsChildOf(player))// checks it is active in hirarchy to add to the file
        {
            Save_System.balls.Add(this);
        }
    }

    private void OnDestroy()
    {
        Save_System.balls.Remove(this);
    }

    private void Update()
    {
        var distance = (deadLine.position - transform.position).magnitude;
        modifier = distance * GeneralVariables.instance.modifierValue;
        objText.transform.SetPositionAndRotation(transform.position + offset, Quaternion.Euler(new Vector3(80 - modifier, 0, 0)));


        //clamp y value so that the ball doesnt jump;
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -1.2f, 7.5f), transform.position.z);


        //explode ball if it is beyond failline
        if (!transform.IsChildOf(player) && startDestroy)
        {
            timerForBallpop += 1 * Time.deltaTime;
            if (timerForBallpop > 3f)   //timelimit
            {
                BallExplode();
            }
        }
        else if (!startDestroy)
        {
            timerForBallpop = 0;
        }

        BulgeEffect();

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(gameObject.tag))
        {
            var obj = ObjectPooling.instance.GetFromPool(GeneralVariables.instance.smokeParticleEffect);
            if (obj != null)
            {
                obj.transform.position = transform.position;
            }
            rBody.drag = 1000f;
            rBody.angularDrag = 1000f;
            SpawnBallsUponCollide.instance.Spawn(collision.gameObject);
            StartCoroutine(DelayDeactivate());
        }


        //ball hitting wall particleEffect
        var speed = GeneralVariables.instance.speedForParticle;
        if (Mathf.Abs(rBody.velocity.z) > speed || Mathf.Abs(rBody.velocity.x) > speed)
        {
            if (collision.gameObject.tag != ("platform"))
            {
                var obj = ObjectPooling.instance.GetFromPool(GeneralVariables.instance.particleEffect);
                if (obj != null)
                {
                    obj.transform.position = transform.position;
                }
                //sound of ball hit
                if (!collision.gameObject.CompareTag(gameObject.tag))
                    AudioManager.instance.Play("ballhit");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        enterdestroyZoneCount++;
        ///death marker
        if (other.gameObject.CompareTag("destroyball") && !transform.IsChildOf(player) && enterdestroyZoneCount > 1)
        {
            SpawnDeathMarker();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("destroyball") && !transform.IsChildOf(player))
            enterdestroyZoneCount++;

        if (other.gameObject.CompareTag("destroyball"))
        {
            if (!transform.IsChildOf(player))
            {
                SpawnBallsUponCollide.instance.ChangeColorOfFailLine();
            }
            startDestroy = true;
        }
        //death marker
        if (other.gameObject.CompareTag("destroyball") && !transform.IsChildOf(player) && enterdestroyZoneCount > 1)
        {
            if (aimHolder != null)
            {
                aimHolder.transform.position = transform.position;
                SizeDecrease();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("failline"))
        {
            transform.SetParent(null);
            if (!failLine)
            {
                Save_System.balls.Add(this); //save this ball in save system
                failLine = true;
            }
        }

        if (other.gameObject.CompareTag("destroyball"))
        {
            if (!transform.IsChildOf(player))
            {
                SpawnBallsUponCollide.instance.ReturnBackOriginalColor();
            }
            startDestroy = false;
        }
        //death marker
        if (other.gameObject.CompareTag("destroyball") && !transform.IsChildOf(player) && enterdestroyZoneCount > 1)
            DeactivateMarker();
    }

    IEnumerator DelayDeactivate()
    {
        RemoveTheObject();
        yield return new WaitForSeconds(0f);
        objText.transform.localScale = originalTextSize;
        ObjectPooling.instance.AddToPool(objText);
        transform.localScale = new Vector3(size, size, size);
       
        ObjectPooling.instance.AddToPool(gameObject);
    }

    public void DeactivateText()   //upon pressing bomb button
    {
        objText.transform.localScale = originalTextSize;
        ObjectPooling.instance.AddToPool(objText);
    }

    public void SimpleDeactivate() //when selecting bomb or color ball
    {
        transform.localScale = new Vector3(size, size, size);
        RemoveTheObject();
        ObjectPooling.instance.AddToPool(gameObject);
    }

    public void DeactivateBall()
    {
        var effect = ObjectPooling.instance.GetFromPool(GeneralVariables.instance.bombParticleEffect);
        if (effect != null)
        {
            effect.transform.position = transform.position;
        }
        RemoveTheObject();
        ObjectPooling.instance.AddToPool(gameObject);
    }

    void BallExplode() //ball exlpoding effect
    {
        DeactivateMarker();//death marker+

        var obj = ObjectPooling.instance.GetFromPool(GeneralVariables.instance.bombParticleEffect);
        if (obj != null)
        {
            obj.transform.position = transform.position;
            AudioManager.instance.Play("bombexplosion");  //sound explosion           
            
            PauseMenu.instance.RestartSecond();
        }
        DeactivateText();
        SpawnBallsUponCollide.instance.ReturnBackOriginalColor();
        RemoveTheObject();
        gameObject.SetActive(false);
    }

    private void MoveBallTowardsSimilarColor()//move ball to the similiar color when it is spawn 
    {
        tempObjs = new List<GameObject>();
        ballOfSimilarColor = GameObject.FindGameObjectsWithTag(gameObject.tag); //array of same objects
        foreach (GameObject ele in ballOfSimilarColor)
        {
            if (ele.GetInstanceID() != this.gameObject.GetInstanceID() && !ele.transform.IsChildOf(player))
            {
                tempObjs.Add(ele);
            }
        }

        foreach (GameObject obj in tempObjs)
        {
            var distanceVector = obj.transform.position - transform.position;
            if (distanceVector.magnitude < GeneralVariables.instance.thresoldDis && previousDistance > distanceVector.magnitude)
            {
                directionHolder = distanceVector.normalized;
            }
            previousDistance = distanceVector.magnitude;
        }
        rBody.AddForce(directionHolder * GeneralVariables.instance.force * Time.fixedDeltaTime, ForceMode.Impulse);
    }

    IEnumerator StartMoving()
    {
        yield return new WaitForSeconds(0f);
        MoveBallTowardsSimilarColor();
    }

    void BulgeEffect()
    {
        if (gameObject.transform.IsChildOf(player))
        {
            //bugle
            timer += 2 * Time.deltaTime;
            var scaler = Mathf.Abs(Mathf.Sin(timer));
            var modifier = scaler * .2f;
            transform.localScale = new Vector3(size + modifier, size + modifier, size + modifier);

            // is child 
            objText.transform.SetParent(this.transform);
        }
        else
        {
            transform.localScale = new Vector3(size, size, size);
            objText.transform.localScale = originalTextSize;
            objText.transform.SetParent(null);
        }
    }

    IEnumerator PopOut()
    {
        yield return new WaitForSeconds(0);
        if (!transform.IsChildOf(player))
        {
            transform.localScale = new Vector3(size + .5f, size + .5f, size + .5f);
        }
    }

    public string GetColour()
    {
        return type;
    }

    //death marker on the ball
    void SpawnDeathMarker()
    {
        aimHolder = Instantiate(GeneralVariables.instance.aim);
        aimHolder.transform.position = transform.position;
    }

    void DeactivateMarker()
    {
        //ObjectPooling.instance.AddToPool(aimHolder);
        aimHolder.SetActive(false);
    }

    void SizeDecrease() // marker size decrease
    {
        LeanTween.scale(aimHolder, Vector3.zero, 5f);
    }


    void RemoveTheObject()
    {
        Save_System.balls.Remove(this); // Remove this object from the savesystem
    }
}
