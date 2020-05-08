using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attach this to a game object and all instantiated prefabs will be children to this game object
public class ObjectManagerCircle : MonoBehaviour
{
    //The object we want to add
    public GameObject prefabGO;

    //Whats the radius of the circle we will add objects inside of?
    public float radius = 5f;

    //How many GOs will we add each time we press a button?
    public int howManyObjects = 5;

    //Should we add or remove objects within the circle
    public enum Actions { AddObjects, RemoveObjects }

    public Actions action;



    //Add a prefab that we instantiated in the editor script
    public void AddPrefab(GameObject newPrefabObj, Vector3 center)
    {
        //Get a random position within a circle in 2d space
        Vector2 randomPos2D = Random.insideUnitCircle * radius;

        //But we are in 3d, so make it 3d and move it to where the center is
        Vector3 randomPos = new Vector3(randomPos2D.x, 0f, randomPos2D.y) + center;

        newPrefabObj.transform.position = randomPos;

        newPrefabObj.transform.parent = transform;
    }



    //Returns all objects within the circle
    public List<GameObject> GetAllObjectsWithinCircle(Vector3 center)
    {
        //Get an array with all children to this transform
        GameObject[] allChildren = GetAllChildren();

        //All objects within the circle
        List<GameObject> allChildrenWithinCircle = new List<GameObject>();

        foreach (GameObject child in allChildren)
        {
            //If this child is within the circle
            if (Vector3.SqrMagnitude(child.transform.position - center) < radius * radius)
            {
                //DestroyImmediate(child);
                allChildrenWithinCircle.Add(child);
            }
        }

        return allChildrenWithinCircle;
    }



    //Get an array with all children to this GO
    public GameObject[] GetAllChildren()
    {
        //This array will hold all children
        GameObject[] allChildren = new GameObject[transform.childCount];

        //Fill the array
        int childCount = 0;
        foreach (Transform child in transform)
        {
            allChildren[childCount] = child.gameObject;
            childCount += 1;
        }

        return allChildren;
    }
}
