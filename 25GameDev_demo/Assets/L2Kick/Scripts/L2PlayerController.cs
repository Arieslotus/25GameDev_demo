using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L2PlayerController : MonoBehaviour
{
    [Header("��������")]
    public Vector3 normalScale = Vector3.one; // ����״̬�µ�����ֵ
    public Vector3 pressedScale = new Vector3(1.5f, 1.5f, 1.5f); // ���¿ո���������ֵ

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // ���ո���Ƿ���
        if (Input.GetKey(KeyCode.Space))
        {
            // ���¿ո��ʱ���� scale ����Ϊ pressedScale
            transform.localScale = pressedScale;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            // �ɿ��ո��ʱ���� scale �ָ�Ϊ normalScale
            transform.localScale = normalScale;
        }
    }
}
