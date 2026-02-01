using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickupTruck : MonoBehaviour, IInteractable
{
    
    public event Action onExitPickup;
    [SerializeField] private GameObject pickupInventory;
    private InputAction exitAction;
    private PlayerMovement playerMovement;
    private PlayerLook playerLook;
    private bool playerInsidePickup = true;

    public bool CanInteract()
    {
        return playerInsidePickup;
    }

    public void Interact(Interactor interactor)
    {
        Debug.Log("Attempting to interact with the truck pickup.");
        if (!playerInsidePickup)
            return;
        Debug.Log("Interacted with the truck pickup.");
        pickupInventory.SetActive(true);
        
        // Disable player movement
        if (playerMovement == null)
        {
            playerMovement = FindFirstObjectByType<PlayerMovement>();
            playerLook = FindFirstObjectByType<PlayerLook>();
        }
        
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
            playerLook.enabled = false;
        }
        
        // Show cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        exitAction = InputSystem.actions.FindAction("ExitPickup");
        onExitPickup += HandleExitPickup;
    }

    // Update is called once per frame
    void Update()
    {
        if (exitAction.WasPressedThisFrame())
        {
            onExitPickup?.Invoke();
        }
    }

    void HandleExitPickup()
    {
        Debug.Log("Exited the truck pickup area.");
        if (pickupInventory != null)
        {
            pickupInventory.SetActive(false);
        }
        
        // Re-enable player movement
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
        
        if (playerLook != null)
        {
            playerLook.enabled = true;
        }
        
        // Hide cursor and lock it
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
