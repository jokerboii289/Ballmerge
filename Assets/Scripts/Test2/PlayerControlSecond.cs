using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlSecond : MonoBehaviour
{

    //spawn Differrent balls
    [SerializeField] GameObject prefabsOfBalls;

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
  

    private void Start()
    {
        showPath = false;
        points = new Vector3[3];
        lineRenderer = GetComponent<LineRenderer>();

        points[0] = transform.position;
        SpawnBalls();    //spawn ball at start of the game
    }
    // Update is called once per frame
    void Update()
    {
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

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            ShootBalls();
            transform.eulerAngles = Vector3.zero;
            showPath = false;
        }
        if (transform.childCount < 1)
        {
            SpawnBalls();  //ball spawn function
        }


        PointOnWallFirst();
        if (showPath)
        {
            lineRenderer.enabled = true;
            LineRenderer();
        }
        else
            lineRenderer.enabled = false;
    }

    void LineRenderer()
    {
        lineRenderer.positionCount = points.Length;
        for (int i = 0; i < points.Length; i++)
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
            DirectionVectorOnsurface = Vector3.Reflect(transform.forward, hit.normal);

            if (!hit.collider.CompareTag("noreflect"))
            {
                if (Physics.Raycast(secondPoint, DirectionVectorOnsurface, out RaycastHit hitTwo, distance))
                {
                    points[2] = hitTwo.point;
                }
            }
            else
            {
                points[2] = Vector3.zero; // if there is no third point when  raycast is done on no reflect area
            }
        }
    }

    void ShootBalls()
    {
        objectToShoot = transform.GetChild(0).gameObject;
        if (objectToShoot.transform.IsChildOf(transform))
        {
            Rigidbody rBody = objectToShoot.GetComponent<Rigidbody>();
            rBody.drag = 1f;
            rBody.angularDrag = 1f;
            rBody.AddForce(transform.forward * shootSpeed * Time.deltaTime, ForceMode.Impulse);
        }
    }

    void SpawnBalls()
    {
        var obj = ObjectPool.instance.GetFromPool(prefabsOfBalls);
        if (obj != null)
        {
            obj.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            obj.transform.SetParent(transform);
        }
    }
}
