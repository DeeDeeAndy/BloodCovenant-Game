using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float distance = 5f;
    public float height = 2f;
    public float smoothSpeed = 10f;
    public float rotationSmoothSpeed = 5f;

    private float currentYaw;

    void Start()
    {
        if (target != null)
            currentYaw = target.eulerAngles.y;
    }

    void LateUpdate()
    {
        if (target == null) return;

        currentYaw = Mathf.LerpAngle(currentYaw, target.eulerAngles.y, rotationSmoothSpeed * Time.deltaTime);

        Vector3 offset = Quaternion.Euler(0, currentYaw, 0) * new Vector3(0, height, -distance);
        Vector3 desiredPosition = target.position + offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}