using System;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField, Range(0f, 1f)] private float airControl = 0.3f;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float jumpCooldown = 0.6f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance = 1.9f;
    [SerializeField] private LayerMask groundMask = ~0;
    [SerializeField] private float gravity = 9.81f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private GameObject camera;

    private CharacterController controller;
    private Vector2 input;
    private float jumpCooldownTimer;
    private bool jumpOnCooldown;
    private bool groundSnapEnabled = true;
    private UpgradeEventsController upgradeEventsController;
    private Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        upgradeEventsController = UpgradeEventsController.GetUpgradeEventsController();
        upgradeEventsController.onMoveSpeedUpgrade += UpgradeMoveSpeed;
    }

    private void Start()
    {
        if (!groundCheck)
        {
            groundCheck = transform;
        }
    }

    private void FixedUpdate()
    {
        HandleJumpCooldown();

        if (!groundSnapEnabled)
        {
            return;
        }

        Vector3 move = transform.right * input.x + transform.forward * input.y;
        move = Vector3.ClampMagnitude(move, 1f);
        Vector3 desiredHorizontal = move * moveSpeed;

        Vector3 currentHorizontal = new Vector3(velocity.x, 0f, velocity.z);

        bool grounded = controller.isGrounded;
        float control = grounded ? 1f : airControl;
        Vector3 newHorizontal = Vector3.Lerp(currentHorizontal, desiredHorizontal, control);

        velocity.x = newHorizontal.x;
        velocity.z = newHorizontal.z;
        
        // Apply gravity
        if (grounded && velocity.y < 0)
        {
            velocity.y = 0f;
        }
        else
        {
            velocity.y -= gravity * Time.deltaTime;
        }

        // Add bus momentum
        Vector3 busMomentum = ApplyVehicleMomentum();
        controller.Move((velocity + busMomentum) * Time.deltaTime);
    }

    public void OnMove(InputValue value)
    {
        input = value.Get<Vector2>();
    }

    private Vector3 ApplyVehicleMomentum()
    {
        RaycastHit hit;

        // Ignore "Ignore Raycast" (player) layer
        int player_layer = ~(1 << LayerMask.NameToLayer("Ignore Raycast"));

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 20f, player_layer, QueryTriggerInteraction.Ignore))
        {
            // TODO: Refactor to use tags or layers instead of name checks
            if (hit.transform.name.Contains("Bus") || hit.transform.name.Contains("Pickup"))
            {
                SplineAnimate splineAnimate = hit.transform.GetComponent<SplineAnimate>();
                if (splineAnimate != null)
                {
                    return hit.transform.forward * splineAnimate.MaxSpeed;
                }
            }
        }
        return Vector3.zero;
    }

    public void OnJump()
    {
        if (controller.isGrounded && !jumpOnCooldown)
        {
            velocity.y = jumpForce;
            jumpOnCooldown = true;
            jumpCooldownTimer = 0f; // Reset timer when jump starts
        }
    }


    public void DisableGroundSnap(float duration = 0.2f)
    {
        groundSnapEnabled = false;
        CancelInvoke(nameof(EnableGroundSnap));
        Invoke(nameof(EnableGroundSnap), duration);
    }

    private void EnableGroundSnap()
    {
        groundSnapEnabled = true;
    }

    private void HandleJumpCooldown()
    {
        if (!jumpOnCooldown) return;

        jumpCooldownTimer += Time.deltaTime;
        if (jumpCooldownTimer >= jumpCooldown)
        {
            jumpOnCooldown = false;
            jumpCooldownTimer = 0f;
        }
    }

    public void UpgradeMoveSpeed()
    {
        moveSpeed += 2f;
        Debug.Log("Player move speed upgraded to: " + moveSpeed);
    }
}
