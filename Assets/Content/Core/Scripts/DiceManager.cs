using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DiceManager : MonoBehaviour
{   
    List<GameObject> allObjects = new List<GameObject>();
    private DiceBehaviour _diceBehaviour;
    private TargetScore _targetScore;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // should ever only exist one Dicemanager
        _targetScore = FindFirstObjectByType<TargetScore>();
    }

    public void CountTotal()
    {
        int total = 0;
        foreach (GameObject go in allObjects)
        {
            _diceBehaviour = go.GetComponent<DiceBehaviour>();
            total += _diceBehaviour.GetTopFaceValue();
        }
        _targetScore.AddToDiceTotal(total);
    }
    
    public void AddDice(GameObject go)
    {
        allObjects.Add(go);
        Debug.Log("Dice added: " + go.name);
    }

    public void RemoveDice(GameObject go)
    {
        allObjects.Remove(go);
        Debug.Log("Dice removed: " + go.name);
    }
    
}
