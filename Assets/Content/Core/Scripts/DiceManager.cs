using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DiceManager : MonoBehaviour
{   
    private Dictionary<GameObject, int> SettledDice = new Dictionary<GameObject, int>();
    
    List<GameObject> allDice = new List<GameObject>();
    private DiceBehaviour _diceBehaviour;
    private TargetManager _targetScore;

    void Awake()
    {
        _targetScore = FindFirstObjectByType<TargetManager>();
    }

    public void CountTotal()
    {
        int total = 0;
        foreach (GameObject go in allDice)
        {
            _diceBehaviour = go.GetComponent<DiceBehaviour>();
            total += _diceBehaviour.ReadTopFace().value;
        }
        _targetScore.AddToDiceTotal(total);
    }
    
    public void AddDice(GameObject go)
    {
        allDice.Add(go);
        Debug.Log("Dice added: " + go.name);
    }

    public void RemoveDice(GameObject go)
    {
        allDice.Remove(go);
        Debug.Log("Dice removed: " + go.name);
    }

    public void AddSettledDieValue(GameObject die, int value)
    {
        SettledDice.Add(die, value);
        Debug.Log(die + " Rolled " + value);
    }

    public void RemoveSettledDieValue(GameObject die)
    {
        SettledDice.Remove(die);
    }
}
