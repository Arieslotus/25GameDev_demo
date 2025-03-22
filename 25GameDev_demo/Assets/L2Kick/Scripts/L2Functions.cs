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
    public GameObject pauseCanvas; // 暂停画布
    public string mainMenuSceneName = "MainMenu"; // 主菜单场景名称
    public string reloadScene = "L2Kick";
    private bool isPaused = false; // 是否已暂停
    private AudioSource[] allAudioSources; // 所有音频源

    public float hoverScale = 1.2f; // 鼠标悬停时的缩放比例
    public float animationDuration = 0.2f; // 动画持续时间
    private Vector3 originalScale; // 按钮的原始大小
    private Coroutine scaleCoroutine; // 缩放动画的协程


    void Start()
    {
        // 记录按钮的原始大小
        originalScale = transform.localScale;

        // 添加事件触发器
        AddEventTrigger();
    }
    void Update()
    {
        // 检测按下 Escape 键或 P 键暂停
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    // 切换暂停状态
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

    // 暂停游戏
    void PauseGame()
    {
        Time.timeScale = 0f; // 停止游戏时间
        pauseCanvas.SetActive(true); // 显示暂停画布
        FindObjectOfType<L2Checker>().enabled = false;
        PauseAllAudio();
    }

    // 继续游戏
    public void ResumeGame()
    {
        Time.timeScale = 1f; // 恢复游戏时间
        pauseCanvas.SetActive(false); // 隐藏暂停画布
        isPaused = false;
        FindObjectOfType<L2Checker>().enabled = true;
        // 恢复所有音频
        ResumeAllAudio();
    }

    // 返回主菜单
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // 恢复游戏时间
        SceneManager.LoadScene(mainMenuSceneName); // 加载主菜单场景
    }

    public void Reload()
    {
        Time.timeScale = 1f; // 恢复游戏时间
        SceneManager.LoadScene(reloadScene); // 加载主菜单场景
    }

    void PauseAllAudio()
    {
        allAudioSources = FindObjectsOfType<AudioSource>(); // 获取场景中所有音频源
        foreach (AudioSource audioSource in allAudioSources)
        {
            audioSource.Pause(); // 暂停音频
        }
    }
    // 恢复所有音频
    void ResumeAllAudio()
    {
        if (allAudioSources != null)
        {
            foreach (AudioSource audioSource in allAudioSources)
            {
                audioSource.UnPause(); // 恢复音频
            }
        }
    }


    // 添加事件触发器
    void AddEventTrigger()
    {
        EventTrigger eventTrigger = gameObject.GetComponent<EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = gameObject.AddComponent<EventTrigger>();
        }

        // 鼠标进入事件
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) => OnPointerEnter());
        eventTrigger.triggers.Add(entryEnter);

        // 鼠标退出事件
        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => OnPointerExit());
        eventTrigger.triggers.Add(entryExit);
    }

    // 鼠标进入时放大按钮
    void OnPointerEnter()
    {
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine); // 停止之前的动画
        }
        scaleCoroutine = StartCoroutine(ScaleButton(originalScale * hoverScale));
    }

    // 鼠标退出时恢复按钮大小
    void OnPointerExit()
    {
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine); // 停止之前的动画
        }
        scaleCoroutine = StartCoroutine(ScaleButton(originalScale));
    }

    // 缩放动画
    IEnumerator ScaleButton(Vector3 targetScale)
    {
        float elapsedTime = 0f;
        Vector3 startScale = transform.localScale;

        while (elapsedTime < animationDuration)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / animationDuration);
            elapsedTime += Time.unscaledDeltaTime; // 使用 unscaledDeltaTime 避免受 Time.timeScale 影响
            yield return null;
        }

        // 确保最终大小准确
        transform.localScale = targetScale;
    }
}

