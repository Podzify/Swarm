﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour {

	[Header("References")]
	public GameObject bugPrefab;
	public GameObject attractorPrefab;

	[Header("Variables")]
	public int maxBugs = 100;
	public int startBugs = 50;
	public float speedMin = 1f;				// Minimum bug speed
	public float speedMax = 50f;			// Maximum bug speed
	public float turnMin = 0f;				// Shortest time before turn
	public float turnMax = 5f;				// Longest time before turn
	public float distanceMax = 10;			// Further away objects start approaching the Attractor
	public float attractorVolume = 10;		// Variation for Attractor direction
	public float attractorThreshold = 100;	// Percentage of chance for object too far away to approach the Attractor
	public float colliderScale = 1;			// Used to scale the bug collider for paranoia level
	public float averageLifespan = 10;		// Average lifespan in seconds

	private Slider sliderBugsCount;

	// Bugs pool

	private List<GameObject> bugsPool;
	private int requestedBugs;

	void Awake() {

		// Create the Attractor
		GameObject attractor = Instantiate (attractorPrefab, Vector3.zero, Quaternion.identity);
		attractor.name = "Attractor";
		attractor.tag = "Attractor";

		// Create the Bugs empty parent object
		GameObject bugsRoot = new GameObject ();
		bugsRoot.name = "Bugs";

		// Create the Bugs object pool
		bugsPool = new List<GameObject> ();
		for (int i = 0; i < maxBugs; i++) {
			Vector3 pos = Utilities.randomVectorInRange (50);
			bugsPool.Add(Instantiate(bugPrefab, pos, Quaternion.identity));
			BugController bc = bugsPool [i].GetComponent<BugController> ();
			bc.gender = (BugController.Gender)Random.Range(0, 2);
			bugsPool [i].name = "Bug " + i + " (" + bc.gender + ")";
			bugsPool [i].transform.parent = bugsRoot.transform;
			bc.AddAttractor(attractor);
			bugsPool [i].SetActive (false);
		}

		// Setup the population slider min, max and default
		sliderBugsCount = GameObject.Find ("SliderBugsCount").GetComponent<Slider> ();
		sliderBugsCount.maxValue = (float)maxBugs;
		sliderBugsCount.minValue = 0f;
		sliderBugsCount.value = startBugs;
		requestedBugs = (int)sliderBugsCount.value;
		Debug.Log ("-- END MAIN AWAKE --");
		Debug.Log ("Requested bugs = " + requestedBugs);
	}

	void Start() {

		// Spawn initial bugs from the pool
		AddRemoveBugs(requestedBugs);
		Debug.Log ("-- END MAIN START --");
	}


	// Get the requested bug count from the UI Slider
	public void SliderBugsCount(float newValue) {
		requestedBugs = (int)newValue;
	}


	void Update() {
//		if (requestedBugs != BugController.countActive) {
//			Debug.Log ("Requested (" + requestedBugs + ") differs from active (" + BugController.countActive + "), adjusting...");
//			AddRemoveBugs (requestedBugs - BugController.countActive);
//		}

		// End if all bugs are dead
		if (BugController.countActive == 0) {
			Debug.Log ("All bugs have died. Terminating.");
			Debug.Break ();
		}
	}


	// Get a positive or negative number and adjust the number of active bugs in the pool
	void AddRemoveBugs(int quantity) {
		int targetNumberOfBugs = BugController.countActive + quantity;
		while(BugController.countActive != targetNumberOfBugs) {
			if (BugController.countActive < targetNumberOfBugs) {
				bugsPool [BugController.countActive].SetActive (true);
			}
			else {
				bugsPool [BugController.countActive].SetActive (false);
			}
			Debug.Log ("Added/Removed, Target is " + targetNumberOfBugs + ", Now active is " + BugController.countActive);
		}
		Debug.Log ("AddRemove finished. Active bugs : " + BugController.countActive);
	}

	public void Encounter(GameObject obj1, GameObject obj2) {
		if (obj2.name == "Sphere") {
			BugController.countEncountersWithLight++;
		} else {
			BugController.countEncounters++;
		}
	}

	public void Death(GameObject obj) {
		obj.SetActive (false);
		Debug.Log (obj.name + " has died. Death count is now " + BugController.countDeaths);
		obj.name = obj.name + " (Dead) ";
	}
}
