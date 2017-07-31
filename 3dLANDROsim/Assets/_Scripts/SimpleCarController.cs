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

    // Keeping track of number of LDR and IR sensors.
    public int LDRnumber = 0;
	public int IRnumber = 0;

	// Motor torque and steering parameters.
    public float maxMotorTorque;
    public float maxSteeringAngle;

    // BOOLEANS: Toggle to change further function behaviors.
    	// True if you want to print IR and LDR data.
    public bool displayFitnessInfo;
    	// True if you want to use network, false for default IR/Light behavior.
	public bool useNetwork;
		// True if you want bumper debug info.
	public bool debugBumper;

	public int baseMovementRate;

    // SENSOR ARRAYS
    public IR[] ir_sensors;
    public LDR[] ldr_sensors;
    // public BumpSensor[] bump_sensors;
    // public BumpSensorBack[] backBump_sensors;

	public WheelCollider rightMotor;
	public WheelCollider leftMotor;

    // public NeuralNetwork neuralNet;
	public int[] irReadingArray;
	public float[] ldrReadingArray;
	public float maxOfLDRArray;
	public int maxLDRIndex;

	// IR DATA COLLECTION VARIABLES 
	public int rightMovemntValue;
	public int leftMovementValue;
	public int frontMovementValue;
	public int backMovementValue;
    public string moveType;
    public int numberCollidedIR;
    public int numberCollidedLeftIR;
    public int numberCollidedRightIR;
    public int numberCollidedFrontIR;
    public int numberCollidedBackIR;

    public static float[] rawirDataArray;
    public static float[] irDataArray;
    public float[] irCollisionDataArray;
    public int irSensorNumber = 0;
    public static int[] chosenSensorArray;
    public int[][] listOfChosenSensorArrays;

    // LDR DATA COLLECTION VARIABLES
    public float LEFTfrontLDRreadings;
    public float RIGHTfrontLDRreadings;
    public float rightLDRreadings; 
    public float leftLDRreadings;
    public float backLDRreadings;

    public static float[] rawldrDataArray;
    public static float[] ldrDataArray;
    public int ldrSensorNumber = 0;

	public float fitnessLDRscore;
	public float fitnessIRscore;

    public float rightWheelTorque;
    public float leftWheelTorque;

	BumpSensor[] bump_sensorsPOSITION;
	BumpSensorBack[] backBump_sensorsPOSITION;

	public static BumpSensor[] bump_sensors;
	public static BumpSensorBack[] backBump_sensors;

    // INITIALIZE SIMULATION.
    public void Start(){

    	bump_sensors =  GameObject.FindObjectsOfType<BumpSensor>();
    	backBump_sensors = GameObject.FindObjectsOfType<BumpSensorBack>();


		int maxSpawn = 100;
		int vMax = 5;
		int vDurationMin = 1;
		int vDurationMax = 100;
		int gMax = 3;
		int gDurationMin = 1;
		int gDurationMax = 100;

		int numberOfGenes = 20;
		float dupeRate = 0.5f;
		float muteRate = 0.05f;
		float delRate = 0.01f;
		float changePercent = 0.15f;

		// Test for creating a generation.
		// testGeneration.setGenerationParameters(20, 2, 20); 
		// testGeneration.createStartGeneration();
		// currrentInd = testGeneration.individualIndex;
		// individualLength = testGeneration.collectionOfIndividuals.Length;
		// CREATING A GENOME:
		// testGenome.createRandomFunction();
		// testGenome.setGenomeParameters(numberOfGenes, dupeRate, muteRate, delRate, changePercent);
		// testGenome.createWholeGenome(maxSpawn, vMax, vDurationMin, vDurationMax, gMax, gDurationMin, gDurationMax);
		// testGenome.printGenomeContents();

		// // RUNNING THE G-->P PROCESS:
		// testGtoP.passGenome(testGenome);
		// testGtoP.runDevoGraphics();
		// testGtoP.makeConnectome();
		// testGtoP.printConnectomeContents();

		// // CREATING NEURAL NETWORK PARAMETERS
		// testParams.passConnectionMatrix(testGtoP.sortedConnects, testGenome);
		// testParams.setNodeLayerNumbers();
		// testParams.motorIndexes();
		// testParams.sensorToInputs();
		// testParams.createInputToHidden();
		// testParams.createHiddenToHidden();
		// testParams.createHiddenToOutput();
		// testParams.createInputToOutput();
		// testParams.createOutputToHidden();
		// testParams.finalToArray();
		// // testParams.printParamsContents();

		// // CREATING THE NEURAL NETWORK.
		// testNeuralStruct.setStartVariables(testParams.RMIlength,testParams.LMIlength,testParams.NUM_INPUT,testParams.NUM_HIDDEN,testParams.NUM_OUTPUT);
		// testNeuralStruct.setStartingArrays(testParams.finalRMI, testParams.finalLMI);
		// testNeuralStruct.setConnections(testParams.input_to_output,testParams.input_to_hidden,testParams.hidden_to_hidden, testParams.hidden_to_output, testParams.output_to_hidden);

		/////////////////////////////////////////////////////////////////////////////////////////////////////

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

		// IR DATA VARIABLE INITIALIZATION.
		numberCollidedIR = 0;
		numberCollidedBackIR = 0;
		numberCollidedFrontIR = 0;
		numberCollidedRightIR = 0;
		numberCollidedLeftIR = 0;

		// LDR DATA VARIABLE INITIALIZATION.
		LEFTfrontLDRreadings = 0;
		RIGHTfrontLDRreadings = 0;
		backLDRreadings = 0;
		leftLDRreadings = 0;
		rightLDRreadings = 0;
		baseMovementRate = 500;



		leftMotor.motorTorque = rightWheelTorque;
		rightMotor.motorTorque = leftWheelTorque;

		// // Selected sensors for input into neural network function.
		// chosenSensorArray = new int[4] {2,3,13,14}; 

		//////////////////////////// PLACING SENSORS ON LANDRO BODY //////////////////////////// 
		// PLACE FRONT BUMP SENSORS.
		int i = 0;
		foreach(BumpSensor bump_sensor in bump_sensorsPOSITION){
			bump_rotation = Quaternion.Euler(0, 45 * i, 180);
			bump_position = this.transform.position;
			bump_position.y = 40f;
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

		// sensorReadings currentReadings = new sensorReadings();
		// currentReadings.setSensorArrays();

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

		showArrayInformation(ldrDataArray, false);
		showArrayInformation(irDataArray, false);
		
		//////////////////////COLLECT IR DATA.//////////////////////////////////////
		foreach(IR ir_sensor in ir_sensors){
			if((ir_sensor.hitWall == true) && (ir_sensor.name.Contains("FRONT"))) {
				//print("FRONT IS HIT");
				numberCollidedIR = numberCollidedIR + 1;
				numberCollidedFrontIR = numberCollidedFrontIR + 1;

				ir_sensor.sensorName = ir_sensor.name;
				ir_sensor.collisionScore = ir_sensor.collisionScore +1;
				
				ir_sensor.hitWall = false;
				// print(ir_sensor.sensorName + " Score is: " + ir_sensor.collisionScore);
			} else if((ir_sensor.hitWall == true) && (ir_sensor.name.Contains("BACK"))) {
				//print("BACK IS HIT");
				numberCollidedIR = numberCollidedIR + 1;
				numberCollidedBackIR = numberCollidedBackIR + 1;

				ir_sensor.sensorName = ir_sensor.name;
				ir_sensor.collisionScore = ir_sensor.collisionScore +1;
				
				ir_sensor.hitWall = false;
				// print(ir_sensor.sensorName + " Score is: " + ir_sensor.collisionScore);
			} else if((ir_sensor.hitWall == true) && (ir_sensor.name.Contains("RIGHT"))) {
				//print("RIGHT IS HIT");
				numberCollidedIR = numberCollidedIR + 1;
				numberCollidedRightIR = numberCollidedRightIR + 1;

				ir_sensor.sensorName = ir_sensor.name;
				ir_sensor.collisionScore = ir_sensor.collisionScore +1;
				
				ir_sensor.hitWall = false;
				// print(ir_sensor.sensorName + " Score is: " + ir_sensor.collisionScore);
			} else if((ir_sensor.hitWall == true) && (ir_sensor.name.Contains("LEFT"))) {
				//print("LEFT IS HIT");
				numberCollidedIR = numberCollidedIR + 1;
				numberCollidedLeftIR = numberCollidedLeftIR + 1;

				ir_sensor.collisionScore = ir_sensor.collisionScore +1;
				ir_sensor.sensorName = ir_sensor.name;

				// print(ir_sensor.sensorName + " Score is: " + ir_sensor.collisionScore);
				ir_sensor.hitWall = false;
			} else if ((ir_sensor.hitWall != true) && (numberCollidedLeftIR > 0)) {
				numberCollidedLeftIR = numberCollidedLeftIR - 1;
			} else if ((ir_sensor.hitWall != true) && (numberCollidedRightIR > 0)) {
				numberCollidedRightIR = numberCollidedRightIR - 1;
			} else if ((ir_sensor.hitWall != true) && (numberCollidedFrontIR > 0)) {
				numberCollidedFrontIR = numberCollidedFrontIR - 1;
			} else if ((ir_sensor.hitWall != true) && (numberCollidedBackIR > 0)) {
				numberCollidedBackIR = numberCollidedBackIR - 1;
			}
		}
		//////////////////////COLLECT LDR DATA//////////////////////////////////////
		foreach(LDR ldr_sensor in ldr_sensors) {
			fitnessLDRscore = fitnessLDRscore + ldr_sensor.clacLightScore;

			if( (ldr_sensor.clacLightScore > 0) && (ldr_sensor.name.Contains("FRONT_LEFT")) ) {
				// frontLDRreadings = frontLDRreadings + 1;
				LEFTfrontLDRreadings = (LEFTfrontLDRreadings + ldr_sensor.clacLightScore) / 2;

			} else if( (ldr_sensor.clacLightScore > 0) && (ldr_sensor.name.Contains("FRONT_RIGHT")) ) {
				// frontLDRreadings = frontLDRreadings + 1;
				RIGHTfrontLDRreadings = (RIGHTfrontLDRreadings + ldr_sensor.clacLightScore) / 2;

			} else if ((ldr_sensor.clacLightScore > 0) && (ldr_sensor.name.Contains("BACK")) ) {
				// backLDRreadings = backLDRreadings + 1;
				backLDRreadings = (backLDRreadings + ldr_sensor.clacLightScore) / 2;

			} else if ((ldr_sensor.clacLightScore > 0) && (ldr_sensor.name.Contains("LEFT"))) {
				// leftLDRreadings = leftLDRreadings + 1;
				leftLDRreadings = (leftLDRreadings + ldr_sensor.clacLightScore) / 2;

			} else if ((ldr_sensor.clacLightScore > 0) && (ldr_sensor.name.Contains("RIGHT"))) {
			  	// rightLDRreadings = rightLDRreadings + 10;
			  	rightLDRreadings = (rightLDRreadings + ldr_sensor.clacLightScore) / 2;

			} else if ( (ldr_sensor.clacLightScore < 10) && (ldr_sensor.name.Contains("FRONT_LEFT")) && (LEFTfrontLDRreadings > 0) ) {
				LEFTfrontLDRreadings = LEFTfrontLDRreadings - 10;
			} else if ( (ldr_sensor.clacLightScore < 10) && (ldr_sensor.name.Contains("FRONT_RIGHT")) && (RIGHTfrontLDRreadings > 0) ) {
				RIGHTfrontLDRreadings = RIGHTfrontLDRreadings - 10;
			} else if ( (ldr_sensor.clacLightScore < 10) && (ldr_sensor.name.Contains("LEFT")) && (leftLDRreadings > 0) ) {
				leftLDRreadings = leftLDRreadings - 10;
			} else if ( (ldr_sensor.clacLightScore < 10) && (ldr_sensor.name.Contains("RIGHT")) && (rightLDRreadings > 0) ) {
				rightLDRreadings = rightLDRreadings - 10;
			} else if ( (ldr_sensor.clacLightScore < 10) && (ldr_sensor.name.Contains("BACK")) && (backLDRreadings > 0) ) {
				backLDRreadings = backLDRreadings - 10;
			}
		}

		irReadingArray = new int[4] {numberCollidedFrontIR, numberCollidedLeftIR, numberCollidedRightIR, 
						  numberCollidedBackIR};
		ldrReadingArray = new float[5] {LEFTfrontLDRreadings, RIGHTfrontLDRreadings, backLDRreadings, leftLDRreadings, rightLDRreadings};

		maxOfLDRArray = ldrReadingArray.Max();
 		maxLDRIndex = ldrReadingArray.ToList().IndexOf(maxOfLDRArray);
    }


    // FUNCTION: runMotors() 
    // This function takes two motor torque (left and right) values, checks the bumpers,
    // and drives the motors with the given values unless if the bump sensors are active.
   	public static void runMotors(float leftMotorTorque, float rightMotorTorque) {
   		// Grab wheels in order to drive them.
   		WheelCollider leftMotor = GameObject.Find("frontLeft").GetComponent<WheelCollider>();
		WheelCollider rightMotor = GameObject.Find("frontRight").GetComponent<WheelCollider>();
		float turnTime = 100f;

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
			backBumpType = "";
		}

		if ((backBumpType == "") && (frontBumpType == "")) {
			Debug.Log("MOVEMENT FROM NEURAL NETWORK");

			leftMotor.motorTorque = motorScale(leftMotorTorque);
			rightMotor.motorTorque = motorScale(rightMotorTorque);

		} else if (frontBumpType == "leftFront") {
			// Debug.Log("FORCE APPLIED FOR BRAKES");
			float time = 0f;
			Debug.Log(frontBumpType);
			// while (time < turnTime) {
				leftMotor.motorTorque = -118725;
				rightMotor.motorTorque = -119000;
				time = time + Time.deltaTime;
				Debug.Log("TURNING");
			// } 
		} else if (frontBumpType == "rightFront") {
			// Debug.Log("FORCE APPLIED FOR BRAKES");
			float time = 0f;
			// Debug.Log(frontBumpReading);
			// while (time < turnTime) {
				leftMotor.motorTorque =	-119000;
				rightMotor.motorTorque = -118725;
				time = time + Time.deltaTime;
				Debug.Log("TURNING");
			// }
		} else if (frontBumpType == "middleFront") {
			// Debug.Log("FORCE APPLIED FOR BRAKES");
			float time = 0f;
			// Debug.Log(frontBumpReading);
			// while (time < turnTime) {
				leftMotor.motorTorque = -117500;
				rightMotor.motorTorque = -117725;
				time = time + Time.deltaTime;
				Debug.Log("TURNING");
			// } 
		} else if (backBumpType == "middleBack") {
			// Debug.Log("FORCE APPLIED FOR BRAKES");
			float time = 0f;
			Debug.Log(backBumpType);
			while (time < turnTime) {
				leftMotor.motorTorque = -115000;
				rightMotor.motorTorque = -115000;
				time = time + Time.deltaTime;
				Debug.Log("TURNING");
			}
		} else if (backBumpType == "rightBack") {
			// Debug.Log("FORCE APPLIED FOR BRAKES");
			float time = 0f;
			// Debug.Log(backBumpReading);
			while (time < turnTime) {
				leftMotor.motorTorque = -114000;
				rightMotor.motorTorque = -113725;
				time = time + Time.deltaTime;
				Debug.Log("TURNING");
			}
		} else if (backBumpType == "leftBack") {
			// Debug.Log("FORCE APPLIED FOR BRAKES");
			float time = 0f;
			// Debug.Log(backBumpReading);
			while (time < turnTime) {
				leftMotor.motorTorque = -113725;
				rightMotor.motorTorque = -114000;
				time = time + Time.deltaTime;
				Debug.Log("TURNING");
			}
		}

		arrowMove();

    }



    void autoMovement(int frontIRreading, int leftIRreading, int rightIRreading, int backIRreading, int ldrIndex, float[] ldrArray, string frontBumpType, string backBumpType) {
    	// FIND WHEEL COLLIDERS AND LANDRO RIGID BODY.
    	print("AUTO MOVEMENT");
    	WheelCollider leftMotor = GameObject.Find("frontLeft").GetComponent<WheelCollider>();
		WheelCollider rightMotor = GameObject.Find("frontRight").GetComponent<WheelCollider>();
		rb = GameObject.Find("L16A").GetComponent<Rigidbody>();
		// CALCULATE TORQUE VALUES GIVEN INPUT.
		leftMovementValue = (leftIRreading * 500);
		rightMovemntValue = (rightIRreading * 500);
		frontMovementValue = ((frontIRreading * 1000)* 2);
		backMovementValue = ((backIRreading * 500));
		// ASSIGN TORQUE VALUES TO MOTORS.
		if( (frontIRreading > 0)||(leftIRreading > 0)||(rightIRreading > 0)||(backIRreading > 0) ) {
			// leftMotor.motorTorque = (baseMovementRate*2) + leftMovementValue - (rightMovemntValue/1) - frontMovementValue + backMovementValue;
			// rightMotor.motorTorque = (baseMovementRate*2) + rightMovemntValue - (leftMovementValue/1) - frontMovementValue + backMovementValue;
			leftMotor.motorTorque = -(leftMovementValue) - frontMovementValue -(rightMovemntValue/2) - 2000 + (backMovementValue *3);
			rightMotor.motorTorque = -(rightMovemntValue) - frontMovementValue -(leftMovementValue/2) - 2000 + (backMovementValue * 3);
			// print("IR MOVEMENT: leftM: "+ leftMotor.motorTorque + ", rightM: "+ rightMotor.motorTorque);
		} else if(ldrIndex == 0) {
			leftMotor.motorTorque = baseMovementRate*2;
			rightMotor.motorTorque = baseMovementRate*3;
			// print("LIGHT MOVEMENT: FORWARD LEFT");
		} else if(ldrIndex == 1) {
			leftMotor.motorTorque = baseMovementRate*3;
			rightMotor.motorTorque = -(baseMovementRate*2);
			// print("LIGHT MOVEMENT: FORWARD RIGHT");
		} else if(ldrIndex == 2) {
 			leftMotor.motorTorque = -(baseMovementRate*4);
			rightMotor.motorTorque = baseMovementRate*4;
			// print("LIGHT MOVEMENT: BACKWARDS");
		} else if(ldrIndex == 3) {
			leftMotor.motorTorque = 400;
			rightMotor.motorTorque = baseMovementRate;
			// print("LIGHT MOVEMENT: LEFT");
		} else if(ldrIndex == 4) {
			leftMotor.motorTorque = baseMovementRate;
			rightMotor.motorTorque = 400;
		}
		arrowMove();
    }
   
    
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

    // PRINTS INFORMATION ABOUT AN ARRAY.
    void showArrayInformation (Array arr, bool showORnah) {
    	if (showORnah) {
    		print("The length of array: " + arr.Length);
    	}
    }


    // Allows the user to control Landro using the arrow keys. Good for debugging and testing purposes.
    public static void arrowMove() {
    	WheelCollider leftMotor = GameObject.Find("frontLeft").GetComponent<WheelCollider>();
		WheelCollider rightMotor = GameObject.Find("frontRight").GetComponent<WheelCollider>();
    	 if (Input.GetKey(KeyCode.UpArrow)) {
		    leftMotor.motorTorque = 2000;
		    rightMotor.motorTorque = 2000;
		    print("UP");
		}
		if (Input.GetKey(KeyCode.DownArrow)){
		    leftMotor.motorTorque = -1000;
		    rightMotor.motorTorque = -1000;
		    print("DOWN");
		}
		if (Input.GetKey(KeyCode.RightArrow)){
		    leftMotor.motorTorque = 1000;
		    rightMotor.motorTorque = -1000;
		    print("RIGHT");
		}
		if (Input.GetKey(KeyCode.LeftArrow)){
		    leftMotor.motorTorque = -1000;
		    rightMotor.motorTorque = 1000;
		    print("LEFT");
    	}
	}

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