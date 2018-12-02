using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimplePooler
{
	/// <summary> ///Represents a pool of object prefab instances. Instances can be taken from the pool and returned to it through the ObjectPoolManager/// </summary> ///
	[System.Serializable]
	public class ObjectPool
	{
		//Settable by the editor
		public GameObject objectPrefab;
		public int initialPoolSize;

		public string poolName{get {
			return objectPrefab != null ? objectPrefab.name + " Pool" : "Object Pool";
		}}

		private GameObject rootObject;
		public List<GameObject> poolObjects{get; private set;}
		public int totalNumObjectsInScene {get; private set;} //Track the total number of objects of this pool type that are in the pool or in use in the scene
// 
		public ObjectPool(){}

		public void InitialisePool()
		{
			rootObject = new GameObject();
			rootObject.transform.parent = ObjectPoolManager.Instance.transform;
			rootObject.transform.position = Vector3.zero;
			rootObject.name = poolName;

			poolObjects = new List<GameObject>();
			AddToPool(initialPoolSize);

			UpdateTotalNumSceneObjects();
		}

		public void AddToPool(int numToAdd)
		{
			for(int i = 0; i < numToAdd; i++)
			{
				poolObjects.Add(GameObject.Instantiate(objectPrefab));
				poolObjects[i].name = poolObjects[i].name.Replace("(Clone)", "");
				poolObjects[i].transform.parent = rootObject.transform;
				poolObjects[i].transform.position = Vector3.zero;
				poolObjects[i].transform.rotation = Quaternion.identity;
				poolObjects[i].SetActive(false);
			}
		}

		//Return an existing, individual gameobject to the pool
		public void ReturnToPool(GameObject gameObj)
		{
			//Reset the attributes of this particular gameobject
			gameObj.transform.parent = rootObject.transform;
			gameObj.transform.position = Vector3.zero;
			gameObj.transform.rotation = Quaternion.identity;
			gameObj.SetActive(false);
			poolObjects.Add(gameObj);

			UpdateTotalNumSceneObjects();
		}

		public GameObject TakeFromPool()
		{
			GameObject result = poolObjects[0];
			poolObjects.RemoveAt(0);
			result.transform.parent = null;
			result.SetActive(true);

			UpdateTotalNumSceneObjects();
			return result;
		}

		public GameObject[] TakeObjects(int numToTake)
		{
			GameObject[] result = new GameObject[numToTake];
			for(int i = 0; i < numToTake; i++)
			{
				result[i] = poolObjects[0];	
				poolObjects.RemoveAt(0);
				result[i].transform.parent = null;
				result[i].SetActive(true);
			}
			UpdateTotalNumSceneObjects();
			return result;
		}

		private void UpdateTotalNumSceneObjects()
		{
			if(poolObjects.Count > totalNumObjectsInScene)
				totalNumObjectsInScene = poolObjects.Count;
		}
	}
}

