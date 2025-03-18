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

        // ����ϥ��λ��
        Vector3 kneePosition = CalculateKneePosition(hip, foot, upperLegLength, lowerLegLength);
        

        // ����LineRenderer�Ķ���
        legLineRenderer.positionCount = 3;
        legLineRenderer.SetPosition(0, hip);
        legLineRenderer.SetPosition(1, kneePosition);
        legLineRenderer.SetPosition(2, foot);

        // ��ѡ�������߶���ɫ���ִ��Ⱥ�С��
        legLineRenderer.startColor = startColor;
        legLineRenderer.endColor = endColor;
    }

    Vector3 CalculateKneePosition(Vector3 hip, Vector3 foot, float upperLeg, float lowerLeg)
    {
        Vector3 direction = foot - hip;
        float distance = direction.magnitude;
        direction.Normalize();

        // ���Ŀ�곬���ȵķ�Χ���򷵻��е�
        if (distance > upperLeg + lowerLeg || distance < Mathf.Abs(upperLeg - lowerLeg))
        {
            //Debug.Log("Leg can't reach target, returning middle point.");
            return (hip + foot) * 0.5f;
        }

        // �������Ҷ���Ƕ�
        float cosTheta = (upperLeg * upperLeg + distance * distance - lowerLeg * lowerLeg) / (2 * upperLeg * distance);
        float theta = Mathf.Acos(cosTheta);

        // ����ϥ�������������ϻ����£�
        Vector3 bendDir = bendUp ? Vector3.forward : Vector3.back; // 2D ƽ���У�ʹ�� X �᷽��
        Vector3 right = Vector3.Cross(bendDir, direction).normalized; // �������ƫ�Ʒ���Y �ᣩ

        // ����ϥ��λ��
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

    //    // ���ƴ���
    //    Gizmos.color = startColor;
    //    Gizmos.DrawLine(hip, knee);
    //    Gizmos.DrawSphere(knee, 0.1f);

    //    // ����С��
    //    Gizmos.color = endColor;
    //    Gizmos.DrawLine(knee, foot);
    //}
}