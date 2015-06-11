using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ITile{

	/**
	 * Material used by tile
	 */
	Material Material{get; set;}

	/**
	 * Mesh used by tile
	 */
	Mesh Mesh{get; set;}
	
	/**
	 * Toggle visibility of tile
	 */
	bool ShowTile{get; set;}

	GameObject GameObject {
		get;
	}

	/**
	 * Cast rays from within the shape. Useful for determining overlap. 
	 */
	List<GameObject> Overlap(float height, Vector3 normal);
	
	/**
	 * Check if a point is within tile
	 */
	bool Contains(Vector3 point);

	/**
	 * Generate a random contained point. 
	 */
	Vector3 GetContainedPoint();



}