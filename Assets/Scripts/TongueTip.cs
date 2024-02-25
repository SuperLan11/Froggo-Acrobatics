using UnityEngine;

public class TongueTip : MonoBehaviour
{
    public bool preview = false;
    void Update()
    {
        transform.localPosition = new Vector3(0, preview ? 0.6f : 0.5f , 0);
        if (preview)
        {
            return;
        }
        LayerMask mask = LayerMask.GetMask("fwc");
        var hit = Physics2D.CircleCast(transform.position, 0.4f, Vector2.zero, Mathf.Infinity, layerMask:mask);
        if (hit.collider != null)
        {
            Debug.Log(hit.normal.x + " " + hit.normal.y);
            //Debug.Log("HIT " + hit.collider.gameObject.name);
            Frog.instance.OnTongueCollide(hit.collider.gameObject);
        }
    }
}
