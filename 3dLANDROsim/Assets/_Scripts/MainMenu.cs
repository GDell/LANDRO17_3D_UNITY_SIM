using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour {
	// INPUT FIELDS IN MAIN MENUS.
	public InputField numIndInput;
	public InputField avgGeneInput;
	public InputField numGenerations;
	// VARIABLES TO STORE USER INPUT.
	public static int INPUTnumberOfIndividuals;
	public static int INPUTavgGenomeSize;
	public static int INPUTnumberOfGenerations;
	// INITIALIZE THE DEFAULT VALUES.
	void Start () {
		// DEFAULT VALUES
		INPUTnumberOfGenerations = 20;
		INPUTavgGenomeSize = 10;
		INPUTnumberOfIndividuals = 10;
	}
	// FUNCTION: startSimulation()
	// This function is called by the start button in the main menu.
	// It grabs the user input in the menu's input fields and assigns them
	// to public variables such that they may be references in other scripts.
	public void StartSimulation() {
		// Assign input to storage variables.
		INPUTnumberOfIndividuals = int.Parse(numIndInput.text);
		INPUTavgGenomeSize = int.Parse(avgGeneInput.text);
		INPUTnumberOfGenerations = int.Parse(numGenerations.text);
		// LOAD THE EXPERIMENT.
		SceneManager.LoadScene("Experiment");
		Main.pastStartScreen = true;
	}
	public void QuitSimulation() {
		Application.Quit();
	}
}
