 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SpawnBallsUponCollide : MonoBehaviour
{

    
    public static SpawnBallsUponCollide instance;
    int count;
    [SerializeField] GameObject[] balls;

    //for change of fail line
    [SerializeField] Material material;
    [SerializeField]SpriteRenderer render;
    Material tempMaterialHolder;

    //score system
    Dictionary<int, float> comboPoints = new Dictionary<int, float>();
    public static float score;
    [SerializeField]TextMeshProUGUI scoreDisplay;


    //combo counter
    float counter;
    bool countTrigger;
    int countCombo;
    [SerializeField] float timer; //time limit between each combo

    //bomb and colorball counter
    public static int bombCounter;
    public static int colorBallCounter;

    //slider value
    float sliderValue;
    [SerializeField] Slider slider;
    [SerializeField]
    TextMeshProUGUI bombCounterText;
    public static bool alternate;
    [SerializeField] TextMeshProUGUI colorBallCounterText;
    float modifier;
    //slider image
    [SerializeField] GameObject bomb;
    [SerializeField] GameObject colorball;

    //combo text
    [SerializeField] GameObject combo2;
    [SerializeField] GameObject combo3;
    [SerializeField] GameObject combo4;
    [SerializeField] GameObject combo5;

    //highscore value 
    public static float highScore=0;


       
    private void Start()
    {
        bomb.SetActive(true);
        colorball.SetActive(false);
        modifier = 0;
        alternate = false;
        sliderValue = 0;
        bombCounter = 0;
        colorBallCounter = 0;

        countTrigger = false;
        countCombo = 0;


        score = 0;
        instance = this;
        count = 2;

        tempMaterialHolder = render.material;

        for(int i=0;i<balls.Length;i++)
        {
            comboPoints.Add(i, i * 20);  //store score in dictionary
        }
        //slider
        slider.minValue = 0;
        slider.maxValue = GeneralVariables.instance.sliderUpperLimit;
        
    }

    private void Update()
    {
        if(countTrigger)
        {
            counter += 1 * Time.deltaTime;  //time limit between each combo
            if(counter>timer)
            {
                counter = 0;
                countCombo = 0;
                countTrigger = false;
            }
        }
        // combo counter
        ComboCounter();


        //slider display
        if (slider.value < sliderValue)
        {
            modifier += .5f * Time.deltaTime;   // smooth transition of slider
            slider.value += modifier;
        }
    }
    public void Spawn(GameObject ball)
    {
        Extend(ball);
    }

    void Extend(GameObject ball)
    {
        count++;
        if (count % 2 == 0)
        {
            for (int i = 0; i < balls.Length; i++)
            {
                if (ball.tag == balls[i].tag)
                {
                    StartCoroutine(Delay(ball,i+1));                  
                    break;
                }
            }
        }
    }

    IEnumerator Delay(GameObject ball,int index)  // spawn ball when hit with the same color ball
    {
        CalculateCombo(index, ball.transform.position);
        yield return new WaitForSeconds(.1f);
        var obj = ObjectPooling.instance.GetFromPool(balls[index]);    //shows error on last ball
        if (obj != null)
        {
            obj.transform.position = ball.transform.position;
            //ball sound 
            AudioManager.instance.Play("ballSpawn");
            //vibrate
            Vibration.Vibrate(30);
            //shockwave particle effect
            SpawnShockWave(GetBallColor(obj), obj.transform.position);
        }
    }    

    public void Spawnball(int index, Vector3 position)//Spawn ball after the collison of colorball with regular ball
    {
        var obj = ObjectPooling.instance.GetFromPool(balls[index]);    //shows error on last ball
        if (obj != null)
        {
            var effect = ObjectPooling.instance.GetFromPool(GeneralVariables.instance.colorSmoke);
            if(effect!=null)
            {
                effect.transform.position = position;
            }
            obj.transform.position = position;
            //ball sound 
            AudioManager.instance.Play("ballSpawn");
            AudioManager.instance.Play("colorBallupgrade");  
            //shockwave particle effect
            SpawnShockWave(GetBallColor(obj),obj.transform.position);
        }
    }

    public void ChangeColorOfFailLine()
    {
        render.material = material;
    }

    public void ReturnBackOriginalColor()
    {
        render.material = tempMaterialHolder;
    }


    void CalculateCombo(int index,Vector3 position)
    {
        if(comboPoints.TryGetValue(index,out float value))
        {
            //for comboPoint
            var textPoints = ObjectPooling.instance.GetFromPool(GeneralVariables.instance.pointsText);
            if(textPoints!=null)
            {
                textPoints.GetComponent<TextMeshPro>().text = "+"+value.ToString();
                textPoints.transform.SetPositionAndRotation(position,Quaternion.EulerAngles(new Vector3(0,0,0)));
            }
            score += value;
            scoreDisplay.text = score+" "+"pts".ToString();
            countTrigger = true;
            countCombo++;
            //pass high score value
            highScore = score;

            //slider value 
            sliderValue += value;
            if (sliderValue >= slider.maxValue)
            {
                if (alternate)
                {
                    SpawnColorBall();
                }
                else
                {
                    SpawnBomb();
                }
                sliderValue = 0;
                slider.value = 0;
            }
        }       
    }


    string GetBallColor(GameObject obj)
    {
       return obj.GetComponent<Balls>().GetColour();
    }
    void SpawnShockWave(string color,Vector3 ballPos)
    {
        switch(color)
        {
            case "purple":
                var obj = ObjectPooling.instance.GetFromPool(GeneralVariables.instance.purple);
                if(obj!=null)
                {
                    obj.transform.position=ballPos;
                }
                break;
            case "green":
                var obj1 = ObjectPooling.instance.GetFromPool(GeneralVariables.instance.green);
                if (obj1 != null)
                {
                    obj1.transform.position = ballPos;
                }
                break;
            case "orange":
                var obj2 = ObjectPooling.instance.GetFromPool(GeneralVariables.instance.orange);
                if (obj2 != null)
                {
                    obj2.transform.position = ballPos;
                }
                break;
            case "violet":
                var obj3 = ObjectPooling.instance.GetFromPool(GeneralVariables.instance.violet);
                if (obj3 != null)
                {
                    obj3.transform.position = ballPos;
                }
                break;
            case "red":
                var obj4 = ObjectPooling.instance.GetFromPool(GeneralVariables.instance.red);
                if (obj4 != null)
                {
                    obj4.transform.position = ballPos;
                }
                break;
            case "yellow":
                var obj5 = ObjectPooling.instance.GetFromPool(GeneralVariables.instance.yellow);
                if (obj5 != null)
                {
                    obj5.transform.position = ballPos;
                }
                break;
            case "pink":
                var obj6 = ObjectPooling.instance.GetFromPool(GeneralVariables.instance.pink);
                if (obj6 != null)
                {
                    obj6.transform.position = ballPos;
                }
                break;
            case "blue":
                var obj7 = ObjectPooling.instance.GetFromPool(GeneralVariables.instance.blue);
                if (obj7 != null)
                {
                    obj7.transform.position = ballPos;
                }
                break;
            case "white":
                var obj8 = ObjectPooling.instance.GetFromPool(GeneralVariables.instance.white);
                if (obj8 != null)
                {
                    obj8.transform.position = ballPos;
                }
                break;
        }
    }
    
    void SpawnBomb()
    {
        bombCounter++;
        bombCounterText.text = bombCounter.ToString();
        alternate = true;
        bomb.SetActive(false);
        colorball.SetActive(true);  //image change of slider
    }
    void SpawnColorBall()
    {
        colorBallCounter++;
        colorBallCounterText.text = colorBallCounter.ToString();
        alternate = false;
        bomb.SetActive(true);
        colorball.SetActive(false);// image change of slider 
    }
    public void DepleteCounterBomb()
    {
        bombCounter--;
        bombCounterText.text = bombCounter.ToString();
    }
    public void DepleteCounterColorBall()
    {
        colorBallCounter--;
        colorBallCounterText.text = colorBallCounter.ToString();
    }

    void ComboCounter()
    {
        if(countCombo==2)
        {
            combo2.SetActive(true);
        }
        if (countCombo == 3)
        {
            combo3.SetActive(true);
        }
        if (countCombo == 4)
        {
            combo4.SetActive(true);
        }
        if (countCombo == 5)
        {
            combo5.SetActive(true);
        }
    }

    public void ScoreSetFromSavedData(float value, float sliderValuee,float slidertemp,int color,int bomb)
    { //for scoretext start from the saved score value
        score = value;
        scoreDisplay.text = score + " " + "pts".ToString();
        sliderValue = slidertemp;
        slider.value = sliderValuee;
        colorBallCounter = color;
        bombCounter = bomb;
        bombCounterText.text = bombCounter.ToString();
        colorBallCounterText.text = colorBallCounter.ToString();
        //highscore display
        highScore = score;
        //displayHighScore.text = tempHighScoreHolder.ToString();
    }

    public float SliderValueToSave()// for saving it on file
    {
        return slider.value;
    }

    public float SliderTemp()
    {
        return sliderValue;
    }

    public int SendAlternate()// bool value
    {
        int i;
        if (alternate)
        {
            i = 1;
        }
        else
            i = 0;
        return i;
    }

    public void GetAlternate(int boolValue)
    {
        if(boolValue==1)
        {
            alternate = true;
            bomb.SetActive(false);
            colorball.SetActive(true);
        }
        else if(boolValue==0)
        {
            alternate = false;
            colorball.SetActive(false);
            bomb.SetActive(true);
        }
    }
}
