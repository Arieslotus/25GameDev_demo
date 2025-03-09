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

    // Start is called before the first frame update
    void Start()
    {
        originalScale = button_entergame.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.F))
        //{
        //    TriggerButtonEffect();
        //}
        ShowResult();
    }

    void TriggerButtonEffect()
    {
        if (button_entergame != null)
        {
            button_entergame.transform.localScale = originalScale * scaleFactor;
            Invoke("LoadNextScene", 0.2f); // 0.2秒后跳转场景，模拟放大效果
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
    public void OnPointerEnter()
    {
        button_entergame.transform.localScale = originalScale * scaleFactor;
    }

    public void OnPointerExit()
    {
        button_entergame.transform.localScale = originalScale;
    }

    public void OnStartButtonClick()
    {
        if (!string.IsNullOrEmpty(nextSceneName) )
        {
            SceneManager.LoadScene(nextSceneName);
            if(FindObjectOfType<sceneManager>() != null)
            FindObjectOfType<sceneManager>().Reset();
        }
        else
        {
            Debug.LogWarning("Next scene name is not set!");
        }
    }

    void ShowResult()
    {
        if (sceneManager.gameCount == 3)
        {
            p1Score.text = "P1 score: " + sceneManager.playerJScore;
            p2Score.text = "P2 score: " + sceneManager.playerFScore;

            if(sceneManager.playerJScore > sceneManager.playerFScore)
            {
                finalResult.text = "winner : P1";
            }
            else
            {
                finalResult.text = "winner : P2";
            }
                
        }
    }
}
