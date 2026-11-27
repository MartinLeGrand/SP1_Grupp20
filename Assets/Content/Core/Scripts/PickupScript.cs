using System;
using System.Collections.Generic;
using UnityEngine;

public class PickupScript : MonoBehaviour
{
    [SerializeField] private List<GameObject> heldObjects;
    [SerializeField] private List<Rigidbody> heldObjectRigidbodies;
    private GameObject _holdTarget;
    
    [SerializeField] private float lerpSpeed = 12f;
    [SerializeField] private float maxSpeed = 20f;

    private void Awake()
    {
        _holdTarget = GameObject.Find("HoldTarget");
    }

    public void Pickup(GameObject heldObject)
    {
        heldObjects.Add(heldObject);
        heldObjectRigidbodies.Add(heldObject.GetComponent<Rigidbody>());
    }

    public void Drop(GameObject heldObject)
    {
        heldObjects.Remove(heldObject);
        heldObjectRigidbodies.Remove(heldObject.GetComponent<Rigidbody>());
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
