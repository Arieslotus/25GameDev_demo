using JetBrains.Annotations;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Foam : MonoBehaviour
{
    //float up speed
    public float riseSpeed ;//泡沫上升速度
    public float riseFactor;//泡沫上升时上方水粒子个数影对上升速度的响大小
    public float riseMass ; //上升时的泡沫质量
    private float originalMass, originalGrav;//记录原始泡沫重力，质量

    //scale
    public float maxScale = 1.5f;      // 泡沫最大放大尺寸
    public float scaleSpeed = 0.02f; // 泡沫放大的速度


    private Rigidbody2D rb;
    public bool isFloating = true;//泡沫是否正在上浮
    float rayDistance = 5f;//射线检测长度


    

void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        originalGrav = rb.gravityScale;
        originalMass = rb.mass;
    }

    void FixedUpdate()
    {

        //ray cast the num of the fluids above foam
        int hitNum = 0;
        LayerMask fluidsLayerMask = LayerMask.GetMask("Fluids");
        RaycastHit2D[] hits_above = Physics2D.RaycastAll(transform.position, Vector2.up, rayDistance, fluidsLayerMask);

        foreach (RaycastHit2D hit in hits_above)
        {
            var fluid = hit.collider.gameObject.GetComponent<Fluid>();
            if (fluid && fluid.isInBottle)
            {
                hitNum++;
            }
        }


        //Debug.DrawLine(transform.position, transform.position + new Vector3(0, rayDistance, 0), Color.red);//test
        //Debug.DrawLine(transform.position, transform.position + new Vector3(0, hitNum, 0), Color.green);//test
        
       
        //decide whether is floating up
        if (hitNum >= 1)
        {
           isFloating = true;
        }
        else
        {
            isFloating = false;
        }


        
        if (isFloating)
        {
            //if is floating up
            //scale bigger
            if (transform.localScale.x < maxScale)
            {
                transform.localScale += new Vector3(scaleSpeed, scaleSpeed, 0);
            }


            //force
            float f = hitNum * riseFactor;

            //float r = 1 - ((transform.position.y - originHeight) / (stopHeight - originHeight));
            //if (r < 0)
            //{
            //    r = 0;
            //}

            rb.gravityScale = -Mathf.Abs(riseSpeed * f);
            rb.mass = riseMass;

            //// 判断泡沫是否到达水面高度
            ////transform.position.y >= stopHeight
            //if (transform.position.y >= stopHeight)
            //{
            //    // 停止上浮，可以通过设置 velocity 为零来停止
            //    rb.mass = originalMass;
            //    rb.gravityScale = originalGrav;
            //    rb.velocity = Vector3.zero;

            //    isFloating = false;

            //    //add list
            //    FindObjectOfType<FoamsController>().AddToFoamsList(transform);
            //}
        }
        else
        {
            //if is stop floating up
            rb.mass = originalMass;
            rb.gravityScale = originalGrav;
            
            //if floam is on water, v = 0 ; if floam is dropping out the bottle , dont change v
            RaycastHit2D[] hits_below = Physics2D.RaycastAll(transform.position, Vector2.down, rayDistance, fluidsLayerMask);
            if(hits_below.Length != 0)
            {
                rb.velocity = Vector2.zero;
            }

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (FindObjectOfType<gameController>().gameStart)
        {
            //check
            if (collision.gameObject.name == "desk")
            {
                FindObjectOfType<gameController>().roundResult = 2;
                FindObjectOfType<gameController>().gameStart = false;
                Debug.Log("foam touch desk");
            }
        }
    }
}
