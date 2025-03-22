using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L2Grader : MonoBehaviour
{
    float targetHeight; // 停顿时的高度
    float riseDuration; // 上浮持续时间
    float pauseDuration; // 停顿时间
    float fadeOutDuration; // 淡出持续时间
    float destroyDelay; // 销毁延迟时间
    float continueRiseSpeed;

    private Vector3 initialPosition; // 初始位置
    private Vector3 initialScale;
    private float riseStartTime; // 上浮开始时间
    private float fadeOutStartTime; // 淡出开始时间
    private bool isRising = true; // 是否正在上浮
    private bool isPaused = false; // 是否正在停顿
    private bool isFadingOut = false; // 是否正在淡出
    private SpriteRenderer spriteRenderer; // 用于控制透明度的 SpriteRender

    void Start()
    {
        L2gameController controller = FindObjectOfType<L2gameController>();
        targetHeight = controller.targetHeight;
        riseDuration = controller.riseDuration;
        pauseDuration = controller.pauseDuration;
        fadeOutDuration = controller.fadeOutDuration;
        destroyDelay = controller.destroyDelay;
        continueRiseSpeed = controller.continueRiseSpeed;

        L2CheckList.graderList.Add(this);
        // 记录初始位置
        initialPosition = transform.position;
        initialScale = transform.localScale;
        // 设置初始缩放为 0.5 倍
        transform.localScale = initialScale * 0.5f;

        // 获取 SpriteRenderer 组件
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("物体上没有 SpriteRenderer 组件！");
        }

        
        // 记录上浮开始时间
        riseStartTime = Time.time;
    }

    void Update()
    {
        float elapsedTime = Time.time - riseStartTime;

        // 上浮阶段
        if (isRising)
        {
            // 计算上浮进度
            float riseProgress = elapsedTime / riseDuration;
            if (riseProgress < 1f)
            {
                // 平滑上浮到目标高度
                float currentHeight = Mathf.Lerp(0f, targetHeight, riseProgress);
                transform.position = initialPosition + Vector3.up * currentHeight;
                // 平滑放大到原始大小
                transform.localScale = Vector3.Lerp(initialScale * 0.5f, initialScale, riseProgress);
            }
            else
            {
                // 上浮完成，进入停顿阶段
                isRising = false;
                isPaused = true;
                riseStartTime = Time.time; // 重置计时器
            }
        }
        // 停顿阶段
        else if (isPaused)
        {
            if (elapsedTime >= pauseDuration)
            {
                // 停顿结束，进入淡出阶段
                isPaused = false;
                isFadingOut = true;
                fadeOutStartTime = Time.time; // 记录淡出开始时间
            }
        }
        // 淡出阶段
        else if (isFadingOut)
        {
            // 继续上浮
            transform.position += Vector3.up * continueRiseSpeed * Time.deltaTime;

            // 计算淡出进度
            float timeSinceFadeOut = Time.time - fadeOutStartTime;
            float alpha = Mathf.Lerp(1f, 0f, timeSinceFadeOut / fadeOutDuration);
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
            }

            // 销毁物体
            if (timeSinceFadeOut >= fadeOutDuration)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnDestroy()
    {
        // 从列表中移除
        L2CheckList.graderList.Remove(this);
    }
}
