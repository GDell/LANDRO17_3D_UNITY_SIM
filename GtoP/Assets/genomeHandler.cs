using System.Collections;
using System.Collections.Generic;
using System;
// using System.Math;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
// using microsoft.xna.framework.dll;

public class genomeHandler : MonoBehaviour {

	System.Random rand = new System.Random();


	////// STRUCT: gene
	// 	This structure represents a single gene.
	// 	Functions:
	// 		setGeneParameters() : creates a gene and sets its parameters.
	//			INPUT: int maxSpawn,  int vMax, int vDurationMin, int vDurationMax, int gMax, int gDurationMin, int gDurationMax, int index
	//			OUTPUT: a gene with --> (partType, angle, startTime, velocity, traveltime, growthRate, growthTime, index).
	public struct gene {
		

		public double x;
		public double y;
		public float size;

		public float partType;// # 0 = Part Type (0 = IR, 1 = Photo, 2 = Neuron, 3 = R Motor, 4 = L Motor)
		public float angle;// # 1 = Angle
		public float startTime;// # 2 = Start Time
		public float velocity;// # 3 = Velocity
		public float travelTime;// # 4 = Travel Time
		public float growthRate;// # 5 = Growth Rate
		public float growthTime;// # 6 = Growth Time
		public float index;// # 7 = Index


		public void setGeneParameters(int pt,  int ang, int sT, int vel, int tT, int gR, int gT, int geneNumber) {
			// Debug.Log("GENES ARE BEING SET");
			partType = pt;
			angle = ang;
			startTime = sT;
			velocity = vel;
			travelTime = tT;
			growthRate = gR;
			growthTime = gT;
			index = geneNumber;
		}	
	}

	////// STRUCT: genome
	// 	This structure represents a genome, or a set of genes as an array[] of genes.
	// 	Functions:
	// 		setGenomeParameters() : specifies number of genes in the genome.
	//		createWholeGenome() : creates all the genes in the genome and sets their initial variables.
	// 		duplicateAndDelete() : Duplicates and deletes genes in the genome.
	// 		mutate() : Mutates genes in the genome.
	public struct genome {
		// System.Random rand;

		public gene[] arrayOfGenes;
		public gene[] newMutatedGenome;

		public int numberOfGenes;
		public float dupeRate;
		public float muteRate;
		public float delRate;
		public float changePercent;
		public bool firstDupe;

		public int numberOfDuplications;


		System.Random rand;
		public void createRandomFunction() {
			rand = new System.Random();
		}

		// Function to set the genome parameters.
		public void setGenomeParameters(int numberOfGenesInGenome, float dupR, float muteR, float delR, float changeP) {
			numberOfGenes = numberOfGenesInGenome;
			arrayOfGenes = new gene[numberOfGenesInGenome];
			dupeRate = dupR;
			muteRate = muteR;
			delRate = delR;
			changePercent = changeP;	
			numberOfDuplications = 0;
			firstDupe = true;
			// Debug.Log("PARAMETERS SET");
		}

		// Creates the specified genome after setting parameters.
		public void createWholeGenome(int maxSpawn, int vMax, int vDurationMin, int vDurationMax, int gMax, int gDurationMin, int gDurationMax) {
			Debug.Log("Number of genes: " +numberOfGenes);
			for (int j = 0; j < numberOfGenes; j++) {
				int partType = rand.Next(0,4);
				int angle = rand.Next(0, 360);
				int startTime =  rand.Next(0, maxSpawn);
				int velocity = rand.Next(1, vMax);
				int travelTime = rand.Next(vDurationMin, vDurationMax);
				int growthRate = rand.Next(1, gMax);
				int growthTime = rand.Next(gDurationMin, gDurationMax);
				// maxSpawn,  int vMax, int vDurationMin, int vDurationMax, int gMax, int gDurationMin, int gDurationMax, int geneNumber
				arrayOfGenes[j].setGeneParameters(partType, angle, startTime, velocity, travelTime, growthRate, growthTime, j);
			}
			// Debug.Log("GENOME CREATED");
		}

		// Duplicates and deletes the different genes in a genome.
		public void duplicateAndDelete() {
			int numDeleted = 0;
			// Int i for indexing which gene is doubled.
			List<gene> geneDuplicatedAndNotDeleted = new List<gene>();
			int countOfnonDeleted = 0;
			foreach (var item in arrayOfGenes) {
				if (rand.NextDouble() > delRate) {
					geneDuplicatedAndNotDeleted.Add(item);
					countOfnonDeleted = countOfnonDeleted + 1;
				} else {
					numDeleted = numDeleted + 1;
					Debug.Log("THERE HAS BEEN A DELETION, num: " + numDeleted + ", index: " + item.index);
				} 
			};

			for (int k = 0; k < countOfnonDeleted; k++) {
				if (rand.NextDouble() <= dupeRate) {
					numberOfDuplications = numberOfDuplications + 1;
					geneDuplicatedAndNotDeleted.Add(geneDuplicatedAndNotDeleted[k]);
					Debug.Log("THERE HAS BEEN A DUPLICATION, num dups: " + numberOfDuplications);
					if (firstDupe) {
						dupeRate = 0.5f;
						firstDupe = false;
					} else {
						dupeRate = dupeRate/2;
					}
				} else {
					dupeRate = 0.05f;
					firstDupe = true;
				}
			};
				

			// foreach (var item in arrayOfGenes) {

			// 	// i = i +1;
			// }

			
			int listCount =  geneDuplicatedAndNotDeleted.Count();
			numberOfGenes = listCount;
			Debug.Log("SIZE OF LIST: "+listCount);
			// arrayOfGenes = geneDuplicatedAndNotDeleted.ToArray();
			arrayOfGenes = new gene[listCount];
			arrayOfGenes = geneDuplicatedAndNotDeleted.ToArray();
			int lengthOfNewArray = arrayOfGenes.Length;
			for (int j = 0; j < lengthOfNewArray; j++) {
				arrayOfGenes[j].index = j;	
			}

		}

		// Mutates the genes in the genome.
		public void mutate() {
			int lengthOfArray = arrayOfGenes.Length;
			Debug.Log("MUTE RATE: "+ muteRate);
			for (int j = 0; j < lengthOfArray; j++) {
				for (int i = 0; i < 7; i++) {
					if (rand.NextDouble() <= muteRate) {
						Debug.Log("THERE HAS BEEN A MUTATION");
						if (i == 0) {
							arrayOfGenes[j].partType = rand.Next(0,4);
							// Debug.Log("THERE HAS BEEN A MUTATION");
						} else if (rand.NextDouble() > 0.5) {
							if (i == 1) {
								arrayOfGenes[j].angle = arrayOfGenes[j].angle + arrayOfGenes[j].angle*changePercent;
								// Debug.Log("THERE HAS BEEN A MUTATION");
							} else if (i == 2) {
								arrayOfGenes[j].startTime = arrayOfGenes[j].startTime  + arrayOfGenes[j].startTime *changePercent;
								// Debug.Log("THERE HAS BEEN A MUTATION");
							} else if (i == 3) {
								arrayOfGenes[j].velocity = arrayOfGenes[j].velocity + arrayOfGenes[j].velocity*changePercent;
								// Debug.Log("THERE HAS BEEN A MUTATION");
							} else if (i == 4) {
								arrayOfGenes[j].travelTime = arrayOfGenes[j].travelTime + arrayOfGenes[j].travelTime*changePercent;
								// Debug.Log("THERE HAS BEEN A MUTATION");
							} else if (i == 5) {
								arrayOfGenes[j].growthRate = arrayOfGenes[j].growthRate + arrayOfGenes[j].growthRate*changePercent;
								// Debug.Log("THERE HAS BEEN A MUTATION");
							} else if (i == 6) {
								arrayOfGenes[j].growthTime = arrayOfGenes[j].growthTime + arrayOfGenes[j].growthTime*changePercent;
								// Debug.Log("THERE HAS BEEN A MUTATION");
							} else if (i == 7) {
								Debug.Log(arrayOfGenes[j].index);
							}
						}
					}
				}
			}
		}

		public void printGenomeContents() {
			int lengthOfArray = numberOfGenes;
			string printString = "";
			for (int j = 0; j < lengthOfArray; j++) {
				printString += "|| ID NUMBER: " + arrayOfGenes[j].index;
				printString += ", Part type: "+arrayOfGenes[j].partType;
				printString += ", Angle: "+arrayOfGenes[j].angle;
				printString += ", Start time: "+arrayOfGenes[j].startTime;
				printString += ", Velocity: "+arrayOfGenes[j].velocity;
				printString += ", Travel time: "+arrayOfGenes[j].travelTime;
				printString += ", Growth rate: "+arrayOfGenes[j].growthRate;
				printString += ", Growth time: "+arrayOfGenes[j].growthTime;

				// Debug.Log("PART TYPE: "+arrayOfGenes[j].partType);// 1
				// Debug.Log("ANGLE: "+arrayOfGenes[j].angle);// 2
				// Debug.Log("START TIME: "+arrayOfGenes[j].startTime);// 3
				// Debug.Log("VELOCITY: "+arrayOfGenes[j].velocity);// 4
				// Debug.Log("TRAVEL TIME: "+arrayOfGenes[j].travelTime);// 5
				// Debug.Log("GROWTH RATE: "+arrayOfGenes[j].growthRate);// 6
				// Debug.Log("GROWTH TIME: "+arrayOfGenes[j].growthTime);// 7
				// Debug.Log("INDEX: "+arrayOfGenes[j].index);// 8
			}
			Debug.Log(printString);
		}

	}

	////// STRUCT: generation
	// 	This structure represents a single generation, or a collection of genomes (individuals).
	// 	Functions:
	// 		setGenerationParameters() : specifies number of genes.
	// 			INPUT: (int meanNumberOfGenes, int meanStandardDeviationOfGenes, int numberOfIndividualsInGeneration)
	//			OUTPUT: assigns the generation's parameters.
	//		createStartGeneration() : creates a generation given the parameters set above.
	// 		createNumberOfGenes() : creates the number of genes for a given genome in the generation.
	public struct generation {
		System.Random rand;
		public int numberOfIndividualsInGeneration;
		genome[] startGenerationGenomeCollection;
		int numberOfGenes;
		int meanNumberOfGenes;
		int meanStandardDeviationOfGenes;

		public void setGenerationParameters(int meanNumberOfGenes, int meanStandardDeviationOfGenes, int numberOfIndividualsInGeneration) {
			rand = new System.Random(); 
			meanNumberOfGenes = meanNumberOfGenes;
			meanStandardDeviationOfGenes = meanStandardDeviationOfGenes;
			numberOfIndividualsInGeneration = numberOfIndividualsInGeneration;
			
		}

		public void createStartGeneration() {
			startGenerationGenomeCollection = new genome[numberOfIndividualsInGeneration];
			int numGenes;
			for (int i = 0; i < numberOfIndividualsInGeneration; i++) {
				numGenes = createNumberOfGenes();
				// 	public const float dupeRate = 0.05f;
				// 	public const float muteRate = 0.05f;
				// 	public const float delRate = 0.01f;
				// 	public const float changePercent = 0.15f;

				// public int maxSpawn = 100;
				// public int vMax = 5;
				// public int vDurationMin = 1;
				// public int vDurationMax = 100;
				// public int gMax = 3;
				// public int gDurationMin = 1;
				// public int gDurationMax = 100;

				// public float dupeRate = 0.05f;
				// public float muteRate = 0.05f;
				// public float delRate = 0.01f;
				// public float changePercent = 0.15f;

				// int numberOfGenesInGenome, float dupeRate, float muteRate, float delRate, float changePercent
				startGenerationGenomeCollection[i].setGenomeParameters(numGenes, 0.05f, 0.05f, 0.01f, 0.15f);
				// int maxSpawn, int vMax, int vDurationMin, int vDurationMax, int gMax, int gDurationMin, int gDurationMax
				startGenerationGenomeCollection[i].createWholeGenome(100, 5, 1, 100, 3, 1, 100);
			}
		}

		public int normalizeRandom(int minVal, int maxVal) {
			int mean = (minVal  + maxVal) /2;
			int sigma = (maxVal = mean) / 3;
			return rand.Next(mean, sigma);
		}

		public int createNumberOfGenes() {
			numberOfGenes = normalizeRandom(4,16);
			return numberOfGenes;
		}
	}


	////// STRUCT: genomeToPhenotype
	// 	This structure contains all the functions for taking a genome and running the
	//  G-->P process on it. It will produce the nessary output to feed into the createParams 
	//  structure.
	// 	Functions:
	// 			passGenome() : passes a genome into this structure for processing.
	// 			devoGrpahics() : given a genome, creates an array of x,y coordinates and sizes for 
	//							 each gene.
	public struct genomeToPhenotype {

		List<float[]> arrayList;
		List<float[]> connectionList;

		public genome givenGenome; 

		public int genomeLength;

		public float[] center;
		public const int pointRadius = 3;
		public int theCount;

		public int[] irPointList;
		public int[] photoPointList;
		public int[] genePosList;

		public int[] currentList;

		public double x;
		public double y;

		public float size;


		public void passGenome(genome thisGenome) {
			givenGenome = thisGenome;
			center = new float[2] {375,325};
			theCount = 0;
			genomeLength = thisGenome.arrayOfGenes.Length;
		}


		public void runDevoGraphics() {

		}


		// "Runs" the graphics in order to determine the x,y coordinate and size of each node created 
		// by each gene in a genome.
		// Returns a list of arrays that contain the x,y coordinate and size of each gene in the 
		// provided genome.
		public void devoGraphics(int count) {

			arrayList = new List<float[]>();
			//0 public float partType;// # 0 = Part Type (0 = IR, 1 = Photo, 2 = Neuron, 3 = R Motor, 4 = L Motor)
			//1 public float angle;// # 1 = Angle
			//2 public float startTime;// # 2 = Start Time
			//3 public float velocity;// # 3 = Velocity
			//4 public float travelTime;// # 4 = Travel Time
			//5 public float growthRate;// # 5 = Growth Rate
			//6 public float growthTime;// # 6 = Growth Time
			//7 public float index;// # 7 = Index
			
			// for (count = 0; count < 500; count++) {
				for (int j = 0; j < genomeLength; j++) {
					Debug.Log("IN DEVO GRAPHICS");
					if (givenGenome.arrayOfGenes[j].startTime <= count) {
						if ((givenGenome.arrayOfGenes[j].travelTime + givenGenome.arrayOfGenes[j].startTime) >= count) {
							x = center[0] + givenGenome.arrayOfGenes[j].velocity * (count - givenGenome.arrayOfGenes[j].startTime)*Math.Cos(degreeToRadians(givenGenome.arrayOfGenes[j].angle));
							x = center[1] + givenGenome.arrayOfGenes[j].velocity * (count - givenGenome.arrayOfGenes[j].startTime)*Math.Sin(degreeToRadians(givenGenome.arrayOfGenes[j].angle));
						} else {
							x = (center[0] + givenGenome.arrayOfGenes[j].velocity*givenGenome.arrayOfGenes[j].travelTime*Math.Cos(degreeToRadians(givenGenome.arrayOfGenes[j].angle)));
							y = (center[1] + givenGenome.arrayOfGenes[j].velocity*givenGenome.arrayOfGenes[j].travelTime*Math.Sin(degreeToRadians(givenGenome.arrayOfGenes[j].angle)));
						}

						if ((givenGenome.arrayOfGenes[j].growthTime + givenGenome.arrayOfGenes[j].startTime) >= count) {
							size = (1 + givenGenome.arrayOfGenes[j].growthRate*(count - givenGenome.arrayOfGenes[j].startTime));
						} else {
							size =  (1 + givenGenome.arrayOfGenes[j].growthRate * givenGenome.arrayOfGenes[j].growthTime);
						}

						float[] tempArray = new float[4];
						tempArray[0] = (float)x;
						tempArray[1] = (float)y;
						tempArray[2] = size;
						tempArray[3] = givenGenome.arrayOfGenes[j].index;
						// givenGenome.arrayOfGenes[j].x = x;
						// givenGenome.arrayOfGenes[j].y = y;
						// givenGenome.arrayOfGenes[j].size = size;
						Debug.Log(tempArray[0]);
						arrayList.Add(tempArray);

					}
					// givenGenome[j].setGeneParameters(100, 5, 1, 100, 3, 1, 100, i);
				}

			// }

			float tempFLoat = arrayList[0][0];
			// return arrayList;
			// Debug.Log(tempFLoat);
			// int lengthfList = arrayList.Count;

			// for (int i = 0; i < lengthfList; i ++) {
			// 	Debug.Log(arrayList[i]);
			// }

		}


		// Creates the list of connection arrays.
		public void checkConds(List<float[]> currentLocations) {
			connectionList = new List<float[]>();
			int currentLocationsLength = currentLocations.Count();
			for (int i = 0; i < (currentLocationsLength - 1); i++) {
				for (int j = i+1; j < currentLocationsLength; j++) {
					float dist = distance(currentLocations[i][0], currentLocations[j][0], currentLocations[i][1], currentLocations[j][1]);
					float combRad = currentLocations[i][2] + currentLocations[j][2];
					if (dist <= combRad) {
						float[] newComb = new float[2];
						newComb[0] = currentLocations[i][3];
						newComb[1] = currentLocations[j][3];
						connectionList.Add(newComb);
					}
				}
			}
		}



		// HELPER FUNCTIONS:
		////////////////////
		// Converts a degreee to a Radian
		public double degreeToRadians(double angle) {
			return (Math.PI * angle / 180f);
		}	
		// Converts two x,y pairs into the distance between those two coordinates.
		public float distance(float x1, float x2, float y1, float y2){
			return ((float)(Math.Sqrt(Math.Pow((x1 - x2), 2) + Math.Pow((y1 - y2), 2))));
		}
	

	}



	////// STRUCT: createParams
	// 	This structure represents a single generation, or a collection of genomes (individuals).
	// 	Functions:
	// 		setGenerationParameters() : specifies number of genes.
	// 			INPUT: (int meanNumberOfGenes, int meanStandardDeviationOfGenes, int numberOfIndividualsInGeneration)
	//			OUTPUT: assigns the generation's parameters.
	//		createStartGeneration() : creates a generation given the parameters set above.
	// 		createNumberOfGenes() : creates the number of genes for a given genome in the generation.
	public struct createParams {
		int RMI;
		int LMI;
		int NUM_INPUT;
		int NUM_HIDDEN;
		int NUM_OUTPUT;

		float[,] TESTinput_to_output;
		float[,] TESTinput_to_hidden;
		float[,] TESThidden_to_hidden;
		float[,] TESThidden_to_output;
		float[,] TESToutput_to_hidden;
		int[] rmiVal;
		int[] lmiVal;	
	}


	// Use this for initialization
	void Start () {

		Debug.Log("START");

		// float[,] TESTinput_to_output = new float[4,2] {{0,0},{0,0},{0,0},{0,0}};
		// float[,] TESTinput_to_hidden = new float[4,6] {{0,0,1,-1,0,0},{0,0,-1,1,0,0},{1,-1,0,0,0,0},{-1,1,0,0,0,0}};
		// float[,] TESThidden_to_hidden = new float[6,6] {{0,0,0,0,1,0},{0,0,0,0,1,0},{0,0,0,0,0,1},{0,0,0,0,0,1},{0,0,0,0,0,0},{0,0,0,0,0,0}};
		// float[,] TESThidden_to_output = new float[6,2] {{0,0},{0,0},{0,0},{0,0},{1,-0.5f},{-0.5f,1}};
		// float[,] TESToutput_to_hidden = new float[2,6] {{0,0,0,0,0,0},{0,0,0,0,0,0}};

		// int[] rmiVal = new int[1] {0};
		// int[] lmiVal = new int[1] {1};


		int maxSpawn = 100;
		int vMax = 5;
		int vDurationMin = 1;
		int vDurationMax = 100;
		int gMax = 3;
		int gDurationMin = 1;
		int gDurationMax = 100;

		int numberOfGenes = 8;

		float dupeRate = 0.5f;
		// .05
		float muteRate = 0.05f;
		// .05
		float delRate = 0.01f;
		// .01
		float changePercent = 0.15f;
		genome testGenome = new genome();
		genomeToPhenotype testGtoP = new genomeToPhenotype();

		testGenome.createRandomFunction();
		// Set the genome parameters.
		testGenome.setGenomeParameters(numberOfGenes, dupeRate, muteRate, delRate, changePercent);
		// Create the whole genome with the provided gene params.
		testGenome.createWholeGenome(maxSpawn, vMax, vDurationMin, vDurationMax, gMax, gDurationMin, gDurationMax);
		// Print the contents of the genome.
		testGenome.printGenomeContents();

		testGtoP.passGenome(testGenome);
		// testGtoP.devoGraphics(0);


		// MUTATE AND DELETE.
		// testGenome.mutate();
		// testGenome.printGenomeContents();
		// testGenome.duplicateAndDelete();
		// testGenome.printGenomeContents();




		// // Number of genes in a genom
		// public float numGenes = 10;

		// DECLARE INSTANCE OF NEURAL NETWORK.
		// neuralNetworkHandler.NeuralNetworkParameters testStruct = new neuralNetworkHandler.NeuralNetworkParameters();
		// genome testGenome = new genome();
		// testGenome.setGenomeParameters(8, 0.05f, 0.05f, 0.01f,0.15f);
		// testGenome.createWholeGenome(maxSpawn, vMax, vDurationMin, vDurationMax, gMax, gDurationMin, gDurationMax);
	
		// CALLS TO CONSTRUCTORS.
		// (RMI, LMI, NUM_INPUT, NUM_HIDDEN, NUM_OUTPUT).
		// testStruct.setStartVariables(1,1,4,6,2);
		// testStruct.setStartingArrays(rmiVal, lmiVal);

		// Grab the correctly generated arrays
		// testStruct.setConnections(TESTinput_to_output,TESTinput_to_hidden,TESThidden_to_hidden, TESThidden_to_output, TESToutput_to_hidden);

		// genome testGenome = new genome();
		// testGenome.setGenomeVariables(8);
		// testGenome.makeGenome();
		
	}
	
	// Update is called once per frame
	void Update () {



		// makeGenome(8);
		// distance(100,100, 100,100);
		// Debug.Log("WE MADE it!");
		// print("WE MADE IT!");
	}




}
