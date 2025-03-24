using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[RequireComponent(typeof(EventTrigger))]
public class L2Functions : MonoBehaviour
{
    public GameObject pauseCanvas; // ��ͣ����
    public string mainMenuSceneName = "MainMenu"; // ���˵���������
    public string reloadScene = "L2Kick";
    private bool isPaused = false; // �Ƿ�����ͣ
    private AudioSource[] allAudioSources; // ������ƵԴ

    public float hoverScale = 1.2f; // �����ͣʱ�����ű���
    public float animationDuration = 0.2f; // ��������ʱ��
    private Vector3 originalScale; // ��ť��ԭʼ��С
    private Coroutine scaleCoroutine; // ���Ŷ�����Э��


    void Start()
    {
        // ��¼��ť��ԭʼ��С
        originalScale = transform.localScale;

        // ����¼�������
        AddEventTrigger();
    }
    void Update()
    {
        // ��ⰴ�� Escape ���� P ����ͣ
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    // �л���ͣ״̬
    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    // ��ͣ��Ϸ
    void PauseGame()
    {
        Time.timeScale = 0f; // ֹͣ��Ϸʱ��
        pauseCanvas.SetActive(true); // ��ʾ��ͣ����
        FindObjectOfType<L2Checker>().enabled = false;
        PauseAllAudio();
    }

    // ������Ϸ
    public void ResumeGame()
    {
        Time.timeScale = 1f; // �ָ���Ϸʱ��
        pauseCanvas.SetActive(false); // ������ͣ����
        isPaused = false;
        FindObjectOfType<L2Checker>().enabled = true;
        // �ָ�������Ƶ
        ResumeAllAudio();
    }

    // �������˵�
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // �ָ���Ϸʱ��
        L2CheckList.ResetCheckList();
        SceneManager.LoadScene(mainMenuSceneName); // �������˵�����
    }

    public void Reload()
    {
        Time.timeScale = 1f; // �ָ���Ϸʱ��
        L2CheckList.ResetCheckList();
        SceneManager.LoadScene(reloadScene); // �������˵�����
    }

    void PauseAllAudio()
    {
        allAudioSources = FindObjectsOfType<AudioSource>(); // ��ȡ������������ƵԴ
        foreach (AudioSource audioSource in allAudioSources)
        {
            audioSource.Pause(); // ��ͣ��Ƶ
        }
    }
    // �ָ�������Ƶ
    void ResumeAllAudio()
    {
        if (allAudioSources != null)
        {
            foreach (AudioSource audioSource in allAudioSources)
            {
                audioSource.UnPause(); // �ָ���Ƶ
            }
        }
    }


    // ����¼�������
    void AddEventTrigger()
    {
        EventTrigger eventTrigger = gameObject.GetComponent<EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = gameObject.AddComponent<EventTrigger>();
        }

        // �������¼�
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) => OnPointerEnter());
        eventTrigger.triggers.Add(entryEnter);

        // ����˳��¼�
        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => OnPointerExit());
        eventTrigger.triggers.Add(entryExit);
    }

    // ������ʱ�Ŵ�ť
    void OnPointerEnter()
    {
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine); // ֹ֮ͣǰ�Ķ���
        }
        scaleCoroutine = StartCoroutine(ScaleButton(originalScale * hoverScale));
    }

    // ����˳�ʱ�ָ���ť��С
    void OnPointerExit()
    {
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine); // ֹ֮ͣǰ�Ķ���
        }
        scaleCoroutine = StartCoroutine(ScaleButton(originalScale));
    }

    // ���Ŷ���
    IEnumerator ScaleButton(Vector3 targetScale)
    {
        float elapsedTime = 0f;
        Vector3 startScale = transform.localScale;

        while (elapsedTime < animationDuration)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / animationDuration);
            elapsedTime += Time.unscaledDeltaTime; // ʹ�� unscaledDeltaTime ������ Time.timeScale Ӱ��
            yield return null;
        }

        // ȷ�����մ�С׼ȷ
        transform.localScale = targetScale;
    }
}

