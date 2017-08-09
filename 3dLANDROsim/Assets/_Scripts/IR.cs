using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// IR CLASS: This class belongs to each of simlated Landro's IR sensors.
// 			 Each IR is simulated as a cone pointing out from Landro.
//		FUNCTIONS: 
// 		OnTriggerStay() - This function checks to see whether the IR cone has collided with the wall,
//						  simulating IR sensor readings by taking the distance from the wall and the start of
//						  the cone.
public class IR : MonoBehaviour {
	public bool hitWall;
	public float maxIRdistance = 533f;
	public float irDistance;
	public float irScore;
	public int collisionScore = 0;
	public string sensorName;
	public float IRthreshold = 336;

	float irDistanceMax = 0;
	public bool isBumped = false;

	public float[] irDataArray;


	public static int IRindex; 
	public static string IRstring;

	public static float phiOrientation;

 
	GameObject landroBody;
	

	void Start () {
		hitWall = false;
	}	
	
	void Update () {


		float tempPhi = (float)this.transform.eulerAngles.x;
		phiOrientation = calcPhi(tempPhi);

		Debug.Log(this + " IR rotation = "+ phiOrientation);
		// Constantly checks to see whether the IR is in range to sense the wall.
		// If it isn't, we know the ir sensor readings are 0.
		if ((this.hitWall == false)) {
			this.irScore = 0;
		}

	}

	void OnTriggerStay(Collider source){
			

			// Checks to see whether the IR cone has collided with the wall.
			if (source.name.Contains ("Wall")) {

				hitWall = true;
				irDistance = Vector3.Distance(this.transform.position,source.transform.position);
				// Debug.Log("THE IR DISTANCE IS: " + irDistance);

				// if (irDistance > irDistanceMax) {
				// 	irDistanceMax = irDistance;
				// }

				if (irDistance <= 336) {
					irScore = 336 - irDistance;
				}
				

				if (irScore > 0) {
					print("IR threshold exceeded!");
				} 
			} 

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
