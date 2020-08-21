using System.Collections.Generic;
using UnityEngine;

// Here we create an Object pooler class
public class ObjectPooler : MonoBehaviour
{

    // First we create the pool as a class which has all the atrributes of a pool
    [System.Serializable] // this enables us to see it in the Unity Inspector
    public class Pool
    {
        public string tag; // the name of the Object in the pool
        public GameObject Prefab; // the prefab object itself
        public int sizeOfPool; // the maximum number of objects in the pool
    }

    public static ObjectPooler instance;
    public void Awake()
    {
        instance = this;
    }

    public List<Pool> pools; // Lets us input our pool in the inspector
    public Dictionary<string, Queue<GameObject>> poolDictionary;


    // Start is called before the first frame update
    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.sizeOfPool; i++)
            {
                GameObject obj = Instantiate(pool.Prefab);
                //GameMaster.SharedInstance.bullets[i] = obj;
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 pos, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
#if UNITY_EDITOR
            Debug.Log("Pool with tag " + tag + " doesnt exist on this side of the planet");
#endif
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        IPooledObject ObjPool = objectToSpawn.GetComponent<IPooledObject>();
        if (ObjPool != null)
        {
            ObjPool.OnObjectSpawn();
        }
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = pos;
        objectToSpawn.transform.rotation = rotation;

        poolDictionary[tag].Enqueue(objectToSpawn);
        return objectToSpawn;
    }

}
