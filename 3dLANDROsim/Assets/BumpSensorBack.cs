using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumpSensorBack : MonoBehaviour {

	public bool bumpWall = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

			
	}

	void OnTriggerStay(Collider source){
		
		//if(!source.name.Contains("L16A")){
			//print(this.name + " hit " + source.name);
			if (source.name.Contains ("Wall")) {
				bumpWall = true;
				// print(irScore);
				// print(this + " IR SCORE iS: " + irScore);
				// print ("HIT THE WALL");
			} 


		//}
	}
	
}
