using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LDR : MonoBehaviour {

	// Create a light object to reference the main light
	public GameObject lightSource;
	public float clacLightScore;
	// public float maxSpotRange = 1550f;//641;
	public float lightThreshold = 2426.46f;
	// public float comulativeIndividualLightScore = 0;
	public string sensorName;
	
	void Start () {
		clacLightScore = 0.0f;
		// maxSpotRange = 1550f;
		// Find the light and assign it to the light game object
		lightSource =  GameObject.Find("EnvLight");
		print("Found " + lightSource.name);
	}

	// Update is called once per frame
	void Update () {
		lightSource =  GameObject.Find("EnvLight");
		// print("Found " + lightSource.name);
		// Calculate the distance to the light from this sensor
		// float xyVectorLandro = Mathf.Sqrt(Mathf.Pow(this.transform.position.x, 2) + Mathf.Pow(this.transform.position.y, 2));
		// float xyVectorLight = Mathf.Sqrt(Mathf.Pow(this.transform.position.x, 2) + Mathf.Pow(this.transform.position.y, 2));
		float distance = Vector3.Distance(this.transform.position,lightSource.transform.position);
		// Debug.Log("DISTANCE TO LIGHT IS: "+ distance);
		if (distance < 2426.460) {
			// Debug.Log("We are now in high light!!!!!!!!!!!!!!!!!!!!!!!!");
			clacLightScore = 2426.460f - distance;
			// Debug.Log("calc light score: " + clacLightScore);
		} else {
			clacLightScore = 0;
		}

		// Debug.Log(this + "ldr distance is: "+ distance);
		// if (distance < lightThreshold) {
		// 	print("LDR threshold exceeded!");
		// 	clacLightScore = 2426.46f - distance;
		// 	Debug.Log("light score: " + clacLightScore);
		// }
		// if (distance > lightThreshold) {
		// 	clacLightScore = 0;
		// 	Debug.Log("light score: " + 0);
		// }

		// print("DISTANCE: " + distance);
		// Calculate how much light the sensor has collected
		// clacLightScore = maxSpotRange - distance;

		// if (clacLightScore > 0) {
		// 	print("LDR threshold exceeded!");
		// }
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
