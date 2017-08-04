using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour {
	// Grab the main script so that we can declare it as 
	// "dont destroy on load".
	public GameObject mainScript;

	// TEST STRUCTURES \\
   	// Genome
	public static genomeHandler.genome testGenome = new genomeHandler.genome();
	// G-->P
	public static genomeHandler.genomeToPhenotype testGtoP = new genomeHandler.genomeToPhenotype();
	// Parameters 
	public static genomeHandler.createParams testParams = new genomeHandler.createParams();
	// Neural Network
	public static NeuralNetworkHandler.NeuralNetworkParameters testNeuralStruct = new NeuralNetworkHandler.NeuralNetworkParameters();
	// Generation
	public genomeHandler.generation testGeneration = new genomeHandler.generation();

	// Bools to keep track of what point the program is at during 
	// generation and trial initialization.
	public static bool pastStartScreen = false;
	public static bool firstGenerationRun = true;
	public static bool firstRun = true;

	// G-->P variables.
	public static int maxSpawn = 100;
	public static int vMax = 5;
	public static int vDurationMin = 1;
	public static int vDurationMax = 100;
	public static int gMax = 3;
	public static int gDurationMin = 1;
	public static int gDurationMax = 100;

	// Genome variables.
	public static int numberOfGenes = 20;
	public static float dupeRate = 0.5f;
	public static float muteRate = 0.05f;
	public static float delRate = 0.01f;
	public static float changePercent = 0.15f;

	// Timing variables to keep track of overall 
	// trial length and the current time in a trial.
	public float trialTime;
    public float timeCurrent;

	// Threshold values for the fitness function. 
    // Currently a rough 50:50 ratio for good/bad XOR ratio in simulated arena.
	public float IRthreshold = 366f;
	public float LDRthreshold = 2426.46f;

	// Variables for calculating fitness.
	public float meanIRscore;
   	public float meanLDRscore;

   	// Keeps track of the which individual in a generation we are loading.
    public int currentIndex;
   	// public int numberOfindividuals;

   	public bool simulationRunning;

    public int overallFitnessScore;
    public int hIRlLDRfitnessScore;
    public int lIRhLDRfitnessScore;
    public int totalIterations;

    public float[] rawldrDataArray;
    public float[] ldrDataArray;

    public float[] rawirDataArray;
    public float[] irDataArray;

	void Start () {
		simulationRunning = true;

		mainScript = GameObject.Find("MainScript");
		DontDestroyOnLoad(mainScript);

		timeCurrent = 0;
		currentIndex = 0;

		overallFitnessScore = 0;
		totalIterations = 0; 

		hIRlLDRfitnessScore = 0;
        lIRhLDRfitnessScore = 0; 

		rawldrDataArray = new float[8];
		rawirDataArray = new float[8];
		irDataArray = new float[8];
		ldrDataArray = new float[8];	
	}
	
	void Update () {	

		timeCurrent = Time.timeSinceLevelLoad;	

		Debug.Log("RUNNING GENERATION: " + testGeneration.generationIndex);
		Debug.Log("RUNNING INDIVIDUAL: " + currentIndex);

		rawirDataArray = SimpleCarController.returnRawIRdata();
		rawldrDataArray = SimpleCarController.returnRawLDRdata();

		ldrDataArray = SimpleCarController.returnLDRdata();
		irDataArray = SimpleCarController.returnIRdata();


		if (simulationRunning) {

		// IF we have passed the start screen...
			if (pastStartScreen) {
				// If this is the first run after passing the start screen...
				if (firstRun) {

					trialTime = MainMenu.INPUTtrialLength;
					Debug.Log("THIS IS THE TRIAL TIME: " + trialTime);
					// Create a generation.
					testGeneration.setGenerationParameters(MainMenu.INPUTavgGenomeSize, MainMenu.INPUTnumberOfGenerations, MainMenu.INPUTnumberOfIndividuals); 
					testGeneration.createStartGeneration();
				}
				if (!(firstGenerationRun)) {

					if (timeCurrent <  trialTime) {	

						evaluateTrialFitness(rawldrDataArray, rawirDataArray, testGeneration.collectionOfIndividuals[currentIndex].paramsCollection.chosenSensorArray);
						beginRun(ldrDataArray, irDataArray);

					} else if (timeCurrent >= trialTime) {
						SimpleCarController.stopMovement();
						Debug.Log("DONE RUNNING INDIVIDUAL: "+ currentIndex);
						testGeneration.collectionOfIndividuals[currentIndex].fitnessScore = finalFitnessCalculation();
						int numberOfindividuals = testGeneration.numberOfIndividualsInGeneration;

						if (currentIndex < numberOfindividuals) {
							// testGeneration.collectionOfIndividuals[currentIndex].fitnessScore = finalFitnessCalculation();
							SimpleCarController.stopMovement();
							reset();
						} else if (currentIndex >= numberOfindividuals) {

				   			if(testGeneration.generationIndex < testGeneration.numberOfGenerations) {
				   				Debug.Log("Starting next generation");
					   			testGeneration.createNewGeneration();
					   			currentIndex = 0;
					   			SceneManager.LoadScene("Experiment");
				   			} else if (testGeneration.generationIndex >= testGeneration.numberOfGenerations) {
					   			Debug.Log("END OF SIMULATION REACHED, NUMBER OF GENERATIONS EVALUATED: " + testGeneration.numberOfGenerations);
					   			simulationRunning = false;
				   			}   	
						}  				
					}
					
				} else {
					totalIterations = 0;
				}
				firstGenerationRun = false;
			}

		} else {
			Debug.Log("Simulation has finished!");
		}

	}

	// Checks to see whether or not we are in an XOR space at the current time step in the trial.
	void evaluateTrialFitness(float[] ldrSensorArray, float[] irSensorArray, int[] selectedSensors) {
    	meanLDRscore = 0;
    	meanIRscore = 0;
    	int selectedArraySize = selectedSensors.Length;
    	bool exceededIRthreshold = false;
    	bool exceededLDRthreshold = false;
    	// Count total number of iterations.
    	totalIterations = totalIterations + 1;
    	// Find means of LDR and IR sensor values but only for those that are currently active/chosen.
    	for (int runs = 0; runs < selectedArraySize; runs++){
    		// Debug.Log("GETTING INTO THE EVALUATE TRIAL FITNESS FUNCTION");
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
    	if (meanIRscore > 0) {
    		exceededIRthreshold = true;
    	}
    	if (meanLDRscore > 0) {
    		exceededLDRthreshold = true;
    	}
    	// Check if Landro is in an XOR location.
    	if(exceededLDRthreshold && exceededIRthreshold) {
    		print("NOT FIT");
    	} else if ((!exceededIRthreshold) && (!exceededLDRthreshold)) {
    		print("NOT FIT");
    	} else if(exceededIRthreshold && (!exceededLDRthreshold)) {
    		hIRlLDRfitnessScore = hIRlLDRfitnessScore + 1;
    		print("FIT");
    		// print("hIRlLDRfitnessScore: " + hIRlLDRfitnessScore);
    	} else if(exceededLDRthreshold && (!exceededIRthreshold)) {
    		lIRhLDRfitnessScore = lIRhLDRfitnessScore + 1;
    		print("FIT");
    		// print("lIRhLDRfitnessScore: " + lIRhLDRfitnessScore);
    	}
    }

    // This function begins running the individual at the current index given the LDR and IR
    // data arrays obtained from the SimpleCarControllerScipt.
    void beginRun(float[] ldrData, float[] irData) {

    	float[] currentLDRdata;
		float[] currentIRdata;
		float[] rawLDRdata;
		float[] rawIRdata;

		// If it's the first run we set the time to zero and begin 
		// neural network drive.s
		if (firstRun) {
    		timeCurrent = 0;
    		firstRun = false;
    	}

		if (timeCurrent <  trialTime) {	
			// evaluateTrialFitness(rawLDRdata, rawIRdata, testGeneration.collectionOfIndividuals[currentIndex].paramsCollection.chosenSensorArray);
			testGeneration.runNeuralNetOnIndividual(ldrData, irData);
		}

    }

    // FUNCTION: finalFitnessCalculation() 
   	//	This function determines an individual's final fitness, taking into account its' fitness scores throughout the trial.
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

    // FUNCTION: reset()
	//	This function resets the trial experiment with the current individual in the generation being tested.
	//  If the entire generation has been run, this function will create a new generation of the offspring from the previous
	//	that are mutated and deleted. The program will then start running trials for this new experiment.
    public void reset() {
    	// Reset fitness and timing variables.
    	hIRlLDRfitnessScore = 0;
        lIRhLDRfitnessScore = 0; 
    	totalIterations = 0;
    	overallFitnessScore = 0;
    	timeCurrent = 0;
    	meanLDRscore = 0;
    	meanIRscore = 0;
    	// Load the next individual.
		testGeneration.nextIndividual();
		currentIndex = currentIndex + 1;
		// Reload the scene.
		SceneManager.LoadScene("Experiment");
    }

	// FUNCTION: singleIndividualTest()
	// 		This function runs a single test on the whole genome to phenotype process:
	//		genome creation --> G->P development process --> Params creation --> Neural Network Creation
	public static void singleIndivualTest() {
		// CREATING A GENOME:
		testGenome.createRandomFunction();
		testGenome.setGenomeParameters(2, dupeRate, muteRate, delRate, changePercent);
		testGenome.createWholeGenome(maxSpawn, vMax, vDurationMin, vDurationMax, gMax, gDurationMin, gDurationMax);
		testGenome.printGenomeContents();

		// RUNNING THE G-->P PROCESS:
		testGtoP.passGenome(testGenome);
		testGtoP.runDevoGraphics();
		testGtoP.makeConnectome();

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
		// testParams.printParamsContents();

		// CREATING THE NEURAL NETWORK.
		testNeuralStruct.setStartVariables(testParams.RMIlength,testParams.LMIlength,testParams.NUM_INPUT,testParams.NUM_HIDDEN,testParams.NUM_OUTPUT);
		testNeuralStruct.setStartingArrays(testParams.finalRMI, testParams.finalLMI);
		testNeuralStruct.setConnections(testParams.input_to_output,testParams.input_to_hidden,testParams.hidden_to_hidden, testParams.hidden_to_output, testParams.output_to_hidden);
	}


}
