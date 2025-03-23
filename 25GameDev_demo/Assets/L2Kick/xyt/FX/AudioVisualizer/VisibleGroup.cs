using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibleGroup : MonoBehaviour
{
    public Transform link1;
    public Transform link2;
    public int groupIndex;
    public float mult = 500;
    [HideInInspector]public CircleType circleType;

    private void LateUpdate()
    {
        int index = 511 / groupIndex; //511 300 / groupIndex;groupIndex/4
        Vector3 link1pos = link1.localPosition;
        Vector3 link2pos = link2.localPosition;

        switch (circleType)
        {
            case CircleType.BothJump:
                link1.localPosition = Vector3.Lerp(link1pos, new Vector3(AudioVisible.Instance.samples[index] * mult, AudioVisible.Instance.samples[index] * mult, 0), 0.1f);
                link2.localPosition = Vector3.Lerp(link2pos, new Vector3(-(AudioVisible.Instance.samples[index] * mult), -(AudioVisible.Instance.samples[index] * mult), 0), 0.1f);

                break;
            case CircleType.OutlineJump:
                link1.localPosition = Vector3.Lerp(link1pos, new Vector3(AudioVisible.Instance.samples[index] * mult, AudioVisible.Instance.samples[index] * mult, 0), 0.1f);
                link2.localPosition = Vector3.Lerp(link2pos, new Vector3(0, 0, 0), 0.1f);

                break;
            case CircleType.InlineJump:
                link1.localPosition = Vector3.Lerp(link1pos, new Vector3(0, 0, 0), 0.1f);
                link2.localPosition = Vector3.Lerp(link2pos, new Vector3(-(AudioVisible.Instance.samples[index] * mult), -(AudioVisible.Instance.samples[index] * mult), 0), 0.1f);

                break;
            default: break;
        }

    }
}
