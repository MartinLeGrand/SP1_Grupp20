using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// solution found in: https://discussions.unity.com/t/how-to-get-all-gameobjects-in-scene/53026/6
// and: https://docs.unity.com/en-us/engine/6000.6/script-reference/unityengine/resources/findobjectsoftypeall

// next checking https://discussions.unity.com/t/how-do-i-call-a-function-in-another-gameobjects-script/111
// to see if i find a solution to call on the die's GetTopFaceValue function from the script DiceBehaviour.
// found solution in the documentation of GetComponent
public class DiceManager : MonoBehaviour
{   
    List<GameObject> allObjects = new List<GameObject>();
    DiceBehaviour diceBehaviour;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    { }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (GameObject go in allObjects)
            {
                diceBehaviour = go.GetComponent<DiceBehaviour>();
                Debug.Log(go.name + " dice value: " + diceBehaviour.GetTopFaceValue());
            }
        }
    }

    public void AddDice(GameObject go)
    {
        allObjects.Add(go);
        Debug.Log("Dice added: " + go.name);
    }

    public void RemoveDice(GameObject go)
    {
        
    }
    
    /*
    List<GameObject> GetAllSceneObjects()
    {
        List<GameObject> objectsInScene = new List<GameObject>();

        // searched for each gameobject which is of the type GameObject and does a loop
        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            
            // checks if the game object is in the scene and puts it in a list of gameobjects
            // EditorUtility.IsPersistent is true for objects saved to disk as assets (such as prefabs).
            // with ! on it, it ignores prefabs.
            // for now also checks if the game object has the "Interactable" tag, as only dice have it now, there is no problem
            if (!EditorUtility.IsPersistent(go.transform.root.gameObject) && !(go.hideFlags == HideFlags.NotEditable || 
            go.hideFlags == HideFlags.HideAndDontSave) && go.tag == "Interactable")
                objectsInScene.Add(go);
        }

        return objectsInScene;
    }
    */
}
