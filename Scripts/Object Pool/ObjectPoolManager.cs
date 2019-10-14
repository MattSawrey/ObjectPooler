using UnityEngine;
using System.Collections.Generic;

namespace ObjectPooler
{
    /// <summary> 
	/// Manages and provides access to a collection of Object Pools for taking and returning object prefab instances
	/// </summary>
    public class ObjectPoolManager : Singleton<ObjectPoolManager>
	{
		public bool addToPoolIfNoObjectsPresent;
		public List<ObjectPool> poolList;

		public void AddToPool(GameObject objToPool)
		{
			AddManyToPool(new []{objToPool});
		}

		public void AddManyToPool(GameObject[] objsToPool)
		{
			if(objsToPool != null)
			{
				int poolIndex;
				if(PoolTypeExists(objsToPool[0].name, out poolIndex))
				{
					for(int i = 0; i < objsToPool.Length; i++)
						poolList[poolIndex].ReturnToPool(objsToPool[i]);
				}
				else
					Debug.LogWarning("No pool for " + objsToPool[0].name + "Gameobjects exists");
			}
			else
				Debug.Log("Cannot add a null gameobject to object pool!");
		}

		//Take an individual object from the pool, specified by the name
		public GameObject TakeFromPool(string objectType)
		{
			return TakeManyFromPool(objectType, 1)[0];
		}

		public GameObject[] TakeManyFromPool(string objectType, int numObjectsToTake) //Take a number of objects from the pool, specified by the name
		{
			int poolIndex;
			if(PoolTypeExists(objectType, out poolIndex))
			{
				if(poolList[poolIndex].poolObjects.Count >= numObjectsToTake) //check that there are enough of this object type in the pool
				{
					GameObject[] result = poolList[poolIndex].TakeManyFromPool(numObjectsToTake);
					return result;
				}
				else
				{
					if(addToPoolIfNoObjectsPresent)
					{
						Debug.LogWarning(objectType + " Pool is empty. Adding more " + objectType + " GameObjects to the pool");
						poolList[poolIndex].AddToPool(numObjectsToTake - poolList[poolIndex].poolObjects.Count);
						
						GameObject[] result = new GameObject[numObjectsToTake];
						for(int i = 0; i < numObjectsToTake; i++)
							result[i] = poolList[poolIndex].TakeFromPool();
						return result;
					}
					else
					{
						Debug.LogWarning("No more " + objectType + " Gameobjects left in the pool! Add more to the pool, or tick Add To Pools If Dry");
						return null;
					}
				}
			}
			else
			{
				Debug.LogWarning("No object called " + objectType + " exists in the pool! Cannot Retreive");
				return null;
			}
		}

		private bool PoolTypeExists(string gameObjName, out int indexOfPool)
		{
			for(int i = 0; i < poolList.Count; i++)
			{
				if(gameObjName + " Pool" == poolList[i].Name)
				{
					indexOfPool = i;
					return true;
				}
			}
			indexOfPool = 0;
			return false;
		}

		private void InititalisePools()
		{
			if(poolList != null)
				for(int i = 0; i < poolList.Count; i++)
					poolList[i].InitialisePool();
		}

		void Awake()
		{
			InititalisePools();
		}
	}
}
