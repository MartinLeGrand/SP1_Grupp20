using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    void OnInteract(InputAction.CallbackContext context);
}

public class MouseInteractor : MonoBehaviour
{
    private Camera _camera;
    private Rigidbody _rigidbody;
    private IInteractable _activeInteractable;
    private Vector3 _mouseWorldPosition;
    
    [Header("Interaction Settings")]
    public bool followMouse = true;
    [Tooltip("Which collision layers block the mouse and along with that the collision sphere?")]
    [SerializeField] private LayerMask mouseBlockLayer;
    [SerializeField] private List<GameObject> interactionCandidates;
    [SerializeField] private GameObject currentInteractable;

    void Awake()
    {
        _camera = Camera.main;
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        currentInteractable = GetClosestInteractibleCandidate();
    }

    void FixedUpdate()
    {
        SetInteractionSpherePosition();
    }
    
    public void OnLeftClick(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _activeInteractable = null;

            if (currentInteractable != null && currentInteractable.TryGetComponent<IInteractable>(out var interactable))
            {
                _activeInteractable = interactable;
                _activeInteractable.OnInteract(context);
            }
        }
        else if (context.performed)
        {
            _activeInteractable?.OnInteract(context);
        }
        else if (context.canceled)
        {
            _activeInteractable?.OnInteract(context);
            _activeInteractable = null;
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        RaycastHit hit;
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit,  Mathf.Infinity, mouseBlockLayer))
        {
            _mouseWorldPosition = hit.point;
        }
        return _mouseWorldPosition;
    }
    
    void SetInteractionSpherePosition()
    {
        //gameObject.transform.position = GetMouseWorldPosition(); 
        _rigidbody.MovePosition(GetMouseWorldPosition());
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
