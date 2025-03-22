using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L2Grader : MonoBehaviour
{
    float targetHeight; // ͣ��ʱ�ĸ߶�
    float riseDuration; // �ϸ�����ʱ��
    float pauseDuration; // ͣ��ʱ��
    float fadeOutDuration; // ��������ʱ��
    float destroyDelay; // �����ӳ�ʱ��
    float continueRiseSpeed;

    private Vector3 initialPosition; // ��ʼλ��
    private Vector3 initialScale;
    private float riseStartTime; // �ϸ���ʼʱ��
    private float fadeOutStartTime; // ������ʼʱ��
    private bool isRising = true; // �Ƿ������ϸ�
    private bool isPaused = false; // �Ƿ�����ͣ��
    private bool isFadingOut = false; // �Ƿ����ڵ���
    private SpriteRenderer spriteRenderer; // ���ڿ���͸���ȵ� SpriteRender

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
        // ��¼��ʼλ��
        initialPosition = transform.position;
        initialScale = transform.localScale;
        // ���ó�ʼ����Ϊ 0.5 ��
        transform.localScale = initialScale * 0.5f;

        // ��ȡ SpriteRenderer ���
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("������û�� SpriteRenderer �����");
        }

        
        // ��¼�ϸ���ʼʱ��
        riseStartTime = Time.time;
    }

    void Update()
    {
        float elapsedTime = Time.time - riseStartTime;

        // �ϸ��׶�
        if (isRising)
        {
            // �����ϸ�����
            float riseProgress = elapsedTime / riseDuration;
            if (riseProgress < 1f)
            {
                // ƽ���ϸ���Ŀ��߶�
                float currentHeight = Mathf.Lerp(0f, targetHeight, riseProgress);
                transform.position = initialPosition + Vector3.up * currentHeight;
                // ƽ���Ŵ�ԭʼ��С
                transform.localScale = Vector3.Lerp(initialScale * 0.5f, initialScale, riseProgress);
            }
            else
            {
                // �ϸ���ɣ�����ͣ�ٽ׶�
                isRising = false;
                isPaused = true;
                riseStartTime = Time.time; // ���ü�ʱ��
            }
        }
        // ͣ�ٽ׶�
        else if (isPaused)
        {
            if (elapsedTime >= pauseDuration)
            {
                // ͣ�ٽ��������뵭���׶�
                isPaused = false;
                isFadingOut = true;
                fadeOutStartTime = Time.time; // ��¼������ʼʱ��
            }
        }
        // �����׶�
        else if (isFadingOut)
        {
            // �����ϸ�
            transform.position += Vector3.up * continueRiseSpeed * Time.deltaTime;

            // ���㵭������
            float timeSinceFadeOut = Time.time - fadeOutStartTime;
            float alpha = Mathf.Lerp(1f, 0f, timeSinceFadeOut / fadeOutDuration);
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
            }

            // ��������
            if (timeSinceFadeOut >= fadeOutDuration)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnDestroy()
    {
        // ���б����Ƴ�
        L2CheckList.graderList.Remove(this);
    }
}
