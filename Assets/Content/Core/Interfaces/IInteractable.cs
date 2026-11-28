using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    void OnInteract(InputAction.CallbackContext context);
    void OnSecondaryInteract(InputAction.CallbackContext context);
    void OnBecomeCurrentInteractable(bool isInteractable);
}
