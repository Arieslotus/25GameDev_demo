using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class L2StartManager : MonoBehaviour
{
    public AudioSource sfx;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickStartButton()
    {
        sfx.Play();
        StartCoroutine("startWithDelay");
        
        
    }

    public void OnClickStartDiffButton()
    {
        sfx.Play();
        StartCoroutine("startAWithDelay");


    }

    public void OnClickTeachButton()
    {
        sfx.Play();
        StartCoroutine("teachWithDelay");


    }

    IEnumerator teachWithDelay()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("L2Teach");
    }

    IEnumerator startWithDelay()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("L2KickTotal");
    }
    IEnumerator startAWithDelay()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("L2Kick");
    }
}
