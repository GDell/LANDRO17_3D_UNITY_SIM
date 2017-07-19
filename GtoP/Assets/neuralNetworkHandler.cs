using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class neuralNetworkHandler {

	// STRUCTURE FOR BUILDING AND RUNNING A NEURAL NET. \\
	public struct NeuralNetworkParameters {
		public int RMILength; 
		public int LMILength; 
		public int NUM_INPUT; 
		public int NUM_HIDDEN; 
		public int NUM_OUTPUT;

		public float[] hidden; 
		public float[] old_hidden;
		public float[] old_output;
		public float[] output;

		public float[] motorValsLR;
		public float[] scaledMotorValsLR;

		public float rmSpeed;
		public float nonScaledRMspeed;

 		public float lmSpeed;
 		public float nonScaledLMspeed;

		int[] RMI;
		int[] LMI;

		float[,] input_to_output;
		float[,] input_to_hidden;
		float[,] hidden_to_hidden;
		float[,] hidden_to_output;
		float[,] output_to_hidden;

		// Sets the starting input, hidden, and outut variables.
		public void setStartVariables(int rmi, int lmi, int numIn, int numHid, int numOut) {
			RMILength = rmi;
			LMILength = lmi;
			NUM_INPUT = numIn;
			NUM_HIDDEN = numHid;
			NUM_OUTPUT = numOut;


			RMI = new int[RMILength];
			LMI = new int[LMILength];

			rmSpeed = 0;
			nonScaledRMspeed = 0;

 			lmSpeed = 0;
 			nonScaledLMspeed = 0;

			motorValsLR = new float[2];
			scaledMotorValsLR = new float[2];

		}

		// Sets the beginning connection and node arrays, allocating the correct data sizes.
		public void setStartingArrays(int[] rmiVal, int[] lmiVal) {
			hidden = new float[NUM_HIDDEN]; 
			old_hidden = new float[NUM_HIDDEN];
			old_output = new float[NUM_OUTPUT];
			output = new float[NUM_OUTPUT];


			RMI = rmiVal;
			LMI = lmiVal;


			input_to_output = new float[NUM_INPUT, NUM_OUTPUT];
			input_to_hidden = new float[NUM_INPUT, NUM_HIDDEN];
			hidden_to_hidden = new float[NUM_HIDDEN, NUM_HIDDEN];
			hidden_to_output = new float[NUM_HIDDEN, NUM_OUTPUT];
			output_to_hidden = new float[NUM_OUTPUT,NUM_HIDDEN];
		}

		// Sets the connection arrays to the appropriate weight arrays generated in the G-P params
		// generator.
		public void setConnections(float[,] inToOut, float[,] inToHid, float[,] hidToHId, float[,] hidToOut, float[,] outToHid) {
			input_to_output = inToOut;
			input_to_hidden = inToHid;
			hidden_to_hidden = hidToHId;
			hidden_to_output = hidToOut;
			output_to_hidden = outToHid;
		}


		public void beginNeuralNet(float[] ldrSensorArray, float[] irSensorArray, int[] selectedSensors) {
			int selectedArraySize = selectedSensors.Length;
	    	int h, p, o, i;
	    	float[] input = new float[selectedArraySize];

	    	bool debugNeuralNetwork = false;

	    	// MAPPING INPUT FROM SELECTED ACTIVE SENSOR VALUES. \\
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
			// UPDATES OUTPUT (MOTOR) NODES BASED ON INPUT NODES. \\
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
		}
		
		// GRAB AND UPDATE MOTOR OUTPUT VALUES. //
		// Updates both nonScaled and scaled motor value arrays.
    	public void updateMotorValues() {
			for(int i = 0; i < RMILength; i++){
				nonScaledRMspeed += output[RMI[i]];
				rmSpeed = motorScale(nonScaledRMspeed);
				// print("RM SPEED: "+ motorScale(rmSpeed));
				// rightMotor.motorTorque = rmSpeed;
				scaledMotorValsLR[1] = rmSpeed;
				motorValsLR[1] = nonScaledRMspeed;
			}
			// CALCULATE LM SPEED FROM OUTPUT.
			for(int i = 0; i < LMILength; i++){
				nonScaledLMspeed += output[LMI[i]];
				lmSpeed = motorScale(nonScaledLMspeed);
				// print("LM SPEED: "+ motorScale(lmSpeed));
			 	// leftMotor.motorTorque = lmSpeed;
				scaledMotorValsLR[0] = lmSpeed;
				motorValsLR[0] = nonScaledLMspeed;
			}
    	}

    	// HELPER FUNCTIONS. \\
    	// Tanh activation function.
		public float activation(float val) {
	    	float update_value;
	    	update_value = (float)(Math.Tanh(val - 2) + 1)/2.0f;
	    	return update_value;
    	}
    	// Motor scaling function.
		public float motorScale(float val) {
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


	}


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
