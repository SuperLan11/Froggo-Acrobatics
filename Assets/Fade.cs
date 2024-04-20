using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    // Update is called once per frame
    void FixedUpdate()
    {
        this.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, Mathf.Max(this.GetComponent<SpriteRenderer>().color.a - 0.04f, 0));
    }
}
