# **LANDRO17_3D_UNITY_SIM**
The following is a 3D Unity simulation for an evolving robotics reaserch project (Vassar URSI 2017).
This project simulates Landro and the XOR developmental/evolutionary task.

This project uses the Unity 5.6.1 game engine, you will need it downloaded in order to run simulations.

Reach me at: gadellaccio@vassar.edu

## Finishing the Simulation:
#### Closing the reality gap
- Sensor readings need to be calibrated and scaled to match that of physical Landro's. We need to determine at what length from the light (inluding effects of orientation) that light sensors recieve zero input.
- Need to double check that everything is to scale. Length and width of the arena needs to be altered slightly.
- Bump sensor reactions need to be calibrateed to match those of physical Landro. (Change movement time and the motor torque inputs in SimpleCarController script)
- Create a function to randomly place Landro at the start of trials.
- G-->P process needs to be slightly adapted to incorporate the new ranges for velocity and growth duration, in addition to the growth start variable.

#### Improving the speed of simulations
- Further, we need to implement a mode in which we can run the simulations without 3D rendering turned on. 
- With 3D rendering turned off, we need to implement a way in which to run multiple individuals in a generation at once.
- Finalize the fitness function (in main script).

#### Coding Practices
- Create a trial stucture to manage trial and generation information. 

## Important Functions and Structs by File Name:
**genomeHandler**: This file manages creating genomes, the G->P process, and creating generations of individuals.
- gene 
- genome
- genotypeToPhenotype
- createParams
- individual
- generation

**IR**: This file/class is attached to all the IR sensors on Landro's body and assigns sensor readings.

- onTriggerStay() - This function is constantly checking to see whether an IR has collided with (sensed) a wall. If it does, the script assinges a sensor reading to that IR by taking the distance from said wall and said IR variable. 

*NOTE*: This file should be updated such that IR sensors are assigned readings even if they aren't recieving enough input to reach the IR threshold.

**LDR**: This file/class is attached to all the LDR sensors on Landro's body. It is constantly computing the distance between the light source and all of Landro's LDR sensors and using that to assign sensor reading values. Currently, LDR sensors are assigned sensor readings above 0 only if the distance reaches the LDR distance threshold.

*NOTE*: This file should be updated such that LDR sensors are assigned readings even if the LDR threshold has not been reached. We need to figure out at what point/distance LDR readings read zero light. 


**SimpleCarController**: Controlling Simulated Landro and gathering sensor data.
- runMotors(): A function that runs the motors provided left and right motor torque values.
- irScale(): Scales gathered IR data.
- photoScale(): Scales gathered photo data.

**NeuralNetworkHandler**: Contains the neuralNetworkParamters structure which creates an instance of a neural network provided certain parameters.
- setStartVariables() - sets the starting node layers provided parameter inputs. 
- setStartingArrays() - creates the arrays that will hold connection weights between each layer of the network.
- setConnections() - assignes the correct weights to the arrays created in setStartingArrays()
- beginNeuralNet() - provided arrays of LDR and IR sensor reading data in addition to an array of the active sensors, runs the neural network.
- updateMotorValues() - calls the runMotors() function in SimpleCarController using the neural network output to power the motors.
- activation() - used to apply the Tanh function.

**MainMenu**: This file manages the main menu
	

**Main**: This file is constantly active while the simulation is running. It manages trials through creating generations and reseting the simulation between trials and generation runs. It also manages assigning and computing fitness scores for individuals of generations.
	
- beginRun() - Takes the ldrData and irData arrays from the SimpleCarController script and runs the neural net on the current individual of the current generation.

- evaluateTrialFitness() - This function finds the mean IR and LDR sensor values at the current step in the trial to determine whether or not the robot is in an XOR space.

- finalFitnessCalculation() - This function assings a final fitness to the current individual of the current generation.

- reset() - This function resets Landro and the environment and iterates to the next individual of the generation.

- singleIndividualTest() - This function tests the genome creation --> G->P development process --> Params creation --> Neural Network Creation process. Rather than creating a whole generation it creates each struct present in an individual to test that the process is working.
	
Adjust irThreshold and ldrThreshold values in this script in order to change the percentages of "good XOR" and "bad XOR" areas in the simulated environment. (May also need to adjust these threshold values in the LDR and IR scripts)


