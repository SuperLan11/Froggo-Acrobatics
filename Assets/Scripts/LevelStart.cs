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
