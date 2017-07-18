using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

// AXLE CLASS: creates accessible Wheel classes in order to control movement. 
[System.Serializable]
public class AxleInfo {
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
}

public class SimpleCarController : MonoBehaviour {
    public List<AxleInfo> axleInfos; 

	GameObject wheelColliders;
    List<WheelCollider> wheels = new List<WheelCollider>();
    public Rigidbody rb;

    // Keeping track of number of LDR and IR sensors.
    public int LDRnumber = 0;
	public int IRnumber = 0;

	// Motor torque and steering parameters.
    public float maxMotorTorque;
    public float maxSteeringAngle;

    // BOOLEANS: Toggle to change further function behaviors.
    	// True if you want to print IR and LDR data.
    public bool displayFitnessInfo;
    	// True if you want to use exampleXORnetwork, otherwise uses others.
    public bool exampleXORnetwork;
    	// True if you want to use network, false for default IR/Light behavior.
	public bool useNetwork;
		// True if you want neural network debug info.
	public bool debugNeuralNetwork;
		// True if you want bumper debug info.
	public bool debugBumper;

	public int baseMovementRate;
	public string frontBumpType;
	public string backBumpType;

	public float timeTrial;

    public float timeNext;
    public float timeSet;
    public float timeCurrent;
    public Light morningstar;

    // SENSOR ARRAYS
    private IR[] ir_sensors;
    private LDR[] ldr_sensors;
    private BumpSensor[] bump_sensors;
    private BumpSensorBack[] backBump_sensors;

	public int[] irReadingArray;
	public float[] ldrReadingArray;
	public float maxOfLDRArray;
	public int maxLDRIndex;

	public float IRthreshold;
	public float LDRthreshold;

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

    public float[] rawirDataArray;
    public float[] irDataArray;
    public float[] irCollisionDataArray;
    public int irSensorNumber = 0;
    public int[] chosenSensorArray;
    public int[][] listOfChosenSensorArrays;

    // LDR DATA COLLECTION VARIABLES
    public float LEFTfrontLDRreadings;
    public float RIGHTfrontLDRreadings;
    public float rightLDRreadings; 
    public float leftLDRreadings;
    public float backLDRreadings;

    public float[] rawldrDataArray;
    public float[] ldrDataArray;
    public int ldrSensorNumber = 0;

	public float fitnessLDRscore;
	public float fitnessIRscore;

    public float rightWheelTorque;
    public float leftWheelTorque;

    public int firstRun;
    public int overallFitnessScore;
    public int hIRlLDRfitnessScore;
    public int lIRhLDRfitnessScore;
    public int totalIterations;

   	public float meanIRscore;
   	public float meanLDRscore;

    // INITIALIZE SIMULATION.
    public void Start(){
    	wheelColliders = GameObject.Find("WheelColliders");

		// VECTORS FOR PLACING SENSORS.///////////////
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
		useNetwork = true;
			// T: use the example neural network.
		exampleXORnetwork = true;
			// T: enable print statements in neural network function.
		debugNeuralNetwork = false;
			// T: enable print statements for bumper functions.
		debugBumper = false;

		firstRun = 0;

		// TIMING VARIABLES.
		// Time set is the length of a trial (in seconds).
		timeTrial = 60f;
		timeSet  = timeTrial;
		timeCurrent = 0;

		// Threshold values for the fitness function. 
        // Currently 25:75 ratio for good/bad XOR ratio in simulated arena.
		IRthreshold = 200f;
		LDRthreshold = 500f;

    	//morningstar = GameObject.Find("Directional Light").GetComponent<Light>();
    	wheels.Add(wheelColliders.transform.Find("frontRight").GetComponent<WheelCollider>());
		wheels.Add(wheelColliders.transform.Find("frontLeft").GetComponent<WheelCollider>());
		WheelCollider leftMotor = GameObject.Find("frontLeft").GetComponent<WheelCollider>();
		WheelCollider rightMotor = GameObject.Find("frontRight").GetComponent<WheelCollider>();

		// GRAB THE THREE SENSOR TYPES.
		ir_sensors = GameObject.FindObjectsOfType<IR>();
		ldr_sensors = GameObject.FindObjectsOfType<LDR>();
		bump_sensors = GameObject.FindObjectsOfType<BumpSensor>();
		backBump_sensors = GameObject.FindObjectsOfType<BumpSensorBack>();

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

		overallFitnessScore = 0;
		totalIterations = 0; 

		hIRlLDRfitnessScore = 0;
        lIRhLDRfitnessScore = 0; 


		leftMotor.motorTorque = rightWheelTorque;
		rightMotor.motorTorque = leftWheelTorque;

		// Selected sensors for input into neural network function.
		chosenSensorArray = new int[4] {2,3,13,14}; 

		//////////////////////////// PLACING SENSORS ON LANDRO BODY //////////////////////////// 
		// PLACE FRONT BUMP SENSORS.
		int i = 0;
		foreach(BumpSensor bump_sensor in bump_sensors){
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
		foreach(BumpSensorBack backBump_sensor in backBump_sensors){
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
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////// 

		// ALLOCATE SPACE IN SENSOR DATA ARRAYS.
		irCollisionDataArray = new float[irSensorNumber];
		rawldrDataArray = new float[ldrSensorNumber];
		rawirDataArray = new float[irSensorNumber];
		irDataArray = new float[irSensorNumber];
		ldrDataArray = new float[ldrSensorNumber];
    }
 
    public void FixedUpdate() {	
    	timeCurrent = Time.timeSinceLevelLoad;
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
		// POLLING FRONT BUMP SENSORS:
		// 	Checks to see i
		foreach(BumpSensor bump_sensor in bump_sensors) {
			if(bump_sensor.bumpWall == true) {
				print(bump_sensor + " done DID hit the WALL!");
				bump_sensor.bumpWall = false;
				if (bump_sensor.name.Contains("leftFront")) {
					frontBumpType = "leftFront";
					print("FRONT HAS BEEN HIT!!");
				}
				if (bump_sensor.name.Contains("rightFront")) {
					frontBumpType = "rightFront";
					print("RIGHT HAS BEEN HIT!!");
				}
				if (bump_sensor.name.Contains("middleFront")) {
					frontBumpType = "middleFront";
					print("MIDDLE HAS BEEN HIT!!");
				}
			}
		}
		// POLLING BACK FRONT BUMP SENSORS.
		foreach(BumpSensorBack bump_sensor in backBump_sensors) {
			if(bump_sensor.bumpWall == true) {
				print(bump_sensor + " done DID hit the WALL!");
				bump_sensor.bumpWall = false;
				if (bump_sensor.name.Contains("leftBack")) {
					backBumpType = "leftBack";
				}
				if (bump_sensor.name.Contains("rightBack")) {
					backBumpType = "rightBack";
				}
				if (bump_sensor.name.Contains("middleBack")) {
					backBumpType = "middleBack";
				} 
			}
		}

		print("Front Bump Type: "+frontBumpType);
		print("Back Bump Type: "+backBumpType);
		
		// DISPLAY FITNESS VALUES.
		if (displayFitnessInfo){
			print("IR collection score: "+numberCollidedIR);
			print("LDR collection score: "+fitnessLDRscore);
		}
		irReadingArray = new int[4] {numberCollidedFrontIR, numberCollidedLeftIR, numberCollidedRightIR, 
						  numberCollidedBackIR};
		ldrReadingArray = new float[5] {LEFTfrontLDRreadings, RIGHTfrontLDRreadings, backLDRreadings, leftLDRreadings, rightLDRreadings};

		maxOfLDRArray = ldrReadingArray.Max();
		// print("Max of array is: "+ maxOfLDRArray);
 		maxLDRIndex = ldrReadingArray.ToList().IndexOf(maxOfLDRArray);
 		// print("Max index is: " + maxLDRIndex);

 		if (timeCurrent >= (timeSet)) {	
			finalFitnessCalculation();
		}		

 		// NEURALNETWORK && MOVEMENT CONTROL.
 		if(useNetwork) {
			if (timeCurrent <  timeSet) {	
				evaluateTrialFitness(rawldrDataArray, rawirDataArray, chosenSensorArray);
 				// RUN THE NEURAL NETWORK: powers motors based on neural network calculation using provided params.h.
 				neuralNetwork(ldrDataArray, irDataArray, chosenSensorArray, frontBumpType, backBumpType, timeCurrent);
 				// timeSet = (timeSet - timeCurrent);
 				// print("TIME SET: " + timeCurrent);
 			} else if (timeCurrent >= timeSet){
 				stopMovement();
 				reset();
 				// Application.LoadLevel(0);
 			}
 		} else {
 			// RUN MOVEMENT FUNCTION: powers motors based user input and fixed IR/Light calculations.
			autoMovement(numberCollidedFrontIR, numberCollidedLeftIR, numberCollidedRightIR, numberCollidedBackIR, maxLDRIndex, ldrReadingArray, frontBumpType, backBumpType);
 		}
 		// backBumpType = "";
 		frontBumpType = "";
    }
    // PRINTS INFORMATION ABOUT AN ARRAY.
    void showArrayInformation (Array arr, bool showORnah) {
    	if (showORnah) {
    		print("The length of array: " + arr.Length);
    	}
    }
    /* This function implements the activation function for updating
	* network nodes(specifically hidden nodes).  There are two
	* slightly different formulations, one for recurrent connections
	* and one for non-recurrent connections. */
    float activation(float val) {
    	float update_value;
    	update_value = (float)(Math.Tanh(val - 2) + 1)/2.0f;
    	return update_value;
    }
    // SCALES MOTOR VALUES.
	float motorScale(float val) {
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
	// SCALES IR VALUES GIVEN MIN AND MAX POSSIBLE IR VALUES.
	float irScale(float val) {
		float fromLow = 0;
		float fromHigh = 409;
		float toLow = 0;
		float toHigh = 1;
		float mapVal = (((val - fromLow) * (toHigh - toLow)) / (fromHigh - fromLow)) + toLow;
		return (mapVal);
	}
	// SCALES LDR VALUES GIVEN MIN AND MAX POSSIBLE IR VALUES.
	float photoScale(float val) {
		float fromLow = 0;
		float fromHigh = 1550f;
		float toLow = 0;
		float toHigh = 1;
		float mapVal = (((val - fromLow) * (toHigh - toLow)) / (fromHigh - fromLow)) + toLow;
		return (mapVal);
	}
    // RUNS AN ITERATION OF THE NEURAL NETWORK.
    void neuralNetwork(float[] ldrSensorArray, float[] irSensorArray, int[] selectedSensors, string frontBumpType, string backBumpType, float timeCurrent) {
    	WheelCollider leftMotor = GameObject.Find("frontLeft").GetComponent<WheelCollider>();
		WheelCollider rightMotor = GameObject.Find("frontRight").GetComponent<WheelCollider>();
		// timeNext = timeSinceLevelLoad + 20;

			float turnTime = 10000f;

			const int RMILength = 1; 
			const int LMILength = 1; 
			const int NUM_INPUT = 4; 
			const int NUM_HIDDEN = 6; 
			const int NUM_OUTPUT = 2;

			float[] hidden = new float[NUM_HIDDEN]; 
			float[] old_hidden = new float[NUM_HIDDEN]; 
			float[] old_output = new float [NUM_OUTPUT];
			float[] output = new float[NUM_OUTPUT];

			int[] RMI = new int[RMILength] {0};
			int[] LMI = new int[LMILength] {1};

	    	float[,] input_to_output = new float[NUM_INPUT,NUM_OUTPUT] {{0,0},{0,0},{0,0},{0,0}};
			float[,] input_to_hidden = new float[NUM_INPUT,NUM_HIDDEN] {{0,0,1,-1,0,0},{0,0,-1,1,0,0},{1,-1,0,0,0,0},{-1,1,0,0,0,0}};
			float[,] hidden_to_hidden = new float[NUM_HIDDEN,NUM_HIDDEN] {{0,0,0,0,1,0},{0,0,0,0,1,0},{0,0,0,0,0,1},{0,0,0,0,0,1},{0,0,0,0,0,0},{0,0,0,0,0,0}};
			float[,] hidden_to_output = new float[NUM_HIDDEN,NUM_OUTPUT] {{0,0},{0,0},{0,0},{0,0},{1,-0.5f},{-0.5f,1}};
			float[,] output_to_hidden = new float[NUM_OUTPUT,NUM_HIDDEN] {{0,0,0,0,0,0},{0,0,0,0,0,0}};

			// const int RMILength = 1; 
			// const int LMILength = 1; 

			// const int NUM_INPUT = 2; 
			// const int NUM_HIDDEN = 1; 
			// const int NUM_OUTPUT = 2;

			// float[] hidden = new float[NUM_HIDDEN]; 
			// float[] old_hidden = new float[NUM_HIDDEN]; 
			// float[] old_output = new float [NUM_OUTPUT];
			// float[] output = new float[NUM_OUTPUT];

			// int[] RMI = new int[RMILength] {0};
			// int[] LMI = new int[LMILength] {1};

			// float[,] input_to_hidden = new float[NUM_INPUT,NUM_HIDDEN] {{0},{0}};
			// float[,] hidden_to_hidden = new float[NUM_HIDDEN,NUM_HIDDEN] {{0}};
			// float[,] hidden_to_output = new float[NUM_HIDDEN,NUM_OUTPUT] {{0,0}};
			// float[,] input_to_output = new float[NUM_INPUT,NUM_OUTPUT] {{1,-0.5f},{-0.5f,1}}; 
			// float[,] output_to_hidden = new float[NUM_OUTPUT,NUM_HIDDEN] {{0},{0}};


		int selectedArraySize = selectedSensors.Length;
    	int h, p, o, i;
    	float[] input = new float[selectedArraySize];

		float rmSpeed = 0;
		float nonScaledRMspeed = 0;

 		float lmSpeed = 0;
 		float nonScaledLMspeed = 0;

    	// MAPPING INPUT FROM SELECTED ACTIVE SENSOR VALUES.
    	// Updates input nodes from sensor values
		for (int runs = 0; runs < selectedArraySize; runs++){
			if(selectedSensors[runs] == 0) {
				input[runs] = irSensorArray[0];
    		} else if(selectedSensors[runs] == 1) {
    			input[runs] = ldrSensorArray[2];
    		} else if(selectedSensors[runs] == 2) {
    			input[runs] = irSensorArray[1];
    		} else if(selectedSensors[runs] == 3) {
    			input[runs] = ldrSensorArray[3];
    		} else if(selectedSensors[runs] == 4) {
    			input[runs] = irSensorArray[2];
    		} else if(selectedSensors[runs] == 5) {
    			input[runs] = ldrSensorArray[4];
    		} else if(selectedSensors[runs] == 6) {
    			input[runs] = irSensorArray[3];
    		} else if(selectedSensors[runs] == 7) {
    			input[runs] = ldrSensorArray[5];
    		} else if(selectedSensors[runs] == 8) {
    			input[runs] = irSensorArray[4];
    		} else if(selectedSensors[runs] == 9) {
    			input[runs] = ldrSensorArray[6];
    		} else if(selectedSensors[runs] == 10) {
    			input[runs] = irSensorArray[5];
    		} else if(selectedSensors[runs] == 11) {
    			input[runs] = ldrSensorArray[7];
    		} else if(selectedSensors[runs] == 12) {
    			input[runs] = irSensorArray[6];
    		} else if(selectedSensors[runs] == 13) {
    			input[runs] = ldrSensorArray[0];
    		} else if(selectedSensors[runs] == 14) {
    			input[runs] = irSensorArray[7];
    		} else if(selectedSensors[runs] == 15) {
    			input[runs] = ldrSensorArray[1];
    		}
    	}
		for (h = 0; h < NUM_HIDDEN; h++) {
			hidden[h] = 0;
			// Update hidden nodes using inputs for time t.
			for (i = 0; i < NUM_INPUT; i++) {
		  		hidden[h] = hidden[h] + input[i] * input_to_hidden[i,h];
		  		if (debugNeuralNetwork) {
	  		        print("Input: " + (input[i].ToString()));
	  				print("Input to Hidden: " + (input_to_hidden[i,h].ToString()));
		  		}
			}
			//Update hidden nodes using hidden (last) values) from time t-1.
			for (p = 0; p < NUM_HIDDEN; p++) {
		  		hidden[h] = hidden[h] + old_hidden[p] * hidden_to_hidden[p,h];
		  		if (debugNeuralNetwork) {
	    			print("Old Hidden: " + (old_hidden[p].ToString()));
	 				print("Hidden to Hidden: " + (hidden_to_hidden[p,h].ToString()));
		  		}
			}
			//Update hidden based on old motor vals.
			for(o = 0; o < NUM_OUTPUT; o++){
		  		hidden[h] = hidden[h] + old_output[o] * output_to_hidden[o,h];
			}
			if (debugNeuralNetwork) {
				print("Hidden: " + (hidden[h].ToString()));
			}
			// Apply the tanh function.
			hidden[h] = activation(hidden[h]);
			// Update old_hidden with new tanh values.
			old_hidden[h] = hidden[h];
			if (debugNeuralNetwork) {
				print("Tanh Hidden: " + (hidden[h].ToString()));
			}
		}
		// UPDATES OUTPUT (MOTOR) NODES BASED ON INPUT NODES.
		for (o = 0; o < NUM_OUTPUT; o++) {
			output[o] = 0;
			for (i = 0; i < NUM_INPUT; i++) {
				output[o] = output[o] + input[i] * input_to_output[i,o];
				if (debugNeuralNetwork) {
					print("Input: " + (input[i].ToString()));
					print("Connect: " + (input_to_output[i,o].ToString()));
				}
			}
			for (h = 0; h < NUM_HIDDEN; h++) {
		  		output[o] = output[o] + hidden[h] * hidden_to_output[h,o];
			}
			if (debugNeuralNetwork) {
				print("Output: " + (output[o].ToString()));
			}
			output[o] = activation(output[o]);
			if (debugNeuralNetwork) {
				print("Output Tanh: " + (output[o].ToString()));
			}
			old_output[o] = output[o];
		}
		// MOVING LANDRO: as long as bumpers are not pressed, neural network output drives the motors.
		// Else there are fixed responses contingent on the exact bumper that collided with a wall.
		if ((backBumpType == "") && (frontBumpType == "")) {
			// CALCULATE RM SPEED FROM OUTPUT.
			for(i = 0; i < RMILength; i++){
				nonScaledRMspeed = output[RMI[i]];
				rmSpeed += motorScale(nonScaledRMspeed);
				// print("RM SPEED: "+ motorScale(rmSpeed));
				rightMotor.motorTorque = rmSpeed;
			}
			// CALCULATE LM SPEED FROM OUTPUT.
			for(i = 0; i < LMILength; i++){
				nonScaledLMspeed = output[LMI[i]];
				lmSpeed += motorScale(nonScaledLMspeed);
				// print("LM SPEED: "+ motorScale(lmSpeed));
			 	leftMotor.motorTorque = lmSpeed;
			}
		} else if (frontBumpType == "leftFront") {
			float time = 0f;
			while (time < turnTime) {
				leftMotor.motorTorque = 5725;
				rightMotor.motorTorque = -6000;
				time = time + Time.deltaTime;
			}
		} else if (frontBumpType == "rightFront") {
			float time = 0f;
			while (time < turnTime) {
				leftMotor.motorTorque =	-6000;
				rightMotor.motorTorque = 5725;
				time = time + Time.deltaTime;
			}
		} else if (frontBumpType == "middleFront") {
			float time = 0f;
			while (time < turnTime) {
				leftMotor.motorTorque = 4500;
				rightMotor.motorTorque = -4725;
				time = time + Time.deltaTime;
			} 
		} else if (backBumpType == "middleBack") {
			float time = 0f;
			while (time < turnTime) {
				leftMotor.motorTorque = 2000;
				rightMotor.motorTorque = 2000;
				time = time + Time.deltaTime;
			}
			backBumpType = "";
		} else if (backBumpType == "rightBack") {
			float time = 0f;
			while (time < turnTime) {
				leftMotor.motorTorque = -1000;
				rightMotor.motorTorque = 725;
				time = time + Time.deltaTime;
			}
			backBumpType = "";
		} else if (backBumpType == "leftBack") {
			float time = 0f;
			while (time < turnTime) {
				leftMotor.motorTorque = 725;
				rightMotor.motorTorque = -1000;
				time = time + Time.deltaTime;
			}
			backBumpType = "";
		}
		arrowMove();
    }
    void evaluateTrialFitness(float[] ldrSensorArray, float[] irSensorArray, int[] selectedSensors) {
    	int selectedArraySize = selectedSensors.Length;
    	bool exceededIRthreshold = false;
    	bool exceededLDRthreshold = false;
    	// Count total number of iterations.
    	totalIterations = totalIterations + 1;
    	// Find means of LDR and IR sensor values but only for those that are currently active/chosen.
    	for (int runs = 0; runs < selectedArraySize; runs++){
			if(selectedSensors[runs] == 0) {
				meanIRscore = (meanIRscore + irSensorArray[0])/2;
    		} else if(selectedSensors[runs] == 1) {
    			meanLDRscore = (meanLDRscore + ldrSensorArray[2])/2;
    		} else if(selectedSensors[runs] == 2) {
    			meanIRscore = (meanIRscore + irSensorArray[1])/2;
    		} else if(selectedSensors[runs] == 3) {
    			meanLDRscore = (meanLDRscore + ldrSensorArray[3])/2;
    		} else if(selectedSensors[runs] == 4) {
    			meanIRscore = (meanIRscore + irSensorArray[2])/2;
    		} else if(selectedSensors[runs] == 5) {
    			meanLDRscore = (meanLDRscore + ldrSensorArray[4])/2;
    		} else if(selectedSensors[runs] == 6) {
    			meanIRscore = (meanIRscore + irSensorArray[3])/2;
    		} else if(selectedSensors[runs] == 7) {
    			meanLDRscore = (meanLDRscore + ldrSensorArray[5])/2;
    		} else if(selectedSensors[runs] == 8) {
    			meanIRscore = (meanIRscore + irSensorArray[4])/2;
    		} else if(selectedSensors[runs] == 9) {
    			meanLDRscore = (meanLDRscore + ldrSensorArray[6])/2;
    		} else if(selectedSensors[runs] == 10) {
    			meanIRscore = (meanIRscore + irSensorArray[5])/2;
    		} else if(selectedSensors[runs] == 11) {
    			meanLDRscore = (meanLDRscore + ldrSensorArray[7])/2;
    		} else if(selectedSensors[runs] == 12) {
    			meanIRscore = (meanIRscore + irSensorArray[6])/2;
    		} else if(selectedSensors[runs] == 13) {
    			meanLDRscore = (meanLDRscore + ldrSensorArray[0])/2;
    		} else if(selectedSensors[runs] == 14) {
    			meanIRscore = (meanIRscore + irSensorArray[7])/2;
    		} else if(selectedSensors[runs] == 15) {
    			meanLDRscore = (meanLDRscore + ldrSensorArray[1])/2;
    		}
    	}
    	// Check if the thresholds have been reached for both IR and LDR.
    	if (meanIRscore >= IRthreshold) {
    		exceededIRthreshold = true;
    	}
    	if (meanLDRscore >= LDRthreshold) {
    		exceededLDRthreshold = true;
    	}
    	// Check if Landro is in an XOR location.
    	if(exceededLDRthreshold && exceededIRthreshold) {
    		print("NOT FIT");
    	} else if ((!exceededIRthreshold) && (!exceededLDRthreshold)) {
    		print("NOT FIT");
    	} else if(exceededIRthreshold && (!exceededLDRthreshold)) {
    		hIRlLDRfitnessScore = hIRlLDRfitnessScore + 1;
    		// print("hIRlLDRfitnessScore: " + hIRlLDRfitnessScore);
    	} else if(exceededLDRthreshold && (!exceededIRthreshold)) {
    		lIRhLDRfitnessScore = lIRhLDRfitnessScore + 1;
    		// print("lIRhLDRfitnessScore: " + lIRhLDRfitnessScore);
    	}
    }
    float finalFitnessCalculation() {
    	overallFitnessScore = (((hIRlLDRfitnessScore/totalIterations)*100) + ((lIRhLDRfitnessScore/totalIterations)*100) + 
    			((((hIRlLDRfitnessScore/totalIterations)*100) + 
    				((lIRhLDRfitnessScore/totalIterations)*100))/10));
    	// print("Total iterations!!!!!: " + totalIterations);
    	// print("hIRlLDRfitnessScore!!!!!!!!: "+ hIRlLDRfitnessScore);
    	// print("lIRhLDRfitnessScore!!!!!!!!: "+ lIRhLDRfitnessScore);
    	// print("INTERMEDIATE: "+ ((hIRlLDRfitnessScore/totalIterations)*100));
    	print("THE FINAL FITNESS: " + overallFitnessScore);
    	return overallFitnessScore;
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
    // Use to record the data collected from a specific trial.
    void recordTrial() {
    	// THRESHOLDS AND FITNESS FUNCTION
    }
    // Use to reset timing variables between trial runs.
    void reset() {
    	hIRlLDRfitnessScore = 0;
        lIRhLDRfitnessScore = 0; 
    	totalIterations = 0;
    	overallFitnessScore = 0;
    	timeCurrent = 0;
    	timeSet = timeTrial;
    	SceneManager.LoadScene("Experiment");
    }
    // Use to stop all movement of Landro.
    void stopMovement() {
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
    // Allows the user to control Landro using the arrow keys. Good for debugging and testing purposes.
    void arrowMove() {
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

}