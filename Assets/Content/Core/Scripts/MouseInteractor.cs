using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    void OnInteract(InputAction.CallbackContext context);
    void OnSecondaryInteract(InputAction.CallbackContext context);
}

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
    
    
    private Vector3 _mouseWorldPosition;

    void Awake()
    {
        _camera = Camera.main;
        _rigidbody = GetComponent<Rigidbody>();
        _pickupScript = GetComponent<PickupScript>();
    }

    void Update()
    {
        currentInteractable = GetClosestInteractibleCandidate();
    }

    void FixedUpdate()
    {
        SetInteractionSpherePosition();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
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
