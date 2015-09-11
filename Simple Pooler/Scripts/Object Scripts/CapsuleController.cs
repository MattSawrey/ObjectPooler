using UnityEngine;
using System.Collections;
using Pooling;

public class CapsuleController : MonoBehaviour 
{
	private Transform objTransform;
	private Vector3 MoveDirection;
	
	void Awake()
	{
		//assign the component to reduce calls in update
		objTransform = gameObject.transform;
		MoveDirection = Vector3.up * 0.8f;
	}
	
	void OnEnable()
	{
		objTransform.position = Vector3.zero - new Vector3(0f, 2.5f, 0f);
	}
	
	void Update () 
	{
		objTransform.position += MoveDirection;
	}
	
	void OnTriggerExit(Collider other)
	{
		if(other.name == "Main Camera")
		{
			ObjectPoolController.Pool.AddToExistingPool(this.gameObject);
		}
	}
}