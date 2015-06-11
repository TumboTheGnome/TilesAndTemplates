using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Triangle
{
	private List<Vector3> _points = new List<Vector3> ();

	public void AddPt (Vector3 pt)
	{
		this._points.Add (pt);
	}

	public bool Complete {
		get {
			return (this._points.Count == 3);
		}
	}

	public float SideOne {
		get {
			if (this.Complete) {
				return Vector3.Distance (this._points [0], this._points [1]);
			} else {
				return 0;
			}
		}
	}

	public float SideOneAngle {
		get {
			float num = Mathf.Pow (this.SideTwo, 2) - Mathf.Pow (this.SideThree, 2) - Mathf.Pow (this.SideOne, 2);
			float denum = -2 * (this.SideThree * this.SideOne);
			return Mathf.Acos (num / denum);
		}
	}

	public Vector3 SideOneDir{
		get{
			if(this.Complete)
			{
			return (this._points[1]-this._points[0]).normalized;
			}else{
				return Vector3.zero;
			}
		}
	}

	public float SideTwo {
		get {
			if (this.Complete) {
				return Vector3.Distance (this._points [1], this._points [2]);
			} else {
				return 0;
			}
		}
	}

	public float SideTwoAngle {
		get {
			float num = Mathf.Pow (this.SideThree, 2) - Mathf.Pow (this.SideTwo, 2) - Mathf.Pow (this.SideOne, 2);
			float denum = -2 * (this.SideTwo * this.SideOne);
			return Mathf.Acos (num / denum);
		}
	}

	public Vector3 SideTwoDir{
		get{
			if(this.Complete)
			{
				return (this._points[2]-this._points[1]).normalized;
			}else{
				return Vector3.zero;
			}
		}
	}

	public float SideThree {
		get {
			if (this.Complete) {
				return Vector3.Distance (this._points [2], this._points [0]);
			} else {
				return 0;
			}
		}
	}

	public float SideThreeAngle {
		get {
			float num = Mathf.Pow (this.SideOne, 2) - Mathf.Pow (this.SideThree, 2) - Mathf.Pow (this.SideTwo, 2);
			float denum = -2 * (this.SideThree * this.SideTwo);
			return Mathf.Acos (num / denum);
		}
	}

	public Vector3 SideThreeDir{
		get{
			if(this.Complete)
			{
				return (this._points[0]-this._points[2]).normalized;
			}else{
				return Vector3.zero;
			}
		}
	}

}
