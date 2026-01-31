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
    
    public bool CanInteract()
    {
        return true;
    }

    public void Interact(Interactor interactor)
    {
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
