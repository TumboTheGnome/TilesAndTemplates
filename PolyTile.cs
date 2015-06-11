using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PolyTile : MonoBase, ITile
{
	private Mesh _mesh;

	#region ITile implementation

	public GameObject GameObject {
		get {
			return this.gameObject;
		}
	}

	public List<GameObject> Overlap (float height, Vector3 normal)
	{
		return Shape.Raycast (this.transform, this._mesh, height, normal);
	}

	public bool Contains (Vector3 point)
	{
		return Shape.PolygonContains (this.transform, this._mesh, point);
	}

	public Vector3 GetContainedPoint ()
	{
		return Shape.GetRandomPoint (this.transform, this._mesh, Shape.PolygonContains);
	}

    public bool Overlap(PolyTile tile)
    {
        bool result = false;
        if (tile != null)
        {
            foreach (Vector3 vert in tile.Mesh.vertices)
            {
                if(this.Contains(vert+tile.transform.position))
                {
                    result = true;
                    break;
                }
            }
        }

        return result;
    }

	public Material Material {
		get {
			return this.GetComponent<MeshRenderer>().material;
		}
		set {
			this.GetComponent<MeshRenderer>().material = value;
		}
	}

	public Mesh Mesh {
		get {
			return this._mesh;
		}
		set {
			this._mesh = value;
			this.GetComponent<MeshFilter>().mesh = this._mesh;
		}
	}

	public bool ShowTile {
		get {
			return this.GetComponent<MeshRenderer>().enabled;
		}
		set {
			this.GetComponent<MeshRenderer>().enabled = value;
		}
	}

	#endregion
	
}
