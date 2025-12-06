using UnityEngine;

public class Punchbag : MonoBehaviour
{
    [Header("Pendulum Physics")]
    [Tooltip("Force applied to the pendulum when punched")]
    [SerializeField] private float punchForce = 10f;
    [Tooltip("Gravity strength affecting the pendulum swing")]
    [SerializeField] private float gravity = 9.81f;
    [Tooltip("Air resistance factor (0.95-0.99 recommended). Lower = faster slowdown")]
    [SerializeField] private float dampingFactor = 0.98f;
    [Tooltip("Maximum angle (in degrees) the punchbag can swing")]
    [SerializeField] private float maxSwingAngle = 30f;
    [Tooltip("Tag of the collider that can hit the punchbag (e.g., Player, Punch, Hitbox)")]
    [SerializeField] private string hitboxTag = "Player";

    [Header("Pivot Settings")]
    [Tooltip("Transform that acts as the rotation pivot point. Leave empty to use this GameObject")]
    [SerializeField] private Transform pivotPoint;
    [Tooltip("Distance from pivot to center of mass (affects swing speed)")]
    [SerializeField] private float pendulumLength = 2f;

    private Vector3 currentAngularVelocity = Vector3.zero;
    private Quaternion initialRotation;

    private void Start()
    {
        if (pivotPoint == null)
            pivotPoint = transform;

        initialRotation = pivotPoint.localRotation;
    }

    private void Update()
    {
        ApplyPendulumPhysics();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(hitboxTag))
        {
            //Debug.Log("hit punchbag");
            ApplyPunchImpact(other);
        }
    }
    private void ApplyPunchImpact(Collider hitbox)
    {
        Vector3 hitPoint = hitbox.ClosestPoint(transform.position);
        Vector3 hitDirection = (transform.position - hitPoint).normalized;

        Vector3 horizontalForce = new Vector3(hitDirection.x, 0f, hitDirection.z);

        currentAngularVelocity.x += horizontalForce.z * punchForce;
        currentAngularVelocity.z += -horizontalForce.x * punchForce;

        //Debug.Log($"Hit Direction: {hitDirection}");
        //Debug.Log($"Horizontal Force: {horizontalForce}");
        //Debug.Log($"Angular Velocity After Hit: {currentAngularVelocity}");
    }
    private void ApplyPendulumPhysics()
    {
        Vector3 currentRotation = pivotPoint.localRotation.eulerAngles;
        currentRotation = new Vector3(
            NormalizeAngle(currentRotation.x),
            NormalizeAngle(currentRotation.y),
            NormalizeAngle(currentRotation.z)
        );

        Vector3 gravityTorque = new Vector3(-Mathf.Sin(currentRotation.x * Mathf.Deg2Rad), 0f, -Mathf.Sin(currentRotation.z * Mathf.Deg2Rad)) * (gravity / pendulumLength);

        currentAngularVelocity += gravityTorque * Time.deltaTime * 50f;
        currentAngularVelocity *= dampingFactor;

        currentRotation += 100f * Time.deltaTime * currentAngularVelocity;

        currentRotation.x = Mathf.Clamp(currentRotation.x, -maxSwingAngle, maxSwingAngle);
        currentRotation.z = Mathf.Clamp(currentRotation.z, -maxSwingAngle, maxSwingAngle);
        currentRotation.y = 0f;

        if (currentAngularVelocity.magnitude > 0.01f)
        {
            //Debug.Log($"Current Rotation: {currentRotation}, Angular Vel: {currentAngularVelocity}");
        }

        pivotPoint.localRotation = Quaternion.Euler(currentRotation);
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180f)
            angle -= 360f;
        return angle;
    }
}
