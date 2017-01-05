using UnityEngine;
using System.Collections;

public class BallMovement : MonoBehaviour
{
    float xDistance, yDistance, zDistance, slope;       // Used to determine how far the ball will move each frame
    public GameObject[] fielders = new GameObject[9];   // Every fielder on the field, including the pitcher
	GameObject thisFielder;                             // The fielder that will get to the ball the fastest after it is hit                                

	// Use this for initialization
	void Start ()
    {
        // There will be more here in the future, such as pickoffs and pitches including strikeouts and walks
		Hit ();                                         // Hit the ball
	}
	
	// Update is called once per frame
	void Update ()
	{
        // Moves the ball based on whether it is in the air or on the ground
		if (gameObject.transform.position.y > 0.02f)
			gameObject.transform.Translate (xDistance, yDistance, zDistance);
		else
            gameObject.transform.Translate (xDistance, 0.0f, zDistance);

        // Slows the ball down if it's moving in the x direction
		if (xDistance < 0.0f)
		{
			xDistance += 0.0000981f;
			if (xDistance > 0.0f)
				xDistance = 0.0f;
		}
		else if (xDistance > 0.0f)
		{
			Debug.Log (xDistance);
			xDistance -= 0.0000981f;
			if (xDistance < 0.0f)
				xDistance = 0.0f;
		}

        // Makes the ball drop to the ground faster if it is not yet on the ground
		if (gameObject.transform.position.y > 0.02f)
			yDistance -= 0.0004905f;

        // Slows the ball down if it's moving in the z direction
		if (zDistance < 0.0f)
		{
			zDistance += 0.0000981f * slope;
			if (zDistance > 0.0f)
				zDistance = 0.0f;
		}
		else if (zDistance > 0.0f)
		{
			Debug.Log (zDistance);
			zDistance -= 0.0000981f * slope;
			if (zDistance < 0.0f)
				zDistance = 0.0f;
		}
	}

	void Hit()
	{
        int shortestElement = 0, shortestTime = int.MaxValue;                       // The time it takes for the closest fielder to get to the ball and their element in the fielders array
        float xIntersect = 0.0f, zIntersect = 0.0f;                                 // The location that the fielder reaches the ball
        Vector3 targetPosition1 = gameObject.transform.position, targetPosition2;   // The locations of where the ball will hit the ground and where the ball will stop moving
        float tempXDistance, tempYDistance, tempZDistance;                          // Temporary distances to determine where the ball will hit the ground and where it will stop rolling

        // Experimenting with different values 
		/*height = Random.value * 2;
		distance = height - 0.1f;
		xDistance = Random.value /-10;
		zDistance = Random.value /-10;
		/*xDistance = Random.value /5 - 0.1f;
		zDistance = Random.value /5 - 0.1f;
		height = 1.0F;
		distance = height - 0.1f;
		xDistance = -0.05f;
		zDistance = -0.05f;
        xDistance = 0.06f - Random.value * 0.12f;
        zDistance = 0.06f - Random.value * 0.12f;*/

		yDistance = Random.value / 10.0f - 0.04f;                                   // How fast the ball will rise into the air
		xDistance = Random.value * -0.06f;                                          // How fast the ball will move in the x direction
		zDistance = Random.value * -0.06f;                                          // How fast the ball will move in the z Direction

        // Determines the slope of the movement of the ball to be able to determine how much to change the zDistance later
		if (xDistance != 0.0f)                      
			slope = zDistance / xDistance;
		else
			slope = 1f;

        // Sets the temporary distances
        tempXDistance = xDistance;
        tempYDistance = yDistance;
        tempZDistance = zDistance;

        // Moves the target position along the ball's path to determine where it will hit the ground
		while (targetPosition1.y > 0.02f)
		{
			targetPosition1 = new Vector3 (targetPosition1.x + tempXDistance, targetPosition1.y + tempYDistance, targetPosition1.z + tempZDistance);

			if (tempXDistance < 0.0f)
			{
				tempXDistance += 0.0000981f;
				if (tempXDistance > 0.0f)
					tempXDistance = 0.0f;
			}

			if (gameObject.transform.position.y > 0.02f)
				tempYDistance -= 0.0004905f;

			if (tempZDistance < 0.0f)
			{
				tempZDistance += 0.0000981f * slope;
				if (tempZDistance > 0.0f)
					tempZDistance = 0.0f;
			}
		}

		targetPosition2 = targetPosition1;                                          // Starts the target position off where the ball landed

        // Moves the target position to where the ball stops rolling
		while (tempXDistance > 0.0f && tempZDistance > 0.0f)
		{
			targetPosition2 = new Vector3 (targetPosition2.x + tempXDistance, 0.02f, targetPosition2.z + tempZDistance);

			if (tempXDistance < 0.0f)
			{
				tempXDistance += 0.0000981f;
				if (tempXDistance > 0.0f)
					tempXDistance = 0.0f;
			}

			if (tempZDistance < 0.0f)
			{
				tempZDistance += 0.0000981f * slope;
				if (tempZDistance > 0.0f)
					tempZDistance = 0.0f;
			}
		}

        // Goes through each fielder and determines how long it will take for each one to reach the ball after it is hit
		for (int i = 0; i < fielders.Length; i++)
		{
			int thisTime = 0;                   
            Vector3 playerPosition1, playerPosition2, playerPosition3;              // The starting location of the current fielder for each target position
            Vector3 targetPosition3;                                                // Target position that is the shortest path from the fielder to the path that the ball is moving
            Vector3 tempBallPosition = gameObject.transform.position;               // Temporary position of the ball, used to determine if the fielder is in range to catch it
            float targetX,                                                          // x-coordinate for the third target position
            thisSlope, b,                                                           // Slope and b used to calculate the x- and z-coordinate of the third target position
            numerator1, numerator2, denominator,                                    // numerators and denominator used to calculate 1 unit of the fielder's movement towards each target position
            xDistance1, xDistance2, xDistance3, zDistance1, zDistance2, zDistance3, // The amount of x and z movement of the fielder to each target position
            distance = 1.0f, distance1 = 1.0f, distance2 = 1.0f, distance3 = 1.0f;  // The distance to the ball and the fielder's distance to each target position
            bool intersecting1, intersecting2, intersecting3;                       // Whether the fielder is able to catch the ball at each target position

            thisSlope = -1 / slope;
            b = -thisSlope * fielders[i].transform.position.x + fielders[i].transform.position.z;

            // Sets the starting location of the current fielder for each target position
            playerPosition1 = (playerPosition2 = (playerPosition3 = fielders[i].transform.position));

            // Determines whether the fielder can currently catch the ball
            intersecting1 = (intersecting2 = (intersecting3 =  Mathf.Sqrt(Mathf.Pow(tempBallPosition.x - playerPosition1.x, 2.0f) + Mathf.Pow(tempBallPosition.z - playerPosition1.z, 2.0f)) <= 0.022f && tempBallPosition.y <= 0.2f));

			// Calculates the x- and z-coordinates of the third target position
			targetX = (- 4.5f * slope + 4.5f - b) / (thisSlope - slope);                                                                                                    // Calculates the x-position of the third target position
			targetPosition3 = new Vector3 (targetX, 0.02f, thisSlope * targetX + b);                                                                                        // Se

			// Calculates the x and z distances to move 1 unit closer to the first target position
			numerator1 = targetPosition1.x - playerPosition1.x;
			numerator2 = targetPosition1.z - playerPosition1.z;
			denominator = Mathf.Abs((float)System.Math.Sqrt(numerator1 * numerator1 + numerator2 * numerator2));

			if (denominator == 0.0f)
			{
				xDistance1 = 0.0f;
				zDistance1 = 0.0f;
			}
			else
			{
				xDistance1 = 0.04f * numerator1 / denominator;
				zDistance1 = 0.04f * numerator2 / denominator;
			}

			// Calculates the x and z distances to move 1 unit closer to the second target position
			numerator1 = targetPosition2.x - playerPosition2.x;
			numerator2 = targetPosition2.z - playerPosition2.z;
			denominator = Mathf.Abs((float)System.Math.Sqrt(numerator1 * numerator1 + numerator2 * numerator2));

			if (denominator == 0.0f)
			{
				xDistance2 = 0.0f;
				zDistance2 = 0.0f;
			}
			else
			{
				xDistance2 = 0.04f * numerator1 / denominator;
				zDistance2 = 0.04f * numerator2 / denominator;
			}

			// Calculates the x and z distances to move 1 unit closer to the third target position
			numerator1 = targetPosition3.x - playerPosition3.x;
			numerator2 = targetPosition3.z - playerPosition3.z;
			denominator = Mathf.Abs((float)System.Math.Sqrt(numerator1 * numerator1 + numerator2 * numerator2));

			if (denominator == 0.0f)
			{
				xDistance3 = 0.0f;
				zDistance3 = 0.0f;
			}
			else
			{
				xDistance3 = 0.04f * numerator1 / denominator;
				zDistance3 = 0.04f * numerator2 / denominator;
			}


			// Loops until the fielder is close enough to catch the ball
			//while (thisTime < shortestTime && !(intersecting1 || intersecting2 || intersecting3))
			for(int q = 0; q < 1000; q++)
			{
				// Moves the ball based on whether it's in the air or on the ground
                if (tempBallPosition.y > 0.02f)
                    tempBallPosition = new Vector3( tempBallPosition.x + tempXDistance, tempBallPosition.y + tempYDistance, tempBallPosition.z + tempZDistance);
				else
                    tempBallPosition = new Vector3( tempBallPosition.x + tempXDistance, 0.02f, tempBallPosition.z + tempZDistance);

				// If the ball hits a wall, it bounces
                if (tempBallPosition.x <= -4.8f)
					xDistance = -xDistance / 2;

				// If the ball hits a wall, it bounces
                if (tempBallPosition.z <= -4.8f)
					zDistance = -zDistance / 2;

				// Slows down the speed of the ball in the x direction
				if (tempXDistance < 0.0f)
				{
					tempXDistance += 0.0000981f;
					if (tempXDistance > 0.0f)
						tempXDistance = 0.0f;
				}
				else if (tempXDistance > 0.0f)
				{
					tempXDistance -= 0.0000981f;
					if (tempXDistance < 0.0f)
						tempXDistance = 0.0f;
				}

				// Makes the ball drop to the ground faster
                if (tempBallPosition.y > 0.02f)
					tempYDistance -= 0.0004905f;

				// Slows down the speed of the ball in the z direction
				if (tempZDistance < 0.0f)
				{
					tempZDistance += 0.0000981f * slope;
					if (tempZDistance > 0.0f)
						tempZDistance = 0.0f;
				}
				else if (tempZDistance > 0.0f)
				{
					tempZDistance -= 0.0000981f * slope;
					if (tempZDistance < 0.0f)
						tempZDistance = 0.0f;
				}

				// Moves the player closer to the first target position and determines whether the player can catch the ball
				if (!intersecting1)
				{
					if (Mathf.Sqrt (Mathf.Pow (targetPosition1.x - playerPosition1.x, 2.0f) + Mathf.Pow (targetPosition1.z - playerPosition1.z, 2.0f)) <= 0.04f)
						playerPosition1 = targetPosition1;
					else
						playerPosition1 = new Vector3 (playerPosition1.x + xDistance1, playerPosition1.y, playerPosition1.z + zDistance1);

                    distance1 = Mathf.Sqrt (Mathf.Pow (tempBallPosition.x - playerPosition1.x, 2.0f) + Mathf.Pow (tempBallPosition.z - playerPosition1.z, 2.0f));

                    intersecting1 = distance1 <= 0.042f && tempBallPosition.y <= 0.2f;
				}

				// Moves the player closer to the second target position and determines whether the player can catch the ball
				if (!intersecting2)
				{
					if (Mathf.Sqrt (Mathf.Pow (targetPosition2.x - playerPosition2.x, 2.0f) + Mathf.Pow (targetPosition2.z - playerPosition2.z, 2.0f)) <= 0.04f)
						playerPosition2 = targetPosition2;
					else
						playerPosition2 = new Vector3 (playerPosition2.x + xDistance2, playerPosition2.y, playerPosition2.z + zDistance2);

                    distance2 = Mathf.Sqrt (Mathf.Pow (tempBallPosition.x - playerPosition2.x, 2.0f) + Mathf.Pow (tempBallPosition.z - playerPosition2.z, 2.0f));

                    intersecting2 = distance2 <= 0.042f && tempBallPosition.y <= 0.2f;
				}

				// Moves the player closer to the third target position and determines whether the player can catch the ball
				if (!intersecting3)
				{
					if (Mathf.Sqrt (Mathf.Pow (targetPosition3.x - playerPosition3.x, 2.0f) + Mathf.Pow (targetPosition3.z - playerPosition3.z, 2.0f)) <= 0.04f)
						playerPosition3 = targetPosition3;
					else
						playerPosition3 = new Vector3 (playerPosition3.x + xDistance3, playerPosition3.y, playerPosition3.z + zDistance3);

                    distance3 = Mathf.Sqrt (Mathf.Pow (tempBallPosition.x - playerPosition3.x, 2.0f) + Mathf.Pow (tempBallPosition.z - playerPosition3.z, 2.0f));

                    intersecting3 = distance3 <= 0.042f && tempBallPosition.y <= 0.2f;
				}

				// Determines which target position gets the player the closest to the ball
				if (intersecting1 && distance1 < distance2 && distance1 < distance3)
				{
					xIntersect = playerPosition1.x;
					zIntersect = playerPosition1.z;
					distance = distance1;
				}
				else if (intersecting2 && distance2 < distance1 && distance2 < distance3)
				{
					xIntersect = playerPosition2.x;
					zIntersect = playerPosition2.z;
					distance = distance2;
				}
				else if (intersecting3 && distance3 < distance1 && distance3 < distance2)
				{
					xIntersect = playerPosition3.x;
					zIntersect = playerPosition3.z;
					distance = distance3;
				}

				// Adds time based on whether the player can catch the ball standing up or has to dive to catch it
				if(!(intersecting1 || intersecting2 || intersecting3))
					if (distance > 0.42f || distance <= 0.22f)
						thisTime++;
					else
						thisTime += 2;
			}

			// Determines whether this fielder will reach the ball before the other fielders
			if (thisTime < shortestTime)
			{
				shortestTime = thisTime;
				thisFielder = fielders[i];
                xIntersect = tempBallPosition.x;
                zIntersect = tempBallPosition.z;
			}
		}

		// Sets the destination of the fielder to the best target position
		if (xDistance <= 0.0f && zDistance <= 0.0f)
		{
			GameObject target = new GameObject ();
			target.transform.position = new Vector3 (xIntersect, 0.0f, zIntersect);
			thisFielder.GetComponent<NavMeshAgent> ().destination = target.transform.position;
		}
	}

	// Slows down and redirects the ball if it collides with a wall
	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.name == "wall1")
			xDistance = -xDistance / 2;
		else if (collision.gameObject.name == "wall2")
			zDistance = -zDistance / 2;
	}
}