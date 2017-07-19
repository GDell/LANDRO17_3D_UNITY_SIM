using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public class NeuralNetwork {
	public static bool debugNeuralNetwork;


	// 4, 6, 2
	
	// Use this for initialization
	void Start () {
		debugNeuralNetwork = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}



	public static void neuralNetwork(float[] ldrSensorArray, float[] irSensorArray, int[] selectedSensors, string frontBumpType, string backBumpType, int numInput, int numHidden, int numOutput) {
    	WheelCollider leftMotor = GameObject.Find("frontLeft").GetComponent<WheelCollider>();
		WheelCollider rightMotor = GameObject.Find("frontRight").GetComponent<WheelCollider>();
		// timeNext = timeSinceLevelLoad + 20;

			float turnTime = 10000f;

			 // 4, 6, 2

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

			///////////

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
	  		        Debug.Log("Input: " + (input[i].ToString()));
	  				Debug.Log("Input to Hidden: " + (input_to_hidden[i,h].ToString()));
		  		}
			}
			//Update hidden nodes using hidden (last) values) from time t-1.
			for (p = 0; p < NUM_HIDDEN; p++) {
		  		hidden[h] = hidden[h] + old_hidden[p] * hidden_to_hidden[p,h];
		  		if (debugNeuralNetwork) {
	    			Debug.Log("Old Hidden: " + (old_hidden[p].ToString()));
	 				Debug.Log("Hidden to Hidden: " + (hidden_to_hidden[p,h].ToString()));
		  		}
			}
			//Update hidden based on old motor vals.
			for(o = 0; o < NUM_OUTPUT; o++){
		  		hidden[h] = hidden[h] + old_output[o] * output_to_hidden[o,h];
			}
			if (debugNeuralNetwork) {
				Debug.Log("Hidden: " + (hidden[h].ToString()));
			}
			// Apply the tanh function.
			hidden[h] = activation(hidden[h]);
			// Update old_hidden with new tanh values.
			old_hidden[h] = hidden[h];
			if (debugNeuralNetwork) {
				Debug.Log("Tanh Hidden: " + (hidden[h].ToString()));
			}
		}
		// UPDATES OUTPUT (MOTOR) NODES BASED ON INPUT NODES.
		for (o = 0; o < NUM_OUTPUT; o++) {
			output[o] = 0;
			for (i = 0; i < NUM_INPUT; i++) {
				output[o] = output[o] + input[i] * input_to_output[i,o];
				if (debugNeuralNetwork) {
					Debug.Log("Input: " + (input[i].ToString()));
					Debug.Log("Connect: " + (input_to_output[i,o].ToString()));
				}
			}
			for (h = 0; h < NUM_HIDDEN; h++) {
		  		output[o] = output[o] + hidden[h] * hidden_to_output[h,o];
			}
			if (debugNeuralNetwork) {
				Debug.Log("Output: " + (output[o].ToString()));
			}
			output[o] = activation(output[o]);
			if (debugNeuralNetwork) {
				Debug.Log("Output Tanh: " + (output[o].ToString()));
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

    static float activation(float val) {
    	float update_value;
    	update_value = (float)(Math.Tanh(val - 2) + 1)/2.0f;
    	return update_value;
    }
    // SCALES MOTOR VALUES.
	static float motorScale(float val) {
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
	static float irScale(float val) {
		float fromLow = 0;
		float fromHigh = 409;
		float toLow = 0;
		float toHigh = 1;
		float mapVal = (((val - fromLow) * (toHigh - toLow)) / (fromHigh - fromLow)) + toLow;
		return (mapVal);
	}
	// SCALES LDR VALUES GIVEN MIN AND MAX POSSIBLE IR VALUES.
	static float photoScale(float val) {
		float fromLow = 0;
		float fromHigh = 1550f;
		float toLow = 0;
		float toHigh = 1;
		float mapVal = (((val - fromLow) * (toHigh - toLow)) / (fromHigh - fromLow)) + toLow;
		return (mapVal);
	}

	    // Allows the user to control Landro using the arrow keys. Good for debugging and testing purposes.
    public static void arrowMove() {
    	WheelCollider leftMotor = GameObject.Find("frontLeft").GetComponent<WheelCollider>();
		WheelCollider rightMotor = GameObject.Find("frontRight").GetComponent<WheelCollider>();
    	 if (Input.GetKey(KeyCode.UpArrow)) {
		    leftMotor.motorTorque = 2000;
		    rightMotor.motorTorque = 2000;
		    Debug.Log("UP");
		}
		if (Input.GetKey(KeyCode.DownArrow)){
		    leftMotor.motorTorque = -1000;
		    rightMotor.motorTorque = -1000;
		    Debug.Log("DOWN");
		}
		if (Input.GetKey(KeyCode.RightArrow)){
		    leftMotor.motorTorque = 1000;
		    rightMotor.motorTorque = -1000;
		    Debug.Log("RIGHT");
		}
		if (Input.GetKey(KeyCode.LeftArrow)){
		    leftMotor.motorTorque = -1000;
		    rightMotor.motorTorque = 1000;
		    Debug.Log("LEFT");
    	}
	}

	void makeGenome(int nGenes) {
    	int maxSpawn = 100;
    	int vMax = 5;
    	int vDurationMin = 1;
    	int vDurationMax = 100;
    	int gMax = 3;
    	int gDurationMin = 1;
    	int gDurationMax = 100;
  		// # 0 = Part Type (0 = IR, 1 = Photo, 2 = Neuron, 3 = R Motor, 4 = L Motor)
		// # 1 = Angle
		// # 2 = Start Time
		// # 3 = Velocity
		// # 4 = Travel Time
		// # 5 = Growth Rate
		// # 6 = Growth Time
		// # 7 = Index

    	// Creates an empty genome of nGenes filled with 0s.
    	int[,] genome = new int[nGenes, 8];
    	for (int x = 0; x < nGenes; x++) {
    		for (int y = 0; y < 8; y ++) {
    			genome[x,y] = 0;
    		};
    	};
    }


}
