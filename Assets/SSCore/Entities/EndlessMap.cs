using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndlessMap : MonoBehaviour
{
	public GameObject[] template;

	Vector3 currPos;

	Vector3 currVel;
	float currVelMag = 2;

	public class MapInfo
	{
		public Vector3 offset;
		public GameObject go;
		public MapInfo(Vector3 _offset, GameObject _go)
		{
			go = _go;
			offset = _offset;
		}
	}

	List<MapInfo> mapInfo = new List<MapInfo>();

	Vector3 gridSize = new Vector3(5, 1, 5);
	Vector3 gridSizeXZ;

	Vector3 tileOffset = Vector3.zero;
	Vector3 prevTileOffset = Vector3.zero;

	float fovMin = 10;

	// Use this for initialization
	void Start ()
	{
		gridSizeXZ = gridSize;
		gridSizeXZ.y = 0;

		FillFOV();
	}

	bool CheckTileExist(Vector3 offset)
	{
		foreach (MapInfo mi in mapInfo)
		{
			if ((offset - mi.offset).magnitude < (gridSizeXZ / 2).magnitude)
			{
				return true;
			}
		}
		return false;
	}

	void SpawnMap(Vector3 offset)
	{
		if (!CheckTileExist(offset))
		{
			if (template.Length > 0)
			{
				int id = Random.Range(0, template.Length);
				if (template[id] != null)
				{
					GameObject go = (GameObject)Instantiate(template[id]);
					if (go != null)
					{
						go.transform.position = offset;
						MapInfo mi = new MapInfo(offset, go);
						mapInfo.Add(mi);
					}
				}
			}
		}
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(Vector3.zero, fovMin);
		Gizmos.DrawCube(-tileOffset, new Vector3 (0.2f, 2.0f, 0.2f));
		Gizmos.DrawCube(currPos, Vector3.one * 0.1f);
	}
	
	bool CheckTileInside(Vector3 _offset)
	{
		return _offset.magnitude - (gridSize / 2.0f).magnitude < fovMin;
	}

	bool CheckTileInside(MapInfo mi)
	{
		if (mi != null && mi.go != null)
		{
			return CheckTileInside(mi.offset);
		}
		return false;
	}

	void FillFOV_Z(Vector3 _pos)
	{
		Vector3 spawnPos = _pos;
		if (CheckTileInside(spawnPos))
		{
			SpawnMap(spawnPos);
		}

		bool bCheck = false;
		do {
			spawnPos.z += gridSize.z;
			bCheck = CheckTileInside(spawnPos);
			if (bCheck)
			{
				SpawnMap(spawnPos);
			}
			
		} while (bCheck);
		
		spawnPos = _pos;

		bCheck = false;
		do {
			spawnPos.z -= gridSize.z;
			bCheck = CheckTileInside(spawnPos);
			if (bCheck)
			{
				SpawnMap(spawnPos);
			}
			
		} while (bCheck);
	}

	void FillFOV()
	{
		Vector3 spawnPos = -tileOffset;
		if (CheckTileInside(spawnPos))
		{
			FillFOV_Z(spawnPos);
		}

		bool bCheck = false;
		do {
			spawnPos.x += gridSize.x;
			bCheck = CheckTileInside(spawnPos);
			if (bCheck)
			{
				FillFOV_Z(spawnPos);
			}
			
		} while (bCheck);

		spawnPos = -tileOffset;
		bCheck = false;
		do {
			spawnPos.x -= gridSize.x;
			bCheck = CheckTileInside(spawnPos);
			if (bCheck)
			{
				FillFOV_Z(spawnPos);
			}
			
		} while (bCheck);
	}

	void UpdateMapInfo()
	{
		List<MapInfo> removeList = new List<MapInfo>();
		if (mapInfo != null)
		{
			bool inside = false;
			foreach (MapInfo mi in mapInfo)
			{
				if (mi.go != null)
				{
					mi.offset -= currVel * Time.deltaTime;
					mi.go.transform.position = mi.offset;

					if (CheckTileInside(mi))
					{
						inside = true;
					}
					else
					{
						// Remove map outside fov
						Destroy(mi.go);
						removeList.Add(mi);
					}
				}
			}

			if (!inside)
			{
				Vector3 newMapPos = new Vector3();
				newMapPos.x = currPos.x % gridSize.x;
				newMapPos.y = currPos.y % gridSize.y;
				newMapPos.z = currPos.z % gridSize.z;
				SpawnMap(-newMapPos);
			}

			// Remove map outside fov
			foreach (MapInfo rm in removeList)
			{
				mapInfo.Remove(rm);
			}
		}

		// Fill FOV
		FillFOV();
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Update velocity
		currVel = transform.forward;
		currVel.y = 0;
		currVel = currVel.normalized * currVelMag;

		prevTileOffset = tileOffset;

		// Move first
		currPos += currVel * Time.deltaTime;

		// Relative position (original position) = currPos
		// Calculate the latest tile position under the player
		tileOffset = new Vector3(currPos.x % gridSize.x, currPos.y % gridSize.y, currPos.z % gridSize.z);

		UpdateMapInfo();
	}
}
