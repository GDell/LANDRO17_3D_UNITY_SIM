# LANDRO17_3D_UNITY_SIM
The following is a 3D Unity simulation for an evolving robotics reaserch project (Vassar URSI 2017).
This project simulates Landro and the XOR developmental/evolutionary task.

This project uses the Unity 5.6.1 game engine, you will need it downloaded in order to run simulations.

Important Functions and Structs by File Name

genomeHandler: This file manages creating genomes, the G->P process, and creating generations of individuals.
	- gene 
	- genome
	- genotypeToPhenotype
	- createParams
	- individual
	- generation

IR: This file/class is attached to all the IR sensors on Landro's body and assigns sensor readings.

	onTriggerStay() - This function is constantly checking to see whether an IR has collided with (sensed) a wall. If it does, the script assinges a sensor reading to that IR by taking the distance from said wall and said IR variable. 

	NOTE: This file should be updated such that IR sensors are assigned readings even if they aren't recieving enough input to reach the IR threshold.

LDR: This file/class is attached to all the LDR sensors on Landro's body. It is constantly computing the distance between the light source and all of Landro's LDR sensors and using that to assign sensor reading values. Currently, LDR sensors are assigned sensor readings above 0 only if the distance reaches the LDR distance threshold.

	NOTE: This file should be updated such that LDR sensors are assigned readings even if the LDR threshold has not been reached. We need to figure out at what point/distance LDR readings read zero light. 


SimpleCarController: Controlling Simulated Landro and gathering sensor data.
	runMotors(): 

NeuralNetworkHandler:

MainMenu: 
	

Main: This file is constantly active while the simulation is running. It manages trials through creating generations and reseting the simulation between trials and generation runs. It also manages assigning and computing fitness scores for individuals of generations.
	
	beginRun() - Takes the ldrData and irData arrays from the SimpleCarController script and runs the neural net on the current individual of the current generation.

	evaluateTrialFitness() - This function finds the mean IR and LDR sensor values at the current step in the trial to determine whether or not the robot is in an XOR space.

	finalFitnessCalculation() - This function assings a final fitness to the current individual of the current generation.

	reset() - This function resets Landro and the environment and iterates to the next individual of the generation.

	singleIndividualTest() - This function tests the genome creation --> G->P development process --> Params creation --> Neural Network Creation process. Rather than creating a whole generation it creates each struct present in an individual to test that the process is working.
		
	Adjust irThreshold and ldrThreshold values in this script in order to change the percentages of "good XOR" and "bad XOR" areas in the simulated environment. (May also need to adjust these threshold values in the LDR and IR scripts)


gadellaccio@vassar.edu
