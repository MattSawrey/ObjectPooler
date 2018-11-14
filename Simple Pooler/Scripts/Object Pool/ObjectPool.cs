using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pooling
{	
	//Represents the pool of an individual object type
	public class ObjectPool
	{
		//Name of the pool
		public string poolName{get; private set;}	
		//GameObject to store all of the individual object types underneath the ObjectPoolController
		public GameObject objectPoolRoot{get; private set;}
		//Reference to what type of object is stored in this pool
		public GameObject objectType{get; private set;}
		//List of all the gameobjects in this pool
		public List<GameObject> gameObjectList{get; private set;}
		//Number of objects current residing in the pool
		public int numObjectsInPool{get{return gameObjectList.Count;}}
		public int totalPoolObjectsInScene {get; private set;}

		public ObjectPool(GameObject objectPooler, GameObject gameObj, int numObject)
		{
			//Create the root pool object
			objectPoolRoot = new GameObject();
			//Assign the name of the pool
			poolName = gameObj.name + " Pool";
			objectPoolRoot.name = poolName;
			//Assign parent GameObject and position
			objectPoolRoot.transform.parent = objectPooler.transform;
			objectPoolRoot.transform.position = Vector3.zero;
			
			//create the gameObjectList
			gameObjectList = new List<GameObject>();
			//Take a reference of the object type
			objectType = gameObj;
			//Add the required objects on construction of the class
			AddToPool(numObject, gameObj);
		}

		//****ADDTOPOOL****//

		//Add a specified number of gameobjects to the pool
		public void AddToPool(int numToAdd, GameObject gameObj)
		{
			for(int i = 0; i < numToAdd; i++)
			{
				//Instantiate the individual gameobject
				GameObject obj = (GameObject)MonoBehaviour.Instantiate(gameObj);
				obj.name = gameObj.name;
				obj.transform.parent = objectPoolRoot.transform;
				obj.transform.position = Vector3.zero;
				obj.transform.rotation = Quaternion.identity;
				obj.SetActive(false);

				//Add the gameobject to the list
				gameObjectList.Add(obj);
			}

			if(gameObjectList.Count > totalPoolObjectsInScene)
			{
				totalPoolObjectsInScene = gameObjectList.Count;
			}
		}

		//****TAKEFROMPOOL****//

		//Allows the retreival of one gameobject from the pool		
		public GameObject TakeFromPool()
		{
			GameObject result = gameObjectList[0];
			gameObjectList.RemoveAt(0);
			
			result.transform.parent = null;
			result.SetActive(true);
			
			return result;
		}

		//****RETURNTOPOOL****//
		
		//Return an existing, individual gameobject to the pool
		public void ReturnToPool(GameObject gameObj)
		{
			//Reset the attributes of this particular gameobject
			gameObj.transform.parent = objectPoolRoot.transform;
			gameObj.transform.position = Vector3.zero;
			gameObj.transform.rotation = Quaternion.identity;
			gameObj.SetActive(false);
			
			//if the object has a rigidbody, reset the forces to prevent movement artifacts when re-enabled
			if(gameObj.GetComponent<Rigidbody2D>() != null)
			{
				gameObj.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
				gameObj.GetComponent<Rigidbody2D>().angularVelocity = 0f;
			}
			else if(gameObj.GetComponent<Rigidbody>() != null)
			{
				gameObj.GetComponent<Rigidbody>().velocity = Vector3.zero;
				gameObj.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
			}
			
			//Add the gameobject to the list
			gameObjectList.Add(gameObj);

			if(gameObjectList.Count > totalPoolObjectsInScene)
			{
				totalPoolObjectsInScene = gameObjectList.Count;
			}
		}
	}
}

