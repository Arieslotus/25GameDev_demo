using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatController_start : MonoBehaviour
{
    //
    public float duration = 0.3f; // �񶯳���ʱ��
    public float strength = 0.2f; // ����ǿ��

    private Vector3 originalScale;

    private void Start()
    {
        //dot
        originalScale = transform.localScale; // ��¼��ʼ��С
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // �������������
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null) // �ж��Ƿ�����������
            {
                if (hit.collider.gameObject == gameObject) // ȷ��������ǵ�ǰ����
                {
                    OnClick();
                }
            }
        }
    }

    void OnClick()
    {

        transform.DOKill(); // �����֮ǰ�Ķ����������ͻ

        transform.DOScale(new Vector3(originalScale.x * (1 + strength), originalScale.y * (1 - strength), originalScale.z), duration / 2)
            .SetEase(Ease.OutQuad) // �ȷŴ�һ��
            .OnComplete(() =>
            {
                transform.DOScale(originalScale, duration / 2).SetEase(Ease.OutBounce); // Ȼ��ص�
            });
    }
}
