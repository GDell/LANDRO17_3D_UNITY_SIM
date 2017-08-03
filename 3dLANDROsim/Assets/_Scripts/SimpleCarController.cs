using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class SimpleCarController : MonoBehaviour {    

	GameObject wheelColliders;
    List<WheelCollider> wheels = new List<WheelCollider>();
    public static Rigidbody rb;

    // BOOLEANS: Toggle to change further function behaviors.
    	// True if you want to print IR and LDR data.
    public bool displayFitnessInfo;
    	// True if you want to use network, false for default IR/Light behavior.
	public bool useNetwork;
		// True if you want bumper debug info.
	public bool debugBumper;

	public WheelCollider rightMotor;
	public WheelCollider leftMotor;

    // public NeuralNetwork neuralNet;
	public int[] irReadingArray;
	public float[] ldrReadingArray;
	public float maxOfLDRArray;
	public int maxLDRIndex;

    public static float[] rawirDataArray;
    public static float[] irDataArray;
    public float[] irCollisionDataArray;
    public int irSensorNumber = 0;
    public static int[] chosenSensorArray;
    public int[][] listOfChosenSensorArrays;


    public static float[] rawldrDataArray;
    public static float[] ldrDataArray;
    public int ldrSensorNumber = 0;

	public float fitnessLDRscore;
	public float fitnessIRscore;

	BumpSensor[] bump_sensorsPOSITION;
	BumpSensorBack[] backBump_sensorsPOSITION;

	public static BumpSensor[] bump_sensors;
	public static BumpSensorBack[] backBump_sensors;
	
    public IR[] ir_sensors;
    public LDR[] ldr_sensors;


    // INITIALIZE SIMULATION.
    public void Start(){

    	bump_sensors =  GameObject.FindObjectsOfType<BumpSensor>();
    	backBump_sensors = GameObject.FindObjectsOfType<BumpSensorBack>();

    	wheelColliders = GameObject.Find("WheelColliders");
		// VECTORS FOR PLACING SENSORS.
		Quaternion backBump_rotation = new Quaternion();
		Vector3 backBump_position = new Vector3();
		/////
    	Quaternion ir_rotation = new Quaternion();
    	Vector3 ir_position    = new Vector3();
    	/////
    	Quaternion bump_rotation = new Quaternion();
    	Vector3 bump_position = new Vector3();
    	/////
		Quaternion ldr_rotation = new Quaternion();
    	Vector3 ldr_position    = new Vector3();
    	/////////////////////////////////////////////

    	// BOOLEAN TOGGLES.
		displayFitnessInfo = false;
			// T: use network, F: auto movement.
		useNetwork = false;
			// T: enable print statements for bumper functions.
		debugBumper = false;

		// CREATE LANDRO'S WHEELS AND ACCESS THEIR MOTORS.
    	wheels.Add(wheelColliders.transform.Find("frontRight").GetComponent<WheelCollider>());
		wheels.Add(wheelColliders.transform.Find("frontLeft").GetComponent<WheelCollider>());
		WheelCollider leftMotor = GameObject.Find("frontLeft").GetComponent<WheelCollider>();
		WheelCollider rightMotor = GameObject.Find("frontRight").GetComponent<WheelCollider>();

		// GRAB THE THREE SENSOR TYPES.
		// neuralNet = GameObject.GetComponent<NeuralNetwork>();
		// neuralNet = GetComponent<NeuralNetwork>();
		ir_sensors = GameObject.FindObjectsOfType<IR>();
		ldr_sensors = GameObject.FindObjectsOfType<LDR>();
		bump_sensorsPOSITION = GameObject.FindObjectsOfType<BumpSensor>();
		backBump_sensorsPOSITION = GameObject.FindObjectsOfType<BumpSensorBack>();


		//////////////////////////// PLACING SENSORS ON LANDRO BODY //////////////////////////// 
		// PLACE FRONT BUMP SENSORS.
		int i = 0;
		foreach(BumpSensor bump_sensor in bump_sensorsPOSITION){
			bump_rotation = Quaternion.Euler(0, 45 * i, 180);
			bump_position = this.transform.position;
			bump_position.y = 50f;
			bump_position.z += Mathf.Cos(Mathf.PI/4 * i) * 1f;//Divide by 2 because scaled IRs by 0.5
			bump_position.x += Mathf.Sin(Mathf.PI/4 * i) * 1f;
			bump_sensor.transform.rotation = bump_rotation;
			bump_sensor.transform.position = bump_position;
			i++;
		}
		//////////////////////////// 
		// PLACE BACK BUMP SENSORS.
		i = 4;
		foreach(BumpSensorBack backBump_sensor in backBump_sensorsPOSITION){
			backBump_rotation = Quaternion.Euler(0, 45 * i, 180);
			backBump_position = this.transform.position;
			backBump_position.y = 40f;
			backBump_position.z += Mathf.Cos(Mathf.PI/4 * i) * 0f;//Divide by 2 because scaled IRs by 0.5
			backBump_position.x += Mathf.Sin(Mathf.PI/4 * i) * 0f;
			backBump_sensor.transform.rotation = backBump_rotation;
			backBump_sensor.transform.position = backBump_position;
			i++;
		}
		//////////////////////////// 
		// PLACE IR SENSORS.
		i = 0;
		foreach(IR ir_sensor in ir_sensors){
			ir_rotation = Quaternion.Euler(0, 45 * i, 90);
			ir_position = this.transform.position;
			ir_position.y = 115f;
			ir_position.z += Mathf.Cos(Mathf.PI/4 * i) * 525f/2f;//Divide by 2 because scaled IRs by 0.5
			ir_position.x += Mathf.Sin(Mathf.PI/4 * i) * 525f/2f;
			ir_sensor.transform.rotation = ir_rotation;
			ir_sensor.transform.position = ir_position;
			i++;
			irSensorNumber = irSensorNumber + 1;
		}
		//////////////////////////// 
		// PLACE LIGHT SENSORS.
		i = -2;
		foreach(LDR ldr_sensor in ldr_sensors){
			ldr_rotation = Quaternion.Euler(0, 45f * float.Parse(i.ToString()), 90f);
			ldr_position = this.transform.position;
			ldr_position.y = 115f;
			ldr_position.z += Mathf.Cos(Mathf.PI/8 + Mathf.PI/4 * i) * 80f;//i-4 only for use of a subset of sensors
			ldr_position.x += Mathf.Sin(Mathf.PI/8 + Mathf.PI/4 * i) * 80f;
			ldr_sensor.transform.rotation = ldr_rotation;
			ldr_sensor.transform.position = ldr_position;
			i++;

			ldrSensorNumber = ldrSensorNumber + 1;
		}
		/////////////////////////////////////////////////////////////////////////

		// ALLOCATE SPACE IN SENSOR DATA ARRAYS.
		irCollisionDataArray = new float[irSensorNumber];
		rawldrDataArray = new float[ldrSensorNumber];
		rawirDataArray = new float[irSensorNumber];
		irDataArray = new float[irSensorNumber];
		ldrDataArray = new float[ldrSensorNumber];
    }
 
    public void FixedUpdate() {	

    	leftMotor = GameObject.Find("frontLeft").GetComponent<WheelCollider>();
		rightMotor = GameObject.Find("frontRight").GetComponent<WheelCollider>();

    	// GATHERING IR DATA INFO.
    	int i = 0;
    	foreach(IR ir_sensor in ir_sensors) {
    		irCollisionDataArray[i] = ir_sensor.collisionScore;
    		rawirDataArray[i] = ir_sensor.irScore;
    		irDataArray[i] = irScale(ir_sensor.irScore);
    		// print((irScale(ir_sensor.irScore)).ToString());
     		i++;
    	}
    	// GATHERING LDR DATA INFO.
    	int j = 0;
    	foreach(LDR ldr_sensor in ldr_sensors) {
    		rawldrDataArray[j] = ldr_sensor.clacLightScore;
    		ldrDataArray[j] = photoScale(ldr_sensor.clacLightScore);
    		// print((photoScale(ldr_sensor.clacLightScore)).ToString());
    		j++;	
    	}
    }

    // FUNCTION: runMotors() 
    // This function takes two motor torque (left and right) values, checks the bumpers,
    // and drives the motors with the given values unless if the bump sensors are active.
   	public static void runMotors(float leftMotorTorque, float rightMotorTorque) {
   		// Grab wheels in order to drive them.
   		WheelCollider leftMotor = GameObject.Find("frontLeft").GetComponent<WheelCollider>();
		WheelCollider rightMotor = GameObject.Find("frontRight").GetComponent<WheelCollider>();
		float turnTime = 15f;

		string frontBumpType = "";
		string backBumpType = "";

		// Reading the front bumpers.
		bool hasBumped = false;
		for (int k = 0; k < 3; k++) {
			if (bump_sensors[k].bumpWall == true) {
				hasBumped = true;
				bump_sensors[k].bumpWall = false;
				if (bump_sensors[k].name.Contains("leftFront")) {
					frontBumpType = "leftFront";
					print("FRONT HAS BEEN HIT!!");
				} else if (bump_sensors[k].name.Contains("rightFront")) {
					frontBumpType = "rightFront";
					print("RIGHT HAS BEEN HIT!!");
				} else if (bump_sensors[k].name.Contains("middleFront")) {
					frontBumpType = "middleFront";
					print("MIDDLE HAS BEEN HIT!!");
				}
			}
		}
		if (hasBumped == false) {
			// Debug.Log("NO BUMP FRONT");
			frontBumpType = "";
		}

		// Reading the back bumpers.
		bool hasBumpedB = false;
		for (int p = 0; p < 3; p++) {
			if (backBump_sensors[p].bumpWall == true) {
				hasBumpedB = true;
				backBump_sensors[p].bumpWall = false;
				if (backBump_sensors[p].name.Contains("leftBack")) {
					backBumpType = "leftBack";
				} else if (backBump_sensors[p].name.Contains("rightBack")) {
					backBumpType = "rightBack";
				} else if (backBump_sensors[p].name.Contains("middleBack")) {
					backBumpType = "middleBack";
				} 
			}
		}
		if (hasBumpedB == false) {
			// Debug.Log("NO BUMP BACK");
			backBumpType = "";
		}

		// Runs motors if the bumpers havent been hit, otherwise 
		// applies fixed bumper reactions.
		if ((backBumpType == "") && (frontBumpType == "")) {
			// Debug.Log("MOVEMENT FROM NEURAL NETWORK");
			leftMotor.motorTorque = motorScale(leftMotorTorque);
			rightMotor.motorTorque = motorScale(rightMotorTorque);

		} else if (frontBumpType == "leftFront") {
			// Debug.Log("FORCE APPLIED FOR BRAKES");
			float time = 0f;
			// Debug.Log(frontBumpType);
			while (time < turnTime) {
				leftMotor.motorTorque = 15000;
				rightMotor.motorTorque = -12500;
				time = time + Time.deltaTime;
				Debug.Log("TURNING RIGHT");
			} 
		} else if (frontBumpType == "rightFront") {
			// Debug.Log("FORCE APPLIED FOR BRAKES");
			float time = 0f;
			// Debug.Log(frontBumpReading);
			while (time < turnTime) {
				leftMotor.motorTorque =	-12500;
				rightMotor.motorTorque = 15000;
				time = time + Time.deltaTime;
				Debug.Log("TURNING LEFT");
			}
		} else if (frontBumpType == "middleFront") {
			// Debug.Log("FORCE APPLIED FOR BRAKES");
			float time = 0f;
			// Debug.Log(frontBumpReading);
			while (time < turnTime) {
				leftMotor.motorTorque = -10000;
				rightMotor.motorTorque = -20000;
				time = time + Time.deltaTime;
				Debug.Log("MOVING BACK");
			} 
		}
		// } else if (backBumpType == "middleBack") {
		// 	// Debug.Log("FORCE APPLIED FOR BRAKES");
		// 	float time = 0f;
		// 	Debug.Log(backBumpType);
		// 	while (time < turnTime) {
		// 		leftMotor.motorTorque = -115000;
		// 		rightMotor.motorTorque = -115000;
		// 		time = time + Time.deltaTime;
		// 		Debug.Log("TURNING");
		// 	}
		// } else if (backBumpType == "rightBack") {
		// 	// Debug.Log("FORCE APPLIED FOR BRAKES");
		// 	float time = 0f;
		// 	// Debug.Log(backBumpReading);
		// 	while (time < turnTime) {
		// 		leftMotor.motorTorque = -114000;
		// 		rightMotor.motorTorque = -113725;
		// 		time = time + Time.deltaTime;
		// 		Debug.Log("TURNING");
		// 	}
		// } else if (backBumpType == "leftBack") {
		// 	// Debug.Log("FORCE APPLIED FOR BRAKES");
		// 	float time = 0f;
		// 	// Debug.Log(backBumpReading);
		// 	while (time < turnTime) {
		// 		leftMotor.motorTorque = -113725;
		// 		rightMotor.motorTorque = -114000;
		// 		time = time + Time.deltaTime;
		// 		Debug.Log("TURNING");
		// 	}
		// }
		arrowMove();
    }

    // FUNCTION: stopMovement()
    // This function stops all movement of the landro body.	
    public static void stopMovement() {
    	WheelCollider leftMotor = GameObject.Find("frontLeft").GetComponent<WheelCollider>();
		WheelCollider rightMotor = GameObject.Find("frontRight").GetComponent<WheelCollider>();
		rb = GameObject.Find("L16A").GetComponent<Rigidbody>();
		leftMotor.motorTorque = 0;
		leftMotor.motorTorque = 0;
		rb.velocity = new Vector3(0, 0, 0); 
		leftMotor.brakeTorque = 1000;
		rightMotor.brakeTorque = 1000;
		print("STOPPED");
    }

   	// SCALES IR VALUES GIVEN MIN AND MAX POSSIBLE IR VALUES.
	public static float irScale(float val) {
		float fromLow = 0;
		float fromHigh = 409;
		float toLow = 0;
		float toHigh = 1;
		float mapVal = (((val - fromLow) * (toHigh - toLow)) / (fromHigh - fromLow)) + toLow;
		return (mapVal);
	}
	// SCALES LDR VALUES GIVEN MIN AND MAX POSSIBLE IR VALUES.
	public static float photoScale(float val) {
		float fromLow = 0;
		float fromHigh = 1550f;
		float toLow = 0;
		float toHigh = 1;
		float mapVal = (((val - fromLow) * (toHigh - toLow)) / (fromHigh - fromLow)) + toLow;
		return (mapVal);
	}
	// SCALES MOTOR VALUES PRIOR TO ASSIGNING MOTOR TORQUE.
    public static float motorScale(float val) {
		float fromLow = -1;
		float fromHigh = 1;
		float toLow = -1000;
		// 0
		// -400 in arduino code.
		float toHigh = 2000;
		// 500
		// 400 in arduino code.
		float mapVal = (((val - fromLow) * (toHigh - toLow)) / (fromHigh - fromLow)) + toLow;
		return (mapVal);
	}
    // FUNCTION: arrowMove()
    // This function allows the user to control Landro using the arrow keys. Good for debugging and testing purposes.
    public static void arrowMove() {
    	WheelCollider leftMotor = GameObject.Find("frontLeft").GetComponent<WheelCollider>();
		WheelCollider rightMotor = GameObject.Find("frontRight").GetComponent<WheelCollider>();
    	 if (Input.GetKey(KeyCode.UpArrow)) {
		    leftMotor.motorTorque = 2000;
		    rightMotor.motorTorque = 2000;
		    // print("UP");
		}
		if (Input.GetKey(KeyCode.DownArrow)){
		    leftMotor.motorTorque = -1000;
		    rightMotor.motorTorque = -1000;
		    // print("DOWN");
		}
		if (Input.GetKey(KeyCode.RightArrow)){
		    leftMotor.motorTorque = 1000;
		    rightMotor.motorTorque = -1000;
		    // print("RIGHT");
		}
		if (Input.GetKey(KeyCode.LeftArrow)){
		    leftMotor.motorTorque = -1000;
		    rightMotor.motorTorque = 1000;
		    // print("LEFT");
    	}
	}

	// FUNCTIONS FOR RETURNING SENSOR DATA ARRAYS.
	// Raw readings have not been scaled using the sensor scaling functions.
    public static float[] returnRawLDRdata() {
    	return rawldrDataArray;
    } 
    public static float[] returnRawIRdata() {
    	return rawirDataArray;
    }
    public static float[] returnLDRdata() {
    	return ldrDataArray;
    }
    public static float[] returnIRdata() {
    	return irDataArray;
    }

}