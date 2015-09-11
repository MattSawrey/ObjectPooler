using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Pooling;

//Customisation for the editor inspector window for the ObjectPoolController
[CustomEditor(typeof(ObjectPoolController))]
public class ObjectPoolEditor : Editor 
{
	private int poolBoxHeight = 50;

	private bool isResetButtonVisible;

	ObjectPoolController objPooler;

	void OnEnable()
	{
		objPooler = (ObjectPoolController)target;

		if(objPooler.objsToSpawn.Count == 0)
		{
			objPooler.objsToSpawn.Add(null);
			objPooler.numsToSpawn.Add(0);	
		}
	}

	public override void OnInspectorGUI()
	{
		if(!Application.isPlaying)
		{
			EditorGUILayout.BeginVertical(GUILayout.Height(poolBoxHeight * objPooler.objsToSpawn.Count));
		}
		else
		{
			EditorGUILayout.BeginVertical(GUILayout.Height(20 * objPooler.objsToSpawn.Count));
		}

		EditorGUILayout.Space();

		if(!Application.isPlaying)
		{
			isResetButtonVisible = EditorGUILayout.Foldout(isResetButtonVisible, "Reset Pool");
			
			if(isResetButtonVisible)
			{
				//Button to reset the overall pool
				if(GUILayout.Button("Reset Object Pool"))
				{							
					if(objPooler.objsToSpawn.Count > 1)
					{
						objPooler.objsToSpawn.RemoveRange(1, objPooler.objsToSpawn.Count-1);
						objPooler.numsToSpawn.RemoveRange(1, objPooler.numsToSpawn.Count-1);
					}
					
					objPooler.objsToSpawn[0] = null;
					objPooler.numsToSpawn[0] = 0;
				}
			}

			EditorGUILayout.Space();
			
			objPooler.addToPoolIfNoObjectsPresent = EditorGUILayout.Toggle("Add To Pools If Dry", 
			                                                               objPooler.addToPoolIfNoObjectsPresent); 
		}

		//Draw the objects to pool in the inspector
		if(!Application.isPlaying)
		{
			for(int i = 0; i < objPooler.objsToSpawn.Count; i++)
			{
				EditorGUILayout.BeginHorizontal();

				EditorGUILayout.LabelField(objPooler.objsToSpawn[i] != null ? objPooler.objsToSpawn[i].name : "Object To Pool"
				                           , EditorStyles.objectFieldThumb);
				
				//Button to remove object
				
				if(objPooler.objsToSpawn.Count != 1)
				{
					GUI.backgroundColor = Color.red;
					if(GUILayout.Button("-"))
					{
						objPooler.objsToSpawn.RemoveAt(i);
						objPooler.numsToSpawn.RemoveAt(i);	
						break;
					}
					GUI.backgroundColor = Color.white;
				}
				
				EditorGUILayout.EndHorizontal();
				
				objPooler.objsToSpawn[i] =  (GameObject)EditorGUILayout.ObjectField("Object",
				                                                                    objPooler.objsToSpawn[i],
				                                                                    typeof(object), true); 
				
				objPooler.numsToSpawn[i] = Mathf.Clamp(EditorGUILayout.IntField("Number To Pool: ", objPooler.numsToSpawn[i])
				                                       , 0, int.MaxValue);
			}
		}
		//On play
		else
		{
			for(int i = 0; i < objPooler.listOfObjectPools.Count; i++)
			{
				EditorGUILayout.BeginHorizontal();

				EditorGUILayout.LabelField(objPooler.listOfObjectPools[i].poolName, EditorStyles.objectFieldThumb);

				EditorGUILayout.LabelField(objPooler.listOfObjectPools[i].numObjectsInPool.ToString() + " / " + 
				                           objPooler.listOfObjectPools[i].totalPoolObjectsInScene.ToString());
				                           
				this.Repaint();
				
				EditorGUILayout.EndHorizontal();
			}
		}

		if(!Application.isPlaying)
		{
			EditorGUILayout.Space();

			GUI.backgroundColor = Color.green;
			
			//button to add new entries
			if(GUILayout.Button("+ New Pool"))
			{
				objPooler.objsToSpawn.Add(null);
				objPooler.numsToSpawn.Add(0);	
			}
		}
			
		EditorGUILayout.EndVertical();

		if(GUI.changed)
		{
			EditorUtility.SetDirty(objPooler);
		}
	}
}
