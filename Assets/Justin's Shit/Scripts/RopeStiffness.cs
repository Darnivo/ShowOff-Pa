using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteAlways]
public class RopeStiffness : MonoBehaviour
{
    [Header("Angle Limits (in degrees)")]
    public float maxAngleTop = 90f;
    public float maxAngleBottom = 10f;

    // Store each segment's rest pose
    private Dictionary<Transform, Vector3> _initialLocalPositions = new Dictionary<Transform, Vector3>();
    private Dictionary<Transform, Quaternion> _initialLocalRotations = new Dictionary<Transform, Quaternion>();

    void Awake()
    {
        // Find all hinge segments (assume each segment has a Rigidbody + HingeJoint)
        var joints = GetComponentsInChildren<HingeJoint>(true);
        foreach (var j in joints)
        {
            Transform t = j.transform;
            _initialLocalPositions[t] = t.localPosition;
            _initialLocalRotations[t] = t.localRotation;
        }
    }

    void OnEnable()
    {
        // 1) Restore each transform to its recorded rest pose
        foreach (var kv in _initialLocalPositions)
        {
            Transform seg = kv.Key;
            seg.localPosition = kv.Value;
            seg.localRotation = _initialLocalRotations[seg];
        }

        // 2) Clear any leftover motion & wake up (even if kinematic)
        foreach (var rb in GetComponentsInChildren<Rigidbody>(true))
        {
            bool wasKin = rb.isKinematic;
            rb.isKinematic = false;         // temporarily allow velocity changes
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.WakeUp();
            rb.isKinematic = wasKin;        // restore original kinematic state
        }

        // 3) Re-apply your hinge limits so they hold the rest pose
        ApplyLimits();
    }

    void Start() => ApplyLimits();
    void OnValidate() => ApplyLimits();

    void ApplyLimits()
    {
        var joints = GetComponentsInChildren<HingeJoint>(true);
        if (joints.Length == 0) return;

        // sort by world-space Y (highest = index 0)
        var sorted = joints.OrderByDescending(j => j.transform.position.y).ToArray();
        int count = sorted.Length;

        for (int i = 0; i < count; i++)
        {
            float t = (float)i / (count - 1);
            float angle = Mathf.Lerp(maxAngleTop, maxAngleBottom, t);

            var limits = sorted[i].limits;
            limits.min = -angle;
            limits.max = angle;
            sorted[i].limits = limits;
            sorted[i].useLimits = true;
        }
    }
}
