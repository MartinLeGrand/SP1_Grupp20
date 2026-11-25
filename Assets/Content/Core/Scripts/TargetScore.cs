using UnityEngine;
using UnityEngine.Events;

// Add this script to a UI object with a Text - TextMeshPro component
// It contains methods that can be called from UnityEvents to change the value of the counter.
public class TargetScore : MonoBehaviour
{
    [Tooltip("The Text component where the counter value should be shown")]
    [SerializeField] private TMPro.TMP_Text targetText;
    [SerializeField] private TMPro.TMP_Text diceTotalText;

    private int target = 0;
    private int diceTotal = 0;

    public UnityEvent onComparedAndTrue;

    // calls on setCounter when created
    public void Awake()
    {
        SetCounter();
        diceTotalText.text = "have: " + diceTotal.ToString();
    }

    public void AddToDiceTotal(int total)
    {
        diceTotal += total;
        diceTotalText.text = "have: " + diceTotal.ToString();
    }
    
    // SetCounter gives the counter a new value.
    // Can be used to reset the counter when the player dies.
    public void SetCounter()
    {
        target = Random.Range(10,20);
        targetText.text = "get: " + target.ToString();
    }

    // Compares the total added by the dice, if they are the same - invoke the onComparedAndTrue Unity Event
    public void CompareCounter()
    {
        if (diceTotal == target)
        {
            Debug.Log("got the dice total");
            onComparedAndTrue.Invoke();

        }
        else if (diceTotal < target)
        {
            Debug.Log("dice total is less than counter");
        }
        else
        {
            Debug.Log("die total is more than counter");
        }
    }
}
