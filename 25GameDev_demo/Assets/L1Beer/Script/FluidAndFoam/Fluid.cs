using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fluid : MonoBehaviour
{
    [HideInInspector]
    public bool isInBottle = false;//水粒子是否位于瓶中
    
    
    Rigidbody2D rb;

    //not use
    int neighborFluidNum, neighborFoamNum;
    bool flag_neverRaycast = false;
    bool flag = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {

        if (isInBottle)
        {
            GetComponent<SpriteRenderer>().color = Color.green;//test

            //if(flag)
            //{
            //    //FindObjectOfType<FluidsController>().AddToFluidInBottleList(transform);
            //    flag = false;
            //}         
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.blue;//test
        }

        if(Mathf.Abs(rb.velocity.magnitude) > 3)/*edit*/
        {
            isInBottle = false;
        }

        ////debug is in bottle
        //////not in bottle
        //if(!flag_neverRaycast)
        //{
        //    neighborFluidNum = 0;
        //    neighborFoamNum = 0;

        //    RaycastFromCenter();
        //    if (neighborFluidNum <= 1)
        //    {
        //        isInBottle = false;
        //    }
        //    else
        //    {
        //        isInBottle = true;
        //    }

        //    if (neighborFoamNum >= 4)//edit!!!
        //    {
        //        isInBottle = false;
        //        //GetComponent<SpriteRenderer>().color = Color.white;//test
        //    }
        //}
        //else
        //{
        //    GetComponent<SpriteRenderer>().color = Color.black;//test
        //}

    }

    void RaycastFromCenter()
    {
       
        //raycast

        int numberOfRays = 10;  // 射线数量
        float radius = 0.2f;      // 射线的长度
        float angleOffset = 0f; // 可选的角度偏移，帮助旋转射线的起始位置

        // 计算每条射线之间的角度间隔
        float angleStep = 360f / numberOfRays;

        // 从物体的位置开始
        Vector3 start = transform.position;

        for (int i = 0; i < numberOfRays; i++)
        {
            // 计算当前射线的角度
            float angle = i * angleStep + angleOffset;

            // 将角度转换为弧度
            float radian = angle * Mathf.Deg2Rad;

            // 计算射线方向
            Vector3 direction = new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0f); // 2D平面上发射



            // 可以进行射线检测
            RaycastHit2D hit = Physics2D.Raycast(start, direction, radius);
            if (hit)
            {
                var fluid = hit.collider.gameObject.GetComponent<Fluid>();
                if(fluid != null)
                {
                    if (fluid.isInBottle)
                    {
                        neighborFluidNum++;//**


                        //Debug.DrawRay(start, direction * radius, Color.red);//debug
                    }
                    else
                    {
                        //Debug.DrawRay(start, direction * radius, Color.blue);//debug
                    }
                }
                else
                {
                    //Debug.DrawRay(start, direction * radius, Color.white);//debug
                }
            }
            else
            {
               //Debug.DrawRay(start, direction * radius, Color.black);//debug
            }


            //foam

            LayerMask layer_foam = LayerMask.GetMask("Foams");
            RaycastHit2D hit_foam = Physics2D.Raycast(start, direction, radius, layer_foam);
            if (hit_foam)
            {
                var foam = hit_foam.collider.gameObject.GetComponent<Foam>();
                if (foam != null)
                {
                    neighborFoamNum++;
                    //Debug.DrawRay(start, direction * radius, Color.white);//debug
                }
                else
                {
                    //Debug.DrawRay(start, direction * radius, Color.white);//debug
                }
            }
            else
            {
                //Debug.DrawRay(start, direction * radius, Color.black);//debug
            }


        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var fluid = collision.gameObject.GetComponent<Fluid>();

        //decide whether in bottle
        if (fluid && fluid.isInBottle)
        {         
            isInBottle = true;
            //flag_startRaycast = true;
        }

        if(collision.gameObject.name == "bottleBottom")
        {
            isInBottle = true;
            //flag_neverRaycast = true;
        }
    }

    //游戏结束
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //水位线
        if (collision.gameObject.name == "waterLine" && isInBottle)
        {
            FindObjectOfType<gameController>().gameStart = false;
            if (FindObjectOfType<gameController>().roundResult != 2)
            {
                FindObjectOfType<gameController>().roundResult = 1;
            }
            //Debug.Log("water touch line");
        }
    }
}
