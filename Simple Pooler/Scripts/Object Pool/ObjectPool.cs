using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimplePooler
{
	/// <summary> ///Represents a pool of object prefab instances. Instances can be taken from and returned to the pool through the ObjectPoolManager/// </summary> ///
	[System.Serializable]
	public class ObjectPool
	{
		//Settable by the editor
		public GameObject objectPrefab;
		public int initialPoolSize;
		public string poolName{get { return objectPrefab != null ? objectPrefab.name + " Pool" : "Empty Pool"; }}
		public List<GameObject> poolObjects{get; private set;}
		public int totalNumObjectsInScene {get; private set;} //Track the total number of objects of this pool type that are in the pool or in use in the scene
		private GameObject rootObject;

		public ObjectPool(){}

		public void InitialisePool()
		{
			rootObject = new GameObject();
			rootObject.transform.parent = ObjectPoolManager.Instance.transform;
			rootObject.transform.position = Vector3.zero;
			rootObject.name = poolName;
			poolObjects = new List<GameObject>();
			AddToPool(initialPoolSize);
		}

		public void AddToPool(int numToAdd)
		{
			rootObject.transform.hierarchyCapacity = initialPoolSize;
			for(int i = 0; i < numToAdd; i++)
			{
				poolObjects.Add(GameObject.Instantiate(objectPrefab));
				poolObjects[i].name = poolObjects[i].name.Replace("(Clone)", "");
				poolObjects[i].transform.parent = rootObject.transform;
				poolObjects[i].transform.position = Vector3.zero;
				poolObjects[i].transform.rotation = Quaternion.identity;
				poolObjects[i].SetActive(false);
			}

			if(poolObjects.Count > totalNumObjectsInScene) //Update the total num of objects in the pool
				totalNumObjectsInScene = poolObjects.Count;
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
		}

		public GameObject TakeFromPool()
		{
			return TakeManyFromPool(1)[0];
		}

		public GameObject[] TakeManyFromPool(int numToTake)
		{
			GameObject[] result = new GameObject[numToTake];
			for(int i = 0; i < numToTake; i++)
			{
				result[i] = poolObjects[0];	
				poolObjects.RemoveAt(0);
				result[i].transform.SetParent(null);
				result[i].SetActive(true);
			}
			return result;
		}
	}
}

