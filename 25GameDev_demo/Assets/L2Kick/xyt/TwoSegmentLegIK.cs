using UnityEngine;
using UnityEngine.Rendering;

public class TwoSegmentLegIK : MonoBehaviour
{
    public LineRenderer legLineRenderer;
    public Transform hipPosition;
    public Transform footTarget;
    public float upperLegLength = 1f;
    public float lowerLegLength = 1f;
    public bool bendUp = true;

    public Color startColor, endColor;

    void Update()
    {
        Vector3 hip = hipPosition.position;
        Vector3 foot = footTarget.position;

        // 计算膝盖位置
        Vector3 kneePosition = CalculateKneePosition(hip, foot, upperLegLength, lowerLegLength);
        

        // 更新LineRenderer的顶点
        legLineRenderer.positionCount = 3;
        legLineRenderer.SetPosition(0, hip);
        legLineRenderer.SetPosition(1, kneePosition);
        legLineRenderer.SetPosition(2, foot);

        // 可选：设置线段颜色区分大腿和小腿
        legLineRenderer.startColor = startColor;
        legLineRenderer.endColor = endColor;
    }

    Vector3 CalculateKneePosition(Vector3 hip, Vector3 foot, float upperLeg, float lowerLeg)
    {
        Vector3 direction = foot - hip;
        float distance = direction.magnitude;
        direction.Normalize();

        // 如果目标超出腿的范围，则返回中点
        if (distance > upperLeg + lowerLeg || distance < Mathf.Abs(upperLeg - lowerLeg))
        {
            //Debug.Log("Leg can't reach target, returning middle point.");
            return (hip + foot) * 0.5f;
        }

        // 计算余弦定理角度
        float cosTheta = (upperLeg * upperLeg + distance * distance - lowerLeg * lowerLeg) / (2 * upperLeg * distance);
        float theta = Mathf.Acos(cosTheta);

        // 计算膝盖弯曲方向（向上或向下）
        Vector3 bendDir = bendUp ? Vector3.forward : Vector3.back; // 2D 平面中，使用 X 轴方向
        Vector3 right = Vector3.Cross(bendDir, direction).normalized; // 计算侧向偏移方向（Y 轴）

        // 计算膝盖位置
        Vector3 kneeOffset = direction * (upperLeg * Mathf.Cos(theta)) + right * (upperLeg * Mathf.Sin(theta));
        Vector3 kneePosition = hip + kneeOffset;

        return kneePosition;
    }

    //void OnDrawGizmos()
    //{
    //    if (hipPosition == null || footTarget == null) return;

    //    Vector3 hip = hipPosition.position;
    //    Vector3 foot = footTarget.position;
    //    Vector3 knee = CalculateKneePosition(hip, foot, upperLegLength, lowerLegLength);

    //    // 绘制大腿
    //    Gizmos.color = startColor;
    //    Gizmos.DrawLine(hip, knee);
    //    Gizmos.DrawSphere(knee, 0.1f);

    //    // 绘制小腿
    //    Gizmos.color = endColor;
    //    Gizmos.DrawLine(knee, foot);
    //}
}