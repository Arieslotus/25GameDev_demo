using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L2Score_test : MonoBehaviour
{
    private float eclapsedTime;

    void Start()
    {
        eclapsedTime = 0;

    }

    // Update is called once per frame
    void Update()
    {
        eclapsedTime += Time.deltaTime;
        for(int i = 0; i <= 100; i++)
        {
            if (eclapsedTime >= 2)
            {
                FindObjectOfType<L2gameController>().SpawnNote();
            }
        }
    }
    

}
