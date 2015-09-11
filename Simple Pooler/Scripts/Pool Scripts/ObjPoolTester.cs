using UnityEngine;
using System.Collections;
using Pooling;

public class ObjPoolTester : MonoBehaviour 
{
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
			GameObject[] spheres = ObjectPoolController.Pool.TakeFromPool("Sphere", 636);
			for(int i = 0; i < spheres.Length; i++)
			{
				if(spheres[i] != null)
				{
					float q = Mathf.Floor(i/36);
					float z = Mathf.Floor(i/6);
					spheres[i].transform.position = new Vector3((i - (Mathf.Floor(z)*6) - z) + (q*6),
					                                            Mathf.Floor(z) - (q*6),
					                                            q);
				}	
			}
		}
	}
}