using UnityEngine;
using System.Collections.Generic;

namespace ObjectPooler
{
	/// <summary>
	/// Represents a pool of object prefab instances. Instances can be taken from and returned to the pool through the ObjectPoolManager
	/// </summary>
    [System.Serializable]
	public class ObjectPool
	{
		public int ID { get; private set; }
		[Tooltip("GameObject prefab for this pool")]
		public GameObject objectPrefab;

		public string Name{ get { return objectPrefab != null ? objectPrefab.name + " Pool" : "Empty Pool"; } }
		public int initialPoolSize;
		public Stack<GameObject> poolObjects{ get; private set; }
		public int totalNumObjectsInScene { get; private set; } //Track the total number of objects of this pool type that are in the pool or in use in the scene
		private GameObject rootObject;

		public ObjectPool()
		{

		}

		public void InitialisePool()
		{
			rootObject = new GameObject();
			rootObject.transform.parent = ObjectPoolManager.Instance.transform;
			rootObject.transform.position = Vector3.zero;
			rootObject.name = Name;
			poolObjects = new Stack<GameObject>();
			AddToPool(initialPoolSize);
		}

		public void AddToPool(int numToAdd)
		{
			rootObject.transform.hierarchyCapacity = initialPoolSize;
			for(int i = 0; i < numToAdd; i++)
			{
				GameObject objectToAdd = GameObject.Instantiate(objectPrefab, Vector3.zero, Quaternion.identity, rootObject.transform);
				objectToAdd.name = objectToAdd.name.Replace("(Clone)", "");
				objectToAdd.SetActive(false);
				poolObjects.Push(objectToAdd);
			}

			if(poolObjects.Count > totalNumObjectsInScene) //Update the total num of objects in the pool
				totalNumObjectsInScene = poolObjects.Count;
		}

		//Return an existing, individual gameobject to the pool
		public void ReturnToPool(GameObject gameObject)
		{
			//Reset the attributes of this particular gameobject
			gameObject.transform.parent = rootObject.transform;
			// TODO - Unsure if this is needed for passing an object back into the pool
			// gameObject.transform.position = Vector3.zero;
			// gameObject.transform.rotation = Quaternion.identity;
			gameObject.SetActive(false);
			poolObjects.Push(gameObject);
		}

		public GameObject TakeFromPool()
		{
			GameObject result = poolObjects.Pop();
			return result;
		}

		public GameObject[] TakeManyFromPool(int numToTake)
		{
			GameObject[] result = new GameObject[numToTake];
			for(int i = 0; i < numToTake; i++)
			{
				result[i] = poolObjects.Pop();	
				result[i].transform.SetParent(null);
				result[i].SetActive(true);
			}
			return result;
		}
	}
}

