using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public string[] movements;
    public GameObject[] targets;
    
    private MovingPlatformInstruction[] instructions;

    private int currentIndex = 0;

    void Start()
    {
        if (movements != null)
        {
            instructions = new MovingPlatformInstruction[movements.Length];
            for (int i = 0; i < movements.Length; i++)
            {
                float time = -1;
                float speed = -1;
                if (movements[i].EndsWith('s'))
                {
                    time = float.Parse(movements[i].Substring(0, movements[i].Length - 1));
                }
                else
                {
                    speed = float.Parse(movements[i]);
                }

                instructions[i] = new MovingPlatformInstruction(targets[i], time, speed, this);
            }
        }
        instructions[0].start();

        for (int i = 0; i < instructions.Length; i++)
        {
            Vector2 current = targets[i].transform.position;
            Vector2 next = targets[(i + 1) % instructions.Length].transform.position;
            if (current == next)
            {
                continue;
            }
            GameObject track = Instantiate(CommandManager.instance.trackPrefab, current, Quaternion.identity);
            track.GetComponent<Track>().SetEndpoints(current, next);
        }
    }

    public Vector3 FroggoFixedUpdate()
    {
        if (instructions[currentIndex].isDone())
        {
            currentIndex++;
            if (currentIndex >= instructions.Length)
            {
                currentIndex = 0;
            }

            instructions[currentIndex].start();
        }

        return instructions[currentIndex].step();
    }
}
