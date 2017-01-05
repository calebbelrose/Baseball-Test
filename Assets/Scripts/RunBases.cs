using UnityEngine;
using System.Collections;

public class RunBases : MonoBehaviour {
	public GameObject[] bases = new GameObject[4];  // The bases on the field
	NavMeshAgent agent;                             // The NavMeshAgent for the player that will be running around the bases
	int currBase = -1;                              // The current base that the player is running to
	bool rightHanded;                               // Whether the player is a right- or left-handed batter

	// Use this for initialization
	void Start ()
	{
		agent = GetComponent<NavMeshAgent> ();              // Gets the NavMeshAgent for the player

        // Random chance that the player is right- or left-handed
		if (Random.value < 0.5f)
			rightHanded = true;
		else
			rightHanded = false;

        // Sets the location of the player while batting based on if they're right- or left-handed
		if (rightHanded)                                                    
			gameObject.transform.position = new Vector3 (4.7f, 0.0f, 4.3f);
		else
			gameObject.transform.position = new Vector3 (4.3f, 0.0f, 4.7f);
	}
	
	// Update is called once per frame
	void Update ()
	{
        // If the player reaches home plate, the player stops running
		if (currBase == 3)
		{
			if (agent.remainingDistance == 0.0f)
				gameObject.SetActive (false);
		}
        // If the player reaches any other base, the player starts running to the next base
		else if(agent.remainingDistance < 0.1f)
            agent.destination = bases [++currBase].transform.position;;
	}
}