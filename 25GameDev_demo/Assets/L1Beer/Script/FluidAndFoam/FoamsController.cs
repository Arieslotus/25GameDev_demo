using System.Collections.Generic;
using UnityEngine;

public class FoamsController : MonoBehaviour
{
    public GameObject foamPrefab;     // 泡沫小球预制件

    public Transform parentTrans_foam;  // 生成泡沫的父物体

    List<Transform> FoamsList = new List<Transform>();//not use
    


    //spawn
    public bool startSpawn = false;// edit 开始生成泡沫

    public int spawNum;// edit 一组泡沫生成总个数
    public float spawnRate;//edit 每组泡沫生成的时间间隔
    private float timeSinceLastSpawn = 0f;

    CalFoamSpawnPos calFoamSpawnPosController;


    //delete
    public Transform parentFoam; // 需要删除子物体的父对象


    void Update()
    {
        if(calFoamSpawnPosController == null)
        {
            calFoamSpawnPosController = FindObjectOfType<CalFoamSpawnPos>();
        }


        //spawn
        if (!startSpawn) return;

        //generate
        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= spawnRate)
        {
            for (int i = 0; i <spawNum; i++)
            {               
                SpawnFoam();
            }           
            timeSinceLastSpawn = 0f;
        }

    }

    void SpawnFoam()
    {
        //spawn foam

        //pos
        Vector3 spawnPos = calFoamSpawnPosController.CalPos();

        //instancitaye
        GameObject foam = Instantiate(foamPrefab, spawnPos, Quaternion.identity);
        foam.transform.SetParent(parentTrans_foam);
       
    }

    public void AddToFoamsList(Transform foam)
    {
        FoamsList.Add(foam);
    }

    public void DeleteFoams(int foamDeleteCount)
    {
        List<Transform> childList = new List<Transform>();

        // 获取所有子物体
        foreach (Transform child in parentFoam)
        {
            if(child.GetComponent<Foam>().isFloating==false)
                childList.Add(child);
        }

        if(childList.Count > 0)
        {
            //sfx
            AudioManager.instance.PlayOneShot(FMODEvents.instance.L1Beer_reduceFoam, this.transform.position);

        }

        // 确保不会删除超过实际子物体数量
        int actualDeleteCount = Mathf.Min(foamDeleteCount, childList.Count);

        for (int i = 0; i < actualDeleteCount; i++)
        {
            // 随机选择一个子物体索引
            int randomIndex = Random.Range(0, childList.Count);

            // 删除子物体
            Destroy(childList[randomIndex].gameObject);

            // 从列表中移除已删除的子物体，防止重复删除
            childList.RemoveAt(randomIndex);
        }
    }
}
