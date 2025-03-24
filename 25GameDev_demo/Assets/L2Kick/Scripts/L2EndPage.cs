using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class L2EndPage : MonoBehaviour
{
    public RectTransform background; // ����
    public RectTransform scoreText; // �÷��ı�
    public RectTransform button; // ��ť

    public float slideDuration = 1f; // ��������ʱ��
    public float delayBetweenElements = 0.5f; // Ԫ��֮����ӳ�

    private Vector2 backgroundStartPos; // ��������ʼλ��
    private Vector2 scoreTextStartPos; // �÷��ı�����ʼλ��
    private Vector2 buttonStartPos; // ��ť����ʼλ��

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
        // ��¼��ʼλ��
        backgroundStartPos = background.anchoredPosition;
        scoreTextStartPos = scoreText.anchoredPosition;
        buttonStartPos = button.anchoredPosition;

        // ������Ԫ���ƶ�����Ļ��-��
        background.anchoredPosition = new Vector2(Screen.width, backgroundStartPos.y);
        scoreText.anchoredPosition = new Vector2(Screen.width, scoreTextStartPos.y);
        button.anchoredPosition = new Vector2(Screen.width, buttonStartPos.y);

        // ��ʼ����
        StartCoroutine(AnimateResultScreen());


        //����
        controller = FindObjectOfType<L2gameController>();
        score = controller.score;
        //text_bestCombo.text = "best combo " + bestCombo;
        text_score.text = "" + score;
        if (controller.health > 0)
        {
            if (score / controller.totalScore >= 1)
            {
                myLevel = level.SS;
                image_level.sprite = sprite_level_S;
            }
            else if (score / controller.totalScore >= 0.8)
            {
                myLevel = level.S;
                image_level.sprite = sprite_level_S;
            }
            else
            {
                myLevel = level.A;
                image_level.sprite = sprite_level_A;
            }
        }
        else
        {
            myLevel = level.B;
            image_level.sprite = sprite_level_B;
        }
    }

    IEnumerator AnimateResultScreen()
    {
        // �����һ�����
        yield return StartCoroutine(SlideElement(background, backgroundStartPos, slideDuration));

        // �ӳ�
        yield return new WaitForSeconds(delayBetweenElements);

        // �÷��ı��һ�����
        yield return StartCoroutine(SlideElement(scoreText, scoreTextStartPos, slideDuration));

        // �ӳ�
        yield return new WaitForSeconds(delayBetweenElements);

        // ��ť�һ�����
        yield return StartCoroutine(SlideElement(button, buttonStartPos, slideDuration));
    }

    IEnumerator SlideElement(RectTransform element, Vector2 targetPosition, float duration)
    {
        Vector2 startPosition = element.anchoredPosition;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // ʹ�û���������ɲ���У�
            float t = EaseOutQuart(elapsedTime / duration);
            element.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ȷ������λ��׼ȷ
        element.anchoredPosition = targetPosition;
    }

    // ����������EaseOutQuart��ɲ���У�
    float EaseOutQuart(float t)
    {
        return 1 - Mathf.Pow(1 - t, 4);
    }

    public void OnClickRestartButton()
    {
        SceneManager.LoadScene("L2SKick");
    }
    public void OnClickBackButton()
    {
        SceneManager.LoadScene("L2Start");
    }

}
