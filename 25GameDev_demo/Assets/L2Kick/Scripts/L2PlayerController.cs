using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L2PlayerController : MonoBehaviour
{
    [Header("缩放设置")]
    public Vector3 normalScale = Vector3.one; // 正常状态下的缩放值
    public Vector3 pressedScale = new Vector3(1.5f, 1.5f, 1.5f); // 按下空格键后的缩放值

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 检测空格键是否按下
        if (Input.GetKey(KeyCode.Space))
        {
            // 按下空格键时，将 scale 设置为 pressedScale
            transform.localScale = pressedScale;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            // 松开空格键时，将 scale 恢复为 normalScale
            transform.localScale = normalScale;
        }
    }
}
