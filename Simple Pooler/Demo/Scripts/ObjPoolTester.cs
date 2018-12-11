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

	private bool spawnCapsuleStreamFromPool = false;
	private bool spawnCapsuleStreamFromInstantiation = false;

	#region - capsule stream 

		public void StartCapsuleSpawnStreamFromPool()
		{
			spawnCapsuleStreamFromPool = true;
			StartCoroutine(RunCapsuleSpawnStream());

		}

		public void EndCapsuleSpawnStreamFromPool()
		{
			spawnCapsuleStreamFromPool = false;
		}

		private IEnumerator RunCapsuleSpawnStream()
		{
			float timer = 0f;
			while(spawnCapsuleStreamFromPool) 
			{
				timer += Time.deltaTime*10f;
				if(timer >= 1f)
				{
					PullPoolCapsule();
					timer = 0f;
				}
				yield return null;
			}
		}

		private void PullPoolCapsule()
		{
			GameObject obj = ObjectPoolManager.Instance.TakeFromPool("Capsule");
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

			ObjectPoolManager.Instance.AddToPool(obj);
		}

		public void StartCapsuleSpawnStreamFromInstantiation()
		{
			spawnCapsuleStreamFromInstantiation = true;
			StartCoroutine(RunCapsuleSpawnStreamFromInstantiation());

		}

		public void EndCapsuleSpawnStreamFromInstantiation()
		{
			spawnCapsuleStreamFromInstantiation = false;
		}

		private IEnumerator RunCapsuleSpawnStreamFromInstantiation()
		{
			float timer = 0f;
			while(spawnCapsuleStreamFromInstantiation) 
			{
				timer += Time.deltaTime*10f;
				if(timer >= 1f)
				{
					InstantiateCapsule();
					timer = 0f;
				}
				yield return null;
			}
		}

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

	#endregion

	#region - sphere grid

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
			
			DrawSphereGrid(objects);
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

			GameObject[] objects = ObjectPoolManager.Instance.TakeManyFromPool("Sphere", sphereCollectionDimensions.SumOfValues());
			if(objects == null)
			{
				Debug.Log("Not enough items in the Sphere object pool to satisfy request");
				return;
			}

			DrawSphereGrid(objects);
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
			ObjectPoolManager.Instance.AddManyToPool(objects);
			sphereGridExists = false;
			sphereCollectionTimer = 0f;
		}

		private void DrawSphereGrid(GameObject[] spheres)
		{
			float xOffset = sphereCollectionDimensions.x/2;
			float yOffset = sphereCollectionDimensions.y/2;
			float zOffset = sphereCollectionDimensions.z/2;
			int i = 0;
			for(int x = 0; x < sphereCollectionDimensions.x; x++)
			{
				for(int y = 0; y < sphereCollectionDimensions.y; y++)
				{
					for(int z = 0; z < sphereCollectionDimensions.z; z++)
					{
						spheres[i].transform.position = new Vector3(x-xOffset, y-yOffset, z-zOffset);
						i++;
					}
				}
			}
		}

	#endregion
}