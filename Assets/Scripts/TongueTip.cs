using UnityEngine;

public class TongueTip : MonoBehaviour
{
    void Update()
    {
        transform.localPosition = new Vector3(0, 0.5f, 0);
        LayerMask mask = LayerMask.GetMask("fwc");
        var hit = Physics2D.CircleCast(transform.position, 0.1f, Vector2.zero, Mathf.Infinity, layerMask:mask);
        if (hit.collider != null)
        {
            //Debug.Log("HIT " + hit.collider.gameObject.name);
            Frog.instance.OnTongueCollide(hit.collider.gameObject);
        }
    }
}
