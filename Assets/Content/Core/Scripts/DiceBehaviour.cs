using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]

public class DiceBehaviour : MonoBehaviour, IInteractable, IHoldTarget
{
    [Serializable]
    public struct DieFaces
    {
        public int value;
        public Vector3 localNormal;
    }
    
    [Header("References")]
    [SerializeField] private PhysicsMaterial diceMaterial;
    [SerializeField] private PhysicsMaterial tableMaterial;
    
    [Header("Variables")]
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

    [Header("Audio")]
    [SerializeField] private AudioClip[] collisionAudioDieOnDie;
    [SerializeField] private AudioClip[] collisionAudioDieOnTable;
    [SerializeField] private AudioClip[] throwAudioClips;
    
    [Range(0f, 3f)]
    [SerializeField] private float minPitch =  0.5f;
    [Range(0f, 3f)]
    [SerializeField] private float maxPitch = 1f;
    
    [Tooltip("This speed maps to the volume below, at minImpactSpeed the audio will be minVolume, at maxImpactSpeed the audio will be maxVolume.")]
    [Range(0f, 0.5f)]
    [SerializeField] private float minImpactSpeed = 0.5f;
    [Tooltip("This speed maps to the volume below, at minImpactSpeed the audio will be minVolume, at maxImpactSpeed the audio will be maxVolume.")]
    [Range(0f, 10f)]
    [SerializeField] private float maxImpactSpeed = 8f;
    
    [Tooltip("THIS IS NOT A RANDOM VOLUME, This changes the volume of the audio based on impact speed.")]
    [Range(0f, 1f)]
    [SerializeField] private float minVolume = 0.05f;
    [Tooltip("THIS IS NOT A RANDOM VOLUME, This changes the volume of the audio based on impact speed.")]
    [Range(0f, 2f)]
    [SerializeField] private float maxVolume = 1f;
    
    [SerializeField] private float minSoundInterval = 0.05f;

    [Header("Interaction rotation settings")]
    [SerializeField] private float torqueOnMove = 0.25f;
    [SerializeField] private float maxAngularSpeed = 12f;
    
    [Header("Layers")]
    [SerializeField] private string heldLayerName = "HeldDice";

    // Other GameObjects in the scene
    private MouseInteractor _mouseInteractor;
    private PickupScript _pickupScript;
    private DiceManager _diceManager;
    
    // Other components on the dice
    private Outline _outline;
    private Rigidbody _rb;
    private AudioSource _audioSource;
    
    // Private variables
    private int _topFaceValue = -1;
    private Boolean _isHeld = false;
    private float _lastDieSoundTime = -10f;
    private int _defaultLayer;
    private int _heldLayer;

    
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
        _outline = GetComponent<Outline>();

        
        //Make DAMN sure there is never another one of these fuckass scripts in the world or ill bite your face off
        _mouseInteractor = FindFirstObjectByType<MouseInteractor>();
        _pickupScript = FindFirstObjectByType<PickupScript>();
        _diceManager = FindFirstObjectByType<DiceManager>();
        
        
        // Add itself to the dice manager list of dice
        _diceManager.AddDice(gameObject);
        
        // Collision layers saved
        _defaultLayer = gameObject.layer;
        _heldLayer = LayerMask.NameToLayer(heldLayerName);
    }

    private void FixedUpdate()
    {
        AddSpin();
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
    
    private void OnCollisionEnter(Collision collision)
    {
        float impactSpeed = collision.relativeVelocity.magnitude;
        
        // If collision speed is too low, we do nothing.
        // THIS IS SAFE AS LONG AS COLLISIONS ARE ONLY USED FOR AUDIO
        if (impactSpeed < minImpactSpeed)
            return;
        
        float normalizedSpeed = Mathf.InverseLerp(minImpactSpeed, maxImpactSpeed, impactSpeed);
        float volume = Mathf.Lerp(minVolume, maxVolume, normalizedSpeed);
        
        PhysicsMaterial hitMat = collision.collider.sharedMaterial;

        if (hitMat == diceMaterial)
        {
            if (collision.gameObject.TryGetComponent<DiceBehaviour>(out DiceBehaviour otherDie))
            {
                // Since we only want one die to play the audio we compare the unique instance ID's and play the sound on the higher one
                if (GetInstanceID() < otherDie.GetInstanceID())
                    return;
            }

            // Have less overlapping audio bits for when there is a bunch of collisions quickly
            if (Time.time - _lastDieSoundTime < minSoundInterval)
                return;

            _lastDieSoundTime = Time.time;
            PlayRandomClip(collisionAudioDieOnDie, volume);
        }
        else if (hitMat == tableMaterial)
        {
            if (Time.time - _lastDieSoundTime < minSoundInterval)
                return;

            _lastDieSoundTime = Time.time;
            PlayRandomClip(collisionAudioDieOnTable, volume);
        }
    }
    private void OnDestroy()
    {
        _diceManager.RemoveDice(gameObject);
    }
    
    void IInteractable.OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log(GetTopFaceValue());
        if (context.performed)
        {
            _pickupScript.Pickup(gameObject);
        }

        if (context.canceled)
        {
            _pickupScript.DropAll();
        }
    }

    void IInteractable.OnSecondaryInteract(InputAction.CallbackContext context)
    {
        if (context.performed && _pickupScript.IsHolding)
        {
            _pickupScript.Pickup(gameObject);

        }
    }

    void IInteractable.OnBecomeCurrentInteractable(bool isNewInteractable)
    {
        if (_outline != null)
            _outline.enabled = isNewInteractable;
    }

    void IHoldTarget.OnPickup()
    {
        _isHeld = true;
        SetLayerRecursive(gameObject, _heldLayer);
    }

    void IHoldTarget.OnDrop()
    {
        //PlayRandomClip(throwAudioClips, 1);
        _rb.rotation = UnityEngine.Random.rotation;
        _isHeld = false;
        SetLayerRecursive(gameObject, _defaultLayer);
    }
    
    // Helper function to get random audio clip from an array

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
        _topFaceValue = currentTopFaceValue;
        return currentTopFaceValue;
    }

    private void PlayRandomClip(AudioClip[] clips, float volume)
    {
        if (clips == null || clips.Length == 0)
            return;
        
        _audioSource.pitch = Random.Range(minPitch, maxPitch);
        
        _audioSource.PlayOneShot(clips[Random.Range(0, clips.Length)], Mathf.Clamp01(volume));
    }
    private void AddSpin()
    {
        if (!_isHeld || _rb.IsSleeping())
            return;

        Vector3 flatVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
        float speed = flatVelocity.magnitude;

        if (speed < 0.01f)
            return;

        Vector3 velocityRight = Vector3.Cross(Vector3.up, flatVelocity / speed).normalized;

        // Scales linearly with horizontal speed (m/s)
        _rb.AddTorque(velocityRight * (speed * torqueOnMove), ForceMode.Force);

        // Cap maximum angular speed (rad/s)
        if (_rb.angularVelocity.sqrMagnitude > maxAngularSpeed * maxAngularSpeed)
        {
            _rb.angularVelocity = _rb.angularVelocity.normalized * maxAngularSpeed;
        }
    }
    
    //Debug function that tosses the dice up to randomize the value somewhat
    public void TossDice(float forceMagnitude, float torqueMagnitude)
    {
        _rb.AddForce(Vector3.up * forceMagnitude, ForceMode.Impulse);
        _rb.AddTorque(UnityEngine.Random.onUnitSphere * torqueMagnitude, ForceMode.Impulse);
    }
    
    // Helper function for setting the layers of objects under the die, so that the mouse doesn't collide with held dice
    private static void SetLayerRecursive(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            SetLayerRecursive(child.gameObject, layer);
    }


}
