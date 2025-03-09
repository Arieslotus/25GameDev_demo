using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class startSignController : MonoBehaviour
{
    public float moveSpeed = 3f; // 下降速度
    public float holdTime = 1f;  // 停留时间
    public float exitSpeed = 5f; // 离场速度
    public Text showRound;

    private Vector3 startPos;
    private Vector3 midPos;
    private Vector3 exitPos;
    private float eclapsedTime;
    private bool flag = true;

    void Start()
    {
        
       
    }

    private void Update()
    {
        eclapsedTime += Time.deltaTime;
        if (eclapsedTime > 0.1 && flag)
        {

            //round info
            if (sceneManager.roundCount % 2 != 0)
            {
                switch (sceneManager.gameCount)
                {
                    case 1: showRound.text = "ROUND 1"; break;
                    case 2: showRound.text = "ROUND 2"; break;
                    case 3: showRound.text = "ROUND 3"; break;

                }
            }
            else if (sceneManager.roundCount % 2 == 0)
            {
                showRound.text = "SWITCH!";
            }

            // 计算位置
            startPos = new Vector3(transform.position.x, Screen.height, transform.position.z);
            midPos = new Vector3(transform.position.x, Screen.height / 2, transform.position.z);
            exitPos = new Vector3(transform.position.x, -Screen.height, transform.position.z);

            transform.position = startPos; // 初始位置
            StartCoroutine(PlayIntroAnimation());
            flag = false;
        }
    }

    IEnumerator PlayIntroAnimation()
    {

        // 移动到中间
        yield return MoveToPosition(midPos, moveSpeed);

        // 停留 1 秒
        yield return new WaitForSeconds(holdTime);

        // 继续下降直到离开屏幕
        yield return MoveToPosition(exitPos, exitSpeed);

        // 游戏开始
        FindObjectOfType<gameController>().gameStart = true;
        Debug.Log("游戏开始！");
    }

    IEnumerator MoveToPosition(Vector3 target, float speed)
    {
        while (Vector3.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
        transform.position = target;
    }
}
