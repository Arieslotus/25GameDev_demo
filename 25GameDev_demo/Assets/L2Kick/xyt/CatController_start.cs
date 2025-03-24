using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatController_start : MonoBehaviour
{
    //
    public float duration = 0.3f; // 振动持续时间
    public float strength = 0.2f; // 变形强度

    private Vector3 originalScale;

    private void Start()
    {
        //dot
        originalScale = transform.localScale; // 记录初始大小
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 检测鼠标左键按下
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null) // 判断是否点击到了物体
            {
                if (hit.collider.gameObject == gameObject) // 确保点击的是当前对象
                {
                    OnClick();
                }
            }
        }
    }

    void OnClick()
    {

        transform.DOKill(); // 先清除之前的动画，避免冲突

        transform.DOScale(new Vector3(originalScale.x * (1 + strength), originalScale.y * (1 - strength), originalScale.z), duration / 2)
            .SetEase(Ease.OutQuad) // 先放大一点
            .OnComplete(() =>
            {
                transform.DOScale(originalScale, duration / 2).SetEase(Ease.OutBounce); // 然后回弹
            });
    }
}
