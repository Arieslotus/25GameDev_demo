using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L2Checker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //for(int i = 0; i < L2CheckList.tapCheckList.Count; i++)
        //{
        //    if (Input.GetKeyDown(KeyCode.Space) )
        //    {
        //        //&& this == L2CheckList.tapCheckList[0]
        //        L2CheckList.tapCheckList[i].CheckNote_Tap();
        //    }
        //    //L2CheckList.tapCheckList[i].CheckNote_Tap();
        //    //if (L2CheckList.tapCheckList[i].CheckNote_Tap())
        //    //{

        //    //}
        //}
        if (Input.GetKeyDown(KeyCode.Space) )
        {
            if (L2CheckList.tapCheckList != null && L2CheckList.tapCheckList.Count > 0)
            {
                L2CheckList.tapCheckList[0].CheckNote_Tap();
            }
            //if(L2CheckList.headCheckList != null && L2CheckList.headCheckList.Count > 0)
            //{
            //    L2CheckList.headCheckList[0].CheckNote_Head();
            //}
            
        }
       
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (L2CheckList.headCheckList != null && L2CheckList.headCheckList.Count > 0)
            {
                Debug.Log("´¥·¢head¼ì²â");
                L2CheckList.headCheckList[0].CheckNote_Head();
            }
        }
    }
}
