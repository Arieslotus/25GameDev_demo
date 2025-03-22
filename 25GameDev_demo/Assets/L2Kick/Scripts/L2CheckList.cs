using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class L2CheckList
{
    public static List<L2noteController> tapCheckList = new List<L2noteController>();
    public static List<L2HoldController> holdCheckList = new List<L2HoldController>();
    public static List<L2HoldController> headCheckList = new List<L2HoldController>();
    public static List<L2HoldController> holdRow = new List<L2HoldController>();
    public static List<L2Grader> graderList = new List<L2Grader>();
    public static bool headHadMiss;
}
