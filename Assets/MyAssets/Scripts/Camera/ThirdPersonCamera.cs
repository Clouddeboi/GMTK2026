using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public PlayerInputHandler input;

    [Header("Orbit")]
    public float distance = 5.5f;
    public float minDistance = 1.2f;
    public float height = 1.6f;
    public float mouseSensitivity = 0.15f;
    public float gamepadSensitivity = 120f;
    public float minPitch = -30f, maxPitch = 60f;

    [Header("Smoothing")]
    public float pivotFollowSpeed = 18f;
    public float distanceSmoothSpeed = 14f;

    [Header("Collision")]
    public LayerMask collisionMask;
    public float collisionRadius = 0.25f;

    float yaw, pitch = 15f;
    Vector3 smoothedPivot;
    float currentDistance;
    bool initialized;

    void LateUpdate()
    {
        Vector2 look = input.LookInput;
        if (input.IsUsingGamepad)
        {
            yaw += look.x * gamepadSensitivity * Time.deltaTime;
            pitch -= look.y * gamepadSensitivity * Time.deltaTime;
        }
        else
        {
            yaw += look.x * mouseSensitivity;
            pitch -= look.y * mouseSensitivity;
        }
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Vector3 targetPivot = target.position + Vector3.up * height;
        if (!initialized)
        {
            smoothedPivot = targetPivot;
            currentDistance = distance;
            initialized = true;
        }
        smoothedPivot = Vector3.Lerp(smoothedPivot, targetPivot, 1f - Mathf.Exp(-pivotFollowSpeed * Time.deltaTime));

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 castDir = -(rot * Vector3.forward);

        float desiredDistance = distance;
        if (Physics.SphereCast(smoothedPivot, collisionRadius, castDir,
                out RaycastHit hit, distance, collisionMask, QueryTriggerInteraction.Ignore))
        {
            desiredDistance = Mathf.Max(minDistance, hit.distance);
        }
        currentDistance = Mathf.Lerp(currentDistance, desiredDistance, 1f - Mathf.Exp(-distanceSmoothSpeed * Time.deltaTime));

        transform.position = smoothedPivot + castDir * currentDistance;
        transform.rotation = rot;
    }

    public Vector3 GetCameraForward()
    {
        Vector3 f = transform.forward; f.y = 0; return f.normalized;
    }

    public Vector3 GetCameraRight()
    {
        Vector3 r = transform.right; r.y = 0; return r.normalized;
    }
}