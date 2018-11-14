using UnityEngine;
using System.Collections;
using Pooling;

public class BoxController : MonoBehaviour 
{
	private Vector3 moveDirection;
	private Transform objTransform;

	void Awake()
	{
		//assign the component to reduce calls in update
		objTransform = gameObject.transform;
		objTransform.position = Vector3.zero;

		moveDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f); 
	}

	void Update()
	{
		objTransform.Translate(moveDirection);
	}

	void OnTriggerExit(Collider other)
	{
		if(other.name == "Main Camera")
		{
			ObjectPoolController.Pool.AddToExistingPool(this.gameObject);
		}
	}
}
