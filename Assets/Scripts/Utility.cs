using UnityEngine;

public static class Utility
{
    public static float getAngle(GameObject wall)
    {
        return wall.transform.rotation.eulerAngles.z;
    }
    public static bool IsWall(GameObject other)
    {
        return other.layer == LayerMask.NameToLayer("fwc") &&
               Mathf.Abs(90 - Mathf.Abs(getAngle(other)) % 180) < 10;
    }
    public static bool IsFloor(GameObject other)
    {
        return other.layer == LayerMask.NameToLayer("fwc") &&
               Mathf.Abs(getAngle(other)) < 1;
    }
}
