using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public static class Shape
{

	public delegate bool ShapeTest (Transform root,Mesh mesh,Vector3 point);

	/**
	 * Create tile mesh with supplied verts and normal.
	 */
	public static Mesh MakeMesh (Vector3 normal, List<Vector3> verts)
	{
		Mesh m = new Mesh ();
		m.vertices = verts.ToArray ();

		List<Vector2> triVerts = _surfacePoints (verts);
		List<Vector3> normals = new List<Vector3> ();

		for (int i = 0; i<verts.Count; i++) {
			normals.Add (normal);
		}

		Triangulator t = new Triangulator (triVerts.ToArray ());
		m.triangles = t.Triangulate ();
		m.normals = normals.ToArray ();
		m.uv = _genUV (triVerts);

		return m;
	}

	/**
 	* Create a plane mesh of given dimensions 
 	*/
	public static Mesh Plane (float width, float depth)
	{
		float pWidth = width / 2;
		float pDepth = depth / 2;
		Vector3 startPt = new Vector3 (-1 * pWidth, 0, -1 * pDepth);
		List<Vector3> verts = new List<Vector3> ();
		verts.Add (startPt);
		verts.Add (startPt + new Vector3 (0, 0, depth));
		verts.Add (startPt + new Vector3 (width, 0, depth));
		verts.Add (startPt + new Vector3 (width, 0, 0));

		return MakeMesh (Vector3.up, verts);
	}


	/**
	 * Create a cirle mesh with radius and vertCount. More vertices equals a smoother edge on the circle.  
	 */
	public static Mesh Circle (float radius, int vertCount)
	{
		Mesh m = new Mesh ();
		List<Vector3> verts = _getCircleVerts (vertCount, radius);

		//verts = _sortCircleVerts (verts);

		List<int> tries = new List<int> ();
		int j = verts.Count - 1;
		for (int i = 1; i< verts.Count; i++) {
			tries.Add (0);

			if ((i - 1) > 0) {
				tries.Add (i - 1);
			} else {
				tries.Add (j);
			}

			tries.Add (i);
		}

		List<Vector3> normals = new List<Vector3> ();

		for (int i = 0; i< verts.Count; i++) {
			normals.Add (Vector3.up);
		}

		m.vertices = verts.ToArray ();
		m.triangles = tries.ToArray ();
		m.normals = normals.ToArray ();
		m.uv = _genUV (_surfacePoints (verts));

		return m;
	}

	/**
	 * Raycast from with every tri in the mesh in given direction. Useful for determining tile overlap and detecting contained objects. 
	 */
	public static List<GameObject> Raycast (Transform root, Mesh mesh, float height, Vector3 direction)
	{
		List<GameObject> result = new List<GameObject> ();

		if (mesh != null) {

			List<Triangle> tries = new List<Triangle> ();

			Triangle tri = new Triangle ();
			foreach (int index in mesh.triangles) {
				tri.AddPt (mesh.vertices [index]);

				if (tri.Complete) {
					tries.Add (tri);
					tri = new Triangle ();
				}
			}

			foreach (Triangle t in tries) {

				for (int i = 1; i<10; i++) {
			
					//Side One
					float sOneLength = i * (t.SideOne / 10);
					Vector3 ptOne = t.SideOneDir * sOneLength;
			
					//Side Two 
					float sTwoLength = i * (t.SideTwo / 10);
					Vector3 ptTwo = t.SideTwoDir * sTwoLength;
			
			
			
					Vector3 dir = (ptTwo - ptOne).normalized;
			
					float d = Vector3.Distance (ptTwo, ptOne);
					float dSub = d / i;
			
			
					for (float k = 0; k < d; k += dSub) {
						Vector3 pt = ptOne + ((dir) * k);

						Ray r = new Ray (root.position + pt, direction);
						RaycastHit hit = new RaycastHit ();

						if (Physics.Raycast (r, out hit, height)) {
							if (!result.Contains (hit.collider.gameObject)) {
								result.Add (hit.collider.gameObject);
							}
						}
					}
				}
			}
		}
		return result;
	}

	/**
	 * Check if point is contained within circle mesh. Assumes 0 index vert is center. Only tests x and z values in point. 
	 */
	public static bool CircleContains (Transform root, Mesh mesh, Vector3 point)
	{
		Vector3 center = mesh.vertices [0];
		float radius = Vector3.Distance (center, mesh.vertices [1]);
		return Mathf.Pow((point.x - center.x), 2) + Mathf.Pow((point.z - center.z), 2) < Mathf.Pow(radius,2);
	}

    /**
 	* Check if point is contained within polycon mesh. Only tests x and z values in point. 
 	*/
	public static bool PolygonContains (Transform root, Mesh mesh, Vector3 point)
	{
		bool isInside = false;
        
		if (mesh != null) {
			List<Vector3> verts = new List<Vector3> ();

			foreach (Vector3 vert in mesh.vertices) {

				verts.Add (vert + root.position);
			}

			Vector3[] bounds = verts.ToArray ();
			for (int i = 0, j = bounds.Length - 1; i < bounds.Length; j = i++) {
				if (((bounds [i].z > point.z) != (bounds [j].z > point.z)) &&
					(point.x < (bounds [j].x - bounds [i].x) * (point.z - bounds [i].z) / (bounds [j].z - bounds [i].z) + bounds [i].x)) {
					isInside = !isInside;
				}
			}
		}
		
		return isInside;
	}

	/**
	 * Generate a random point within the passed mesh. 
	 */
	public static Vector3 GetRandomPoint (Transform root, Mesh mesh, ShapeTest test)
	{

		List<Vector3> v = new List<Vector3> ();
		foreach (Vector3 point in mesh.vertices) {
			v.Add (point);
		}
		
		int index = Random.Range (0, v.Count);
		Vector3 start = v [index];
		v.RemoveAt (index);
		
		index = Random.Range (0, v.Count);
		Vector3 end = v [index];
		
		float distance = Vector3.Distance (end, start);
		float interval = distance / Random.Range (3, 10);
		Vector3 direction = (end - start).normalized;
		float range = 0;
		bool done = false;
		
		Vector3 result = root.position;
		while (range < distance && !done) {
			
			Vector3 pt = new Vector3 (start.x + (direction.x * range), root.position.y, start.z + (direction.z * range));

			if (test != null) {
				if (test (root, mesh, pt)) {
					result = pt;
					done = true;
				}
			} else {
				result = pt;
				done = true;
			}

			range += interval;
		}
		
		return result;
	}

	private static List<Vector3> _getCircleVerts (int verts, float radius)
	{
		if (verts < 1) {
			verts = 1;
		}

		List<Vector3> pts = new List<Vector3> ();

		Vector3 root = Vector3.zero;
		float chngDgre = 360 / (float)verts;

		pts.Add (root);

		Vector3 pt = root + new Vector3 (radius, 0, 0);

		for (int i = 0; i < verts; i++) {
			Vector3 k = Quaternion.Euler (new Vector3 (0, chngDgre * i, 0)) * pt;
			pts.Add (k);
		}

		return pts;
	}

	private static Vector2[] _genUV (List<Vector2> points)
	{
		Vector2 min = Vector2.zero;
		Vector2 max = Vector2.zero;
		
		//Find the edges of the shape. 
		foreach (Vector2 point in points) {
			
			//Handling x
			if (point.x < min.x) {
				min.x = point.x;
			} else if (point.x > max.x) {
				max.x = point.x;
			}
			
			//Handling y
			if (point.y < min.y) {
				min.y = point.y;
			} else if (point.y > max.y) {
				max.y = point.y;
			}
		}
		
		List<Vector2> result = new List<Vector2> ();
		
		Vector2 size = max - min;
		
		foreach (Vector2 point in points) {
			
			float x = point.x - min.x; 
			float y = point.y - min.y;
			
			Vector2 uv = new Vector2 (x / size.x, y / size.y);
			
			result.Add (uv);
		}
		
		return result.ToArray ();
		
	}

	private static List<Vector2> _surfacePoints (List<Vector3> verts)
	{
		List<Vector2> result = new List<Vector2> ();

		foreach (Vector3 vert in verts) {
			result.Add (new Vector2 (vert.x, vert.z));
		}
			

		return result;
	}

	private static bool _isEven (float val)
	{
		float k = val / 2;
		return k % 1 == 0;
	}

	private static List<Vector3> _sortCircleVerts (List<Vector3> verts)
	{

		List<Vector3> top = new List<Vector3> ();
		List<Vector3> bottom = new List<Vector3> ();

		foreach (Vector3 vert in verts) {

			if (vert == Vector3.zero) {
			} else if (vert.z > 0) {
				top.Add (vert);
			} else if (vert.z < 0) {
				bottom.Add (vert);
			} else if (vert.x < 0) {
				bottom.Add (vert);
			} else {
				top.Add (vert);
			}
		}

		top.Sort ((x,y) => (x.x.CompareTo (y.x)));
		bottom.Sort ((x,y) => (x.x.CompareTo (y.x)));


		bottom.Reverse ();

		List<Vector3> r = new List<Vector3> ();
		r.Add (Vector3.zero);
		top.ForEach (x => r.Add (x));
		bottom.ForEach (x => r.Add (x));
		return r;


	}
}

public struct BlindPoint
{
	public float MinDegree;
	public float MaxDegree;
}
