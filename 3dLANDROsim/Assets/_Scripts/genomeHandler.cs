using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class genomeHandler {
	
System.Random rand = new System.Random();

	////// STRUCT: gene
	// 	This structure represents a single gene.
	//
	// 	Functions:
	// 		setGeneParameters() : creates a gene and sets its parameters.
	//			INPUT: int maxSpawn,  int vMax, int vDurationMin, int vDurationMax, int gMax, int gDurationMin, int gDurationMax, int index
	//			OUTPUT: a gene with --> (partType, angle, startTime, velocity, traveltime, growthRate, growthTime, index).
	public struct gene {
		// G-P Process information.
		public double x;
		public double y;
		public float size;

		// Gene properties.
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
	//
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

		// FUNCTION: setGenomeParameters()
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


		public void makeTestGenome(int[][] exampleGenome) {
			// [[4, 187, 73, 5, 14, 1, 54, 0], [4, 234, 67, 4, 33, 1, 51, 1], [0, 223, 7, 4, 70, 3, 13, 2], [3, 220, 83, 4, 42, 3, 87, 3], [1, 329, 73, 3, 68, 2, 54, 4], [3, 335, 64, 5, 85, 3, 58, 5], [4, 282, 100, 1, 57, 3, 56, 6], [4, 117, 47, 5, 49, 1, 19, 7], [2, 237, 44, 5, 45, 3, 59, 8], [4, 93, 48, 5, 53, 2, 44, 9], [0, 45, 88, 1, 54, 3, 17, 10]]
			 int rowLength = exampleGenome.GetLength(0);
        	int colLength = exampleGenome.GetLength(1);

        	arrayOfGenes = new gene[rowLength];

			int length = exampleGenome.Length;
			for (int i = 0; i < rowLength; i++) {
				for (int j = 0; j < colLength; j++) {



				}
			}

		}

		// FUNCTION: createWholeGenome()
		// Creates the specified genome after setting parameters.
		public void createWholeGenome(int maxSpawn, int vMax, int vDurationMin, int vDurationMax, int gMax, int gDurationMin, int gDurationMax) {
			// Debug.Log("Number of genes: " +numberOfGenes);
			for (int j = 0; j < numberOfGenes; j++) {
				int partType = rand.Next(0,5);
				int angle = rand.Next(0, 360);
				int startTime =  rand.Next(0, maxSpawn);
				int velocity = rand.Next(1, vMax);
				int travelTime = rand.Next(vDurationMin, vDurationMax);
				int growthRate = rand.Next(1, gMax);
				int growthTime = rand.Next(gDurationMin, gDurationMax);
				// maxSpawn,  int vMax, int vDurationMin, int vDurationMax, int gMax, int gDurationMin, int gDurationMax, int geneNumber
				arrayOfGenes[j].setGeneParameters(partType, angle, startTime, velocity, travelTime, growthRate, growthTime, j);
			}
		}

		// FUNCTION: duplicateAndDelete()
		// This function duplicates and deletes the different genes in a genome.
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
					// Debug.Log("THERE HAS BEEN A DELETION, num: " + numDeleted + ", index: " + item.index);
				} 
			};

			for (int k = 0; k < countOfnonDeleted; k++) {
				if (rand.NextDouble() <= dupeRate) {
					numberOfDuplications = numberOfDuplications + 1;
					geneDuplicatedAndNotDeleted.Add(geneDuplicatedAndNotDeleted[k]);
					// Debug.Log("THERE HAS BEEN A DUPLICATION, num dups: " + numberOfDuplications);
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
				
			int listCount =  geneDuplicatedAndNotDeleted.Count();
			numberOfGenes = listCount;
			// Debug.Log("SIZE OF LIST: "+listCount);
			// arrayOfGenes = geneDuplicatedAndNotDeleted.ToArray();
			arrayOfGenes = new gene[listCount];
			arrayOfGenes = geneDuplicatedAndNotDeleted.ToArray();
			int lengthOfNewArray = arrayOfGenes.Length;
			for (int j = 0; j < lengthOfNewArray; j++) {
				arrayOfGenes[j].index = j;	
			}

		}

		// FUNCTION: mutate()
		// This function mutates all of the genes within a genome.
		public void mutate() {
			int lengthOfArray = arrayOfGenes.Length;
			// Debug.Log("MUTE RATE: "+ muteRate);
			for (int j = 0; j < lengthOfArray; j++) {
				for (int i = 0; i < 7; i++) {
					if (rand.NextDouble() <= muteRate) {
						// Debug.Log("THERE HAS BEEN A MUTATION");
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
								// Debug.Log(arrayOfGenes[j].index);
							}
						}
					}
				}
			}
		}

		// FUNCTION: printGenomeContents()
		// This debugging function prints all of the genes in a genome 
		// in addition to their contents/properties.
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
			}
			Debug.Log(printString);
		}
	}

	////// STRUCT: genomeToPhenotype
	// 	This structure contains all the functions for taking a genome and running the
	//  G-->P process on it. It will produce the nessary output to feed into the createParams 
	//  structure.
	//
	// 	Functions:
	// 			passGenome() : passes a genome into this structure for processing.
	// 			devoGrpahics() : given a genome, creates an array of x,y coordinates and sizes for 
	//							 each gene.
	public struct genomeToPhenotype {
		// Step 1
		public List<int[]> currentArrayList;
		// Step 2
		public List<int[]> connectionList;
		// Step 3
		public List<int[]> finalConnections;
		// Step 4
		public List<int[]> sortedConnects;

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

		// Passes a genome into an instance of this struct and sets G-P variables.
		public void passGenome(genome thisGenome) {
			givenGenome = thisGenome;
			center = new float[2] {375,325};
			theCount = 0;
			genomeLength = thisGenome.arrayOfGenes.Length;
		}

		// Combines the functions written below to run the given genome through
		// the G->P process.
		public void runDevoGraphics() {
			while (theCount < 500) {
				devoGraphics(theCount);
				processConnections();
				theCount = theCount +1;
			}

		}

		// "Runs" the graphics in order to determine the x,y coordinate and size of each node created 
		// by each gene in a genome.
		// Returns a list of arrays that contain the x,y coordinate and size of each gene in the 
		// provided genome.
		public void devoGraphics(int count) {

			currentArrayList = new List<int[]>();
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
					// Debug.Log("IN DEVO GRAPHICS");
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

						int[] tempArray = new int[4];
						tempArray[0] = (int)x;
						tempArray[1] = (int)y;
						tempArray[2] = (int)size;
						tempArray[3] = (int)givenGenome.arrayOfGenes[j].index;
						// givenGenome.arrayOfGenes[j].x = x;
						// givenGenome.arrayOfGenes[j].y = y;
						// givenGenome.arrayOfGenes[j].size = size;
						// Debug.Log(tempArray[0]);
						currentArrayList.Add(tempArray);

					}
				}
				checkConds(currentArrayList);
		}


		// Creates the list of connection arrays.
		public void checkConds(List<int[]> currentLocations) {
			connectionList = new List<int[]>();
			int currentLocationsLength = currentLocations.Count();
			for (int i = 0; i < (currentLocationsLength - 1); i++) {
				for (int j = i+1; j < currentLocationsLength; j++) {
					float dist = distance(currentLocations[i][0], currentLocations[j][0], currentLocations[i][1], currentLocations[j][1]);
					float combRad = currentLocations[i][2] + currentLocations[j][2];
					if (dist <= combRad) {
						int[] newComb = new int[2];
						newComb[0] = currentLocations[i][3];
						newComb[1] = currentLocations[j][3];
						connectionList.Add(newComb);
					}
				}
			}
		}

		public void processConnections() {
			finalConnections = new List<int[]>();
			foreach (var item in connectionList) {
				if (!(finalConnections.Contains(item))) {
					finalConnections.Add(item);
				}
			}
		}


		// Proccesses the final connections array.
		public void makeConnectome() {
			sortedConnects = new List<int[]>();
			int lenghtOfFinalConnects = finalConnections.Count();
			string[] partTypes = new string[5] {"IR", "Photo", "Neuron", "Right Motor", "Left Motor"};
			int sortedConnectsLength;

			for (int i = 0; i < lenghtOfFinalConnects; i++) {
				// Debug.Log( finalConnections[i][0]);
				// int temp1 = finalConnections[i][0];
				// int temp2 = finalConnections[i][0];
				int[] tempIntArray  = new int[2];
				float size1 = ((givenGenome.arrayOfGenes[finalConnections[i][0]].growthRate)) * ((givenGenome.arrayOfGenes[finalConnections[i][0]].growthTime));
				float size2 = ((givenGenome.arrayOfGenes[finalConnections[i][1]].growthRate)) * ((givenGenome.arrayOfGenes[finalConnections[i][1]].growthTime));
				if (size1 > size2) {
					sortedConnects.Add(finalConnections[i]);
					// Debug.Log("THIS IS BEING ADDED TO THE SORTED CONNECTS: " + finalConnections[i]);
				} else if(size2 > size1) {
					tempIntArray[0] = finalConnections[i][1];
					// Debug.Log("THIS IS BEING ADDED TO THE SORTED CONNECTS: " + finalConnections[i][0]);
					// Debug.Log("THIS IS BEING ADDED TO THE SORTED CONNECTS: " + finalConnections[i][1]);
					tempIntArray[1] = finalConnections[i][0];
					sortedConnects.Add(tempIntArray);
				} else {
					sortedConnects.Add(finalConnections[i]);
					sortedConnects.Add(tempIntArray);
				}
			}


			sortedConnectsLength = sortedConnects.Count();

			List<int> popList = new List<int>();

			for (int i = 0; i < sortedConnectsLength; i ++) {
				if ((givenGenome.arrayOfGenes[sortedConnects[i][1]].partType == 0) || (givenGenome.arrayOfGenes[sortedConnects[i][1]].partType == 1)) {
					popList.Add(i);
				} else if (((givenGenome.arrayOfGenes[sortedConnects[i][0]].partType == 3) || (givenGenome.arrayOfGenes[sortedConnects[i][0]].partType == 4)) &&  ((givenGenome.arrayOfGenes[sortedConnects[i][1]].partType == 3) || (givenGenome.arrayOfGenes[sortedConnects[i][1]].partType == 4))) {
					popList.Add(i);
				}
			}

			popList.Reverse();

			foreach (var item in popList) {
				sortedConnects.RemoveAt(item);
			}


		}


		public void printConnectomeContents() {
			int[,] sortedConnectsArray = To2D(sortedConnects.ToArray());
			int lengthOfArrayRow = sortedConnectsArray.GetLength(0);
			int lengthOfArrayCol = sortedConnectsArray.GetLength(1);
			string printString = "CONNECTOME: ";
			for (int j = 0; j < lengthOfArrayRow; j++) {
				for (int i = 0; i < lengthOfArrayCol; i++) {
				printString += sortedConnectsArray[j,i] + ", ";
				}
			}
			Debug.Log(printString);
		}

		// HELPER FUNCTIONS:
		// Converts a degreee to a Radian
		public double degreeToRadians(double angle) {
			return (Math.PI * angle / 180f);
		}	
		// Converts two x,y pairs into the distance between those two coordinates.
		public float distance(float x1, float x2, float y1, float y2){
			return ((float)(Math.Sqrt(Math.Pow((x1 - x2), 2) + Math.Pow((y1 - y2), 2))));
		}

		public T[,] To2D<T>(T[][] source) {
			// try{
				int FirstDim = source.Length;
				int SecondDim = source.GroupBy(row => row.Length).Single().Key;
				var result = new T[FirstDim, SecondDim];
				for (int i = 0; i < FirstDim; i++) {
					for (int j = 0; j < SecondDim; j++) {
						result[i,j] = source[i][j];
					}
				}
				return result;
			// } catch (InvalidOperationException) {
			// 	 throw new InvalidOperationException("The given jagged array is not rectangular.");
			// }
		} 

	}

	////// STRUCT: createParams
	// 	This structure houses the functions for creating a set of neural network parameters provided
	//  a connection matrix created in the G->P processing struct.
	//
	// 	Functions:
	// 		passConnectomeMatrix() : passes the matrix created in the G->P struct.
	//		setNodeLayerNumbers() : sets the proper num input, hidden, and outputs nodes for the neural network.
	// 		motorIndexes() : creates the number of genes for a given genome in the generation.
	public struct createParams {

		public genome paramsGenome; 

		// Arrays that hold input information and what nodes to assign sensor input to.
		public List<int> senseToInput;
		public int[] chosenSensorArray;

		public List<float[]> inputToHidden;
		public List<float[]> hiddenToHidden;
		public List<float[]> hiddenToOutput;
		public List<float[]> inputToOutput;
		public List<float[]> outputToHidden;

		public List<int[]> connectionMatrix;
		public List<int> usedList;

		public List<int> inputIndexes;
		public List<int> hiddenIndexes;
		public List<int> outputIndexes;

		public List<int> RMI;
		public List<int> LMI;

		public int[] finalRMI;
		public int[] finalLMI;

		public int motorCount;

		public int RMIlength;
		public int LMIlength;

		// Final resulting Params
		public float[,] input_to_output;
		public float[,] input_to_hidden;
		public float[,] hidden_to_hidden;
		public float[,] hidden_to_output;
		public float[,] output_to_hidden;

		public int NUM_INPUT;
		public int NUM_HIDDEN;
		public int NUM_OUTPUT;

		// Passes the current genome and its resulting connection matrix.
		public void passConnectionMatrix(List<int[]> vConnect, genome thisGenome) {
			connectionMatrix = new List<int[]>();
			connectionMatrix = vConnect;
			paramsGenome = thisGenome;

			usedList = new List<int>();

			inputIndexes = new List<int>();
			hiddenIndexes = new List<int>();
			outputIndexes = new List<int>();

			inputToHidden = new List<float[]>();
			hiddenToHidden = new List<float[]>();
			hiddenToOutput = new List<float[]>();
			inputToOutput = new List<float[]>();
			outputToHidden = new List<float[]>();

			senseToInput = new List<int>();

			RMI = new List<int>();
			LMI = new List<int>();


			NUM_INPUT = 0;
			NUM_HIDDEN = 0;
			NUM_OUTPUT = 0;

			RMIlength = 0;
			LMIlength = 0;

			motorCount = 0;
		}

		// Uses the genome and the connection matrix in order to set num_input,
		// num_hidden, and num_output.
		public void setNodeLayerNumbers() {
			foreach (var item in connectionMatrix) {

				if (!(usedList.Contains(item[0]))) {
					// Debug.Log("CONTAINS WORKS!!!!!!!!!!!!!");
					if ((paramsGenome.arrayOfGenes[item[0]].partType == 0) || (paramsGenome.arrayOfGenes[item[0]].partType == 1)) {
						NUM_INPUT = NUM_INPUT + 1;
						inputIndexes.Add(item[0]);
						// Debug.Log("Set node layer input indexes: "+item[0]);
					} else if (paramsGenome.arrayOfGenes[item[0]].partType == 2) {
						NUM_HIDDEN = NUM_HIDDEN + 1;
						hiddenIndexes.Add(item[0]);
						// Debug.Log("Set node layer hidden indexes: "+item[0]);
					} else if ((paramsGenome.arrayOfGenes[item[0]].partType == 3) || (paramsGenome.arrayOfGenes[item[0]].partType == 4)) {
						NUM_OUTPUT = NUM_OUTPUT + 1;
						outputIndexes.Add(item[0]);
						// Debug.Log("Set node layer output indexes: "+item[0]);
					}
					usedList.Add(item[0]);
				}

				if (!(usedList.Contains(item[1]))) {
					// Debug.Log("CONTAINS WORKS!!!!!!!!!!!!!");
					if (paramsGenome.arrayOfGenes[item[1]].partType == 2) {
						NUM_HIDDEN = NUM_HIDDEN + 1;
						hiddenIndexes.Add(item[1]);
						// Debug.Log("Set node layer hidden indexes: "+item[1]);
					} else if ((paramsGenome.arrayOfGenes[item[1]].partType == 3) || (paramsGenome.arrayOfGenes[item[1]].partType == 4)) {
						NUM_OUTPUT = NUM_OUTPUT + 1;
						outputIndexes.Add(item[1]);
						// Debug.Log("Set node layer output indexes: "+item[1]);
					}
					usedList.Add(item[1]);
				}

			}
			usedList = new List<int>();
		}

		// Calculating which motor nodes to aggregate data from in order to drive the left and right motors.
		public void motorIndexes() {
			foreach (var connection in connectionMatrix) {
				if ((!(usedList.Contains(connection[0])))) {

					if (paramsGenome.arrayOfGenes[connection[0]].partType == 3) {
						RMIlength = RMIlength + 1;
						RMI.Add(motorCount);
						motorCount = motorCount + 1;
					} 
					if (paramsGenome.arrayOfGenes[connection[0]].partType == 4) {
						LMIlength = LMIlength + 1;
						LMI.Add(motorCount);
						motorCount = motorCount + 1;
					}
					usedList.Add(connection[0]);
				} 
				if (!(usedList.Contains(connection[1]))) {

					if (paramsGenome.arrayOfGenes[connection[1]].partType == 3) {
						RMIlength = RMIlength + 1;
						RMI.Add(motorCount);
						motorCount = motorCount + 1;
					}
					if (paramsGenome.arrayOfGenes[connection[1]].partType == 4) {
						LMIlength = LMIlength + 1;
						LMI.Add(motorCount);
						motorCount = motorCount + 1;
					}
					usedList.Add(connection[1]);
				}		
			}
			usedList = new List<int>();
		}

		// Assigns sensor output to input nodes.
		public void sensorToInputs() {
			foreach (var con in connectionMatrix) {
				if (!(usedList.Contains(con[0]))) {
					if (paramsGenome.arrayOfGenes[con[0]].partType == 0) {
						float tempOutput = (float)((((paramsGenome.arrayOfGenes[con[0]].angle)+22.5)/45)%8) * 2;
						senseToInput.Add((int)tempOutput);
						// Debug.Log("SENSE TO INPUT: "+ tempOutput);
					}
					if (paramsGenome.arrayOfGenes[con[0]].partType == 1) {
						float tempOutput = (float)(((((paramsGenome.arrayOfGenes[con[0]].angle))/45)%8) * 2) + 1;
						senseToInput.Add((int)tempOutput);
						// Debug.Log("SENSE TO INPUT: "+ tempOutput);
					}
					usedList.Add(con[0]);
				}
			}
		}

		// Creates the input to hidden connections.
		public void createInputToHidden() {
			// Debug.Log(listIntArrayToString(connectionMatrix));
			for (int i = 0; i < NUM_INPUT; i++) {
				List<float> tempInput = new List<float>();
				for (int j = 0; j < NUM_HIDDEN; j++) {

					int[] currentTempArray = new int[] {inputIndexes[i], hiddenIndexes[j]};
					bool exists = false;
					foreach (var item in connectionMatrix) {
						// Debug.Log("Item: "+ intArrayToString(item));
						// Debug.Log("Temp array: "+ intArrayToString(currentTempArray));
						if ((item[0] == currentTempArray[0]) && item[1] == currentTempArray[1]) {
							// Debug.Log("WE HAVE A MATCH!!");
							exists = true;
						} else {
							// Debug.Log("NO MATCH");
						}
					}

					if (exists) {
						// Debug.Log("IT CONTAINS!!!!!!!!!!!!!");
						gene sensor = paramsGenome.arrayOfGenes[inputIndexes[i]];
						gene hidden = paramsGenome.arrayOfGenes[hiddenIndexes[j]];
						float strength = (float)(((sensor.velocity) * (sensor.travelTime) + (hidden.velocity) * (hidden.travelTime)) / 250.0);
						// Debug.Log("Strength being added to input to hidden: "+strength);
						if ((((sensor.angle) + 180)%360) < hidden.angle) {
							strength = strength * -1;
						}
						tempInput.Add(strength);
						// Debug.Log("Strength was actually added: "+strength);
					} else {
						tempInput.Add(0);
						// Debug.Log("Only a zero was added from input to hidden.");
					}
				}

				// // NOT SURE IF CORRECT
				// if (NUM_HIDDEN == 0) {
				// 	tempInput.Add(0);
				// }

				inputToHidden.Add(tempInput.ToArray());
			}

			if (NUM_INPUT == 0) {
				List<float> tempInput = new List<float>();
				inputToHidden.Add(tempInput.ToArray());
			}

			Debug.Log("Input to Hidden: "+listFloatArrayToString(inputToHidden));
		}


		public void createHiddenToHidden() {
			for (int i = 0; i < NUM_HIDDEN; i++) {
				List<float> tempHidden = new List<float>();
				for (int j = 0; j < NUM_HIDDEN; j++) {

					int[] currentTempArray = new int[] {hiddenIndexes[i], hiddenIndexes[j]};
					bool exists = false;
					foreach (var item in connectionMatrix) {
						// Debug.Log("Item: "+ intArrayToString(item));
						// Debug.Log("Temp array: "+ intArrayToString(currentTempArray));
						if ((item[0] == currentTempArray[0]) && item[1] == currentTempArray[1]) {
							// Debug.Log("WE HAVE A MATCH!!");
							exists = true;
						} else {
							// Debug.Log("NO MATCH");
						}
					}

					if (exists) {
						gene hidden1 = paramsGenome.arrayOfGenes[hiddenIndexes[i]];
						gene hidden2 = paramsGenome.arrayOfGenes[hiddenIndexes[j]];
						float strength = (float)(((hidden1.velocity * hidden1.travelTime) + (hidden2.velocity * hidden2.travelTime)) / 250.0);

						if ((hidden1.angle + 180)%360 < hidden2.angle) {
							strength = strength * -1;
						}
						tempHidden.Add(strength);

					} else {
						tempHidden.Add(0);
					}
				}

				// // NOT SURE IF CORRECT
				// if (NUM_HIDDEN == 0) {
				// 	tempHidden.Add(0);
				// }

				hiddenToHidden.Add(tempHidden.ToArray());
			}

			if (NUM_HIDDEN == 0) {
				List<float> tempHidden = new List<float>();
				hiddenToHidden.Add(tempHidden.ToArray());
			}	

			Debug.Log("Hidden to Hidden: "+listFloatArrayToString(hiddenToHidden));
		}


		// Creates hidden to output.
		public void createHiddenToOutput() {
			for (int i = 0; i < NUM_HIDDEN; i++) {
				List<float> tempOutput = new List<float>();
				for (int j = 0; j < NUM_OUTPUT; j++) {

					int[] currentTempArray = new int[] {hiddenIndexes[i], outputIndexes[j]};
					bool exists = false;
					foreach (var item in connectionMatrix) {
						// Debug.Log("Item: "+ intArrayToString(item));
						// Debug.Log("Temp array: "+ intArrayToString(currentTempArray));
						if ((item[0] == currentTempArray[0]) && item[1] == currentTempArray[1]) {
							// Debug.Log("WE HAVE A MATCH!!");
							exists = true;
						} else {
							// Debug.Log("NO MATCH");
						}
					}

					if (exists) {
						gene hidden = paramsGenome.arrayOfGenes[hiddenIndexes[i]];
						gene output = paramsGenome.arrayOfGenes[outputIndexes[j]];
						float strength = (float)(((hidden.velocity * hidden.travelTime) + (output.velocity * output.travelTime)) / 250.0);

						if ((hidden.angle + 180)%360 < output.angle) {
							strength = strength * -1;
						}
						tempOutput.Add(strength);

					} else {
						tempOutput.Add(0);
					}
				}

				// // NOT SURE IF CORRECT
				// if (NUM_OUTPUT == 0) {
				// 	tempOutput.Add(0);
				// }

				hiddenToOutput.Add(tempOutput.ToArray());
			}

			if (NUM_HIDDEN == 0) {
				List<float> tempOutput = new List<float>();
				hiddenToOutput.Add(tempOutput.ToArray());
			}

			Debug.Log("Hidden to Output: "+listFloatArrayToString(hiddenToOutput));
		}

		// Creates input to output.
		public void createInputToOutput() {
			for (int i = 0; i < NUM_INPUT; i++) {
				List<float> tempOutput = new List<float>();
				for (int j = 0; j < NUM_OUTPUT; j++) {

					int[] currentTempArray = new int[] {inputIndexes[i], outputIndexes[j]};
					bool exists = false;
					foreach (var item in connectionMatrix) {
						// Debug.Log("Item: "+ intArrayToString(item));
						// Debug.Log("Temp array: "+ intArrayToString(currentTempArray));
						if ((item[0] == currentTempArray[0]) && item[1] == currentTempArray[1]) {
							// Debug.Log("WE HAVE A MATCH!!");
							exists = true;
						} else {
							// Debug.Log("NO MATCH");
						}
					}


					if (exists) {
						gene sensor = paramsGenome.arrayOfGenes[inputIndexes[i]];
						gene output = paramsGenome.arrayOfGenes[outputIndexes[j]];
						float strength = (float)(((sensor.velocity * sensor.travelTime) + (output.velocity * output.travelTime)) / 250.0);

						if ((sensor.angle + 180)%360 < output.angle) {
							strength = strength * -1;
						}
						tempOutput.Add(strength);

					} else {
						tempOutput.Add(0);
					}
				}

				// // NOT SURE IF CORRECT
				// if (NUM_OUTPUT == 0) {
				// 	tempOutput.Add(0);
				// }

				inputToOutput.Add(tempOutput.ToArray());
			}

			if (NUM_INPUT == 0) {
				List<float> tempOutput = new List<float>();
				inputToOutput.Add(tempOutput.ToArray());
			}

			Debug.Log("Input to Output: "+listFloatArrayToString(inputToOutput));
		}


		// Creates output to hidden.
		public void createOutputToHidden() {
			for (int i = 0; i < NUM_OUTPUT; i++) {
				List<float> tempOutput = new List<float>();
				for (int j = 0; j < NUM_HIDDEN; j++) {

					int[] currentTempArray = new int[] {outputIndexes[i], hiddenIndexes[j]};
					bool exists = false;
					foreach (var item in connectionMatrix) {
						// Debug.Log("Item: "+ intArrayToString(item));
						// Debug.Log("Temp array: "+ intArrayToString(currentTempArray));
						if ((item[0] == currentTempArray[0]) && item[1] == currentTempArray[1]) {
							// Debug.Log("WE HAVE A MATCH!!");
							exists = true;
						} else {
							// Debug.Log("NO MATCH");
						}
					}

					if (exists) {
						gene output = paramsGenome.arrayOfGenes[outputIndexes[i]];
						gene hidden = paramsGenome.arrayOfGenes[hiddenIndexes[j]];
						float strength = (float)(((output.velocity * output.travelTime) + (hidden.velocity * hidden.travelTime)) / 250.0);
						if ((output.angle + 180)%360 < hidden.angle) {
							strength = strength * -1;
						}
						tempOutput.Add(strength);
					} else {
						tempOutput.Add(0);
					}
				}

				outputToHidden.Add(tempOutput.ToArray());
			}

			if (NUM_OUTPUT == 0) {
				List<float> tempOutput = new List<float>();
				outputToHidden.Add(tempOutput.ToArray());
			}

			Debug.Log("Ouput to Hidden: "+listFloatArrayToString(outputToHidden));
		}

		// Turns the generated lists into jagged arrays and then into 2D arrays such that they
		// may be fed into the neural network.
		public void finalToArray() {
			Debug.Log("IN TO OUT: "+listFloatArrayToString(inputToOutput));
			input_to_output = To2D(inputToOutput.ToArray());

			Debug.Log("IN TO HID: "+listFloatArrayToString(inputToHidden));
			input_to_hidden = To2D(inputToHidden.ToArray());

			Debug.Log("HID TO HID: "+listFloatArrayToString(hiddenToHidden));
			hidden_to_hidden = To2D(hiddenToHidden.ToArray());

			Debug.Log("HID TO OUT: "+listFloatArrayToString(hiddenToOutput));
			hidden_to_output = To2D(hiddenToOutput.ToArray());

			Debug.Log("OUT TO HID: "+listFloatArrayToString(outputToHidden));
			output_to_hidden = To2D(outputToHidden.ToArray());

			chosenSensorArray = senseToInput.ToArray();
			finalRMI = RMI.ToArray();
			finalLMI = LMI.ToArray();
		}


		// FUNCTION: To2D()
		// This function takes a jagged array[][] and transforms it into a 
		// 2Dimensional array[,]. 
		public T[,] To2D<T>(T[][] source) {
			// T[,] result = new T[source.Length, source.Max(x => x.Length)];
			// for(var i = 0; i < source.Length; i++){
			//     for(var j = 0; j < source[i].Length; j++){
			//         result[i, j] = source[i][j];
			//     }
			// }
			// return result;
			int FirstDim = source.Length;
			int SecondDim = source.GroupBy(row => row.Length).Single().Key;
				var result = new T[FirstDim, SecondDim];
				for (int i = 0; i < FirstDim; i++) {
					for (int j = 0; j < SecondDim; j++) {
						result[i,j] = source[i][j];
					}
				}
				return result;
		} 

	

		// FUNCTION: printParamsContents()
		// This function prints the paramters generated by the create 
		// params struct and its methods.
		public void printParamsContents() {
			string printString = "PARAMTERS CONTENTS: ";
			printString += "Input to output: " + print2Darrays(input_to_output) + ". ";
			printString += "Input to hidden: " + print2Darrays(input_to_hidden) + ". ";
			printString += "Hidden to hidden: " + print2Darrays(hidden_to_hidden) + ". ";
			printString += "Hidden to output: " + print2Darrays(hidden_to_output) + ". ";
			printString += "Output to hidden: " + print2Darrays(output_to_hidden) + ". ";
			string printString1 = "NODE NUMBERS: ";
			printString1 += "|| Num input: " + NUM_INPUT;
			printString1 += "|| Num hidden: " + NUM_HIDDEN;
			printString1 += "|| Num output: " + NUM_OUTPUT;
			Debug.Log(printString);
			Debug.Log(printString1);
		}
		public string listIntArrayToString(List<int[]> arr) {
			string specialString = "";
			foreach(var item in arr) {
				specialString += "||||";
				foreach (var thing in item) {
					specialString +=  thing.ToString() + ", ";
				}
			}
			return specialString;
		}
		public string intArrayToString(int[] arr) {
			string specialString = "";
			foreach (var item in arr) {
				specialString += item.ToString() + ", ";
			}
			return specialString;
		}
		public string listFloatArrayToString(List<float[]> arr) {
			string specialString = "";
			foreach(var item in arr) {
				specialString += "||||";
				foreach (var thing in item) {
					specialString +=  thing.ToString() + ", ";
				}
			}
			return specialString;
		}
		public string print2Darrays(float[,] arr) {
			int rowLength = arr.GetLength(0);
	        int colLength = arr.GetLength(1);
	        string outputString = "";
	        for (int i = 0; i < rowLength; i++) {
	            for (int j = 0; j < colLength; j++)	{
	                outputString += arr[i, j] + ", ";
	            } 
	        }
	        return outputString;
		}
	}

	////// STRUCT: individual
	// 	This structure represents an individual in a generation. 
	//  
	// 	Functions:
	// 		createIndividualStructs() : instantiates the structs for an individual.
	public struct individual {
		System.Random rand;
		public genome genomeCollection;
		public genomeToPhenotype GtoPCollection;
		public createParams paramsCollection;
		public NeuralNetworkHandler.NeuralNetworkParameters neuralNetCollection;

		float fitnessScore;

		int numberOfGenes;
		int meanNumberOfGenes;
		int maxSpawn;
		int vMax;
		int vDurationMin;
		int vDurationMax;
		int gMax;
		int gDurationMin;
		int gDurationMax;
		float dupeRate;
		float muteRate;
		float delRate;
		float changePercent;

		// FUNCTION: createIndividualStructs() 
		// This function instantiates all of the structs each individual needs.
		// A genome, G->P process, network parameters, and a neural network.
		public void createIndividualStructs() {
			fitnessScore = 0;
			genomeCollection = new genome();
			GtoPCollection = new genomeToPhenotype();
			paramsCollection = new createParams();
			neuralNetCollection = new NeuralNetworkHandler.NeuralNetworkParameters();
		}
	}

	////// STRUCT: generation
	// 	This structure represents a single generation, or a collection of genomes (individuals).
	//
	// 	Functions:
	// 		setGenerationParameters() : specifies number of genes.
	// 			INPUT: (int meanNumberOfGenes, int meanStandardDeviationOfGenes, int numberOfIndividualsInGeneration)
	//			OUTPUT: assigns the generation's parameters.
	//		createStartGeneration() : creates a generation given the parameters set above.
	// 		createNumberOfGenes() : creates the number of genes for a given genome in the generation.
	public struct generation {
		System.Random rand;

		// A generation is a set of individuals.
		public individual[] collectionOfIndividuals;

		public int numberOfIndividualsInGeneration;
		public int numberOfGenes;
		public int meanStandardDeviationOfGenes;

		public int individualIndex;

		bool done;

		// FUNCTION: setGenerationParamters()
		// This function sets a generations mean/sd regarding number of genes in each genome/individual of the generation.
		// It also determines teh number of individuals in the generation.
		public void setGenerationParameters(int meanNumberOfG, int meanStandardDeviationOfG, int numberOfInd) {
			rand = new System.Random(); 
			// numberOfGenes = meanNumberOfG;
			meanStandardDeviationOfGenes = meanStandardDeviationOfG;
			numberOfIndividualsInGeneration = numberOfInd;
			collectionOfIndividuals = new individual[numberOfInd];
			individualIndex = 0;
			done = false;
		}

		// FUNCTION: createStartGeneration()
		// This function creates a start generation the size of which is determined in 
		// the function setGenerationParameters().
		public void createStartGeneration() {
	
		    int generationLength = collectionOfIndividuals.Length;
		    Debug.Log("Number of individuals in a generation: " + generationLength);

		    // Instantiates all of the individuals in the generation.
			for (int i = 0; i < generationLength; i++) {
				collectionOfIndividuals[i].createIndividualStructs();
			}

			// Creates genome, runs G->P process, creates params, and creates a neural network 
			// for each individual in the generation.
			for (int i = 0; i < generationLength; i++) {

				int maxSpawn = 100;
				int vMax = 5;
				int vDurationMin = 1;
				int vDurationMax = 100;
				int gMax = 3;
				int gDurationMin = 1;
				int gDurationMax = 100;

				numberOfGenes = 20;

				float dupeRate = 0.5f;
				float muteRate = 0.05f;
				float delRate = 0.01f;
				float changePercent = 0.15f;

				// Creates a genome for each individual.
				collectionOfIndividuals[i].genomeCollection.createRandomFunction();
				collectionOfIndividuals[i].genomeCollection.setGenomeParameters(numberOfGenes, dupeRate, muteRate, delRate, changePercent);
				collectionOfIndividuals[i].genomeCollection.createWholeGenome(maxSpawn, vMax, vDurationMin, vDurationMax, gMax, gDurationMin, gDurationMax);
				collectionOfIndividuals[i].genomeCollection.printGenomeContents();

				// Runs the G->P process on each individual's genome.
				collectionOfIndividuals[i].GtoPCollection.passGenome(collectionOfIndividuals[i].genomeCollection);
				collectionOfIndividuals[i].GtoPCollection.runDevoGraphics();
				collectionOfIndividuals[i].GtoPCollection.makeConnectome();
				collectionOfIndividuals[i].GtoPCollection.printConnectomeContents();

				// Creates neural net parameters for each individual.
				collectionOfIndividuals[i].paramsCollection.passConnectionMatrix(collectionOfIndividuals[i].GtoPCollection.sortedConnects, collectionOfIndividuals[i].genomeCollection);
				collectionOfIndividuals[i].paramsCollection.setNodeLayerNumbers();
				collectionOfIndividuals[i].paramsCollection.motorIndexes();
				collectionOfIndividuals[i].paramsCollection.sensorToInputs();
				collectionOfIndividuals[i].paramsCollection.createInputToHidden();
				collectionOfIndividuals[i].paramsCollection.createHiddenToHidden();
				collectionOfIndividuals[i].paramsCollection.createHiddenToOutput();
				collectionOfIndividuals[i].paramsCollection.createInputToOutput();
				collectionOfIndividuals[i].paramsCollection.createOutputToHidden();
				collectionOfIndividuals[i].paramsCollection.finalToArray();

				// Creates a neural network for each individual.
				collectionOfIndividuals[i].neuralNetCollection.setStartVariables(collectionOfIndividuals[i].paramsCollection.RMIlength, collectionOfIndividuals[i].paramsCollection.LMIlength, collectionOfIndividuals[i].paramsCollection.NUM_INPUT, collectionOfIndividuals[i].paramsCollection.NUM_HIDDEN, collectionOfIndividuals[i].paramsCollection.NUM_OUTPUT);
				collectionOfIndividuals[i].neuralNetCollection.setStartingArrays(collectionOfIndividuals[i].paramsCollection.finalRMI, collectionOfIndividuals[i].paramsCollection.finalLMI);
				collectionOfIndividuals[i].neuralNetCollection.setConnections(collectionOfIndividuals[i].paramsCollection.input_to_output, collectionOfIndividuals[i].paramsCollection.input_to_hidden, collectionOfIndividuals[i].paramsCollection.hidden_to_hidden, collectionOfIndividuals[i].paramsCollection.hidden_to_output, collectionOfIndividuals[i].paramsCollection.output_to_hidden);
			}
		}

		


		// FUNCTION: nextIndividual() 
		// This function iterates the individualIndex number as long as it is 
		// less than the number of individuals in the generation.
		public void nextIndividual() {
			int collectionLength = collectionOfIndividuals.Length;
			if (individualIndex < collectionLength) {
				individualIndex = individualIndex + 1;
			} else {
				Debug.Log("ENTIRE GENERATION HAS BEEN RUN");
				done = true;
			}
		}


		// FUNCTION: runNeuralNetOnIndividual() 
		// This function runs the neural net of an individual in the generation.
		public void runNeuralNetOnIndividual(float[] theLDRarray, float[] theIRarray) {
			float[] currentLDRdataArray = theLDRarray;
			float[] currentIRdataArray = theIRarray;
			collectionOfIndividuals[individualIndex].neuralNetCollection.beginNeuralNet(currentLDRdataArray, currentIRdataArray, collectionOfIndividuals[individualIndex].paramsCollection.chosenSensorArray);
			collectionOfIndividuals[individualIndex].neuralNetCollection.updateMotorValues();
		}

		// FUNCTION: mutateAndDuplicateGeneration()
		// This function will mutate and duplicate all of the genomes/individuals in a generation.
		public void mutateAndDuplicateGeneration() {
			int generationLength = collectionOfIndividuals.Length;
			for (int i = 0; i < generationLength; i++) {
				collectionOfIndividuals[i].genomeCollection.duplicateAndDelete();
				collectionOfIndividuals[i].genomeCollection.mutate();
			}
		}

		// FUNCTION: childrenAndNextGeneration()
		// Calculates the number of children each individual in the generation gets 
		// to pass on to the next generation.
		public void childrenAndNextGeneration() {

		}

		// HELPER FUNCTIONS.
		// public int normalizeRandom(int minVal, int maxVal) {
		// 	int mean = (minVal  + maxVal) /2;
		// 	int sigma = (maxVal = mean) / 3;
		// 	return rand.Next(mean, sigma);
		// }

		// public int createNumberOfGenes() {
		// 	numberOfGenes = normalizeRandom(4,16);
		// 	return numberOfGenes;
		// }
	}


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {


	}

}
