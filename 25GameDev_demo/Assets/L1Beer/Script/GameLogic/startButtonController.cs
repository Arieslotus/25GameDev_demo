using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class startButtonController : MonoBehaviour
{
    private Vector3 originalScale;
    public float scaleFactor = 1.2f; // 放大比例
    public string nextSceneName; // 目标场景名称
    public Button button_entergame;
    public Text p1Score;
    public Text p2Score;
    public Text finalResult;
    public Image endImage;  // 在 Inspector 里拖入 UI Image
    public Sprite end1;   // 在 Inspector 里拖入新图片
    public Sprite end2;
    public Button blue;
    public Button red;
    public int start1end0;
    private float bluePressTime = 0;
    private float redPressTime = 0;
    public Image rule;

    // Start is called before the first frame update
    void Start()
    {
        originalScale = button_entergame.transform.localScale;
        blue.image.fillAmount = 0;
        red.image.fillAmount = 0;
        rule.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.F))
        //{
        //    TriggerButtonEffect();
        //}
        if(sceneManager.gameCount==3)
            ShowResult();

        blue.image.fillAmount = bluePressTime / 2;
        red.image.fillAmount = redPressTime / 2;
        if (Input.GetKey(KeyCode.F))
        {
            if(bluePressTime <= 2)
            bluePressTime += Time.deltaTime;
        }
        else
        {
            if(bluePressTime > 0)
            {
                bluePressTime -= Time.deltaTime;
            }
        }
        if (Input.GetKey(KeyCode.J))
        {
            if(redPressTime <= 2)
            redPressTime += Time.deltaTime;
        }
        else
        {
            if (redPressTime > 0)
            {
                redPressTime -= Time.deltaTime;
            }
        }

        if (blue.image.fillAmount == 1 && red.image.fillAmount == 1)
        {
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
                if (FindObjectOfType<sceneManager>() != null)
                    FindObjectOfType<sceneManager>().Reset();
            }
            else
            {
                Debug.LogWarning("Next scene name is not set!");
            }
        }
    }

    void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("Next scene name is not set!");
        }
    }
    public void OnRuleButtonClick()
    {
        rule.gameObject.SetActive(true);
    }
    public void ExitRule()
    {
        rule.gameObject.SetActive(false);
    }

    public void OnRestartButtonClick()
    {
        SceneManager.LoadScene("L1Start");
        if (FindObjectOfType<sceneManager>() != null)
            FindObjectOfType<sceneManager>().Reset();
    }

    void ShowResult()
    {
        if (endImage != null)
        {
            if(sceneManager.playerJScore> sceneManager.playerFScore)
            {
                endImage.sprite = end1;
            }
            else
            {
                endImage.sprite = end2;
            }
        }

        p1Score.text = "" + sceneManager.playerJScore;
        p2Score.text = "" + sceneManager.playerFScore;
            
        if(sceneManager.playerJBestTime< sceneManager.playerFBestTime)
        {
            finalResult.text = "P1 : " + sceneManager.playerJBestTime.ToString("F2");
            finalResult.color = new Color(167f, 33f, 33f);
        }
        else
        {
            finalResult.text = "P2 : " + sceneManager.playerFBestTime.ToString("F2");
            finalResult.color = new Color(7f,98f,165f);
        }

        //if (sceneManager.playerJScore > sceneManager.playerFScore)
        //{
        //    finalResult.text = "winner : P1";
        //}
        //else
        //{
        //    finalResult.text = "winner : P2";
        //}
    }


}
