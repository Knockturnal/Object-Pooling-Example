using System.Collections.Generic;
using UnityEngine;
namespace Pooling
{
	//This is one of many, many ways to implement an Object pool. This implementation takes any number of different GameObjects, but trades some
	//performance for this flexibility. We also have to keep two references to each object for the lookup to run smoothly.
	public static class ObjectPool
	{
		//The object pool
		private static Dictionary<GameObject, Queue<GameObject>> objectPool = new Dictionary<GameObject, Queue<GameObject>>();
		//A list of all pooled objects and their prefabs. We keep track of it separately so we can put objects back in their proper Queues when returned to the pool
		private static Dictionary<GameObject, GameObject> previouslyPooledObjects = new Dictionary<GameObject, GameObject>();

		/// <summary>
		/// Used to explicitly add prefabs to the pool
		/// </summary>
		/// <param name="newObject">Prefab to add an instance of</param>
		public static void AddPrefabToPool(GameObject newObject)
		{
			GameObject pooled = GameObject.Instantiate(newObject);      //Create a new object from the prefab
			pooled.SetActive(false);                                    //Disable the newly created object
			if (!objectPool.ContainsKey(newObject))                     //If the pool doesn't contain this type of object, make a new Queue with the prefab as a Key
				objectPool.Add(newObject, new Queue<GameObject>());
			objectPool[newObject].Enqueue(pooled);                      //Add the newly created object to the queue
			previouslyPooledObjects.Add(pooled, newObject);             //Add a reference to the object and the prefab it was created from to the list of all objects
		}
		/// <summary>
		/// Used to explicitly add prefabs to the pool
		/// </summary>
		/// <param name="newObject">Prefab to add an instance of</param>
		/// <param name="amount">Amount of instances to add to the pool</param>
		public static void AddPrefabToPool(GameObject newObject, int amount)
		{
			for (int i = 0; i < amount; i++)
				AddPrefabToPool(newObject);
		}

		/// <summary>
		/// Return object to the pool
		/// </summary>
		/// <param name="returned">Object to return to pool</param>
		public static void ReturnObjectToPool(GameObject returned)
		{
			//We cannot return an unknown object to the pool because we need to know what prefab it was created from in order to look it up with that prefab later
			if (!previouslyPooledObjects.ContainsKey(returned))
			{
				Debug.LogWarning("Attempted to return a previously unpooled object to the pool");
				return;
			}

			GameObject returnedPrefab = previouslyPooledObjects[returned];      //Locate the prefab that created this object
			returned.SetActive(false);                                          //Disable the object
			if (!objectPool.ContainsKey(returnedPrefab))                        //If the pool doesn't contain this type of object, make a new Queue with the prefab as a Key
				objectPool.Add(returnedPrefab, new Queue<GameObject>());
			objectPool[returnedPrefab].Enqueue(returned);                       //Return the object to the relevant queue
		}

		//Internal method used to find a matching object in the pool - or if none is found, create a new one and add it to the pool
		private static GameObject FindOrCreatePoolObject(GameObject prefabToLookFor)
		{
			if (objectPool.ContainsKey(prefabToLookFor))                        //Check that there is a queue for this prefab
				if (objectPool[prefabToLookFor].Count > 0)                      //Check that the queue is not empty
					return objectPool[prefabToLookFor].Dequeue();               //Dequeue and return an object from the relevant queue

			GameObject newWorldObject = GameObject.Instantiate(prefabToLookFor);    //If else, then instantiate a new object from the prefab
			previouslyPooledObjects.Add(newWorldObject, prefabToLookFor);           //Add this ob
			return newWorldObject;
		}
		/// <summary>
		/// Use this identically to Instantiate()
		/// </summary>
		/// <param name="toGet">The object to create</param>
		/// <returns></returns>
		public static GameObject GetFromPool(GameObject toGet) => GetFromPool(toGet, null, Vector3.zero, Quaternion.identity);
		public static GameObject GetFromPool(GameObject toGet, Vector3 placement, Quaternion rotation) => GetFromPool(toGet, null, placement, rotation);
		public static GameObject GetFromPool(GameObject toGet, Transform parent, Vector3 placement, Quaternion rotation)
		{
			GameObject pooled = FindOrCreatePoolObject(toGet);
			pooled.transform.parent = parent;
			pooled.transform.transform.position = placement;
			pooled.transform.transform.rotation = rotation;
			if (pooled.TryGetComponent(out IPoolable p))
				p.Initialize();
			pooled.SetActive(true);
			return pooled;
		}
	}
}