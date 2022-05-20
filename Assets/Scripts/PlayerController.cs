using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    //path tracer size of balls
    int index;

    //spawn Differrent balls
    [SerializeField] GameObject[] prefabsOfBalls;

    //shoot balls
    bool showPath;

    GameObject objectToShoot;
    [SerializeField] float shootSpeed;
  
    [SerializeField] float rotationSpeed;
    //line renderre
    LineRenderer lineRenderer;
    //second point distance
    Vector3 secondPoint;
    //for third point
    Vector3 DirectionVectorOnsurface;//vector direction opon hitting wall;
   
    Vector3[] points;
   
    [SerializeField] float distance;
    Touch touch;
    int previousValue=0;

    bool spawnBall;

    //bomb and color Ball
    bool bomb;
    bool colorBall;

    float screenHieght;

    //spam Interval
    float timer;

    
   

    private void Start()
    {
        timer = 1;
        Application.targetFrameRate = 120;
        QualitySettings.vSyncCount = 0;

        instance = this;
        bomb = false;
        colorBall = false;
        spawnBall = true;
        showPath = false;
        points = new Vector3[3];
        lineRenderer = GetComponent<LineRenderer>();
        
        points[0] = transform.position;
        SpawnRandomBalls();
        screenHieght = ((Screen.height*2)/3);
    }
 
    // Update is called once per frame
    void Update()
    {
        if (!PauseMenu.load)
        {
            timer += 1 * Time.deltaTime;
            transform.eulerAngles = new Vector3(0, ClampAngleTo(transform.eulerAngles.y, -51, 51), 0);

            if (Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    showPath = true;
                }
                if (Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + touch.deltaPosition.x * rotationSpeed * Time.deltaTime, 0);
                }
            }

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && screenHieght >= touch.position.y && timer>.5)
            {
                ShootBalls();
                transform.eulerAngles = Vector3.zero;
                showPath = false;
                timer = 0;
            }
            if (transform.childCount == 0 && timer > .5)
            {
                SpawnRandomBalls();
            }

            PointOnWallFirst();
            if (showPath && screenHieght >= touch.position.y && timer > .5)
            {
                lineRenderer.enabled = true;
                LineRenderer();
            }
            else
                lineRenderer.enabled = false;
        }
    }

   void LineRenderer()
    {
        var width = prefabsOfBalls[index].transform.localScale.x;
        lineRenderer.SetWidth(width, width);
        ChangePathTrackerColor();

        lineRenderer.positionCount = points.Length;
        for(int i=0;i<points.Length;i++)
        {
            lineRenderer.SetPosition(i, points[i]);
        }        
    }

    void PointOnWallFirst()  // point on wall
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, distance))
        {
            secondPoint = hit.point;
            points[1] = secondPoint;
            DirectionVectorOnsurface= Vector3.Reflect(transform.forward,hit.normal); 
            
            if(!hit.collider.CompareTag("noreflect"))
            {
                if (Physics.Raycast(secondPoint, DirectionVectorOnsurface, out RaycastHit hitTwo, distance))
                {
                      points[2] = hitTwo.point;
                }
            }     
            else
            {
                points[2] = points[1];
            }
        }
    }    
   
     void ShootBalls()
    {
        objectToShoot =transform.GetChild(0).gameObject;
        if(objectToShoot.transform.IsChildOf(transform))
        {
            Rigidbody rBody =objectToShoot.GetComponent<Rigidbody>();
            rBody.drag = 1f;
            rBody.angularDrag = 1f;
            rBody.AddForce(transform.forward * shootSpeed * Time.fixedDeltaTime, ForceMode.Impulse);
            //ball sound 
            AudioManager.instance.Play("ballrelease");
        } 
    }

    void SpawnRandomBalls()
    {
        if (spawnBall)
        {
            var rand = RandomValues();
            index = rand;
            var obj = ObjectPooling.instance.GetFromPool(prefabsOfBalls[rand]);
            if (obj != null)
            {
                var rBody = obj.GetComponent<Rigidbody>();
                rBody.drag = 100f; 
                rBody.angularDrag = 100f;
                obj.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
                obj.transform.SetParent(transform);
            }
        }
        else if (!spawnBall && bomb)
        {
            SpawnBomb();
        }
        else if(!spawnBall && colorBall)
        {
            SpawnColorBall();
        }
    }

    int RandomValues()
    {
        var rand = Random.Range(0, 6); //max value of five so value of small ball can be spawn 
        while(rand==previousValue)
        {
            RandomValues();
        }
        previousValue = rand;
        return rand;
    }

    void ChangePathTrackerColor()   //list of color for pathtracker
    {
        if(index==0 || index==1)
        {
            lineRenderer.SetColors(Color.green, Color.white - new Color(0, 0, 0, 1));
        }
        else if(index==2)
        {
            lineRenderer.SetColors(Color.yellow, Color.white - new Color(0, 0, 0, 1));
        }
        else if(index==3)
        {
            lineRenderer.SetColors(new Color(197, 0, 255, 1), Color.white - new Color(0, 0, 0, 1));
        }
        else if(index==4)
        {
            lineRenderer.SetColors(Color.blue, Color.white - new Color(0, 0, 0, 1));
        }
        else if (index == 5)
        {
            lineRenderer.SetColors(Color.red, Color.white - new Color(0, 0, 0, 1));
        }
    }

    public void Bomb()
    {
        //check the if bomb is already spawn
        if (!transform.GetChild(0).CompareTag("bomb") && SpawnBallsUponCollide.bombCounter>0)
        {
            var obj = transform.GetChild(0);
            obj.transform.SetParent(null);
            var component = obj.GetComponent<Balls>();
            component.DeactivateText();
            component.SimpleDeactivate();
            bomb = true;
            spawnBall = false;
            //bomb counter deplete
            SpawnBallsUponCollide.instance.DepleteCounterBomb();

        }
    }
    public void Reactivate()
    {
        bomb = false;
        colorBall = false;
        spawnBall = true;
    }

    void SpawnBomb()
    {
        var obj = ObjectPooling.instance.GetFromPool(GeneralVariables.instance.bomb);
        if(obj!=null)
        {
            var rBody = obj.GetComponent<Rigidbody>();
            rBody.drag = 100f;
            rBody.angularDrag = 100f;
            obj.transform.SetPositionAndRotation(transform.position, Quaternion.EulerAngles(new Vector3(45f,0,0)));
            obj.transform.SetParent(transform);
        }
    }   

    public void ColorBall()
    {
        if (!transform.GetChild(0).CompareTag("colorball") && SpawnBallsUponCollide.colorBallCounter>0)
        {
            var obj = transform.GetChild(0);
            obj.transform.SetParent(null);
            var component = obj.GetComponent<Balls>();
            component.DeactivateText();
            component.SimpleDeactivate();
            colorBall = true;
            spawnBall = false;
            //deplete counterball
            SpawnBallsUponCollide.instance.DepleteCounterColorBall();
        }
    }

    void SpawnColorBall()
    {
        var obj = ObjectPooling.instance.GetFromPool(GeneralVariables.instance.colorBall);
        if (obj != null)
        {
            var rBody = obj.GetComponent<Rigidbody>();
            rBody.drag = 100f;
            rBody.angularDrag = 100f;
            obj.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            obj.transform.SetParent(transform);
        }
    }


    //rotation clamp
    float ClampAngleTo(float angle, float from, float to)
    {
        if (angle < 0f) angle = 360 + angle;
        return angle > 180f ? Mathf.Max(angle, 360 + @from) : Mathf.Min(angle, to);
    }
}
