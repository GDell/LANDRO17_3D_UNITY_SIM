using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IR : MonoBehaviour {
	public bool hitWall;
	public float maxIRdistance = 533f;
	public float irDistance;
	public float irScore;
	public int collisionScore = 0;
	public string sensorName;
	public float IRthreshold = 336;

	public bool isBumped = false;

	public float[] irDataArray;
 //    WheelFrictionCurve wfcLEFT;
 //    WheelFrictionCurve wfcRIGHT;
	// wfcLEFT = _colliderRR.sidewaysFriction;
	// wfcRIGHT = _colliderRR.sidewaysFriction;
	// myWfc.extremumSlip = 40f;
	// _colliderRR.sidewaysFriction = myWfc;
 
	GameObject landroBody;
	
	// Use this for initialization
	void Start () {
		hitWall = false;
		// rb = GameObject.Find("L16A").GetComponent<Rigidbody>();
	}	
	
	// Update is called once per frame
	void Update () {


		// if ((this.name.Contains("_f")) || (this.name.Contains("FRONT"))) {
		// 	// print(this + " Distance: " +this.irDistance);
		// 	// print(this + " Score: " +this.irDistance);
		// 	// print(this);
		// 	if ((this.irScore > 430)) {
		// 		this.isBumped = true;
		// 		print(this + " has said: " +" IVE DONE BEEN BUMPED!!!");
		// 	} else {
		// 		this.isBumped = false;
		// 	}
		// }


		if ((this.hitWall == false)) {
			this.irScore = 0;
			// Debug.Log("NOT COLLIDING WITH WALL");
		}


		// if (hitWall == true) {
		// 	wfcRIGHT.extremumSlip = 40f;
		// 	wfcLEFT.extremumSlip = 40f;
		// 	leftMotor.forwardFriction = wfcLEFT;
		// 	rightMotor.forwardFriction = wfcRIGHT;
		// 	// leftMotor.forwardFriction.extremumSlip = ;
		// 	// rightMotor.forwardFriction.extremumSlip = ;
		// } else {
		// 	wfcRIGHT.extremumSlip = 4f;
		// 	wfcLEFT.extremumSlip = 4f;
		// // 	leftMotor.forwardFriction.extremumSlip = 4,4;
		// // 	rightMotor.forwardFriction.extremumSlip = 4,4;
		// }
		
	}

	void OnTriggerStay(Collider source){
		
		//if(!source.name.Contains("L16A")){
			//print(this.name + " hit " + source.name);
			if (source.name.Contains ("Wall")) {
				// Debug.Log("IR HAS DETECTED THE WALL");
				hitWall = true;
				irDistance = Vector3.Distance(this.transform.position,source.transform.position);
				// Debug.Log("THE IR DISTANCE IS: " + irDistance);
				// float irTemp = irDistance - IRthreshold;
				// if (irTemp < 0) {
				// 	irTemp = 0; 
				// }

				irScore = irScore + 1;

				if (irScore > 0) {
					print("IR threshold exceeded!");
				} 
				// print(irScore);
				// print(this + " IR SCORE iS: " + irScore);
				// print ("HIT THE WALL");
			} 


		//}
	}
}
