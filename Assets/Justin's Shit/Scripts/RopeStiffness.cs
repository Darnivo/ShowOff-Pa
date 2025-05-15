using System.Linq;
using UnityEngine;

[ExecuteAlways]
public class RopeStiffness : MonoBehaviour
{
    [Header("Angle Limits (in degrees)")]
    [Tooltip("Max swing angle at the top-most segment")]
    public float maxAngleTop = 90f;
    [Tooltip("Max swing angle at the bottom-most segment")]
    public float maxAngleBottom = 10f;

    void Start()
    {
        ApplyLimits();
    }

    // Always update in editor in case you tweak segments or values
    void OnValidate()
    {
        ApplyLimits();
    }

    void ApplyLimits()
    {
        // find every hinge joint in children
        var joints = GetComponentsInChildren<HingeJoint>();

        if (joints.Length == 0) return;

        // sort by world-space Y (highest = index 0)
        var sorted = joints
            .OrderByDescending(j => j.transform.position.y)
            .ToArray();

        int count = sorted.Length;
        for (int i = 0; i < count; i++)
        {
            // t = 0 at top, 1 at bottom
            float t = (float)i / (count - 1);

            // interpolate between your top and bottom angles
            float angle = Mathf.Lerp(maxAngleTop, maxAngleBottom, t);

            // apply symmetric limits
            var limits = sorted[i].limits;
            limits.min = -angle;
            limits.max = angle;
            sorted[i].limits = limits;

            // make sure limits are enabled
            sorted[i].useLimits = true;
        }
    }
}
