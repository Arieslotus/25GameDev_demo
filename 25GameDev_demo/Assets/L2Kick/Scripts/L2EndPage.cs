using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class L2EndPage : MonoBehaviour
{
    public RectTransform background; // 背景
    public RectTransform scoreText; // 得分文本
    public RectTransform button; // 按钮

    public float slideDuration = 1f; // 滑动持续时间
    public float delayBetweenElements = 0.5f; // 元素之间的延迟

    private Vector2 backgroundStartPos; // 背景的起始位置
    private Vector2 scoreTextStartPos; // 得分文本的起始位置
    private Vector2 buttonStartPos; // 按钮的起始位置

    L2gameController controller;
    enum level
    {
        SS,
        S,
        A,
        B
    }
    level myLevel;
    int score;
    //int bestCombo;

    public Image image_level;
    public Sprite sprite_level_S;
    public Sprite sprite_level_A;
    public Sprite sprite_level_B;
    public Text text_score;
    public Text text_bestCombo;
    public Text text_perfect;
    public Text text_good;
    public Text text_miss;

    void Start()
    {
        // 记录初始位置
        backgroundStartPos = background.anchoredPosition;
        scoreTextStartPos = scoreText.anchoredPosition;
        buttonStartPos = button.anchoredPosition;

        // 将所有元素移动到屏幕右-侧
        background.anchoredPosition = new Vector2(Screen.width, backgroundStartPos.y);
        scoreText.anchoredPosition = new Vector2(Screen.width, scoreTextStartPos.y);
        button.anchoredPosition = new Vector2(Screen.width, buttonStartPos.y);

        // 开始动画
        StartCoroutine(AnimateResultScreen());


        //积分
        controller = FindObjectOfType<L2gameController>();
        score = controller.score;
        //text_bestCombo.text = "best combo " + bestCombo;
        text_score.text = "" + score;
        text_bestCombo.text = "" + controller.bestComboNum;
        text_perfect.text = "" + controller.perfectNum;
        text_good.text = "" + controller.goodNum;
        text_miss.text = "" + controller.missNum;
        //if (controller.health > 0)
        //{
        //    if (score / controller.totalScore >= 1)
        //    {
        //        myLevel = level.SS;
        //        image_level.sprite = sprite_level_S;
        //    }
        //    else if (score / controller.totalScore >= 0.8)
        //    {
        //        myLevel = level.S;
        //        image_level.sprite = sprite_level_S;
        //    }
        //    else
        //    {
        //        myLevel = level.A;
        //        image_level.sprite = sprite_level_A;
        //    }
        //}
        //else
        //{
        //    myLevel = level.B;
        //    image_level.sprite = sprite_level_B;
        //}
        Debug.Log("controller.totalScore" + controller.totalScore);
        Debug.Log("score" + score);
        if ((float)score/ (float)controller.totalScore >= 1f)
        {
            myLevel = level.S;
            image_level.sprite = sprite_level_S;
        }
        else if ((float)score / (float)controller.totalScore >= 0.85f)
        {
            Debug.Log("S");
            myLevel = level.S;
            image_level.sprite = sprite_level_S;
        }
        else if ((float)score / (float)controller.totalScore >= 0.65f)
        {
            Debug.Log("A");
            myLevel = level.A;
            image_level.sprite = sprite_level_A;
        }
        else
        {
            Debug.Log("B");
            myLevel = level.B;
            image_level.sprite = sprite_level_B;
        }

    }

    IEnumerator AnimateResultScreen()
    {
        // 背景右滑进入
        yield return StartCoroutine(SlideElement(background, backgroundStartPos, slideDuration));

        // 延迟
        yield return new WaitForSeconds(delayBetweenElements);

        // 得分文本右滑进入
        yield return StartCoroutine(SlideElement(scoreText, scoreTextStartPos, slideDuration));

        // 延迟
        yield return new WaitForSeconds(delayBetweenElements);

        // 按钮右滑进入
        yield return StartCoroutine(SlideElement(button, buttonStartPos, slideDuration));
    }

    IEnumerator SlideElement(RectTransform element, Vector2 targetPosition, float duration)
    {
        Vector2 startPosition = element.anchoredPosition;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // 使用缓动函数（刹车感）
            float t = EaseOutQuart(elapsedTime / duration);
            element.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 确保最终位置准确
        element.anchoredPosition = targetPosition;
    }

    // 缓动函数：EaseOutQuart（刹车感）
    float EaseOutQuart(float t)
    {
        return 1 - Mathf.Pow(1 - t, 4);
    }

    public void OnClickRestartButton()
    {
        L2CheckList.ResetCheckList();
        SceneManager.LoadScene("L2Kick");

    }
    public void OnClickRestartEasyButton()
    {
        L2CheckList.ResetCheckList();
        SceneManager.LoadScene("L2KickTotal");

    }
    public void OnClickBackButton()
    {
        L2CheckList.ResetCheckList();
        SceneManager.LoadScene("L2Start");
    }

}
