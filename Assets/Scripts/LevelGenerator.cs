using UnityEngine;
using System.Collections.Generic;


public class LevelGenerator : MonoBehaviour {
	public GroundTile tilePrefab;
	public GroundTile sourceTile;
	public Player player;
	public float bendProbability = 0.2f;
	public float advanceTiles = 40;

	int bend;
	int nVisitedTiles;
	GroundTile latestTile;
	Bounds tileBounds;
	//float tileRadiusMax;
	List<GroundTile> tiles;
	Quaternion leftRotation, rightRotation;
	Collider[] colliderBuffer;

	public int RemainingTiles {
		get {
			return tiles.Count - nVisitedTiles;
		}
	}

	public void GenerateMoreTiles() {
		while (RemainingTiles < advanceTiles) {
			GenerateNextTile ();
		}
	}

	public void GenerateNextTile() {
		Vector3 dir;
		var nTries = 0;
		while (true) {
			var b = bend;
			var v = Random.value;
			dir = latestTile.transform.forward;
			if (v < bendProbability) {
				// don't bend more than 90 degrees
				if (b < 1 && (b <= -1 || v < bendProbability/2)) {
					++b;
					dir = rightRotation * dir;
				} else {
					--b;
					dir = leftRotation * dir;
				}
			}

			var testPos = NewTilePosition (dir);
			if (Physics.OverlapBoxNonAlloc (testPos, tileBounds.extents * 1.1f, colliderBuffer) <= 2) {
				//print (testPos + " <> " + colliderBuffer[0].transform.position);
				// good!
				bend = b;
				break;
			}

			if (++nTries > 100) {
				print ("Could not generate new tile!");
				break;
			}
		}

		AddTile (dir);
	}

	public void Clear() {
		// delete all tiles
		tiles.ForEach(t => DestroyImmediate(t));
		tiles.Clear ();
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
		AddTile (player.Forward);
	}

	void Update () {
		var currentGroundTile = player.GetCurrentGroundTile ();
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

	Vector3 NewTilePosition(Vector3 direction) {
		// compute new position
		var dist = 2 * Mathf.Abs(Vector3.Dot(direction, tileBounds.extents));
		var pos = latestTile.transform.position + dist * direction;
		return pos;
	}

	GroundTile AddTile(Vector3 direction) {
		return AddTile (NewTilePosition(direction), direction);
	}

	GroundTile AddTile(Vector3 pos, Vector3 direction) {
		// create new tile
		var newTile = (GroundTile)Instantiate (tilePrefab, pos, Quaternion.identity, latestTile.transform.parent);
		newTile.tileIndex = NewIndex(latestTile.tileIndex, direction);
		newTile.transform.forward = direction;
		newTile.back = latestTile;
		latestTile.forward = newTile;
		tiles.Add (newTile);

		// decorate
		DecorateNewTile (newTile);
		latestTile = newTile;
		return newTile;
	}

	void DecorateNewTile(GroundTile newTile) {
		// TODO: Add interesting features to new tiles
	}
}
