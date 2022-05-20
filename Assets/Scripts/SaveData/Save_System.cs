using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections;
using TMPro;
using UnityEngine.UI;


public class Save_System : MonoBehaviour
{

    public static Save_System instance;

    //list of types of balls
    public List<GameObject> typeofballs = new List<GameObject>();
    public GameObject ballprefab;
    
    public static List<Balls> balls = new List<Balls>();   
    const string BALL_FILE = "/ball";
    const string BALL_COUNT = "/ball.count";

    private void Awake() // original awake
    {
        
        instance = this;
        LoadBall();
    }
    
     private void OnApplicationQuit()
     {         
         SaveBall();
     }

    private void OnApplicationFocus(bool focus)
    {
         SaveBall();
    }

    void SaveBall()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + BALL_FILE + SceneManager.GetActiveScene().buildIndex;
        string countPath= Application.persistentDataPath + BALL_COUNT + SceneManager.GetActiveScene().buildIndex;

        FileStream countStream = new FileStream(countPath,FileMode.Create);

        formatter.Serialize(countStream, balls.Count);
        countStream.Close();

        for (int i=0;i<balls.Count;i++)
        {
            FileStream stream = new FileStream(path + i, FileMode.Create);
            BalllDatta data = new BalllDatta(balls[i]);

            formatter.Serialize(stream, data);
            stream.Close();
        }
    }

    void LoadBall()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + BALL_FILE + SceneManager.GetActiveScene().buildIndex;
        string countPath = Application.persistentDataPath + BALL_COUNT + SceneManager.GetActiveScene().buildIndex;
        int ballCount = 0;

        if(File.Exists(countPath))
        {
            FileStream countStream = new FileStream(countPath, FileMode.Open);

            ballCount = (int)formatter.Deserialize(countStream);
            countStream.Close();
        }
        else
        {
            Debug.LogError("path not found in" + countPath);
        }
       
        for (int i = 0; i < ballCount; i++)
        {
            if (File.Exists(path + i))
            {
                FileStream stream = new FileStream(path + i, FileMode.Open);
                BalllDatta data = formatter.Deserialize(stream) as BalllDatta;

                stream.Close();

                Vector3 position = new Vector3(data.position[0], data.position[1], data.position[2]);

                StartCoroutine(Delay(data,position));     
            }
            else
            {
                Debug.LogError("path not found in" + path+i);
            }           
        }
    }

    IEnumerator Delay(BalllDatta data,Vector3 position)
    {
        yield return new WaitForSeconds(0f);
        //instantiate balls
        for (int j = 0; j < typeofballs.Count; j++)
        {
            if (data.id == typeofballs[j].tag)
            {
                var obj = ObjectPooling.instance.GetFromPool(typeofballs[j]);
                
                if(obj!=null)
                {
                    Rigidbody rbody = obj.GetComponent<Rigidbody>();
                    obj.GetComponent<SphereCollider>().enabled=false;
                    rbody.useGravity = false;
                    rbody.drag = 100;
                    obj.transform.position = position;
                    
                    StartCoroutine(MakeBallNormal(rbody,obj));
                }
                SpawnBallsUponCollide.instance.ScoreSetFromSavedData(data.score,data.valueOfSlider,data.slidertempValue,data.colorball,data.bomb); // score value and slider value
                SpawnBallsUponCollide.instance.GetAlternate(data.alternate);
                break;
            }
        }
    }
    IEnumerator MakeBallNormal(Rigidbody rbody, GameObject obj)
    {
        yield return new WaitForSeconds(2f);
        obj.GetComponent<SphereCollider>().enabled = true;
        rbody.useGravity = true;
        rbody.drag = 1f;
    }

    public void ResetTheLevel()
    {
        File.Delete(Application.persistentDataPath + BALL_COUNT + SceneManager.GetActiveScene().buildIndex);
    }
}
