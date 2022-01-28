using System.Collections.Generic;
using UnityEngine;

public class GroundGenerator : MonoBehaviour
{

    // public GameObject player;
    public Transform _pTransform;
    public HeroController _heroController;

    public GameObject[] groundLayouts;
    public GameObject[] tiles;
    public GameObject[] elements;
    public float distToSpawn = 50f;
    public float groundZSize = 40f;

    public int seed = 0;
    public int initialNoOfGrounds = 5;

    public List<GameObject> GeneratedGrounds;
    public Vector3 nextSpawnPosition;

    private System.Random prng;

    void Awake()
    {
        prng = new System.Random(seed);
    }

    void Update()
    {
        if (_pTransform.position.z + distToSpawn > nextSpawnPosition.z)
        {
            GameObject spawnObject = GetRandomGround(prng.Next(groundLayouts.Length));
            SpawnGround(spawnObject, nextSpawnPosition);
            nextSpawnPosition.z += groundZSize;
        }
    }


    public void GenerateInitialGround()
    {
	// for (int i = 0; i < GeneratedGrounds.Count; i++)
	// {
	//     GameObject obj = GeneratedGrounds[i];
	//     GeneratedGrounds.Remove(obj);
	//     DestroyImmediate(obj);
	// }

	// prng = new System.Random(seed);	
	// nextSpawnPosition = new Vector3(0, 0, 0);
        // for (int i = 0; i < initialNoOfGrounds; i++)
        // {
        //     GameObject spawnObject = GetRandomGround(prng.Next(groundLayouts.Length));
        //     SpawnGround(spawnObject, nextSpawnPosition);
        //     nextSpawnPosition.z += groundZSize;
        // }
    }


    void SpawnGround(GameObject ground, Vector3 spawnPosition)
    {
	GameObject instantiated = (Instantiate(ground, spawnPosition, Quaternion.identity));
        instantiated.transform.parent = transform;
        GeneratedGrounds.Add(instantiated);
    }

    GameObject GetRandomGround(int index)
    {
        return groundLayouts[index];
    }

}
