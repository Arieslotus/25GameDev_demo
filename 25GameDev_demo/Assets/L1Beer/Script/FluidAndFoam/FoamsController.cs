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
}
