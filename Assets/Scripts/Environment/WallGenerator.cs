using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WallGenerator : MonoBehaviour {


	public int TilesCount
	{
		get{
			return 1+(int)(1.0f / TileSize);
		}
	}
	public float TileSize
	{
		get{
			return Prefab.transform.localScale.x;
		}
	}

	public WallCubeBehaviour Prefab;

	public Color BaseColor;

	public WallCubeBehaviour.WallConfigurations Config=new WallCubeBehaviour.WallConfigurations();


	List<WallCubeBehaviour> _cubes=new List<WallCubeBehaviour>();

	GameObject CreateInstance(Vector3 pos,Quaternion rot)
	{
		var g = Instantiate(Prefab.gameObject,Vector3.zero,Quaternion.identity,transform) as GameObject;
		g.transform.localPosition = pos;
		g.transform.localRotation = rot;
		g.SetActive (true);

		return g;
	}

	// Use this for initialization
	void Start () {

		//generate a cube mesh
		{
			var mesh=MeshGenerator.GenerateBox (new Vector3 (0, 0, -0.5f));
			Prefab.GetComponent<MeshFilter> ().mesh = mesh;
		}

		Vector3 pos=new Vector3(0,0,0);
		pos.x = -TileSize * TilesCount / 2.0f;

		float maxDistance = TilesCount * TileSize / 2.0f;

		int N = TilesCount;
		for(int i=-N/2;i<=N/2;++i)
		{
			pos.y = TileSize*i;
			for(int j=-N/2;j<=N/2;++j)
			{
				pos.x = TileSize*j;
				var g=CreateInstance (pos, Quaternion.identity);
				float dist = pos.magnitude / maxDistance;
				g.GetComponent<WallCubeBehaviour> ().Config = Config;
				g.GetComponent<WallCubeBehaviour> ().Saturation = dist*0.5f+0.4f+UnityEngine.Random.Range(0,20)/200.0f;
				_cubes.Add (g.GetComponent<WallCubeBehaviour> ());

			}
		}

	}

	void SpawnCubes()
	{
		float maxDistance = TilesCount * TileSize / 2.0f;
		foreach (var c in _cubes) {

			float dist = c.transform.localPosition.magnitude / maxDistance;
			c.OnSpawn (-dist);
		}

	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKey (KeyCode.Space))
			SpawnCubes ();
	}
}
