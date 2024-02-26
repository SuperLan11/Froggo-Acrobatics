using UnityEngine;

public class Lever : MonoBehaviour
{
    public LeverWall wall;

    public void Hit()
    {
        wall.enabled = true;
        GetComponent<SpriteRenderer>().flipX = true;
    }
}
