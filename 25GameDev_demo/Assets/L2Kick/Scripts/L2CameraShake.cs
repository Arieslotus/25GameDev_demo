using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L2CameraShake : MonoBehaviour
{
    [Header("抖动")]
    public float shakeDuration = 0.2f;  // 震动持续时间
    public float shakeMagnitude = 0.1f; // 震动幅度

    private Vector3 originalPos;
    private float shakeTime = 0;

    [Header("放大")]
    public Camera mainCamera;  // 需要调整的摄像机
    public float minSize = 3f; // 最小缩放大小（越小越放大）
    public float normalSize = 5f; // 默认摄像机大小
    public float zoomSpeed = 2f;  // 缩放速度
    public float holdTime = 0.5f; // 触发缩放的长按时间

    private float holdTimer = 0f;
    private bool isZooming = false;
    private Coroutine zoomCoroutine = null;

    void Start()
    {
        originalPos = transform.localPosition;

        mainCamera = Camera.main; // 获取主摄像机
        mainCamera.orthographicSize = normalSize;
    }

    void Update()
    {
        if (shakeTime > 0)
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * shakeMagnitude;
            shakeTime -= Time.deltaTime;
        }
        else
        {
            transform.localPosition = originalPos; // 复位
        }

        if (L2CheckList.headCheckList.Count > 0)
        {
            if (!L2CheckList.headHadMiss)
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    holdTimer += Time.deltaTime;

                    if (holdTimer >= holdTime && !isZooming)
                    {
                        if (zoomCoroutine != null) StopCoroutine(zoomCoroutine);
                        zoomCoroutine = StartCoroutine(ZoomCamera(minSize)); // 开始放大
                        isZooming = true;
                    }
                }
                
            }
        }
        if(!Input.GetKey(KeyCode.Space))
        {
            if (isZooming)
            {
                if (zoomCoroutine != null) StopCoroutine(zoomCoroutine);
                zoomCoroutine = StartCoroutine(ZoomCamera(normalSize)); // 释放时缩小
                isZooming = false;
            }
            holdTimer = 0f; // 重置计时器
        }
    }

    public void ShakeCamra()
    {
        shakeTime = shakeDuration; // 触发震动
    }

    IEnumerator ZoomCamera(float targetSize)
    {
        float startSize = mainCamera.orthographicSize;
        float elapsedTime = 0f;

        while (elapsedTime < zoomSpeed)
        {
            mainCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, elapsedTime / zoomSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCamera.orthographicSize = targetSize; // 确保最终达到目标值
    }
}
