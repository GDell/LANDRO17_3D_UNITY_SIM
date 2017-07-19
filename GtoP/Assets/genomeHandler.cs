using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class genomeHandler : MonoBehaviour {

	public float numGenes = 10;
	public int maxSpawn = 100;

	public int vMax = 5;
	public int vDurationMin = 1;
	public int vDurationMax = 100;
	public int gMax = 3;
	public int gDurationMin = 1;
	public int gDurationMax = 100;


	System.Random rnd = new System.Random();

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
		}

		// Sets the beginning connection and node arrays, allocating the correct data sizes.
		public void setStartingArrays() {
			hidden = new float[NUM_HIDDEN]; 
			old_hidden = new float[NUM_HIDDEN];
			old_output = new float[NUM_OUTPUT];
			output = new float[NUM_OUTPUT];

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
		}

		public float activation(float val) {
	    	float update_value;
	    	update_value = (float)(Math.Tanh(val - 2) + 1)/2.0f;
	    	return update_value;
    	}



	}


	// Use this for initialization
	void Start () {

		float[,] TESTinput_to_output = new float[4,2] {{0,0},{0,0},{0,0},{0,0}};
		float[,] TESTinput_to_hidden = new float[4,6] {{0,0,1,-1,0,0},{0,0,-1,1,0,0},{1,-1,0,0,0,0},{-1,1,0,0,0,0}};
		float[,] TESThidden_to_hidden = new float[6,6] {{0,0,0,0,1,0},{0,0,0,0,1,0},{0,0,0,0,0,1},{0,0,0,0,0,1},{0,0,0,0,0,0},{0,0,0,0,0,0}};
		float[,] TESThidden_to_output = new float[6,2] {{0,0},{0,0},{0,0},{0,0},{1,-0.5f},{-0.5f,1}};
		float[,] TESToutput_to_hidden = new float[2,6] {{0,0,0,0,0,0},{0,0,0,0,0,0}};

		NeuralNetworkParameters testStruct = new NeuralNetworkParameters();
		// testStruct.RMILength = 1;
		// testStruct.LMILength = 1;
		// testStruct.NUM_INPUT = 4;
		// testStruct.NUM_HIDDEN = 6;
		// testStruct.NUM_OUTPUT = 2;

		// CALLS TO CONSTRUCTORS.
		testStruct.setStartVariables(1,1,4,6,2);

		testStruct.setStartingArrays();

		// Grab the correctly generated arrays
		

		testStruct.setConnections(TESTinput_to_output,TESTinput_to_hidden,TESThidden_to_hidden, TESThidden_to_output, TESToutput_to_hidden);

		Debug.Log(testStruct.hidden);

		// Debug.Log("IS THIS WORKING");
		
	}
	
	// Update is called once per frame
	void Update () {
		// makeGenome(8);
		// distance(100,100, 100,100);
		// Debug.Log("WE MADE it!");
		// print("WE MADE IT!");
	}




		// const int RMILength = 1; 
		// const int LMILength = 1; 
		// const int NUM_INPUT = 4; 
		// const int NUM_HIDDEN = 6; 
		// const int NUM_OUTPUT = 2;

		// float[] hidden = new float[NUM_HIDDEN]; 
		// float[] old_hidden = new float[NUM_HIDDEN]; 
		// float[] old_output = new float [NUM_OUTPUT];
		// float[] output = new float[NUM_OUTPUT];

		// int[] RMI = new int[RMILength] {0};
		// int[] LMI = new int[LMILength] {1};

  //   	float[,] input_to_output = new float[NUM_INPUT,NUM_OUTPUT] {{0,0},{0,0},{0,0},{0,0}};
		// float[,] input_to_hidden = new float[NUM_INPUT,NUM_HIDDEN] {{0,0,1,-1,0,0},{0,0,-1,1,0,0},{1,-1,0,0,0,0},{-1,1,0,0,0,0}};
		// float[,] hidden_to_hidden = new float[NUM_HIDDEN,NUM_HIDDEN] {{0,0,0,0,1,0},{0,0,0,0,1,0},{0,0,0,0,0,1},{0,0,0,0,0,1},{0,0,0,0,0,0},{0,0,0,0,0,0}};
		// float[,] hidden_to_output = new float[NUM_HIDDEN,NUM_OUTPUT] {{0,0},{0,0},{0,0},{0,0},{1,-0.5f},{-0.5f,1}};
		// float[,] output_to_hidden = new float[NUM_OUTPUT,NUM_HIDDEN] {{0,0,0,0,0,0},{0,0,0,0,0,0}};




	int[,] makeGenome(int nGenes) {

		int[,] genome = new int[nGenes, 8];
    	for (int x = 0; x < nGenes; x++) {
    		for (int y = 0; y < 8; y ++) {
    			genome[x,y] = 0;
    		};
    	};


    	for(int i = 0; i < nGenes; i ++) {
    		genome[i,0] = rnd.Next(0,4);
    		genome[i,1] = rnd.Next(0,360);
    		genome[i,2] = rnd.Next(0,maxSpawn);
    		genome[i,3] = rnd.Next(1,vMax);
    		genome[i,4] = rnd.Next(vDurationMin,vDurationMax);
    		genome[i,5] = rnd.Next(1,gMax);
    		genome[i,6] = rnd.Next(gDurationMin,gDurationMax);
    		genome[i,7] = i;

    	}
    	// Debug.Log("THIs IS IT: " +genome);
    	return genome;
	}

	float distance(float x1,float x2,float y1,float y2) {
		float calc =  (float)(Math.Sqrt((Math.Pow((x1-x2),2f)) + (Math.Pow((y1-y2),2f))));
		Debug.Log(calc);
		return calc;
	}


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


	void makeGenOne() {
		int[][] allGenomes = new int[][] {};

		int minVal = 0;
		int maxVal = 20;
		int sd = 2;

		for (int x = 1; x < 6; x++) {
			numGenes = NormalizeRandom(minVal, maxVal);
		}


	}

	float NormalizeRandom(int minVal, int maxVal) {
		int mean = (minVal  + maxVal) /2;
		int sigma = (maxVal = mean) / 3;
		return rnd.Next(mean, sigma);
	}
}
