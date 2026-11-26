using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

// Add this script to a UI object with a Text - TextMeshPro component
// It contains methods that can be called from UnityEvents to change the value of the counter.
public class TargetScore : MonoBehaviour
{
    [FormerlySerializedAs("_targetText")]
    [Tooltip("The Text component where the target value should be shown")]
    [SerializeField] private TMPro.TMP_Text targetText;
    [Tooltip("The Text component where the dice total value should be shown")]
    [SerializeField] private TMPro.TMP_Text diceTotalText;

    private int _target = 0;
    private int _diceTotal = 0;
    private int _difficulty = 1;
    private int _targetType = 0;

    public UnityEvent onComparedAndTrue;

    // calls on setCounter when created
    public void Awake()
    {
        GenerateTarget(0);
        diceTotalText.text = "have: " + _diceTotal.ToString();
    }

    public void AddToDiceTotal(int total)
    {
        _diceTotal = total;
        diceTotalText.text = "have: " + _diceTotal.ToString();
    }
    
    // SetCounter gives the counter a new value
    public void GenerateTarget(int difficulty)
    {   
        // rolls a random number between 1 and 10 and if its 10, sets the target type for on value
        if (Random.Range(_difficulty, 11) == 10)
        {
            getOnValue();
        }
        // rolls a random number between 1 and 2 and if its 2 sets target type to above value
        else if (Random.Range(1, 3) == 2)
        {
            getAboveValue();
        }
        // sets target type for below value
        else
        {
            getBellowValue();
        }
        // makes the chance for on value higher
        /*
        _difficulty++;
        _difficulty = Math.Min(5, _difficulty);
        */
    }

    private void getBellowValue()
    {
        _target = Random.Range(10,21);
        targetText.text = "get below: " + _target.ToString();
        _targetType = 1;
    }

    private void getAboveValue()
    {
        _target = Random.Range(10,21);
        targetText.text = "get above: " + _target.ToString();
        _targetType = 2;
    }

    private void getOnValue()
    {
        _target = Random.Range(10,21);
        targetText.text = "get: " + _target.ToString();
        _targetType = 0;
    }

    // Compares the total added by the dice, if they are the target - invoke the onComparedAndTrue Unity Event
    public void CompareCounter()
    {
        bool hit = false;
        // looks for the target type
        if (_targetType == 0) // on value
        {
            if (_diceTotal == _target)
            {
                Debug.Log("got the dice total");
                hit = true; // hit is true when you get the target value

            }
            else if (_diceTotal < _target)
            {
                Debug.Log("dice total is less than target");
            }
            else
            {
                Debug.Log("dice total is more than target");
            }
        }
        else if (_targetType == 1) // below value
        {
            if (_diceTotal <= _target)
            {
                Debug.Log("got the dice total below target");
                hit = true;

            }
            else
            {
                Debug.Log("dice total is more than target");
            }
        }
        else // above value
        {
            if (_diceTotal >= _target)
            {
                Debug.Log("got the dice total above target");
                hit = true;
            }
            else
            {
                Debug.Log("dice total is less than target");
            }
                
        }
        // invokes the unity event if the target got hit
        if (hit)
        {
            onComparedAndTrue.Invoke();
        }
    }
}
