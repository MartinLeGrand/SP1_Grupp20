using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

[RequireComponent(typeof(Rigidbody))]
public class DiceBehaviour : MonoBehaviour, IInteractable
{
    [Serializable]
    public struct DieFaces
    {
        public int value;
        public Vector3 localNormal;
    }
    
    [SerializeField]
    private DieFaces[] faces = new DieFaces[]
    {
        new DieFaces {value = 1, localNormal = Vector3.up},
        new DieFaces {value = 6, localNormal = Vector3.down},
        new DieFaces {value = 2, localNormal = Vector3.right},
        new DieFaces {value = 5, localNormal = Vector3.left},
        new DieFaces {value = 3, localNormal = Vector3.forward},
        new DieFaces {value = 4, localNormal = Vector3.back}
    };

    private MouseInteractor _mouseInteractor;
    private PickupScript _pickupScript;
    
    private DiceManager _diceManager;
    
    private Rigidbody _rb;
    public int topFaceValue = -1;
    private Boolean _isHeld = false;

    [SerializeField] private float torqueOnMove = 0.1f;
    [SerializeField] private float maxTorqueOnMove = 5f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        
        //Make DAMN sure there is never another one of these fuckass scripts in the world or ill bite your face off
        _mouseInteractor = FindFirstObjectByType<MouseInteractor>();
        _pickupScript = FindFirstObjectByType<PickupScript>();
        _diceManager = FindFirstObjectByType<DiceManager>();
        
        
        // Add itself to the dice manager list of dice
        _diceManager.AddDice(gameObject);
    }

    private void OnDestroy()
    {
        _diceManager.RemoveDice(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TossDice(UnityEngine.Random.Range(2f, 4f), UnityEngine.Random.Range(5f, 10f));
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(GetTopFaceValue());
        }
    }

    private void FixedUpdate()
    {
        AddSpin();
    }

    public int GetTopFaceValue()
    {
        float maxDot = float.MinValue;
        int currentTopFaceValue = -1;
        foreach (var face in faces)
        {
            Vector3 worldNormal = transform.TransformDirection(face.localNormal);
            float dot = Vector3.Dot(worldNormal, Vector3.up);
            if (dot > maxDot)
            {
                maxDot = dot;
                currentTopFaceValue = face.value;
            }
        }
        return currentTopFaceValue;
    }

    private void AddSpin()
    {
        if (!_isHeld || _rb.IsSleeping())
        {
            return;
        }
        
        Vector3 flatVelocity = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);

        if (_isHeld)
        {
            Vector3 velocityRight = Vector3.Cross(new Vector3(0, 1, 0), _rb.linearVelocity.normalized).normalized;
            _rb.AddTorque(velocityRight * torqueOnMove, ForceMode.Impulse);
        }
    }

    void IInteractable.OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log(GetTopFaceValue());
        if (context.performed)
        {
            _pickupScript.Pickup(gameObject);
            _isHeld = true;
        }

        if (context.canceled)
        {
            _pickupScript.Drop(gameObject);
            _isHeld = false;
        }
    }
    
    //Debug function that tosses the dice up to randomize the value somewhat
    public void TossDice(float forceMagnitude, float torqueMagnitude)
    {
        _rb.AddForce(Vector3.up * forceMagnitude, ForceMode.Impulse);
        _rb.AddTorque(UnityEngine.Random.onUnitSphere * torqueMagnitude, ForceMode.Impulse);
    }


}
