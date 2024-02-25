using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    public static Dictionary<int, LevelEnd> levelEnds = new();
    public int level;

    void Start()
    {
        levelEnds[level] = this;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var frog = other.gameObject.GetComponent<Frog>();
        if (frog != null)
        {
            frog.EndOfLevel(level);
        }
    }
}