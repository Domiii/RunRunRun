using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class LevelGenerator : MonoBehaviour {
	const int nSafeTilesAfterBend = 3;

	public GroundTile tilePrefab;
	public GroundTile sourceTile;
	public Player player;
	public float bendChance = 0.12f;
	public float obstacleChance = 0.2f;
	public int advanceTiles = 40;
	public Obstacle[] obstaclePrefabs;

	int bend;
	int nVisitedTiles, nTotalTiles;
	int nTilesSinceLastBend;
	GroundTile latestTile;
	Bounds tileBounds;
	//float tileRadiusMax;
	List<GroundTile> tiles;
	Quaternion leftRotation, rightRotation;
	Collider[] colliderBuffer;

	public int RemainingTiles {
		get {
			return nTotalTiles - nVisitedTiles;
		}
	}

	public int MaxTiles {
		get {
			return 2 * advanceTiles;
		}
	}

	public void GenerateMoreTiles() {
		if (RemainingTiles < advanceTiles) {
			GenerateNextTile ();
		}
		if (tiles.Count > MaxTiles) {
			// start removing old tiles
			var tile = tiles[0];
			Destroy (tile.gameObject);
			tiles.RemoveAt(0);
		}
	}

	public void GenerateNextTile() {
		Vector3 dir;
		bool isBend;
		int bendCount;
		var nTries = 0;

		while (true) {
			isBend = false;
			bendCount = bend;
			var v = Random.value;
			dir = latestTile.transform.forward;
			if (v < bendChance) {
				isBend = true;
				// don't bend more than 90 degrees
				if (bendCount < 1 && (bendCount <= -1 || v < bendChance/2)) {
					++bendCount;
					dir = rightRotation * dir;
				} else {
					--bendCount;
					dir = leftRotation * dir;
				}
			}

			var testPos = GetNextTilePosition(dir, GetNextTilePosition ());
			if (Physics.OverlapBoxNonAlloc (testPos, tileBounds.extents * 1.1f, colliderBuffer) <= 2) {
				//print (testPos + " <> " + colliderBuffer[0].transform.position);
				// good!
				break;
			}

			if (++nTries > 100) {
				print ("Could not generate new tile!");
				break;
			}
		}

		// update book-keeping
		bend = bendCount;
		if (isBend) {
			nTilesSinceLastBend = 0;
		}
		else {
			++nTilesSinceLastBend;
		}

		AddTile (dir, nTilesSinceLastBend > nSafeTilesAfterBend);
	}

	public void Clear() {
		// delete all tiles
		tiles.ForEach(t => DestroyImmediate(t));
		tiles.Clear ();
	}

	public Transform TileContainer {
		get {
			return latestTile.transform.parent;
		}
	}

	void Reset() {
		colliderBuffer = new Collider[64];
		tiles = new List<GroundTile>();
		leftRotation = Quaternion.Euler (0, -90, 0);
		rightRotation = Quaternion.Euler (0, 90, 0);
	}

	void Start () {
		Reset ();

		latestTile = sourceTile;
		tileBounds = sourceTile.GetComponent<MeshRenderer>().bounds;
		//tileRadiusMax = Mathf.Max(tileBounds.extents.x, tileBounds.extents.z);

		sourceTile.tileIndex = Vector2.zero;

		// let's get going!
		RegisterTile (sourceTile);
		AddTile (player.Forward, false);
		AddTile (player.Forward, false);
	}

	void Update () {
		var currentGroundTile = player.CurrentGroundTile;
		MarkTilesVisited (currentGroundTile);

		GenerateMoreTiles ();
	}

	void MarkTilesVisited(GroundTile tile) {
		if (tile != null && !tile.Visited) {
			// visited new tile
			tile.Visited = true;
			++nVisitedTiles;
			MarkTilesVisited (tile.back);
		}
	}

	Vector2 NewIndex(Vector2 index, Vector3 direction) {
		index.x += direction.x;
		index.y += direction.z;
		return index;
	}

	Vector3 GetNextTilePosition() {
		// compute new position
		return GetNextTilePosition(latestTile.transform.forward, latestTile.transform.position);
	}

	Vector3 GetNextTilePosition(Vector3 direction, Vector3 position) {
		// compute new position
		var dist = 2 * Mathf.Abs(Vector3.Dot(direction, tileBounds.extents));
		var pos = position + dist * direction;
		return pos;
	}

	GroundTile AddTile(Vector3 direction, bool decorate = true) {
		return AddTile (GetNextTilePosition(), direction, decorate);
	}

	GroundTile AddTile(Vector3 pos, Vector3 direction, bool decorate = true) {
		// create new tile
		var newTile = (GroundTile)Instantiate (tilePrefab, pos, Quaternion.identity, TileContainer);
		newTile.tileIndex = NewIndex(latestTile.tileIndex, direction);
		newTile.DistanceFromStart = latestTile.DistanceFromStart + 2 * tileBounds.extents.x;
		newTile.transform.forward = direction;
		newTile.back = latestTile;
		latestTile.forward = newTile;

		RegisterTile (newTile);

		// decorate
		if (decorate) {
			DecorateNewTile (newTile);
		}
		latestTile = newTile;
		return newTile;
	}

	void RegisterTile(GroundTile tile) {
		tiles.Add (tile);
		++nTotalTiles;
	}

	void DecorateNewTile(GroundTile newTile) {
		if (Random.value < obstacleChance) {
			var dist = newTile.DistanceFromStart;
			var prefabs = obstaclePrefabs.Where (o => dist >= o.minDistance);
			var count = prefabs.Count();
			var prefab = prefabs.ElementAt (Random.Range(0, count));

			if (prefab != null) {
				var obj = (Obstacle)Instantiate (prefab);
				obj.transform.SetParent (newTile.transform, true);
				obj.transform.forward = newTile.transform.forward;
				obj.transform.position += newTile.transform.position;
				//newTile.GetComponent<Renderer> ().material = obj.GetComponent<Renderer> ().material;		// mark tile by painting in object's material
			}
		}
	}
}
