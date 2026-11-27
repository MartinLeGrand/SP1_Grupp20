using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DiceBag : MonoBehaviour, IInteractable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private List<GameObject> dice;
    
    [Header("References")]
    [SerializeField] private Outline _outline;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnSecondaryInteract(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnBecomeCurrentInteractable(bool isInteractable)
    {
        if (_outline != null)
            _outline.enabled = isInteractable;
    }

    public void ShuffleDice()
    {
        throw new System.NotImplementedException();
    }
}
