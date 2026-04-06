using System.Collections.Generic;
using UnityEngine;

public class PhysicsHand : MonoBehaviour
{
    public List<Transform> trackedBones;
    public List<Rigidbody> physicsBones;

    public float positionStrength = 700f;
    public float rotationStrength = 600f;
    public float damping = 40f;

    [Header("Debug Master Hand Visualization")]
    public bool enableMasterHandDebug = false;
    public Renderer masterHandRenderer;
    public float errorThreshold = 0.02f;

    void FixedUpdate()
    {
        int count = Mathf.Min(trackedBones.Count, physicsBones.Count);

        float maxError = 0f;

        for (int i = 0; i < count; i++)
        {
            Transform source = trackedBones[i];
            Rigidbody rb = physicsBones[i];

            if (!source || !rb)
                continue;

            FollowPosition(source, rb);
            FollowRotation(source, rb);

            if (enableMasterHandDebug)
            {
                float error = Vector3.Distance(source.position, rb.position);

                if (error > maxError)
                    maxError = error;
            }
        }

        if (enableMasterHandDebug && masterHandRenderer)
        {
            masterHandRenderer.enabled = maxError > errorThreshold;
        }
    }

    void FollowPosition(Transform target, Rigidbody rb)
    {
        Vector3 delta = target.position - rb.position;

        Vector3 force =
            delta * positionStrength
            - rb.velocity * damping;

        rb.AddForce(force, ForceMode.Acceleration);
    }

    void FollowRotation(Transform target, Rigidbody rb)
    {
        Quaternion delta =
            target.rotation * Quaternion.Inverse(rb.rotation);

        delta.ToAngleAxis(out float angle, out Vector3 axis);

        if (angle > 180f)
            angle -= 360f;

        Vector3 torque =
            axis * angle * Mathf.Deg2Rad * rotationStrength
            - rb.angularVelocity * damping;

        rb.AddTorque(torque, ForceMode.Acceleration);
    }
}