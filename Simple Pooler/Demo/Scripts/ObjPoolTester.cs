using System.Collections;
using UnityEngine;
using SimplePooler;

public class ObjPoolTester : MonoBehaviour 
{
	public GameObject sphere;
	public GameObject box;
	public GameObject capsule;

	public Vector3i sphereCollectionDimensions = new Vector3i(20, 20, 20);
	public float sphereCollectionExistenceTimer = 10f;
	private float sphereCollectionTimer = 0f;
	private bool sphereGridExists = false;

	public void InstantiateCapsule()
	{
		GameObject obj = GameObject.Instantiate(capsule);
		StartCoroutine(CheckCapsuleDestroy(obj));
	}

	private IEnumerator CheckCapsuleDestroy(GameObject obj)
	{
		while(obj.transform.position.y < 20f)
			yield return null;

		Destroy(obj);
	}

	public void PullPoolCapsule()
	{
		GameObject obj = ObjectPoolManager.Instance.TakeObjectFromPool("Capsule");
		if(obj == null)
		{
			Debug.Log("Not enough items in the Capsule object pool to satisfy request");
			return;
		}

		StartCoroutine(CheckPoolCapsuleDestroy(obj));
	}

	private IEnumerator CheckPoolCapsuleDestroy(GameObject obj)
	{
		while(obj.transform.position.y < 20f)
			yield return null;

		ObjectPoolManager.Instance.AddObjectToPool(obj);
	}

	private void InstantiateBox()
	{
		GameObject.Instantiate(box);
	}

	private void PullPoolBox()
	{
		ObjectPoolManager.Instance.TakeObjectFromPool("Box");
	}

	public void InstantiateSphereGrid()
	{
		if(sphereGridExists)
		{
			Debug.Log("Sphere grid already exists");
			return;
		}

		GameObject[] objects = new GameObject[sphereCollectionDimensions.SumOfValues()];

		for(int g = 0; g < sphereCollectionDimensions.SumOfValues(); g++)
			objects[g] = GameObject.Instantiate(sphere);
		
		int i = 0;
		for(int x = 0; x < sphereCollectionDimensions.x; x++)
		{
			for(int y = 0; y < sphereCollectionDimensions.y; y++)
			{
				for(int z = 0; z < sphereCollectionDimensions.z; z++)
				{
					objects[i].transform.position = new Vector3(x, y, z);
					i++;
				}
			}
		}
		sphereGridExists = true;
		StartCoroutine(RemoveCreatedSphereGridTimer(objects));	
	}

	private IEnumerator RemoveCreatedSphereGridTimer(GameObject[] objects)
	{
		while(sphereCollectionTimer <= sphereCollectionExistenceTimer)
		{
			sphereCollectionTimer += Time.deltaTime;
			yield return null;
		}
		foreach(GameObject obj in objects)
			Destroy(obj);
		
		sphereGridExists = false;
		sphereCollectionTimer = 0f;
	}	

	public void PullPoolSphereGrid()
	{
		if(sphereGridExists)
		{
			Debug.Log("Sphere grid already exists");
			return;
		}

		GameObject[] objects = ObjectPoolManager.Instance.TakeObjectsFromPool("Sphere", sphereCollectionDimensions.SumOfValues());
		if(objects == null)
		{
			Debug.Log("Not enough items in the Sphere object pool to satisfy request");
			return;
		}

		int i = 0;
		for(int x = 0; x < sphereCollectionDimensions.x; x++)
		{
			for(int y = 0; y < sphereCollectionDimensions.y; y++)
			{
				for(int z = 0; z < sphereCollectionDimensions.z; z++)
				{
					objects[i].transform.position = new Vector3(x, y, z);
					i++;
				}
			}
		}
		sphereGridExists = true;
		StartCoroutine(RemoveSphereGridTimer(objects));
	}

	private IEnumerator RemoveSphereGridTimer(GameObject[] objects)
	{
		while(sphereCollectionTimer <= sphereCollectionExistenceTimer)
		{
			sphereCollectionTimer += Time.deltaTime;
			yield return null;
		}
		ObjectPoolManager.Instance.AddObjectsToPool(objects);
		sphereGridExists = false;
		sphereCollectionTimer = 0f;
	}
}