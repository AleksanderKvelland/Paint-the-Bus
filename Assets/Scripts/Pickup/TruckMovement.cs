using UnityEngine;
using UnityEngine.Splines;

public class TruckMovement : MonoBehaviour
{
    public Transform targetPoint;
    public float distanceThreshold = 50f;
    public float loopingSpeed = 50f;
    public float drivebySpeed = 3f;
    private SplineAnimate splineAnimate;
    private bool isDriveby;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        splineAnimate = GetComponent<SplineAnimate>();
        if (splineAnimate)
        {
            splineAnimate.AnimationMethod = SplineAnimate.Method.Speed;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!targetPoint) return;

        float distance = Vector3.Distance(transform.position, targetPoint.position);
        bool shouldDriveby = distance < distanceThreshold;
        if (shouldDriveby != isDriveby)
        {
            isDriveby = shouldDriveby;
            SetSpeed(isDriveby ? drivebySpeed : loopingSpeed);
        }
    }

    private void SetSpeed(float newSpeed)
    {
        if (!splineAnimate) return;

        float normalizedTime = splineAnimate.NormalizedTime;
        splineAnimate.MaxSpeed = newSpeed;
        splineAnimate.NormalizedTime = normalizedTime;
    }
}
