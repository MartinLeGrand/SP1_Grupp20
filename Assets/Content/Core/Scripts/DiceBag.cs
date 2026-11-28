using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DiceBag : MonoBehaviour, IInteractable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private List<GameObject> dice;
    
    [Header("References")]
    [SerializeField] private Outline _outline;

    // Other GameObjects in the scene
    private PickupScript _pickupScript;
    
    // Other components on the dice
    private GameObject _diceSpawnPoint;
    void Awake()
    {
        _diceSpawnPoint = GameObject.Find("DiceSpawnPoint");
        
        
        //Make DAMN sure there is never another one of these fuckass scripts in the world or ill bite your face off
        _pickupScript = FindFirstObjectByType<PickupScript>();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
            DrawDieFromBag();
        else if (context.canceled)
            _pickupScript.DropAll();
    }

    public void OnSecondaryInteract(InputAction.CallbackContext context)
    {
        if (context.performed && _pickupScript.IsHolding)
            DrawDieFromBag();
    }

    public void OnBecomeCurrentInteractable(bool isInteractable)
    {
        if (_outline != null)
            _outline.enabled = isInteractable;
    }

    public void DrawDieFromBag()
    {
        if (dice.Count == 0)
            return;
        
        GameObject prefab = dice[0];
        dice.RemoveAt(0);
        
        GameObject die = Instantiate(prefab, _diceSpawnPoint.transform.position, _diceSpawnPoint.transform.rotation);

        if (die.TryGetComponent<DiceBehaviour>(out var diceBehaviour))
            diceBehaviour.SetDieFresh(true);

        _pickupScript.Pickup(die);
        
        ShuffleBag();
    }
    
    public void ShuffleBag()
    {
        // This method was stolen from a comment on the Unity forums, it's stored in Assets/Content/Core/Tools/ListFunctions.cs
        dice.Shuffle();
    }
}
