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

	public int maxSpawn = 100;
	public int vMax = 5;
	public int vDurationMin = 1;
	public int vDurationMax = 100;
	public int gMax = 3;
	public int gDurationMin = 1;
	public int gDurationMax = 100;

	public float dupeRate = 0.05f;
	public float muteRate = 0.05f;
	public float delRate = 0.01f;
	public float changePercent = 0.15f;

	// Number of genes in a genom
	public float numGenes = 10;

	////// STRUCT: gene
	// 	This structure represents a single gene.
	// 	Functions:
	// 		setGeneParameters() : creates a gene and sets its parameters.
	//			INPUT: int maxSpawn,  int vMax, int vDurationMin, int vDurationMax, int gMax, int gDurationMin, int gDurationMax, int index
	//			OUTPUT: a gene with --> (partType, angle, startTime, velocity, traveltime, growthRate, growthTime, index).
	public struct gene {

		System.Random rand;
		// public int maxSpawn;
		// public int vMax;
		// public int vDurationMin;
		// public int vDurationMax;
		// public int gMax;
		// public int gDurationMin;
		// public int gDurationMax;

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

		public void setGeneParameters(int maxSpawn,  int vMax, int vDurationMin, int vDurationMax, int gMax, int gDurationMin, int gDurationMax, int index) {
			rand = new System.Random();

			partType = rand.Next(0, 4);
			angle = rand.Next(0, 360);
			startTime = rand.Next(0, maxSpawn);
			velocity = rand.Next(1, vMax);
			travelTime = rand.Next(vDurationMin, vDurationMax);
			growthRate = rand.Next(1, gMax);
			growthTime = rand.Next(gDurationMin, gDurationMax);
			index = index;
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
		System.Random rand;

		public gene[] arrayOfGenes;
		public gene[] newMutatedGenome;

		public int numberOfGenesInGenome;
		public float dupeRate;
		public float muteRate;
		public float delRate;
		public float changePercent;
		public bool firstDupe;

		public int numberOfDuplications;

		// Function to set the genome parameters.
		public void setGenomeParameters(int numberOfGenesInGenome, float dupeRate, float muteRate, float delRate, float changePercent) {
			rand = new System.Random(); 
			numberOfGenesInGenome = numberOfGenesInGenome;
			dupeRate = dupeRate;
			muteRate = muteRate;
			delRate = delRate;
			changePercent = changePercent;	
			numberOfDuplications = 0;
			firstDupe = true;
		}

		// Creates the specified genome after setting parameters.
		public void createWholeGenome(int maxSpawn, int vMax, int vDurationMin, int vDurationMax, int gMax, int gDurationMin, int gDurationMax) {
			arrayOfGenes = new gene[numberOfGenesInGenome];
			// Use i for indexing the genes in the array of genes. 
			int i = 0;
			for (int j = 0; j < numberOfGenesInGenome; j++) {
				arrayOfGenes[j].setGeneParameters(100, 5, 1, 100, 3, 1, 100, i);
				i = i + 1;
			}
		}

		// Duplicates and deletes the different genes in a genome.
		public void duplicateAndDelete() {
			// Int i for indexing which gene is doubled.
			List<gene> geneDuplicatedAndNotDeleted = new List<gene>();
			int i = 0;
			foreach (var item in arrayOfGenes) {
				if (rand.NextDouble() <= dupeRate) {
					numberOfDuplications = numberOfDuplications + 1;
					geneDuplicatedAndNotDeleted.Add(item);
					geneDuplicatedAndNotDeleted.Add(item);
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
				if (rand.NextDouble() > delRate) {
					geneDuplicatedAndNotDeleted.Add(item);
				}
				i = i +1;
			}
			arrayOfGenes = geneDuplicatedAndNotDeleted.ToArray();
			int lengthOfNewArray = arrayOfGenes.Length;
		
			for (int j = 0; j < lengthOfNewArray; j++) {
				arrayOfGenes[j].index = j;
				
			}
		}

		// Mutates the genes in the genome.
		public void mutate() {
			int lengthOfArray = arrayOfGenes.Length;
			for (int j = 0; j < lengthOfArray; j++) {
				for (int i = 0; i < 7; i++) {
					if (rand.NextDouble() <= muteRate) {
						if (i == 0) {
							arrayOfGenes[j].partType = rand.Next(0,4);
						} else if (rand.NextDouble() > 0.5) {
							if (i == 1) {
								arrayOfGenes[j].angle = arrayOfGenes[j].angle + arrayOfGenes[j].angle*changePercent;
							} else if (i == 2) {
								arrayOfGenes[j].startTime = arrayOfGenes[j].startTime  + arrayOfGenes[j].startTime *changePercent;
							} else if (i == 3) {
								arrayOfGenes[j].velocity = arrayOfGenes[j].velocity + arrayOfGenes[j].velocity*changePercent;
							} else if (i == 4) {
								arrayOfGenes[j].travelTime = arrayOfGenes[j].travelTime + arrayOfGenes[j].travelTime*changePercent;
							} else if (i == 5) {
								arrayOfGenes[j].growthRate = arrayOfGenes[j].growthRate + arrayOfGenes[j].growthRate*changePercent;
							} else if (i == 6) {
								arrayOfGenes[j].growthTime = arrayOfGenes[j].growthTime + arrayOfGenes[j].growthTime*changePercent;
							} else if (i == 7) {
								Debug.Log(arrayOfGenes[j].index);
							}
						}
					}
				}
			}
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



	public struct genomeToPhenotypeParams {

		public genome givenGenome; 

		public int genomeLength;

		public float[] center;
		public const int pointRadius = 3;
		public int count;

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
			count = 0;
			genomeLength = thisGenome.arrayOfGenes.Length;
		}


		public void devoGraphics() {

		//0 public float partType;// # 0 = Part Type (0 = IR, 1 = Photo, 2 = Neuron, 3 = R Motor, 4 = L Motor)
		//1 public float angle;// # 1 = Angle
		//2 public float startTime;// # 2 = Start Time
		//3 public float velocity;// # 3 = Velocity
		//4 public float travelTime;// # 4 = Travel Time
		//5 public float growthRate;// # 5 = Growth Rate
		//6 public float growthTime;// # 6 = Growth Time
		//7 public float index;// # 7 = Index

			for (int j = 0; j < genomeLength; j++) {
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

					givenGenome.arrayOfGenes[j].x = x;
					givenGenome.arrayOfGenes[j].y = y;
					givenGenome.arrayOfGenes[j].size = size;
				}
				// givenGenome[j].setGeneParameters(100, 5, 1, 100, 3, 1, 100, i);
			}

		}



		public double degreeToRadians(double angle) {
			return (Math.PI * angle / 180f);
		}

	}

	


	// Use this for initialization
	void Start () {

		Debug.Log("START");

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
