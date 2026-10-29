using UnityEngine;

/// <summary>Cámara exclusiva de la escena demostrativa. No modifica SideScrolling original.</summary>
public sealed class FinalDemoCamera : MonoBehaviour
{
    public Transform target;
    public float minX = 0;
    public float maxX = 43;
    public float height = 2.5f;
    public float smoothness = 5f;

    private void LateUpdate()
    {
        if (target == null) return;
        Vector3 desired = new Vector3(Mathf.Clamp(target.position.x + 2.4f, minX, maxX), height, -10f);
        transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * smoothness);
    }
}
