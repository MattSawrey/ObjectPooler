	/// <summary> ///Methods that should be present on gameobjects that can be put into and taken from the pool/// </summary> ///
public interface IPoolable
{
	void PrepareForPool();
	void InitiateFromPool();

	// void OnPoolRetreive();
	// void OnPoolReturn();
}