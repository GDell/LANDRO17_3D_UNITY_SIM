using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class genomeHandler : MonoBehaviour {

	System.Random rnd = new System.Random();

	// GENOME GENERATOR INITIALIZATION.
	public float numGenes = 10;
	public int maxSpawn = 100;
	public int vMax = 5;
	public int vDurationMin = 1;
	public int vDurationMax = 100;
	public int gMax = 3;
	public int gDurationMin = 1;
	public int gDurationMax = 100;


	// Use this for initialization
	void Start () {

		float[,] TESTinput_to_output = new float[4,2] {{0,0},{0,0},{0,0},{0,0}};
		float[,] TESTinput_to_hidden = new float[4,6] {{0,0,1,-1,0,0},{0,0,-1,1,0,0},{1,-1,0,0,0,0},{-1,1,0,0,0,0}};
		float[,] TESThidden_to_hidden = new float[6,6] {{0,0,0,0,1,0},{0,0,0,0,1,0},{0,0,0,0,0,1},{0,0,0,0,0,1},{0,0,0,0,0,0},{0,0,0,0,0,0}};
		float[,] TESThidden_to_output = new float[6,2] {{0,0},{0,0},{0,0},{0,0},{1,-0.5f},{-0.5f,1}};
		float[,] TESToutput_to_hidden = new float[2,6] {{0,0,0,0,0,0},{0,0,0,0,0,0}};

		int[] rmiVal = new int[1] {0};
		int[] lmiVal = new int[1] {1};

		// DECLARE INSTANCE OF NEURAL NETWORK.
		neuralNetworkHandler.NeuralNetworkParameters testStruct = new neuralNetworkHandler.NeuralNetworkParameters();
		// CALLS TO CONSTRUCTORS.
		// (RMI, LMI, NUM_INPUT, NUM_HIDDEN, NUM_OUTPUT).
		testStruct.setStartVariables(1,1,4,6,2);
		testStruct.setStartingArrays(rmiVal, lmiVal);
		// Grab the correctly generated arrays
		testStruct.setConnections(TESTinput_to_output,TESTinput_to_hidden,TESThidden_to_hidden, TESThidden_to_output, TESToutput_to_hidden);


		
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
