using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObjectManagerCircle))]
public class ObjectManagerEditor : Editor
{
    private ObjectManagerCircle objectManager;

    //The center of the circle
    private Vector3 center;



    private void OnEnable()
    {
        objectManager = target as ObjectManagerCircle;

        //Hide the handles of the GO so we dont accidentally move it instead of moving the circle
        Tools.hidden = true;
    }



    private void OnDisable()
    {
        //Unhide the handles of the GO
        Tools.hidden = false;

        //So we can save all changes we made
        MarkSceneAsDirty();
    }



    private void OnSceneGUI()
    {
        //Move the circle when moving the mouse
        //A ray from the mouse position
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            //Where did we hit the ground?
            center = hit.point;

            //Need to tell Unity that we have moved the circle or the circle may be displayed at the old position
            SceneView.RepaintAll();
        }


        //Display the circle
        if (Event.current.type == EventType.Repaint)
        {
            Handles.color = Color.white;

            Handles.DrawWireDisc(center, Vector3.up, objectManager.radius);
        }


        //Add or remove objects with left mouse click

        //First make sure we cant select another gameobject in the scene when we click
        HandleUtility.AddDefaultControl(0);

        //Have we clicked with the left mouse button?
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            //Should we add or remove objects?
            if (objectManager.action == ObjectManagerCircle.Actions.AddObjects)
            {
                AddNewPrefabs();
            }
            else if (objectManager.action == ObjectManagerCircle.Actions.RemoveObjects)
            {
                List<GameObject> allObjectsWithinCircle = objectManager.GetAllObjectsWithinCircle(center);

                foreach (GameObject go in allObjectsWithinCircle)
                {
                    //Will both destroy the object and record it so we can undo if we didnt want to remove it
                    Undo.DestroyObjectImmediate(go);
                }
            }
        }
    }



    //Add buttons this scripts inspector
    public override void OnInspectorGUI()
    {
        //Add the default stuff
        DrawDefaultInspector();

        //Remove all objects when pressing this button
        if (GUILayout.Button("Remove all objects"))
        {
            //Pop-up so you don't accidentally remove all objects
            if (EditorUtility.DisplayDialog("Safety check!", "Do you want to remove all objects?", "Yes", "No"))
            {
                GameObject[] allObjects = objectManager.GetAllChildren();

                //Destroy all objects
                foreach (GameObject go in allObjects)
                {
                    //Will both destroy the object and record it so we can undo if we didnt want to remove it
                    Undo.DestroyObjectImmediate(go);
                }
            }
        }
    }



    //Force unity to save changes or Unity may not save when we have instantiated/removed prefabs despite pressing save button
    private void MarkSceneAsDirty()
    {
        UnityEngine.SceneManagement.Scene activeScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(activeScene);
    }



    //Instantiate prefabs at random positions within the circle
    private void AddNewPrefabs()
    {
        //How many prefabs do we want to add
        int howManyObjects = objectManager.howManyObjects;

        //Which prefab to we want to add
        GameObject prefabGO = objectManager.prefabGO;

        for (int i = 0; i < howManyObjects; i++)
        {
            GameObject newGO = PrefabUtility.InstantiatePrefab(prefabGO) as GameObject;

            //To make undo work if we didn't want to add objects at these positions
            //Will actually undo all objects created in this for loop, so it's enough
            //to press ctrl+z once to undo all objects added in this loop
            Undo.RegisterCreatedObjectUndo(newGO, "Spawned Object");

            //Send it to the main script to add it at a random position within the circle
            objectManager.AddPrefab(newGO, center);
        }
    }
}
