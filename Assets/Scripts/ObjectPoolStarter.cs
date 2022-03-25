using UnityEngine;
using Pooling;

public class ObjectPoolStarter : MonoBehaviour
{
	[SerializeField] private InitPoolObject[] initialPool;

	private void Start()
	{
		foreach (InitPoolObject p in initialPool)
			ObjectPool.AddPrefabToPool(p.objectToPool, p.amountToPool);
	}
}

[System.Serializable]
public struct InitPoolObject
{
	public GameObject objectToPool;
	public int amountToPool;
}