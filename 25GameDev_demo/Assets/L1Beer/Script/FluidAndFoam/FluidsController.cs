using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluidsController : MonoBehaviour
{
    public GameObject fluidPrefab;//水粒子预制体

    //spawn
    public bool startSpawn = true;//是否开始生成水

    public Transform parentTrans_fluid;  // 生成水的父物体
    public Transform spawnTrans;//生成水的位置点
    public int spawNum;//edit 一组水粒子生成总个数
    public float spawnRate;    // edit 每组水粒子生成的时间间隔
    private float timeSinceLastSpawn = 0f;

    List<Transform> FluidInBottleList = new List<Transform>();//not use

    void Start()
    {
        
    }


    void Update()
    {
        //spawn
        if (!startSpawn) return;

        //generate
        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= spawnRate)
        {
            for (int i = 0; i < spawNum; i++)
            {
                SpawnFluid();
            }
            timeSinceLastSpawn = 0f;
        }
    }

    void SpawnFluid()
    {
        //spawn fluid

        //pos
        Vector3 spawnPos = spawnTrans.position;

        //instancitae
        GameObject foam = Instantiate(fluidPrefab, spawnPos, Quaternion.identity);
        foam.transform.SetParent(parentTrans_fluid);

    }
    public void AddToFluidInBottleList(Transform fluid)
    {
        FluidInBottleList.Add(fluid);
    }

    public void RemoveFromInBottleList(Transform objToRemove)
    {
        if (FluidInBottleList.Contains(objToRemove))
        {
            FluidInBottleList.Remove(objToRemove);
            Debug.Log(objToRemove.name + " has been removed.");
        }
        else
        {
            Debug.Log("Object not found in the list.");
        }
    }
}
