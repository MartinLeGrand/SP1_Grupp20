using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using Random = UnityEngine.Random;
public class RevolverSystem : MonoBehaviour
{
    private int totalBullets;

    public UnityEvent OnBlank;
    public UnityEvent OnShot;

    public void LoadRevolver() // loads the revolver with a shot
    {
        totalBullets++;
        totalBullets = Math.Min(6, totalBullets);
        Debug.Log("total bullets: " + totalBullets);
    }

    public void Cheating() // shoots then loads
    {
        Shoot();
        LoadRevolver(); 
    }

    public void RemoveShot() // removes a shot from the revolver
    {
        totalBullets--;
        totalBullets = Math.Max(0, totalBullets);
        Debug.Log("total bullets: " + totalBullets);
    }

    public void Shoot()
    {
        int shooting = 0;
        if (totalBullets != 0) // if total bullet count is not 0 it randomly rolls
        {
            shooting = Random.Range(totalBullets, 7); // rolls between X and 6, X is amout of bullets
            Debug.Log("shooting number: " + shooting);
        }
        
        if (shooting == 6) // if you get shooting gets 6,  invokes the onshot event 
        {
            OnShot.Invoke();
            Debug.Log("You got shot");
        }
        else // if miss invokes the OnBlank Event
        {
            OnBlank.Invoke();
            Debug.Log("It was a blank");
        }
       
    }
    
    
}
