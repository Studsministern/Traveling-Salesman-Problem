using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    // Varaibles for line renderer and points to draw between
    private LineRenderer lr;
    private int[] lastOrder;
    private Transform[] originalPoints;

    // Initialize line renderer
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.gameObject.SetActive(true);
    }

    // Set up the variables
    public void SetUpLine(GameObject[] gameObjectIndices)
    {
        // Originalpoints will always stay the same
        originalPoints = new Transform[gameObjectIndices.Length];

        for (int i = 0; i < gameObjectIndices.Length; i++)
        {
            originalPoints[i] = gameObjectIndices[i].transform;
        }

        // Set the number of points to draw between
        lr.positionCount = originalPoints.Length + 1;

        // Activate the gameObject
        lr.gameObject.SetActive(true);
    }

    // Update the order of the points and draw the line
    public void UpdateLine(int[] order)
    {
        lastOrder = new int[order.Length];
        
        for (int i = 0; i < lastOrder.Length; i++)
        {
            lastOrder[i] = order[i];
            lr.SetPosition(i, originalPoints[lastOrder[i]].position);
        }

        lr.SetPosition(originalPoints.Length, originalPoints[0].position);
    }

    public void ClearLine()
    {
        lr.positionCount = 0;
        lr.gameObject.SetActive(false);
    }
}
