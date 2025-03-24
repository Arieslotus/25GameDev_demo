using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L2CameraShake : MonoBehaviour
{
    [Header("����")]
    public float shakeDuration = 0.2f;  // �𶯳���ʱ��
    public float shakeMagnitude = 0.1f; // �𶯷���

    private Vector3 originalPos;
    private float shakeTime = 0;

    [Header("�Ŵ�")]
    public Camera mainCamera;  // ��Ҫ�����������
    public float minSize = 3f; // ��С���Ŵ�С��ԽСԽ�Ŵ�
    public float normalSize = 5f; // Ĭ���������С
    public float zoomSpeed = 2f;  // �����ٶ�
    public float holdTime = 0.5f; // �������ŵĳ���ʱ��

    private float holdTimer = 0f;
    private bool isZooming = false;
    private Coroutine zoomCoroutine = null;

    void Start()
    {
        originalPos = transform.localPosition;

        mainCamera = Camera.main; // ��ȡ�������
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
            transform.localPosition = originalPos; // ��λ
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
                        zoomCoroutine = StartCoroutine(ZoomCamera(minSize)); // ��ʼ�Ŵ�
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
                zoomCoroutine = StartCoroutine(ZoomCamera(normalSize)); // �ͷ�ʱ��С
                isZooming = false;
            }
            holdTimer = 0f; // ���ü�ʱ��
        }
    }

    public void ShakeCamra()
    {
        shakeTime = shakeDuration; // ������
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

        mainCamera.orthographicSize = targetSize; // ȷ�����մﵽĿ��ֵ
    }
}
