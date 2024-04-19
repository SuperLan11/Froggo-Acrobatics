using UnityEngine;

public class MovingPlatformInstruction
{
    public GameObject target;
    public float time = -1;
    public float speed = -1;
    private MovingPlatform platform;
    private float currTime = 0;
    private float dist;

    public MovingPlatformInstruction(GameObject target, float time, float speed, MovingPlatform platform)
    {
        this.target = target;
        this.time = time;
        this.speed = speed;
        this.platform = platform;
    }

    public bool isDone()
    {
        bool done = this.platform.transform.position == this.target.transform.position && (time < 0 || currTime >= time);
        return done;
    }

    public void start()
    {
        currTime = 0;
        dist = Vector3.Distance(platform.transform.position, target.transform.position);
    }

    public Vector3 step()
    {
        if (time > 0)
        {
            float speed = dist / time;
            currTime += Time.fixedDeltaTime;
            Vector3 newPos = Vector3.MoveTowards(platform.transform.position, target.transform.position, speed*Time.fixedDeltaTime);
            Vector3 diff = newPos - platform.transform.position;
            platform.transform.position = newPos;
            return diff;
        }
        else if (speed > 0)
        {
            Vector3 newPos = Vector3.MoveTowards(platform.transform.position, target.transform.position, speed*Time.fixedDeltaTime);
            Vector3 diff = newPos - platform.transform.position;
            platform.transform.position = newPos;
            return diff;
        }

        return Vector3.zero;
    }
}