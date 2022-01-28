using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCreator : MonoBehaviour
{

    public GameObject _groundSteamRoad;
    public GameObject _groundSteamRoadGrate;

    public Vector2 size;
    public Vector2 roadTileSize;

    public int seed;

    public void CreateGround()
    {
        string holderName = "Created Ground";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform groundHolder = new GameObject(holderName).transform;
	groundHolder.parent = transform;

        System.Random prng = new System.Random(seed);

        Debug.Log(roadTileSize);

        for (int i = 0; i < size.y; i++)
	{
	    for (int j = 0; j < size.x; j++)
	    {
                Vector3 pos = transform.position + new Vector3(
		    -size.x / 2f * roadTileSize.x + roadTileSize.x / 2 + roadTileSize.x * j,
		    0,
		    roadTileSize.y/2 + roadTileSize.y * i);
                Instantiate(prng.Next(0, 100) > 30 ? _groundSteamRoad : _groundSteamRoadGrate,
			    pos,
			    Quaternion.identity,
			    groundHolder);
		
            }
	}	
    }


}
