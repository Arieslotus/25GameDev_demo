using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field:Header("Default_SFX")]
    [field:SerializeField]public EventReference Default_write { get;private set; }
    [field: SerializeField] public EventReference Default_gameWin { get; private set; }
    [field: SerializeField] public EventReference Default_gameFailed { get; private set; }
    [field: SerializeField] public EventReference Default_error { get; private set; }
    [field: SerializeField] public EventReference Default_gameEnd { get; private set; }

    [field: Header("ALL_BGM")]
    [field: SerializeField] public EventReference All_BGM { get; private set; }

    [field: Header("L1Beer_SFX")]
    [field: SerializeField] public EventReference L1Beer_water { get; private set; }
    [field: SerializeField] public EventReference L1Beer_reduceFoam { get; private set; }
    [field: SerializeField] public EventReference L1Beer_addFoam { get; private set; }
    [field: SerializeField] public EventReference L1Beer_foamOutOfCup { get; private set; }




    public static FMODEvents instance {  get; private set; }

    private void Awake()
    {
        //if (instance != null)
        //{
        //    Debug.LogError("find more than 1 fmod events instance in scene");
        //}
        //instance = this;
        if (instance != null)
        {
            Debug.LogError("Found More than 1 events manager in this scene");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
