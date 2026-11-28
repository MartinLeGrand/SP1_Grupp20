using System;
using System.Collections.Generic;
using UnityEngine;

public class PickupScript : MonoBehaviour
{
    
    [SerializeField] private List<GameObject> heldObjects;
    [SerializeField] private List<Rigidbody> heldObjectRigidbodies;
    
    // Other scripts can get these values to check if anything is being held or if a specific object is held
    public bool IsHolding => heldObjects.Count > 0;
    public bool IsHeld(GameObject obj) => heldObjects.Contains(obj);
    
    private GameObject _holdTarget;
    
    [SerializeField] private float lerpSpeed = 12f;
    [SerializeField] private float maxSpeed = 20f;

    private void Awake()
    {
        _holdTarget = GameObject.Find("HoldTarget");
    }

    public void Pickup(GameObject heldObject)
    {
        if (heldObject == null || heldObjects.Contains(heldObject))
            return;

        if (heldObject.TryGetComponent<IHoldTarget>(out var holdTarget))
        {
            heldObjects.Add(heldObject);
            heldObjectRigidbodies.Add(heldObject.GetComponent<Rigidbody>());
            holdTarget.OnPickup();
        }
    }

    public void Drop(GameObject heldObject)
    {
        if (heldObject != null && heldObject.TryGetComponent<IHoldTarget>(out var holdTarget))
        {
            heldObjects.Remove(heldObject);
            heldObjectRigidbodies.Remove(heldObject.GetComponent<Rigidbody>());
            holdTarget.OnDrop();
        }
    }

    public void DropAll()
    {
        // Iterate backwards so we can safely modify the list
        for (int i = heldObjects.Count - 1; i >= 0; i--)
        {
            GameObject obj = heldObjects[i];
            if (obj != null && obj.TryGetComponent<IHoldTarget>(out var holdTarget))
            {
                holdTarget.OnDrop();
            }
        }

        heldObjects.Clear();
        heldObjectRigidbodies.Clear();
    }
    
    private void FixedUpdate()
    {
        LerpObjectsToTarget();
    }

    private void LerpObjectsToTarget()
    {
        for (int i = heldObjectRigidbodies.Count - 1; i >= 0; i--)
        {
            Rigidbody rb = heldObjectRigidbodies[i];
            if (rb == null)
            {
                heldObjectRigidbodies.RemoveAt(i);
                continue;
            }

            Vector3 toTarget = _holdTarget.transform.position - rb.position;
            Vector3 desiredVelocity = Vector3.ClampMagnitude(toTarget * lerpSpeed, maxSpeed);

            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, desiredVelocity, lerpSpeed * Time.fixedDeltaTime);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
