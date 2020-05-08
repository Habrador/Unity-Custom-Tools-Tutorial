using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObjectManagerLine))]
public class ObjectManagerLineEditor : Editor
{
    private ObjectManagerLine objectManager;



    private void OnEnable()
    {
        //This is a reference to the script
        objectManager = target as ObjectManagerLine;

        //Hide the handles of the GO
        Tools.hidden = true;
    }



    private void OnDisable()
    {
        //Unhide the handles of the GO
        Tools.hidden = false;
    }



    private void OnSceneGUI()
    {
        //Move the line's start and end positions and add objects if we have moved one of the positions
        //Check if we have moved a point
        EditorGUI.BeginChangeCheck();

        //End position
        objectManager.endOfLinePos = MovePoint(objectManager.endOfLinePos);

        //Start position
        objectManager.transform.position = MovePoint(objectManager.transform.position);

        //If we have moved a point, then we need to update the pieces between the points
        if (EditorGUI.EndChangeCheck())
        {
            MarkSceneAsDirty();

            UpdateObjects();
        }
    }



    private Vector3 MovePoint(Vector3 pos)
    {
        //Change position
        if (Tools.current == Tool.Move)
        {
            //Get the new position and display the position with axis
            pos = Handles.PositionHandle(pos, Quaternion.identity);
        }

        return pos;
    }



    //Update the objects between the path
    private void UpdateObjects()
    {
        //The direction between the points
        Vector3 direction = (objectManager.endOfLinePos - objectManager.transform.position).normalized;

        //The first object should look at the end position
        objectManager.firstObject.transform.forward = direction;


        //Kill all current objects
        if (objectManager.objectsParent == null)
        {
            Debug.Log("Cant add objects because the object parent is null");

            return;
        }

        GameObject[] children = objectManager.GetAllChildren();

        foreach (GameObject child in children)
        {
            Undo.DestroyObjectImmediate(child);
        }


        //Add new objects
        if (objectManager.prefabGO == null)
        {
            Debug.Log("Cant add objects because the object prefab is null");

            return;
        }

        //How many objects fit between the start and end points?

        //Make sure the size of the object is not zero because then we can fit infinite amount of objects
        if (objectManager.objectSize == 0f)
        {
            return;
        }

        //The distance between the points
        float distanceBetween = (objectManager.endOfLinePos - objectManager.transform.position).magnitude;

        //If we divide the distance between the points and the size of one object we know how many can fit between 
        int objectsToAddBetweenThePoints = Mathf.FloorToInt(distanceBetween / objectManager.objectSize);

        //Where should we instantiate the first object
        Vector3 instantiatePos = objectManager.transform.position;

        //Add the objects
        for (int i = 0; i < objectsToAddBetweenThePoints; i++)
        {
            GameObject newGO = PrefabUtility.InstantiatePrefab(objectManager.prefabGO) as GameObject;

            //To make undo work
            Undo.RegisterCreatedObjectUndo(newGO, "Spawned Object");

            //Parent it so we can delete it by killing all children
            newGO.transform.parent = objectManager.objectsParent;

            //Give it the position
            newGO.transform.position = instantiatePos;

            //Orient it by making it look at the position we are going to
            newGO.transform.forward = direction;

            //Move to the next object
            instantiatePos += direction * objectManager.objectSize;
        }


        //Move the end of the line to the end of the last instantiated object
        if (objectsToAddBetweenThePoints > 0)
        {
            objectManager.endOfLinePos = instantiatePos;
        }
    }



    //Force unity to save changes or Unity may not save when we have instantiated/removed prefabs despite pressing save button
    private void MarkSceneAsDirty()
    {
        UnityEngine.SceneManagement.Scene activeScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(activeScene);
    }
}
