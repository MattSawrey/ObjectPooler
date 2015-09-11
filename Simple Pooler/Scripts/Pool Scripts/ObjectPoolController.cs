using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Pooling
{
	//Controls the overall object pool
	public class ObjectPoolController : MonoBehaviour
	{
		//Singleton instance of this class
		public static ObjectPoolController Pool{get; private set;}
		//The ObjectPoolController Gameobject
		public static GameObject objectPoolerRoot{get; private set;}
		//Name of the ObjectPoolController Gameobject
		public static string objectPoolerName{get; private set;}

		//Number of each gameobject to spawn in the individual Object Pools on Awake
		public List<int> numsToSpawn;
		public List<GameObject> objsToSpawn;

		public bool addToPoolIfNoObjectsPresent;
		
		//List to manage all of the Individual Object Pools
		public List<ObjectPool> listOfObjectPools{get; private set;}

		//Delegate pool events
		public delegate void ObjectPoolEvents();
		public static event ObjectPoolEvents PoolInitialised = delegate {};

		void Awake()
		{
			InitialisePool();
		}

		private void InitialisePool()
		{
			//Assignment of the Singleton Instance
			Pool = this;
			
			objectPoolerRoot = gameObject;
			objectPoolerName = gameObject.name;
			
			//Instance the list of object pools
			listOfObjectPools = new List<ObjectPool>();
			
			//Create the list of Object Pools
			for(int i = 0; i < objsToSpawn.Count; i++)
			{
				if(objsToSpawn[i] != null && numsToSpawn[i] != 0)
				{
					AddNewPool(new ObjectPool(objectPoolerRoot, objsToSpawn[i], numsToSpawn[i]));
				}
			}

			//Pool initialised event
			PoolInitialised();
		}

		//****ADDNEWPOOL****//

		//Adds a new pool for a new type of GameObject to the Object Pooler
		private void AddNewPool(ObjectPool objPool)
		{
			int indexOfPool;
			//check to see if a list for this gameobject type already exists
			if(PoolTypeExists(objPool.poolName, out indexOfPool))
			{
				Destroy(objPool.objectPoolRoot);
				Debug.Log("You tried to create 2 of the same pool. Inadvisable.");
				listOfObjectPools[indexOfPool].AddToPool(objPool.numObjectsInPool, objPool.objectType);
			}
			//if it doesnt, create a new object pool in the list of object pools and add to it
			else
			{
				listOfObjectPools.Add(objPool);
			}
		}

		//****ADDTOEXISTINGPOOL****//

		//2 implementations of this method
		//Adds an individual, or multiple object/s back into the appropriate pool.
		//If the appropriate pool does not exist, it adds a pool
		public void AddToExistingPool(GameObject objToPool)
		{
			if(objToPool != null)
			{
				int indexOfPool;
				if(PoolTypeExists(objToPool.name, out indexOfPool))
				{
					listOfObjectPools[indexOfPool].ReturnToPool(objToPool);
				}
				else
				{
					Debug.Log("No pool for " + objToPool.name + "Gameobjects exists");
				}
			}
			else
			{
				Debug.Log("Cannot instantiate a null gameobject!");
			}
		}

		//Add an array of objects back into the pool
		public void AddToExistingPool(GameObject[] objsToPool)
		{
			for(int i = 0; i < objsToPool.Length; i++)
			{
				AddToExistingPool(objsToPool[i]);
			}
		}

		//****TAKEFROMPOOL****//

		//Take an individual object from the pool, specified by the name
		public GameObject TakeFromPool(string objectType)
		{
			int indexOfPool;
			if(PoolTypeExists(objectType, out indexOfPool))
			{
				//check that there are enough of this object type in the pool
				if(listOfObjectPools[indexOfPool].numObjectsInPool >= 1)
				{
					return listOfObjectPools[indexOfPool].TakeFromPool();
				}
				else
				{
					if(addToPoolIfNoObjectsPresent)
					{
						Debug.Log(objectType + " Pool is empty. Adding more " + objectType + " GameObjects to the pool");
						listOfObjectPools[indexOfPool].AddToPool(1, listOfObjectPools[indexOfPool].objectType);
						return listOfObjectPools[indexOfPool].TakeFromPool();
					}
					else
					{
						Debug.Log("No more " + objectType + " Gameobjects left in the pool! Add more to the pool, or tick Add To Pools If Dry");
						return null;
					}
				}
			}
			else
			{
				Debug.Log("No object called " + objectType + " exists in the pool! Cannot Retreive");
				return null;
			}
		}

		//Take a number of objects from the pool, specified by the name
		public GameObject[] TakeFromPool(string objectType, int numObjectsToTake)
		{
			GameObject[] result = new GameObject[numObjectsToTake];

			for(int i = 0; i < numObjectsToTake; i++)
			{
			    result[i] = TakeFromPool(objectType);
			}

			return result;
		}

		//****DESTROYPOOL****//
		
		//Destroys a single gameobject-type pool
		public void DestroyPool(string poolToDestroy)
		{
			int indexOfPool;
			if(PoolTypeExists(poolToDestroy, out indexOfPool))
			{
				Destroy(listOfObjectPools[indexOfPool].objectPoolRoot);
			}
			else
			{
				Debug.Log("Cannot destroy " + poolToDestroy + " pool, as it was not found");
			}
		}

		//****DESTROYOVERALLOBJECTPOOL****//

		//Destroys the overall ObjectPooler GameObject
		public void DestroyOverallObjectPooler()
		{
			Destroy(this.gameObject);
		}

		//****POOLTYPEEXISTS****//
		
		//Checks if the pool currently exists, also provides the index of the pool within the ObjectPooler GameObject
		private bool PoolTypeExists(string gameObjName, out int indexOfPool)
		{
			for(int i = 0; i < listOfObjectPools.Count; i++)
			{
				if(gameObjName + " Pool" == listOfObjectPools[i].poolName)
				{
					indexOfPool = i;
					return true;
				}
			}
			
			indexOfPool = 0;
			return false;
		}
	}
}
