using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ObjectPooler
{
    [CustomEditor(typeof(ObjectPoolManager))]
	public class ObjectPoolEditor : Editor 
	{
		ObjectPoolManager objectPoolManager;
		private int poolBoxHeight = 50;
		private bool isResetButtonVisible;

		private void DrawResetButton()
		{
			if(!Application.isPlaying)
			{
				isResetButtonVisible = EditorGUILayout.Foldout(isResetButtonVisible, "Reset Pool");
				if(isResetButtonVisible)
					if(GUILayout.Button("Reset Object Pool"))
						objectPoolManager.poolList = new List<ObjectPool>();

				EditorGUILayout.Space();				
				objectPoolManager.addToPoolIfNoObjectsPresent = EditorGUILayout.Toggle("Add To Pools If Dry", 
																			objectPoolManager.addToPoolIfNoObjectsPresent); 
			}
		}

		private void DrawObjectPools()
		{
			if(Application.isPlaying)
			{
				if(objectPoolManager.poolList == null)
					return;

				for(int i = 0; i < objectPoolManager.poolList.Count; i++)
				{
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(objectPoolManager.poolList[i].Name, EditorStyles.objectFieldThumb);
					EditorGUILayout.LabelField(objectPoolManager.poolList[i].poolObjects.Count.ToString() + " / " + 
											objectPoolManager.poolList[i].totalNumObjectsInScene.ToString());											
					this.Repaint();					
					EditorGUILayout.EndHorizontal();
				}
			}
			else
			{
				if(objectPoolManager.poolList != null)
				{
					for(int i = 0; i < objectPoolManager.poolList.Count; i++)
					{
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(objectPoolManager.poolList[i].objectPrefab != null ? objectPoolManager.poolList[i].objectPrefab.name : "Object Pool", EditorStyles.objectFieldThumb);
						GUI.backgroundColor = Color.red;
						if(GUILayout.Button("-"))
						{
							objectPoolManager.poolList.RemoveAt(i);
							break;
						}
						GUI.backgroundColor = Color.white;
						EditorGUILayout.EndHorizontal();

						objectPoolManager.poolList[i].objectPrefab =  (GameObject)EditorGUILayout.ObjectField("Object", objectPoolManager.poolList[i].objectPrefab,	typeof(object), true); 
						objectPoolManager.poolList[i].initialPoolSize = Mathf.Clamp(EditorGUILayout.IntField("Number To Pool: ", objectPoolManager.poolList[i].initialPoolSize), 0, int.MaxValue);
					}
				}

				//Adding Object Pool
				EditorGUILayout.Space();
				GUI.backgroundColor = Color.green;
				if(GUILayout.Button("+ Add Pool"))
				{
					if(objectPoolManager.poolList == null)
						objectPoolManager.poolList = new List<ObjectPool>();
					objectPoolManager.poolList.Add(new ObjectPool());
				}
			}
		}

		void OnEnable()
		{
			objectPoolManager = (ObjectPoolManager)target;
		}

		public override void OnInspectorGUI()
		{
			if(!Application.isPlaying)
				EditorGUILayout.BeginVertical(GUILayout.Height(poolBoxHeight * (objectPoolManager.poolList != null ? objectPoolManager.poolList.Count : 0)));
			else
				EditorGUILayout.BeginVertical(GUILayout.Height(20 * (objectPoolManager.poolList != null ? objectPoolManager.poolList.Count : 0)));

			EditorGUILayout.Space();
			DrawResetButton();
			DrawObjectPools();				
			EditorGUILayout.EndVertical();

			if(GUI.changed)
				EditorUtility.SetDirty(objectPoolManager);
		}
	}
}
