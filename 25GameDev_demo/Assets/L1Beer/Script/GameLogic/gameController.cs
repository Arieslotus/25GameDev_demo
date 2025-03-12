using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class gameController : MonoBehaviour
{
    
    private float elapsedTime = 0f; // 记录流逝时间
    private bool scoreSettled = false;
    private float waitToNextR = 0f;

    public bool gameStart = false;
    public Text timeText; // UI Text 组件，用于显示时间
    public Button timeLine;
    [HideInInspector]public int roundResult=0;//0default,1win,2loose
    public Button roundWin;
    public Button roundLoose;
    public float waitToNextRoundTime = 3f;

    //flag
    bool flag1 = true;
    
    //public Button restart;

    void Start()
    {
        waitToNextR = 0f;
        scoreSettled = false;
        //gameStart = true;
        roundLoose.gameObject.SetActive(false);
        roundWin.gameObject.SetActive(false);
        sceneManager.roundCount++;
        //一局的第一round
        if (sceneManager.roundCount % 2 != 0)
        {
            sceneManager.gameCount++;
            sceneManager.playerJTime = 0;
            sceneManager.playerFTime = 0;
            sceneManager.playerJresult = " ";
            sceneManager.playerFresult = " ";
        }
        Debug.Log("sceneManager.roundCount" + sceneManager.roundCount);
        Debug.Log("sceneManager.gameCount" + sceneManager.gameCount);
    }

    void Update()
    {
        if (gameStart)
        {
            elapsedTime += Time.deltaTime; // 累加时间
            timeText.text = "Time: " + elapsedTime.ToString("F2") + "s"; // 显示两位小数的秒数

            timeLine.image.fillAmount = elapsedTime / 25f;

            flag1 = true;
        }
        else if (!gameStart)
        {
            //sfx
            AudioManager.instance.PauseWaterSound();

            //Finish
            if (roundResult == 1)
            {
                roundWin.gameObject.SetActive(true);
                //restart.gameObject.SetActive(true);

                if (flag1)
                {
                    //sfx
                    AudioManager.instance.PlayOneShot(FMODEvents.instance.Default_gameWin, this.transform.position);
                    flag1 = false;
                }

                //游戏结束
                if (roundResult != 0 && sceneManager.gameCount == 3 && sceneManager.roundCount % 2 == 0)
                {
                    //round结束
                    waitToNextR += Time.deltaTime;
                    if (waitToNextR >= waitToNextRoundTime)
                    {
                        SceneManager.LoadScene("L1End");
                    }
                }
                else
                {
                    //round结束
                    waitToNextR += Time.deltaTime;
                    if (waitToNextR >= waitToNextRoundTime)
                    {
                        RoundEnd();
                    }
                }
                
                // 记录结算时间
                if (sceneManager.roundCount % 2 != 0)  //r1
                {
                    sceneManager.playerJTime = elapsedTime;
                    sceneManager.playerJresult = "Finish";
                    if(sceneManager.playerJBestTime==0 || sceneManager.playerJTime< sceneManager.playerJBestTime)
                    {
                        sceneManager.playerJBestTime = sceneManager.playerJTime;
                    }
                }
                else if(sceneManager.roundCount % 2== 0)  //r2
                {
                    sceneManager.playerFTime = elapsedTime;
                    sceneManager.playerFresult = "Finish";
                    if (sceneManager.playerFBestTime==0 || sceneManager.playerFTime < sceneManager.playerFBestTime)
                    {
                        sceneManager.playerFBestTime = sceneManager.playerFTime;
                    }
                    if (!scoreSettled)
                    {
                        ScoreSettle();
                        scoreSettled = true;
                    }
                }
            }
            //lose
            else if (roundResult == 2)
            {
                roundLoose.gameObject.SetActive(true);
                //restart.gameObject.SetActive(true);

                if (flag1)
                {
                    //sfx
                    AudioManager.instance.PlayOneShot(FMODEvents.instance.Default_gameFailed, this.transform.position);
                    flag1 = false;
                }

                //游戏结束
                if (roundResult != 0 && sceneManager.gameCount == 3 && sceneManager.roundCount % 2 == 0)
                {
                    //round结束
                    waitToNextR += Time.deltaTime;
                    if (waitToNextR >= waitToNextRoundTime)
                    {
                        SceneManager.LoadScene("L1End");
                    }
                }
                else
                {
                    //round结束
                    waitToNextR += Time.deltaTime;
                    if (waitToNextR >= waitToNextRoundTime)
                    {
                        RoundEnd();
                    }
                }
                
                // 记录结算时间
                if (sceneManager.roundCount % 2 != 0)  //r1
                {
                    sceneManager.playerJTime = elapsedTime;
                    sceneManager.playerJresult = "Fail";
                }
                else if (sceneManager.roundCount % 2 == 0)  //r2
                {
                    sceneManager.playerFTime = elapsedTime;
                    sceneManager.playerFresult = "Fail";
                    if (!scoreSettled)
                    {
                        ScoreSettle();
                        scoreSettled = true;
                    }
                }
            }

            if (roundResult != 0 && sceneManager.gameCount==3 && sceneManager.roundCount%2==0)
            {
                //round结束
                waitToNextR += Time.deltaTime;
                if (waitToNextR >= waitToNextRoundTime)
                {
                    SceneManager.LoadScene("L1End");
                }
            }
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("L1Start");
    }

    public void RoundEnd()
    {
        waitToNextR = 0f;
        if (sceneManager.roundCount % 2 == 0) //r1
        {
            SceneManager.LoadScene("L1Beer");
        }
        else if (sceneManager.roundCount % 2 != 0)
        {
            SceneManager.LoadScene("L1BeerR2");//r2
        }
    }

    public void ScoreSettle()
    {
        //大局积分
        if (sceneManager.playerFresult == sceneManager.playerJresult)
        {
            if (sceneManager.playerFresult == "Finish")
            {
                if (sceneManager.playerJTime > sceneManager.playerFTime)
                {
                    sceneManager.playerFScore++;
                    Debug.Log("tie Finish + F++");
                }
                else
                {
                    sceneManager.playerJScore++;
                    Debug.Log("tie Finish + J++");
                }
            }
            else
            {
                if (sceneManager.playerJTime < sceneManager.playerFTime)
                {
                    sceneManager.playerFScore++;
                    Debug.Log("tie lose + F++");
                }
                else
                {
                    sceneManager.playerJScore++;
                    Debug.Log("tie lose + J++");
                }
            }
            
        }
        else
        {
            if (sceneManager.playerFresult == "Finish")
            {
                sceneManager.playerFScore++;
                Debug.Log("F Finish");
            }
            else if (sceneManager.playerJresult == "Finish")
            {
                sceneManager.playerJScore++;
                Debug.Log("J Finish");
            }
        }
    }


   
}
