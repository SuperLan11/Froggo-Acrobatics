using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;

public class LevelStart : Checkpoint
{
    public static Dictionary<int, LevelStart> levelStarts = new();
    public int level;

    void Start()
    {
        levelStarts[level] = this;
        GetComponent<SpriteRenderer>().enabled = false;
    }

    private void Update()
    {
        
    }
}
