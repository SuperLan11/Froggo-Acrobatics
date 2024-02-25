using System;
using UnityEngine;

public class CameraZone : MonoBehaviour
{
    public GameObject offsetMarker;
    private void OnTriggerEnter2D(Collider2D col)
    {
        var camera = col.gameObject.GetComponent<GameCamera>();
        camera.SetOffset(offsetMarker.transform.localPosition);
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        var camera = col.gameObject.GetComponent<GameCamera>();
        camera.ResetOffset();
    }
}
