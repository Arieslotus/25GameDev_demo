using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class sceneManager : MonoBehaviour
{
    public static int playerFScore = 0;
    public static int playerJScore = 0;
    public static int roundCount = 0;
    public static int gameCount = 0;
    public static float playerJTime = 0;
    public static float playerFTime = 0;
    public static float playerJBestTime = 0;
    public static float playerFBestTime = 0;
    public static float playerBestTime = 0;
    public static int playerBest = 0;


    public static string playerJresult = " ";
    public static string playerFresult = " ";

    public Text p1Time;
    public Text p2Time;
    

    public Text p1Score;
    public Text p2Score;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        p1Time.text = "P1 time: " + playerJTime.ToString("F2") + playerJresult;
        p2Time.text = "P2 time: " + playerFTime.ToString("F2") + playerFresult;

        p1Score.text = "P1 score: " + playerJScore;
        p2Score.text = "P2 score: " + playerFScore;
    }

    public void Reset()
    {
        playerFScore = 0;
        playerJScore = 0;
        roundCount = 0;
        gameCount = 0;
        playerJTime = 0;
        playerFTime = 0;
        playerJBestTime = 0;
        playerFBestTime = 0;
        playerBestTime = 0;

        playerJresult = " ";
        playerFresult = " ";

        p1Time.text = "P1 time: 0";
        p2Time.text = "P2 time: 0";

        p1Score.text = "P1 score: 0";
        p2Score.text = "P2 score: 0";
    }
}
