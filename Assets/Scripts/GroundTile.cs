using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer))]
public class GroundTile : MonoBehaviour {
	public GroundTile forward {
		get;
		set;
	}
	public GroundTile back {
		get;
		set;
	}
	public GroundTile left {
		get;
		set;
	}
	public GroundTile right {
		get;
		set;
	}

	public Vector2 tileIndex;

	public bool Visited {
		get;
		set;
	}

	public float DistanceFromStart {
		get;
		set;
	}

	void Start () {
	}

	void Update () {
		
	}

	public override string ToString ()
	{
		return string.Format ("[GroundTile: Next={0}, TileIndex={1}, Visited={2}]", forward, tileIndex, Visited);
	}
}
