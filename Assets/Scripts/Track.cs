using UnityEngine;

public class Track : MonoBehaviour
{
    private float startHeight;
    void Start()
    {
        startHeight = GetComponent<SpriteRenderer>().size.y;
    }
    
    public void SetEndpoints(Vector2 start, Vector2 end)
    {
        Vector2 diff = end - start;
        float rotation = Vector2.SignedAngle(Vector2.up, diff);
        transform.position = start + diff / 2;
        transform.rotation = Quaternion.Euler(0, 0, rotation);
        GetComponent<SpriteRenderer>().size = new Vector2(1, diff.magnitude);
    }
}
