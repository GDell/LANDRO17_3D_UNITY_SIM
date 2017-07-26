using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class SimpleCarController : MonoBehaviour {    

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
    	// True if you want to use network, false for default IR/Light behavior.
	public bool useNetwork;
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
    // public NeuralNetwork neuralNet;

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


	// GENOME STRUCT - G->P STRUCT - PARAMS STRUCT
	public genomeHandler.genome testGenome = new genomeHandler.genome();
	public genomeHandler.genomeToPhenotype testGtoP = new genomeHandler.genomeToPhenotype();
	public genomeHandler.createParams testParams = new genomeHandler.createParams();
	public NeuralNetworkHandler.NeuralNetworkParameters testNeuralStruct = new NeuralNetworkHandler.NeuralNetworkParameters();


    // INITIALIZE SIMULATION.
    public void Start(){

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

		// CREATING A GENOME:
		testGenome.createRandomFunction();
		testGenome.setGenomeParameters(numberOfGenes, dupeRate, muteRate, delRate, changePercent);
		testGenome.createWholeGenome(maxSpawn, vMax, vDurationMin, vDurationMax, gMax, gDurationMin, gDurationMax);
		testGenome.printGenomeContents();

		// RUNNING THE G-->P PROCESS:
		testGtoP.passGenome(testGenome);
		testGtoP.runDevoGraphics();
		testGtoP.makeConnectome();
		testGtoP.printConnectomeContents();

		// CREATING NEURAL NETWORK PARAMETERS
		testParams.passConnectionMatrix(testGtoP.sortedConnects, testGenome);
		testParams.setNodeLayerNumbers();
		testParams.motorIndexes();
		testParams.sensorToInputs();
		testParams.createInputToHidden();
		testParams.createHiddenToHidden();
		testParams.createHiddenToOutput();
		testParams.createInputToOutput();
		testParams.createOutputToHidden();
		testParams.finalToArray();
		testParams.printParamsContents();

		// CREATING THE NEURAL NETWORK.
		testNeuralStruct.setStartVariables(1,1,testParams.NUM_INPUT,testParams.NUM_HIDDEN,testParams.NUM_OUTPUT);
		int[] rmiVal = new int[1] {0};
		int[] lmiVal = new int[1] {1};
		testNeuralStruct.setStartingArrays(rmiVal, lmiVal);
		testNeuralStruct.setConnections(testParams.input_to_output,testParams.input_to_hidden,testParams.hidden_to_hidden, testParams.hidden_to_output, testParams.output_to_hidden);

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
		useNetwork = true;
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
		// neuralNet = GameObject.GetComponent<NeuralNetwork>();
		// neuralNet = GetComponent<NeuralNetwork>();
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

		// // Selected sensors for input into neural network function.
		// chosenSensorArray = new int[4] {2,3,13,14}; 

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
		/////////////////////////////////////////////////////////////////////////

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

		// DISPLAY FITNESS VALUES.
		if (displayFitnessInfo){
			print("IR collection score: "+numberCollidedIR);
			print("LDR collection score: "+fitnessLDRscore);
		}

		irReadingArray = new int[4] {numberCollidedFrontIR, numberCollidedLeftIR, numberCollidedRightIR, 
						  numberCollidedBackIR};
		ldrReadingArray = new float[5] {LEFTfrontLDRreadings, RIGHTfrontLDRreadings, backLDRreadings, leftLDRreadings, rightLDRreadings};

		maxOfLDRArray = ldrReadingArray.Max();
 		maxLDRIndex = ldrReadingArray.ToList().IndexOf(maxOfLDRArray);

 		if (timeCurrent >= (timeSet)) {	
			finalFitnessCalculation();
		}		

 		// NEURALNETWORK && MOVEMENT CONTROL.
 		if(useNetwork) {
			if (timeCurrent <  timeSet) {	
				evaluateTrialFitness(rawldrDataArray, rawirDataArray, chosenSensorArray);
 				// RUN THE NEURAL NETWORK: powers motors based on neural network calculation using provided params.h.
 				// test.neuralNetwork(ldrDataArray, irDataArray, chosenSensorArray, frontBumpType, backBumpType, 4, 6, 2);
				testNeuralStruct.beginNeuralNet(ldrDataArray, irDataArray, testParams.chosenSensorArray);
				
				testNeuralStruct.updateMotorValues();

 			} else if (timeCurrent >= timeSet){
 				stopMovement();
 				// reset();
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