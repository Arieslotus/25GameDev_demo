using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class endSFX : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //sfx
        AudioManager.instance.PlayOneShot(FMODEvents.instance.Default_gameEnd, this.transform.position);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
