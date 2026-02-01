using UnityEngine;
using UnityEngine.Splines;

public class BusSplineMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    private SplineAnimate splineAnimate;
    private UpgradeEventsController upgradeEventsController;

    private void Awake()
    {
        // Get the SplineAnimate component
        splineAnimate = GetComponent<SplineAnimate>();
        if (splineAnimate)
        {
            splineAnimate.AnimationMethod = SplineAnimate.Method.Speed;
            splineAnimate.MaxSpeed = 0f; // Start with speed 0
        }

        // Subscribe to truck move upgrade event
        upgradeEventsController = UpgradeEventsController.GetUpgradeEventsController();
        upgradeEventsController.onTruckMoveUpgrade += HandleTruckMoveUpgrade;
    }

    private void OnDestroy()
    {
        if (upgradeEventsController != null)
        {
            upgradeEventsController.onTruckMoveUpgrade -= HandleTruckMoveUpgrade;
        }
    }

    private void HandleTruckMoveUpgrade()
    {
        if (splineAnimate != null)
        {
            splineAnimate.MaxSpeed = moveSpeed;
            Debug.Log($"Bus started moving with speed: {moveSpeed}");
        }
    }
}
