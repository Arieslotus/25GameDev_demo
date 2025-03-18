using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatController : MonoBehaviour
{
    [HideInInspector] public float rotationSpeed; // 旋转速度（度/秒）
    float startAngle = 0;
    float startOffsetAngle = 0;
    [HideInInspector] public float offsetAngle;

    public Transform footR; // 右脚
    public Transform footL; // 左脚
    Transform foot; // 当前该伸出的脚
    int kickNum = 0;

    public float popOutDistance = 0.5f; // 弹出距离
    public float popTime = 0.1f; // 弹出时间（秒）
    public float retractTime = 0.2f; // 收回时间（秒）

    private Vector3 footOriginalLocalPosition, RfootOriginalLocalPosition, LfootOriginalLocalPosition;
    private bool isAnimating = false;
    private float animationProgress = 0f;
    private bool isPopping = false;
    private bool isHolding = false;
    private bool isExtended = false;
    private bool isRetractingAuto = false;
    public float arcHeight = 0.2f; // 运动的弧度高度

    //popTime内松开空格键判定为短按，超过判定为长按

    void Start()
    {
        // 设置初始旋转角度
        transform.eulerAngles = new Vector3(0, 0, offsetAngle);

        // 存储脚的原始位置
        RfootOriginalLocalPosition = footR.localPosition;
        LfootOriginalLocalPosition = footL.localPosition;
    }

    void Update()
    {
        // 旋转主物体
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        // 按下空格键触发踢脚动画
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isAnimating && !isExtended)
            {
                isAnimating = true;
                isPopping = true;
                animationProgress = 0f;
                isHolding = true;

                // 切换脚
                kickNum++;
                foot = (kickNum % 2 == 0) ? footL : footR;
                footOriginalLocalPosition = (kickNum % 2 == 0) ? LfootOriginalLocalPosition : RfootOriginalLocalPosition;
            }
        }

        // 长按保持伸出
        if (Input.GetKey(KeyCode.Space) && isExtended)
        {
            return; // 如果已经伸出并按住空格，则不执行收回逻辑
        }

        // 松开空格键时收回
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



        // 控制脚的弹出和收回
        if (isAnimating && foot != null)
        {
            animationProgress += Time.deltaTime / (isPopping ? popTime : retractTime);
            float t = animationProgress;
            t = t * t * (3f - 2f * t); // 平滑曲线（更有力的踢出）


            if (isPopping)
            {
                //foot.localPosition = Vector3.Lerp(footOriginalLocalPosition, footOriginalLocalPosition + Vector3.down * popOutDistance, t);

                //float arcOffset = Mathf.Sin(t * Mathf.PI) * arcHeight; // 计算弧线高度
                //Vector3 targetPos = footOriginalLocalPosition + new Vector3(arcOffset, -popOutDistance, 0);
                //foot.localPosition = Vector3.Lerp(footOriginalLocalPosition, targetPos, t);

                // 计算抛物线轨迹（贝塞尔曲线）
                Vector3 start = footOriginalLocalPosition;
                Vector3 end = footOriginalLocalPosition + Vector3.down * popOutDistance;

                // **动态计算控制点**
                Vector3 direction = (end - start).normalized; // 获取方向
                Vector3 perpDirection = new Vector3(-direction.y, direction.x, 0); // 获取垂直方向
                //Vector3 control = (start + end) * 0.5f + perpDirection * arcHeight * Mathf.Clamp01(popOutDistance / 0.5f);
                Vector3 control = start + Vector3.up * arcHeight + Vector3.right * (kickNum % 2 == 0 ? -arcHeight : arcHeight);
                // 计算贝塞尔曲线位置
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