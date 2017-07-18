using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LDR : MonoBehaviour {

	// Create a light object to reference the main light
	public GameObject lightSource;
	public float clacLightScore;
	public float maxSpotRange = 1550f;//641;
	public float lightThreshold = 500;

	// public float comulativeIndividualLightScore = 0;
	public string sensorName;

	

	void Start () {
		clacLightScore = 0.0f;
		maxSpotRange = 1550f;
		// Find the light and assign it to the light game object
		lightSource =  GameObject.Find("EnvLight");
		print("Found " + lightSource.name);
	}

	// Update is called once per frame
	void Update () {
		// Calculate the distance to the light from this sensor
		// float xyVectorLandro = Mathf.Sqrt(Mathf.Pow(this.transform.position.x, 2) + Mathf.Pow(this.transform.position.y, 2));
		// float xyVectorLight = Mathf.Sqrt(Mathf.Pow(this.transform.position.x, 2) + Mathf.Pow(this.transform.position.y, 2));
		float distance = Vector3.Distance(this.transform.position,lightSource.transform.position);
		// print("DISTANCE: " + distance);
		// Calculate how much light the sensor has collected
		clacLightScore = maxSpotRange - distance;

		if (clacLightScore > lightThreshold) {
			print("LDR threshold exceeded!");
		}
		// comulativeIndividualLightScore = clacLightScore + comulativeIndividualLightScore;
		// print("Light collected at sensor"+ this + " is: "+ clacLightScore );
		
		// Print it
		// if (distance > maxSpotRange) {
		// 	// print ("No Light Detected");
		// } else {
		// 	// print("Light collection score at sensor "+ this + " is: "+ clacLightScore );
		// 	// print ("LIGHT DETECTED");
		// }
	}
}
