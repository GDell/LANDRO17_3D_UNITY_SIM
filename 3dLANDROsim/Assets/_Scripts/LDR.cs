using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LDR : MonoBehaviour {

	public GameObject lightSource;
	public float clacLightScore;
	
	// The light threshold. Light sensors must be at least this far away from
	// the light source in order to be considered in a high light area of the arena.
	public float lightThreshold = 2426.46f;
	public string sensorName;
	
	void Start () {
		clacLightScore = 0.0f;
		// maxSpotRange = 1550f;
		// Find the light and assign it to the light game object
		lightSource =  GameObject.Find("EnvLight");
		print("Found " + lightSource.name);
	}

	// Constantly checks the distance between the LDR sensor and the light source.
	void Update () {
		lightSource =  GameObject.Find("EnvLight");
		float distance = Vector3.Distance(this.transform.position,lightSource.transform.position);
		// If the light source is within the LDR threshold value, we know we are in a high light area.
		if (distance < 2426.460) {
			clacLightScore = 2426.460f - distance;
		} else {
			clacLightScore = 0;
		}

	}
}
