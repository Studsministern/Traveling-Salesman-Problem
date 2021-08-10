using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    // Varaibles for line renderer and points to draw between
    private LineRenderer lr;
    private Transform[] originalPoints;

    void Start()
    {
        lr = GetComponent<LineRenderer>(); // Initialize line renderer
        lr.gameObject.SetActive(true); // Set the line renderer active at the start
    }

    public void SetUpLine(GameObject[] gameObjectIndices) // Set up the variables
    {
        // Originalpoints will always stay the same
        originalPoints = new Transform[gameObjectIndices.Length];

        for (int i = 0; i < gameObjectIndices.Length; i++)
        {
            originalPoints[i] = gameObjectIndices[i].transform;
        }

        // Set the number of points to draw between (+ 1 to handle going back to the beginning)
        lr.positionCount = originalPoints.Length + 1;

        // Activate the gameObject
        lr.gameObject.SetActive(true);
    }

    public void UpdateLine(int[] order) // Update the order of the points and draw the line
    {
        if (order.Length > 0)
        {
            // Draw between all points
            for (int i = 0; i < order.Length; i++)
            {
                lr.SetPosition(i, originalPoints[order[i]].position);
            }

            // Draw back to the beginning
            lr.SetPosition(originalPoints.Length, originalPoints[0].position);
        }
    }

    public void ClearLine() // Set the points to draw between to 0 and set the line renderer inactive
    {
        lr.positionCount = 0;
        lr.gameObject.SetActive(false);
    }
}
