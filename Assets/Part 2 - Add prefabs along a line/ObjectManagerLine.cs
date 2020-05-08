using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attach this script to an empty gameobject
public class ObjectManagerLine : MonoBehaviour
{
    //The object we want to add
    public GameObject prefabGO;

    //The parent to these objects
    public Transform objectsParent;

    //The first wall piece
    public GameObject firstObject;

    //Whats the size of the prefab we want to add?
    //You can increase the size if you want to have a gap between the objects
    public float objectSize;

    //Where is the line ending? It starts at the position of the gameobject the script is attached to
    public Vector3 endOfLinePos;



    //Get an array with all children to transform where we parent the pieces we instantiate
    public GameObject[] GetAllChildren()
    {
        //This array will hold all children
        GameObject[] allChildren = new GameObject[objectsParent.childCount];

        //Fill the array
        int childCount = 0;
        foreach (Transform child in objectsParent)
        {
            allChildren[childCount] = child.gameObject;
            childCount += 1;
        }

        return allChildren;
    }
}
