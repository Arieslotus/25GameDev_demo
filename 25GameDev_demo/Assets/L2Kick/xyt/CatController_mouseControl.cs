using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CatController_mouseControl : MonoBehaviour
{
    [HideInInspector] public float rotationSpeed; // ��ת�ٶȣ���/�룩
    float startAngle = 0;
    float startOffsetAngle = 0;
    [HideInInspector] public float offsetAngle;

    public Transform footRTrans; // �ҽ�
    public Transform footLTrans; // ���
    public GameObject trailR1, trailR2, trailL1, trailL2;
    int kickNum = 0;

    public float popOutDistance = 0.5f; // ��������
    public float popTime = 0.1f; // ����ʱ�䣨�룩
    public float retractTime = 0.2f; // �ջ�ʱ�䣨�룩
    public float judgeSpaceTime = 0.2f;//�綨�̰��ͳ�����ʱ������
    float currentSpaceTime = 0;
    bool isCalculatingSpaceTime = false;


    public enum KickType
    {
        Tap,
        Hold,
        None
    }

    class Foot
    {
        public int Id;
        public Transform footTrans;
        public Vector3 originalLocalPosition;
        public float animationProgress;
        public bool isPopping;//���ڳ���
        public bool isExtended;//����������
        public bool isRetracting;//�����ս�

        public KickType currentKickType;

    }
    Foot footR, footL;
    Foot currentControlFoot;



    //public float arcHeight = 0.2f; // �˶��Ļ��ȸ߶�

    //popTime���ɿ��ո���ж�Ϊ�̰��������ж�Ϊ����

    private void Awake()
    {
        // ���ó�ʼ��ת�Ƕ�
        transform.eulerAngles = new Vector3(0, 0, offsetAngle);

        // init
        //r foot
        footR = new Foot();
        footR.Id = 2;
        footR.footTrans = footRTrans;
        footR.originalLocalPosition = footRTrans.localPosition;
        footR.animationProgress = 0;
        footR.isPopping = footR.isExtended = footR.isRetracting = false;
        footR.currentKickType = KickType.None;
        //l foot
        footL = new Foot();
        footL.Id = 1;
        footL.footTrans = footLTrans;
        footL.originalLocalPosition = footLTrans.localPosition;
        footL.animationProgress = 0;
        footL.isPopping = footL.isExtended = footL.isRetracting = false;
        footL.currentKickType= KickType.None;

    }

    void Start()
    {
        currentControlFoot = footL;
    }

    void Update()
    {
        // ��ת
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);


        // ���¿ո�������߽Ŷ���
        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentSpaceTime = 0;
            isCalculatingSpaceTime=true;

            if ( !currentControlFoot.isExtended)
            {
                if(currentControlFoot.isRetracting)
                {
                    // change current control foot
                    kickNum++;
                    currentControlFoot = (kickNum % 2 == 0) ? footL : footR;

                }
                else
                {                    
                    kickNum++;
                    currentControlFoot = (kickNum % 2 == 0) ? footL : footR;

                }
                currentControlFoot.isRetracting = false;
                currentControlFoot.isExtended = false;
                currentControlFoot.isPopping = true;
                currentControlFoot.animationProgress = 0f;

 
            }
            else
            {

            }
        }

        if (isCalculatingSpaceTime)
        {
            currentSpaceTime += Time.deltaTime;

            //Debug.LogError($"current{currentSpaceTime} jude{judgeSpaceTime}");

            if (currentSpaceTime >= judgeSpaceTime)
            {
                //Debug.LogError($"{currentControlFoot.Id} holding fx");
                //Debug.LogError("...");
                currentControlFoot.isExtended = true;
                currentControlFoot.currentKickType = KickType.Hold;
            }
            else
            {
                currentControlFoot.currentKickType = KickType.Tap;
            }
        }

        // �ɿ��ո��ʱ�ջ�
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isCalculatingSpaceTime = false;
            //Debug.LogError($"time:{currentSpaceTime}");
            if (currentSpaceTime >= judgeSpaceTime)
            {

                currentControlFoot.isExtended = false;
                currentControlFoot.isPopping = false;
                currentControlFoot.isRetracting = true;
                currentControlFoot.animationProgress = 0f;

                // Debug.LogError("hold");
            }
            else
            {
                //currentControlFoot.currentKickType = KickType.Tap;
            }

            currentControlFoot.currentKickType = KickType.None;
        }





        // �����������
        if (Input.GetKey(KeyCode.Space) && currentControlFoot.isExtended && currentControlFoot.currentKickType == KickType.Hold)
        {
            //Debug.LogError ($"foot{currentControlFoot.Id} is extended, fx , tyep{currentControlFoot.currentKickType}");
            //fx
            if (currentControlFoot.Id == 2)
            {
                trailR1.GetComponent<TrailRenderer>().enabled = true;
                trailR1.GetComponent<TrailRenderer>().emitting = true;

                trailR2.GetComponent<TrailRenderer>().enabled = true;
                trailR2.GetComponent<TrailRenderer>().emitting = true;

            }
            else if (currentControlFoot.Id == 1)
            {

                trailL1.GetComponent<TrailRenderer>().enabled = true;
                trailL1.GetComponent<TrailRenderer>().emitting = true;

                trailL2.GetComponent<TrailRenderer>().enabled = true;
                trailL2.GetComponent<TrailRenderer>().emitting = true;
            }

            //return; // ����Ѿ��������ס�ո���ִ���ջ��߼�
        }
        else
        {
            //fx
            if (currentControlFoot.Id == 2)
            {
                trailR1.GetComponent<TrailRenderer>().emitting = false;
                trailR1.GetComponent<TrailRenderer>().enabled = false;

                trailR2.GetComponent<TrailRenderer>().emitting = false;
                trailR2.GetComponent<TrailRenderer>().enabled = false;



            }
            else if (currentControlFoot.Id == 1)
            {
                trailL1.GetComponent<TrailRenderer>().enabled = false;
                trailL1.GetComponent<TrailRenderer>().emitting = false;
                trailL2.GetComponent<TrailRenderer>().enabled = false;
                trailL2.GetComponent<TrailRenderer>().emitting = false;

            }
        }





        // ���ƽŵĵ������ջ�
        AnimateFoot( footL);
        AnimateFoot( footR);

        //Debug.LogError($"footL animat p{footL.animationProgress}");
        //Debug.LogError($"footR animat p{footR.animationProgress}");


    }

    void AnimateFoot( Foot foot)
    {
        bool isAnimating = foot.isPopping || foot.isRetracting || foot.isExtended;
        if (isAnimating)
        {


            if (foot.isExtended)
            {


                if (foot.currentKickType == KickType.Hold)
                {
                    if (foot.Id == 2)
                    {
                        footRTrans.localPosition = foot.originalLocalPosition + Vector3.down * popOutDistance;

                    }
                    else if (foot.Id == 1)
                    {
                        footLTrans.localPosition =  foot.originalLocalPosition + Vector3.down * popOutDistance;
                    }
                    //return;
                }
                else if (foot.currentKickType == KickType.Tap)
                {
                    foot.isExtended = false;
                    foot.isPopping = false;
                    foot.isRetracting = true;
                    foot.animationProgress = 0f;
                }


            }


            foot.animationProgress += Time.deltaTime / (foot.isPopping ? popTime : retractTime);
            //float t = animationProgress;
            //t = t * t * (3f - 2f * t); // ƽ�����ߣ����������߳���

            if (foot.isPopping)
            {
                //// ���������߹켣�����������ߣ�
                //Vector3 start = footOriginalLocalPosition;
                //Vector3 end = footOriginalLocalPosition + Vector3.down * popOutDistance;

                //// **��̬������Ƶ�**
                //Vector3 direction = (end - start).normalized; // ��ȡ����
                //Vector3 perpDirection = new Vector3(-direction.y, direction.x, 0); // ��ȡ��ֱ����
                ////Vector3 control = (start + end) * 0.5f + perpDirection * arcHeight * Mathf.Clamp01(popOutDistance / 0.5f);
                //Vector3 control = start + Vector3.up * arcHeight + Vector3.right * (kickNum % 2 == 0 ? -arcHeight : arcHeight);
                //// ���㱴��������λ��
                //Vector3 bezierPos = (1 - t) * (1 - t) * start + 2 * (1 - t) * t * control + t * t * end;

                ////foot.localPosition = bezierPos;

                //foot.footTrans.localPosition = Vector3.Lerp(foot.originalLocalPosition, foot.originalLocalPosition + Vector3.down * popOutDistance, foot.animationProgress);
                
                if(foot.Id == 2)
                {
                    footRTrans.localPosition = Vector3.Lerp(foot.originalLocalPosition, foot.originalLocalPosition + Vector3.down * popOutDistance, foot.animationProgress);

                }
                else if(foot.Id == 1)
                {
                    footLTrans.localPosition = Vector3.Lerp(foot.originalLocalPosition, foot.originalLocalPosition + Vector3.down * popOutDistance, foot.animationProgress);
                }


                if (foot.animationProgress >= 1f)
                {
                    foot.isPopping = false;
                    foot.isRetracting = false;
                    foot.isExtended = true;
                    foot.animationProgress = 0f;

                }
            }

            if (foot.isRetracting)
            {
                if (foot.Id == 2)
                {
                    footRTrans.localPosition = Vector3.Lerp(foot.originalLocalPosition + Vector3.down * popOutDistance, foot.originalLocalPosition, foot.animationProgress);


                }
                else if (foot.Id == 1)
                {
                    footLTrans.localPosition = Vector3.Lerp(foot.originalLocalPosition + Vector3.down * popOutDistance, foot.originalLocalPosition, foot.animationProgress);
                }
                
                if (foot.animationProgress >= 1f)
                {
                    foot.isPopping = false;
                    foot.isExtended = false;
                    foot.isRetracting = false;
                    foot.animationProgress = 0f;
                }
            }
        }

    }



}