using UnityEngine;

public class Speedbag : MonoBehaviour
{
    [Header("Physics Settings")]
    [Tooltip("Force applied when the speedbag is hit")]
    [SerializeField] private float swingForce = 8f;
    [Tooltip("How quickly the speedbag returns to center position (higher = faster return)")]
    [SerializeField] private float returnSpeed = 15f;
    [Tooltip("Additional spring force pulling back to center")]
    [SerializeField] private float springForce = 20f;
    [Tooltip("Maximum angle (in degrees) the speedbag can rotate")]
    [SerializeField] private float maxRotationAngle = 35f;
    [Tooltip("Tag of the collider that can hit the speedbag (e.g., Player, Punch, Hitbox)")]
    [SerializeField] private string hitboxTag = "Player";

    [Header("Pivot Settings")]
    [Tooltip("Transform that acts as the rotation pivot point. Leave empty to use this GameObject")]
    [SerializeField] private Transform pivotPoint;

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
        ApplySpeedbagPhysics();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(hitboxTag))
        {
            //Debug.Log("hit speedbag");

            ApplyHitImpact(other);
        }
    }

    private void ApplyHitImpact(Collider hitbox)
    {
        Vector3 hitDirection = (transform.position - hitbox.transform.position).normalized;

        Vector3 forceDirection = new Vector3(/*hitDirection.x*/0, -hitDirection.y, /*hitDirection.z*/0).normalized;
        currentAngularVelocity += forceDirection * swingForce;
    }

    private void ApplySpeedbagPhysics()
    {
        Vector3 currentRotation = pivotPoint.localRotation.eulerAngles;
        currentRotation = new Vector3(
           /* NormalizeAngle(currentRotation.x)*/0,
            /*0f*/NormalizeAngle(currentRotation.y),
          /*  NormalizeAngle(currentRotation.z)*/0
        );

        Vector3 springBackForce = springForce * Time.deltaTime * -currentRotation.normalized;
        currentAngularVelocity += springBackForce;

        currentAngularVelocity = Vector3.Lerp(currentAngularVelocity, Vector3.zero, returnSpeed * Time.deltaTime);

        currentRotation += 50f * Time.deltaTime * currentAngularVelocity;

        //currentRotation.x = Mathf.Clamp(currentRotation.x, -maxRotationAngle, maxRotationAngle);
        currentRotation.y = Mathf.Clamp(currentRotation.y, -maxRotationAngle, maxRotationAngle);
        //currentRotation.z = Mathf.Clamp(currentRotation.z, -maxRotationAngle, maxRotationAngle);

        pivotPoint.localRotation = Quaternion.Euler(currentRotation);
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180f)
            angle -= 360f;
        return angle;
    }
}
