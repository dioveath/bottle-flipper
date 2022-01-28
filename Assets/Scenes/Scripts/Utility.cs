using UnityEngine;

public class Utility : MonoBehaviour
{

    public static Vector3 ScreenToWorld(Camera cam, Vector3 pos)
    {
        pos.z = cam.nearClipPlane;
        return cam.ScreenToWorldPoint(pos);
    }

    private float EaseOutCubic(float x)
    {
        return 1 - Mathf.Pow(1 - x, 3);
    }


}
