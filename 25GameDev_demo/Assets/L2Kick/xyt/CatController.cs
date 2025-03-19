using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatController : MonoBehaviour
{
    [HideInInspector] public float rotationSpeed; // ��ת�ٶȣ���/�룩
    float startAngle = 0;
    float startOffsetAngle = 0;
    [HideInInspector] public float offsetAngle;

    public Transform footR; // �ҽ�
    public Transform footL; // ���
    Transform foot; // ��ǰ������Ľ�
    int kickNum = 0;

    public float popOutDistance = 0.5f; // ��������
    public float popTime = 0.1f; // ����ʱ�䣨�룩
    public float retractTime = 0.2f; // �ջ�ʱ�䣨�룩

    private Vector3 footOriginalLocalPosition, RfootOriginalLocalPosition, LfootOriginalLocalPosition;
    private bool isAnimating = false;
    private float animationProgress = 0f;
    private bool isPopping = false;
    private bool isHolding = false;
    private bool isExtended = false;
    private bool isRetractingAuto = false;
    public float arcHeight = 0.2f; // �˶��Ļ��ȸ߶�

    //popTime���ɿ��ո���ж�Ϊ�̰��������ж�Ϊ����

    void Start()
    {
        // ���ó�ʼ��ת�Ƕ�
        transform.eulerAngles = new Vector3(0, 0, offsetAngle);

        // �洢�ŵ�ԭʼλ��
        RfootOriginalLocalPosition = footR.localPosition;
        LfootOriginalLocalPosition = footL.localPosition;
    }

    void Update()
    {
        // ��ת������
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        // ���¿ո�������߽Ŷ���
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isAnimating && !isExtended)
            {
                isAnimating = true;
                isPopping = true;
                animationProgress = 0f;
                isHolding = true;

                // �л���
                kickNum++;
                foot = (kickNum % 2 == 0) ? footL : footR;
                footOriginalLocalPosition = (kickNum % 2 == 0) ? LfootOriginalLocalPosition : RfootOriginalLocalPosition;
            }
        }

        // �����������
        if (Input.GetKey(KeyCode.Space) && isExtended)
        {
            return; // ����Ѿ��������ס�ո���ִ���ջ��߼�
        }

        // �ɿ��ո��ʱ�ջ�
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (isExtended)
            {
                isExtended = false;
                isPopping = false;
                animationProgress = 0f;
                isAnimating = true;
            }

            if(isAnimating && isPopping &&  !isExtended)
            {
                isRetractingAuto = true;
            }
        }



        // ���ƽŵĵ������ջ�
        if (isAnimating && foot != null)
        {
            animationProgress += Time.deltaTime / (isPopping ? popTime : retractTime);
            float t = animationProgress;
            t = t * t * (3f - 2f * t); // ƽ�����ߣ����������߳���


            if (isPopping)
            {
                //foot.localPosition = Vector3.Lerp(footOriginalLocalPosition, footOriginalLocalPosition + Vector3.down * popOutDistance, t);

                //float arcOffset = Mathf.Sin(t * Mathf.PI) * arcHeight; // ���㻡�߸߶�
                //Vector3 targetPos = footOriginalLocalPosition + new Vector3(arcOffset, -popOutDistance, 0);
                //foot.localPosition = Vector3.Lerp(footOriginalLocalPosition, targetPos, t);

                // ���������߹켣�����������ߣ�
                Vector3 start = footOriginalLocalPosition;
                Vector3 end = footOriginalLocalPosition + Vector3.down * popOutDistance;

                // **��̬������Ƶ�**
                Vector3 direction = (end - start).normalized; // ��ȡ����
                Vector3 perpDirection = new Vector3(-direction.y, direction.x, 0); // ��ȡ��ֱ����
                //Vector3 control = (start + end) * 0.5f + perpDirection * arcHeight * Mathf.Clamp01(popOutDistance / 0.5f);
                Vector3 control = start + Vector3.up * arcHeight + Vector3.right * (kickNum % 2 == 0 ? -arcHeight : arcHeight);
                // ���㱴��������λ��
                Vector3 bezierPos = (1 - t) * (1 - t) * start + 2 * (1 - t) * t * control + t * t * end;


                foot.localPosition = bezierPos;


                if (animationProgress >= 1f)
                {
                    if (isRetractingAuto)
                    {
                        isAnimating = true;
                        isPopping = false;
                        isExtended = false;
                        animationProgress = 0f;

                        isRetractingAuto = false;
                    }
                    else
                    {
                        isAnimating = false;
                        isExtended = true;
                    }
                }
            }
            else
            {
                foot.localPosition = Vector3.Lerp(footOriginalLocalPosition + Vector3.down * popOutDistance, footOriginalLocalPosition, t);
                if (animationProgress >= 1f)
                {
                    isAnimating = false;
                }
            }
        }
    }
}