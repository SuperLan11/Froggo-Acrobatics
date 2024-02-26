using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnd : MonoBehaviour
{
    public static Dictionary<int, LevelEnd> levelEnds = new();
    public int level;

    void Awake()
    {
        levelEnds[level] = this;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var frog = other.gameObject.GetComponent<Frog>();
        if (frog != null)
        {
            if (IsLast())
            {
                SceneManager.LoadScene("Win");
            }
            frog.EndOfLevel(level);
        }
    }

    public bool IsLast()
    {
        return !levelEnds.ContainsKey(level + 1);
    }
}