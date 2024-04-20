using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;

public class LevelStart : Checkpoint
{
    public static Dictionary<int, LevelStart> levelStarts = new();
    public int level;

    void Awake()
    {
        levelStarts[level] = this;
        if (level == 1)
        GetComponent<SpriteRenderer>().enabled = false;
    }

    private void Update()
    {
        
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        var frog = other.gameObject.GetComponentInParent<Frog>();
        if (frog != null)
        {
            frog.CollectCheckpoint(this);
            frog.currentLevel = level;
        }
    }
}
