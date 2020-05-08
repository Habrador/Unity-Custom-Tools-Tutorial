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

    //We are adding prefabs between these points
    public List<Vector3> waypoints = new List<Vector3>();



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



    //Update the first object
    public void UpdateFirstObject()
    {
        //The direction between the points
        Vector3 direction = (waypoints[1] - waypoints[0]).normalized;

        //The first object should look at the end position
        firstObject.transform.forward = direction;

        firstObject.transform.position = waypoints[0];
    }
}
