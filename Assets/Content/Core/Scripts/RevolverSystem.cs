using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using Random = UnityEngine.Random;
public class RevolverSystem : MonoBehaviour
{
    private Queue<int> chamber = new Queue<int>();
    private int CheatAttempts;

    public UnityEvent OnBlank;
    public UnityEvent OnShot;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadRevolver();
    }

    public void LoadRevolver()
    {   
        chamber.Clear();
        Debug.Log("Loading revolver");
        int bulletplace = Random.Range(0, 6);
        for (int i = 0; i < 6; i++)
        {
            if (bulletplace == i)
            {
                chamber.Enqueue(1);
            }
            else
            {
                chamber.Enqueue(0);
            }
        }
        show(chamber);
    }

    public void Cheating()
    {
        
    }
    
    private void show(Queue<int> shows)
    {
        foreach (var VARIABLE in shows)
        {
            Debug.Log(VARIABLE);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
