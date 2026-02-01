using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles raycasting for object interactions
/// Sends out a raycast when a button is pressed to detect interactable objects
/// </summary>
public class Interactor : MonoBehaviour
{
    [SerializeField]
    private float _castDistance = 5f;
    [SerializeField]
    private Vector3 _rayOffset = new Vector3(0, 1.5f, 0);

    private InputAction _interactAction;

    private void Awake()
    {
        // Get the actions from the current Input System
        _interactAction = InputSystem.actions.FindAction("Interact");
    }

    private void Update()
    {
        if (_interactAction.WasPressedThisFrame())
        {
            PerformInteraction();
        }
    }

    private void PerformInteraction()
    {
        var interactables = GetRaycastInteractables();

        interactables
            .Where(interactable => interactable.CanInteract())
            .ToList()
            .ForEach(interactable => interactable.Interact(this));
    }

    private IEnumerable<IInteractable> GetRaycastInteractables()
    {
        Transform sourceTransform = Camera.main != null ? Camera.main.transform : transform;
        Vector3 rayOrigin = sourceTransform.position;

        if (Camera.main == null)
        {
            rayOrigin += _rayOffset;
        }

        Ray ray = new Ray(rayOrigin, sourceTransform.forward);
        Debug.DrawRay(ray.origin, ray.direction * _castDistance, Color.green, 2.0f);

        RaycastHit[] hits = Physics.RaycastAll(ray, _castDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide);

        return hits
            .OrderBy(hit => hit.distance)
            .Where(hit => hit.collider.transform.root != transform.root)
            .SelectMany(hit => GetInteractablesFromHit(hit))
            .Distinct();
    }

    private IEnumerable<IInteractable> GetInteractablesFromHit(RaycastHit hit)
    {
        // First try to find interactables in the object's parent hierarchy (standard approach)
        var parentInteractables = hit.collider.GetComponentsInParent<IInteractable>();
        if (parentInteractables != null && parentInteractables.Length > 0)
        {
            return parentInteractables;
        }

        // Fallback: Check the entire root hierarchy (useful for complex prefabs like the Bus)
        return hit.collider.transform.root.GetComponentsInChildren<IInteractable>();
    }
}
