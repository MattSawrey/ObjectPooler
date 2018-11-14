using UnityEngine;
using System.Collections;
using Pooling;

public class ObjPoolTester : MonoBehaviour 
{
	public Vector3i sphereCollectionDimensions = new Vector3i(4, 4, 4);
	public float sphereCollectionExistenceTimer = 10f;
	private float sphereCollectionTimer = 0f;

	private float capsuleSpawnRate = 0.6f;
	private float capsuleSpawnTimer = 0f;

	void Update()
	{
		if(Input.GetMouseButtonDown(1))
		{
			ObjectPoolController.Pool.TakeFromPool("Box");
		}

		if(Input.GetMouseButton(0))
		{
			capsuleSpawnTimer += 0.2f;

			if(capsuleSpawnTimer >= capsuleSpawnRate)
			{
				ObjectPoolController.Pool.TakeFromPool("Capsule");
				capsuleSpawnTimer = 0f;
			}
		}

		if(Input.GetKeyDown(KeyCode.Space))
		{
			SpawnSphereGrid();
		}
	}

	private IEnumerator RemoveSphereGridTimer(GameObject[] objects)
	{
		while(sphereCollectionTimer <= sphereCollectionExistenceTimer)
		{
			sphereCollectionTimer += Time.deltaTime;
			yield return null;
		}
		ObjectPoolController.Pool.AddToExistingPool(objects);
		sphereCollectionTimer = 0f;
	}

	private void SpawnSphereGrid()
	{
		GameObject[] objects = ObjectPoolController.Pool.TakeFromPool("Sphere", sphereCollectionDimensions.SumOfValues());
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
		StartCoroutine(RemoveSphereGridTimer(objects));
	}
}