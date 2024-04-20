using System;
using UnityEngine;

public class Lever : MonoBehaviour
{
    public LeverWall wall;
    public GameObject leverHandle;
    private bool flipping;
    private float startRotation;
    
    public void Start()
    {
        startRotation = leverHandle.transform.eulerAngles.z;
    }
    public void Hit()
    {
        if (flipping) return;
        wall.retracting = true;
        flipping = true;
        AudioManager.instance.PlayLeverActivate();
    }

    public void ResetPos()
    {
        wall.ResetPos();
        flipping = false;
        leverHandle.transform.eulerAngles = new Vector3(0, 0, startRotation);
    }

    private void FixedUpdate()
    {
        if (flipping)
        {
            float z = leverHandle.transform.localRotation.eulerAngles.z;
            float newZ = Mathf.MoveTowardsAngle(z, 45, 1f);
            leverHandle.transform.localRotation = Quaternion.Euler(0, 0, newZ);
        }
    }
}
