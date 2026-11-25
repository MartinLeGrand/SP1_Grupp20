using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseInteractor : MonoBehaviour
{
    private Camera _camera;
    private Rigidbody _rigidbody;
    private ConfigurableJoint _configurableJoint;
    
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
        _configurableJoint = GetComponent<ConfigurableJoint>();
    }

    void Update()
    {
        currentInteractable = GetClosestInteractibleCandidate();
    }

    void FixedUpdate()
    {
        SetInteractionSpherePosition();

    }
    
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started)
        {
            return;
        }

        if (_configurableJoint.connectedBody != null)
        {
            _configurableJoint.connectedBody = null;
            Debug.Log("Dropped object.");
            return;
        }
        
        if (currentInteractable == null)
        {
            return;
        }

        //if (!currentInteractable.TryGetComponent<Rigidbody>(out var rb))
        //{
        //    Debug.LogWarning($"Candidate '{currentInteractable.name}' has no Rigidbody.", currentInteractable);
        //    return;
        //}

        _configurableJoint.connectedBody = currentInteractable.GetComponent<Rigidbody>();
        Debug.Log($"Picked up {currentInteractable.name}", currentInteractable);
    }

    Vector3 GetMouseWorldPosition()
    {
        RaycastHit hit;
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit,  Mathf.Infinity, mouseBlockLayer))
        {
            return hit.point;
        }
        return Vector3.zero;
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
