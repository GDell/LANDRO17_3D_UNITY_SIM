using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

	public InputField numIndInput;
	public InputField avgGeneInput;
	public InputField numGenerations;
	// // public InputField numIndInput = GameObject.Find("Number_of_Individuals").GetComponent<InputField>();


	public int INPUTnumberOfIndividuals;
	public int INPUTavgGenomeSize;
	public int INPUTnumberOfGenerations;
	// Use this for initialization
	void Start () {
		INPUTnumberOfGenerations = 20;
		INPUTavgGenomeSize = 10;
		INPUTnumberOfIndividuals = 10;
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void StartSimulation() {

		INPUTnumberOfIndividuals = int.Parse(numIndInput.text);
		INPUTavgGenomeSize = int.Parse(avgGeneInput.text);
		INPUTnumberOfGenerations = int.Parse(numGenerations.text);


		// if (avgGeneInput.Text == null) {
		// 	INPUTavgGenomeSize = 10;
		// 	Debug.Log("Average genome size set to default 10");
		// } else {
		// 	INPUTavgGenomeSize = Int32.TryParse(avgGeneInput.text);
		// }

		// if (numIndInput.Text == null) {
		// 	INPUTnumberOfIndividuals = 20;
		// 	Debug.Log("Average generation size set to default 20");
		// } else {
		// 	INPUTnumberOfIndividuals = Int32.TryParse(numIndInput.text);
		// }


		// if (numGenerations.Text == null) {
		// 	INPUTnumberOfGenerations = 20;
		// 	Debug.Log("Number of generations set to default 10");
		// } else {
		// 	INPUTnumberOfGenerations = Int32.TryParse(numIndInput.text);
		// }

		SceneManager.LoadScene("Experiment");
		Main.pastStartScreen = true;
	}
	public void QuitSimulation() {
		Application.Quit();
	}
}
