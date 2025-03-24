using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//public class L2CheckList : MonoBehaviour
//{
//    public  List<L2noteController> tapCheckList = new List<L2noteController>();
//    public  List<L2HoldController> holdCheckList = new List<L2HoldController>();
//    public  List<L2HoldController> headCheckList = new List<L2HoldController>();
//    public  List<L2HoldController> holdRow = new List<L2HoldController>();
//    public  List<L2Grader> graderList = new List<L2Grader>();
//    public  bool headHadMiss;
//    public  bool isSpaceReleased;
//}
public static class L2CheckList
{
    public static List<L2noteController> tapCheckList = new List<L2noteController>();
    public static List<L2HoldController> holdCheckList = new List<L2HoldController>();
    public static List<L2HoldController> headCheckList = new List<L2HoldController>();
    public static List<L2HoldController> holdRow = new List<L2HoldController>();
    public static List<L2Grader> graderList = new List<L2Grader>();
    public static bool headHadMiss;
    public static bool isSpaceReleased;

    public static void ResetCheckList()
    {
        tapCheckList.Clear();
        holdCheckList.Clear();
        headCheckList.Clear();
        holdRow.Clear();
        graderList.Clear();
        headHadMiss = false;
        isSpaceReleased = false;
    }
}


