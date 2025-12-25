using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;  // Set this to CameraHolder
    [SerializeField] private float followSpeed = 20f;

    private void LateUpdate()
    {
        if (target == null) return;

        // Smooth position and rotation following
        // transform.position = Vector3.Lerp(transform.position, target.position, followSpeed * Time.deltaTime);
        // transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, followSpeed * Time.deltaTime);
    }
}
