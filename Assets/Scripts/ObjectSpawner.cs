using System.Collections;
using UnityEngine;
using Pooling;

public class ObjectSpawner : MonoBehaviour
{
	[SerializeField] [Range(1, 100)] private float spawnFreq;
	[SerializeField] private bool useObjectPooling;
	[Space]
	[SerializeField] private GameObject[] objectsToSpawn;

	private void Start()
	{
		StartCoroutine(SpawnObject());
	}

	private IEnumerator SpawnObject()
	{
		while (true)
		{
			GameObject newObject;

			if (useObjectPooling)
				newObject = CreateRandomObject_ObjectPool();
			else
				newObject = CreateRandomObject_Instantiate();

			newObject.GetComponent<OnObjectExample>().FinishedUsingMe(useObjectPooling, 5f);
			yield return new WaitForSeconds(spawnFreq * 0.01f);
		}
	}

	private GameObject CreateRandomObject_Instantiate() =>	Instantiate(			objectsToSpawn[Random.Range(0, objectsToSpawn.Length)], transform.position + Random.onUnitSphere, Quaternion.identity);
	private GameObject CreateRandomObject_ObjectPool()	=>	ObjectPool.GetFromPool(	objectsToSpawn[Random.Range(0, objectsToSpawn.Length)], transform.position + Random.onUnitSphere, Quaternion.identity);

}
