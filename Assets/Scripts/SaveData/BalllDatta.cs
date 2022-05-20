using UnityEngine;

[System.Serializable]
public class BalllDatta
{
    //for ball
    public string id;
    public float[] position;

    // score 
    public float score;

    // value of slider
    public float valueOfSlider;
    public float slidertempValue;

    //no of bombs and color ball
    public int colorball;
    public int bomb;
    public int alternate;
    
    public BalllDatta(Balls ball )
    {
        id = ball.tag;
        Vector3 ballPos = ball.transform.position;
        position = new float[]
        {
            ballPos.x,ballPos.y,ballPos.z
        };
        score = SpawnBallsUponCollide.score;
        valueOfSlider = SpawnBallsUponCollide.instance.SliderValueToSave();
        slidertempValue = SpawnBallsUponCollide.instance.SliderTemp();
        colorball = SpawnBallsUponCollide.colorBallCounter;
        bomb = SpawnBallsUponCollide.bombCounter;
        alternate = SpawnBallsUponCollide.instance.SendAlternate();
    }
}
