using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class MouseInteractor : MonoBehaviour
{
    [Header("Interaction Settings")]
    public bool followMouse = true;
    [Tooltip("Which collision layers block the mouse and along with that the collision sphere?")]
    [SerializeField] private LayerMask mouseBlockLayer;
    [SerializeField] private List<GameObject> interactionCandidates;
    [SerializeField] private GameObject currentInteractable;
    
    private Camera _camera;
    private Rigidbody _rigidbody;
    private PickupScript _pickupScript;
    private IInteractable _activeLeftInteractable;
    private IInteractable _activeRightInteractable;
    
    private IInteractable _currentInteractableComponent;
    
    private Vector3 _mouseWorldPosition;

    void Awake()
    {
        _camera = Camera.main;
        _rigidbody = GetComponent<Rigidbody>();
        _pickupScript = GetComponent<PickupScript>();
    }

    void Update()
    {
        ApplyClosestCandidate();
    }

    void FixedUpdate()
    {
        SetInteractionSpherePosition();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IInteractable>(out var interactable))
        {
            interactionCandidates.Add(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (interactionCandidates.Contains(other.gameObject))
        {
            interactionCandidates.Remove(other.gameObject);
        }
    }
    
    public void OnLeftClick(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _activeLeftInteractable = null;

            if (currentInteractable != null &&
                currentInteractable.TryGetComponent<IInteractable>(out var interactable))
            {
                _activeLeftInteractable = interactable;
                _activeLeftInteractable.OnInteract(context);
            }
        }
        else if (context.performed)
        {
            _activeLeftInteractable?.OnInteract(context);
        }
        else if (context.canceled)
        {
            _activeLeftInteractable?.OnInteract(context);
            _activeLeftInteractable = null;
        }
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _activeRightInteractable = null;

            if (currentInteractable != null &&
                currentInteractable.TryGetComponent<IInteractable>(out var interactable))
            {
                _activeRightInteractable = interactable;
                _activeRightInteractable.OnSecondaryInteract(context);
            }
        }
        else if (context.performed)
        {
            _activeRightInteractable?.OnSecondaryInteract(context);
        }
        else if (context.canceled)
        {
            _activeRightInteractable?.OnSecondaryInteract(context);
            _activeRightInteractable = null;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        RaycastHit hit;
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit,  Mathf.Infinity, mouseBlockLayer))
        {
            _mouseWorldPosition = hit.point;
        }
        return _mouseWorldPosition;
    }
    
    private void SetInteractionSpherePosition()
    {
        //gameObject.transform.position = GetMouseWorldPosition(); 
        _rigidbody.MovePosition(GetMouseWorldPosition());
    }

    private void ApplyClosestCandidate()
    {
        GameObject newCandidate = GetClosestInteractibleCandidate();

        // Nothing changed
        if (newCandidate == currentInteractable)
            return;

        // Turn off the old one (may be null or already destroyed)
        if (currentInteractable != null && _currentInteractableComponent != null)
            _currentInteractableComponent.OnBecomeCurrentInteractable(false);

        currentInteractable = newCandidate;
        _currentInteractableComponent = null;

        // Turn on the new one (newCandidate may be null, which is fine)
        if (newCandidate != null &&
            newCandidate.TryGetComponent<IInteractable>(out var interactable))
        {
            _currentInteractableComponent = interactable;
            interactable.OnBecomeCurrentInteractable(true);
        }
    }
    
    private GameObject GetClosestInteractibleCandidate()
    {
        float closestDistance = float.MaxValue;
        GameObject closestCandidate = null;
        
        foreach (var candidate in interactionCandidates)
        {
            float testDistance = Vector3.Distance(transform.position, candidate.transform.position);
            if (testDistance < closestDistance)
            {
                closestDistance = testDistance;
                closestCandidate = candidate;
            }
        }
        return closestCandidate;
    }
}
