using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour {
	public GameObject mainScript;

	/////  TEST STRUCTURES
   	// Genome
	public genomeHandler.genome testGenome = new genomeHandler.genome();
	// G-->P
	public genomeHandler.genomeToPhenotype testGtoP = new genomeHandler.genomeToPhenotype();
	// Parameters 
	public genomeHandler.createParams testParams = new genomeHandler.createParams();
	// Neural Network
	public NeuralNetworkHandler.NeuralNetworkParameters testNeuralStruct = new NeuralNetworkHandler.NeuralNetworkParameters();
	// Generation
	public genomeHandler.generation testGeneration = new genomeHandler.generation();


	public static bool pastStartScreen = false;
	public static bool firstGenerationRun = true;
	public static bool firstRun = true;

	public int maxSpawn = 100;
	public int vMax = 5;
	public int vDurationMin = 1;
	public int vDurationMax = 100;
	public int gMax = 3;
	public int gDurationMin = 1;
	public int gDurationMax = 100;

	public int numberOfGenes = 20;
	public float dupeRate = 0.5f;
	public float muteRate = 0.05f;
	public float delRate = 0.01f;
	public float changePercent = 0.15f;


	public float trialTime;
    public float timeCurrent;

	// Threshold values for the fitness function. 
    // Currently 25:75 ratio for good/bad XOR ratio in simulated arena.
	public float IRthreshold = 200f;
	public float LDRthreshold = 500f;

	public float meanIRscore;
   	public float meanLDRscore;


    public int currentIndex;
   	// public int numberOfindividuals;

   	public bool simulationRunning;

    public int overallFitnessScore;
    public int hIRlLDRfitnessScore;
    public int lIRhLDRfitnessScore;
    public int totalIterations;



    public int currentGeneration;

	void Start () {

		simulationRunning = true;

		mainScript = GameObject.Find("MainScript");
		DontDestroyOnLoad(mainScript);

		timeCurrent = 0;
		currentIndex = 0;

		currentGeneration = 0;

		overallFitnessScore = 0;
		totalIterations = 0; 

		hIRlLDRfitnessScore = 0;
        lIRhLDRfitnessScore = 0; 

		// Test for creating a generation.

		// // CREATING A GENOME:
		// testGenome.createRandomFunction();
		// testGenome.setGenomeParameters(2, dupeRate, muteRate, delRate, changePercent);
		// testGenome.createWholeGenome(maxSpawn, vMax, vDurationMin, vDurationMax, gMax, gDurationMin, gDurationMax);
		// testGenome.printGenomeContents();

		// // RUNNING THE G-->P PROCESS:
		// testGtoP.passGenome(testGenome);
		// testGtoP.runDevoGraphics();
		// testGtoP.makeConnectome();
		// // testGtoP.printConnectomeContents();

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

	}
	
	void Update () {	
		
		timeCurrent = Time.timeSinceLevelLoad;	

		Debug.Log("RUNNING INDIVIDUAL: " + currentIndex);
		Debug.Log("RUNNING GENERATION: " + testGeneration.generationIndex);

		if (simulationRunning) {
			Debug.Log("WHAT THE HELL");

		// IF we have passed the start screen...
			if (pastStartScreen) {
				// If this is the first run after passing the start screen...
				if (firstRun) {
					trialTime = MainMenu.INPUTtrialLength;
					Debug.Log("THIS IS THE TRIAL TIME: " + trialTime);
					// Create a generation.
					// (int numberOfG, int numGenerations, int numberOfInd)
					testGeneration.setGenerationParameters(MainMenu.INPUTavgGenomeSize, MainMenu.INPUTnumberOfGenerations, MainMenu.INPUTnumberOfIndividuals); 
					testGeneration.createStartGeneration();
				}
				if (!(firstGenerationRun)) {

					if (timeCurrent <  trialTime) {	
						beginRun();
					} else if (timeCurrent >= trialTime) {
						SimpleCarController.stopMovement();
						Debug.Log("DONE RUNNING INDIVIDUAL: "+ currentIndex);
						testGeneration.collectionOfIndividuals[currentIndex].fitnessScore = finalFitnessCalculation();
						// testGeneration.nextIndividual();


						int numberOfindividuals = testGeneration.numberOfIndividualsInGeneration;
						if (currentIndex < numberOfindividuals) {
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
    		print("FIT");
    		// print("hIRlLDRfitnessScore: " + hIRlLDRfitnessScore);
    	} else if(exceededLDRthreshold && (!exceededIRthreshold)) {
    		lIRhLDRfitnessScore = lIRhLDRfitnessScore + 1;
    		print("FIT");
    		// print("lIRhLDRfitnessScore: " + lIRhLDRfitnessScore);
    	}
    }

    // This function begins running the individual at the current index.
    void beginRun() {

    	float[] currentLDRdata;
		float[] currentIRdata;

		float[] rawLDRdata;
		float[] rawIRdata;

		currentIRdata = SimpleCarController.returnIRdata();
		currentLDRdata = SimpleCarController.returnLDRdata();

		rawLDRdata = SimpleCarController.returnRawLDRdata();
		rawIRdata = SimpleCarController.returnRawIRdata();

		if (firstRun) {
    		timeCurrent = 0;
    		firstRun = false;
    	}

		if (timeCurrent <  trialTime) {	

			testGeneration.runNeuralNetOnIndividual(currentLDRdata, currentIRdata);
			evaluateTrialFitness(rawLDRdata, rawIRdata, testGeneration.collectionOfIndividuals[currentIndex].paramsCollection.chosenSensorArray);

		// Else if we have reached the end of a trial, stop Landro, iterate to the next individual, and reload the experiment.
		} else if (timeCurrent >= trialTime){
			testGeneration.collectionOfIndividuals[currentIndex].fitnessScore = finalFitnessCalculation();
			SimpleCarController.stopMovement();
			// testGeneration.nextIndividual();
			reset();
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
    	hIRlLDRfitnessScore = 0;
        lIRhLDRfitnessScore = 0; 
    	totalIterations = 0;
    	overallFitnessScore = 0;
    	timeCurrent = 0;
    	// currentIndex = testGeneration.individualIndex;
    	int numberOfindividuals = testGeneration.numberOfIndividualsInGeneration;

		testGeneration.nextIndividual();
		currentIndex = currentIndex + 1;
		SceneManager.LoadScene("Experiment");
   		// } else if (currentIndex >= numberOfindividuals) {
   		// 	if(testGeneration.generationIndex < testGeneration.numberOfGenerations) {
   		// 		Debug.Log("Starting next generation");
	   	// 		testGeneration.createNewGeneration();
	   	// 		currentIndex = 0;
	   	// 		SceneManager.LoadScene("Experiment");

   		// 	} else if (testGeneration.generationIndex >= testGeneration.numberOfGenerations) {

	   	// 		Debug.Log("END OF SIMULATION REACHED, NUMBER OF GENERATIONS EVALUATED: " + testGeneration.numberOfGenerations);
	   	// 		simulationDone = true;
   		// 	}
   		// }
    }


}
