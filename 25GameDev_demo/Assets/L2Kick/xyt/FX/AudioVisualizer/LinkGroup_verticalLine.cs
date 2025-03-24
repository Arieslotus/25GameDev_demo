using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkGroup_verticalLine : MonoBehaviour
{
    public Transform link1;
    public Transform link2;
    private LineRenderer line;

    public Color color1, color2;
    private void Awake()
    {
        line = GetComponent<LineRenderer>();
    }
    private void Start()
    {
        //if (transform.position.x > 0)
        //    transform.localRotation = Quaternion.Euler(0, 0, Mathf.Asin(transform.position.y / Mathf.Sqrt(transform.position.x * transform.position.x + transform.position.y * transform.position.y)) * Mathf.Rad2Deg - 45);
        //else
        //    transform.localRotation = Quaternion.Euler(0, 0, Mathf.Asin(-transform.position.y / Mathf.Sqrt(transform.position.x * transform.position.x + transform.position.y * transform.position.y)) * Mathf.Rad2Deg + 135);
    
    }
    private void Update()
    {
        line.SetPosition(0, link1.position);
        line.SetPosition(1, link2.position);

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(color1, 0.0f), new GradientColorKey(color2, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
        );
        line.colorGradient = gradient;

    }
}