using UnityEngine;
using System.Collections;
using Pooling;

public class SphereController : MonoBehaviour 
{
	private float timeToRemove = 2f;
	private float timer = 0f;

	void Update () 
	{
		timer += Time.deltaTime;
		if(timer >= timeToRemove)
		{
			ObjectPoolController.Pool.AddToExistingPool(this.gameObject);
		}
	}

	void OnDisable()
	{
		timer = 0f;
	}
}
