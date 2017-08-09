using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;


public class LDR : MonoBehaviour {

	public GameObject lightSource;
	public float clacLightScore;

	public static int LDRIndex; 
	public static string LDRstring;
	public string sensorName;

	public static float phiOrientation;
	
	// The light threshold. Light sensors must be at least this far away from
	// the light source in order to be considered in a high light area of the arena.
	public float lightThreshold = 2426.46f;

	public static float distance;
	
	
	void Start () {
		clacLightScore = 0.0f;
		// maxSpotRange = 1550f;
		// Find the light and assign it to the light game object
		lightSource =  GameObject.Find("EnvLight");
		print("Found " + lightSource.name);
	}

	// Constantly checks the distance between the LDR sensor and the light source.
	void Update () {
		// Debug.Log(this + " LDR rotation = "+ this.transform.eulerAngles.x);

		float tempPhi = (float)this.transform.eulerAngles.x;
		phiOrientation = calcPhi(tempPhi);


		// Debug.Log(this + " LDR PHI is = " + phiOrientation);

		lightSource =  GameObject.Find("EnvLight");
		distance = Vector3.Distance(this.transform.position,lightSource.transform.position);



		// If the light source is within the LDR threshold value, we know we are in a high light area.
		// if (distance < 2426.460) {
		// 	clacLightScore = 2426.460f - distance;
		// } else {
		// 	clacLightScore = 0;
		// }

		float clacLightScore = calculateLDRreading(phiOrientation,distance);

		Debug.Log("The current reading is "+ clacLightScore);

	}


	// FUNCTION: calculateLDRreading()
	// This function implements an LDR sensor model based on a heatmap obtained in lab.
	// The model takes a phi value or orientation value and the distance from the sensor 
	// to the light source in order to  calculate a sensor reading.
	float calculateLDRreading(float phi, float dist) {
		float reading = (float)(84.7906 + (194.2417 * Math.Cos(phi)) + (-71.0813 * Math.Sin(phi)) + (-0.1755 * dist) + (-0.5364 * Math.Cos(phi) * dist));

		return reading;
	}


	float calcPhi(float tempPhi) {
		float calculatedPhi;
		calculatedPhi = tempPhi + 90; 

		if (calculatedPhi > 360) {
			calculatedPhi = calculatedPhi - 360;
		}

		return calculatedPhi;
	}
}
