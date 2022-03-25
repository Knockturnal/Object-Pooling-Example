using System.Collections;
using UnityEngine;
using Pooling;

public class OnObjectExample : MonoBehaviour, IPoolable
{
	//A placeholder to hold a few KB of data
	private int[] randomData = new int[1000];

	public void Initialize()
	{
		//Soft reset object
		AssignRandomData();
	}

	private void AssignRandomData()
	{
		//Just fill the array with random numbers
		for (int i = 0; i < randomData.Length; i++)
			randomData[i] = Random.Range(-99999, 99999);
	}

	public void FinishedUsingMe(bool usingPool, float delay)
	{
		StartCoroutine(FinishMe(usingPool, delay));
	}

	private IEnumerator FinishMe(bool usingPool, float delay)
	{
		yield return new WaitForSeconds(delay);
		if (usingPool)
			ObjectPool.ReturnObjectToPool(gameObject);
		else
			Destroy(gameObject);
	}
}
