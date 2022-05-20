using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject failText;
    [SerializeField] GameObject audio;
    [SerializeField] float loadTime;
    [SerializeField] GameObject loadPanel;
    
    [SerializeField] GameObject platform;
    [SerializeField] Material[] listOfGround;
    [SerializeField] Material[] listOfBorder;
    [SerializeField] GameObject[] listOfBackGround;// list of backgrounds
    [SerializeField] GameObject[] listOfPatterns; // pattern list

    [SerializeField] GameObject[] listLoadingTextImage;
    float timer;
    int loadingCounter;

    [SerializeField] TextMeshProUGUI displayHighScore;
    public static PauseMenu instance;
    float previousValue;

    //for loading 
    public static bool load;

    //call restart for once
    int count;

    bool loading;
    private void Awake()
    {
        Pattern();
    }
    // Start is called before the first frame update
    void Start()
    {
        loading = true;
        StartCoroutine(StopLoading());
        for(int i=0;i<=listLoadingTextImage.Length;i++)
        {
            listLoadingTextImage[loadingCounter].SetActive(false);
        }

        loadingCounter = 0;
        timer = 0;
        count = 0;
        audio.SetActive(false);
        loadPanel.SetActive(true);
        load = true;
        listOfBackGround[PlayerPrefs.GetInt("BackGroundIndex")].SetActive(true);
        //border and ground
        platform.GetComponent<Renderer>().materials[0].color = listOfBorder[PlayerPrefs.GetInt("BackGroundIndex")].color;
        platform.GetComponent<Renderer>().materials[1].color = listOfGround[PlayerPrefs.GetInt("BackGroundIndex")].color;
        instance = this;
        DisplayHighscore();
        StartCoroutine(LoadingTime()); //loading screen
        Vibration.Init();  // write below all statement
    }

    private void Update()
    {
        if(loading)
            LoadingText();
        if (SpawnBallsUponCollide.highScore > PlayerPrefs.GetFloat("HighScore"))
        {
            displayHighScore.text = SpawnBallsUponCollide.highScore.ToString();
            PlayerPrefs.SetFloat("HighScore", SpawnBallsUponCollide.highScore);
        }
    }

    public void Restart()
    {
        if (SpawnBallsUponCollide.highScore > previousValue)
        {
            PlayerPrefs.SetFloat("HighScore", SpawnBallsUponCollide.highScore);
        }
        Save_System.instance.ResetTheLevel();
        PlayerPrefs.SetInt("PattenInducer", 0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void RestartSecond()
    {
        count++;
        if (count <= 1)
        {
            StartCoroutine(DelayRestart());
        }
    }
    public IEnumerator DelayRestart()
    {
        failText.SetActive(true);
        if (SpawnBallsUponCollide.highScore > previousValue)
        {
            PlayerPrefs.SetFloat("HighScore", SpawnBallsUponCollide.highScore);
        }
        Save_System.instance.ResetTheLevel();
        PlayerPrefs.SetInt("PattenInducer", 0);
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ResSetLvl() //when player dies
    {
        StartCoroutine(Delay());
    }

    void DisplayHighscore()
    {
        previousValue = PlayerPrefs.GetFloat("HighScore");
        displayHighScore.text = PlayerPrefs.GetFloat("HighScore").ToString();
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(2f);
        if (SpawnBallsUponCollide.highScore > previousValue)
        {
            PlayerPrefs.SetFloat("HighScore", SpawnBallsUponCollide.highScore);
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void Pattern()// of ball spawn
    {
        if (PlayerPrefs.GetInt("PattenInducer") == 0)
        {
            //spawn patten here along with background
            StartCoroutine(DelayPatternSpawn());
            PlayerPrefs.SetInt("PattenInducer", 1);
        }
    }

    IEnumerator DelayPatternSpawn()
    {
        yield return new WaitForSeconds(0f);
        // for pattern
        var random = Random.Range(0, listOfPatterns.Length);
        listOfPatterns[random].SetActive(true);

        //background
        //deactivate all back ground first;
        foreach (GameObject ele in listOfBackGround)
        {
            ele.SetActive(false);
        }
        var randBackG = Random.Range(0, listOfBackGround.Length);
        PlayerPrefs.SetInt("BackGroundIndex", randBackG);
        listOfBackGround[PlayerPrefs.GetInt("BackGroundIndex")].SetActive(true);
        //border and ground
        platform.GetComponent<Renderer>().materials[0].color= listOfBorder[PlayerPrefs.GetInt("BackGroundIndex")].color;
        platform.GetComponent<Renderer>().materials[1].color = listOfGround[PlayerPrefs.GetInt("BackGroundIndex")].color;
    }

    IEnumerator LoadingTime()
    {
        yield return new WaitForSeconds(loadTime);
        loadPanel.SetActive(false);
        audio.SetActive(true);
        load = false;
    }

    void LoadingText()
    {
        listLoadingTextImage[loadingCounter].SetActive(true);
        timer += 1 * Time.deltaTime;
        if(timer>.8)
        {
            listLoadingTextImage[loadingCounter].SetActive(false);
            loadingCounter++;
            timer = 0;
        }
        if(loadingCounter>3)
        {
            loadingCounter = 0;
        }

    }

    IEnumerator StopLoading()
    {
        yield return new WaitForSeconds(2f);
        loading = false;
    }
}
