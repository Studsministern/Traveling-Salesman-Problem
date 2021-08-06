using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SolveProblem : MonoBehaviour
{
    // The game manager and text
    private GameManager gameManager;
    [SerializeField] private Text completionText;
    
    // Indices for index and gameobjects
    private int[] indices;
    private GameObject[] gameObjectIndices;
    
    // The current best indice combination
    private int[] bestTourIndices;
    private float bestTourDst;

    // The ratio of completed solutions compared to the total
    private float currentSolution;
    private float totalSolutions;

    // The time for starting solve
    private float startTime;

    void Start()
    {
        // Finding the game manager
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    public void Solve()
    {
        // Intialize and reset currentSolutions, totalSolutions, indices and gameObjectIndices
        int count = gameManager.Indices.Count;
        indices = new int[count];
        bestTourIndices = new int[count];
        bestTourDst = float.MaxValue;

        startTime = Time.time;
        
        for (int i = 0; i < count; i++)
        {
            indices[i] = i;
            bestTourIndices[i] = i;
        }

        gameObjectIndices = gameManager.Indices.ToArray();

        // Exception for three of fewer indices
        if (count <= 3)
        {
            currentSolution = 1;
            totalSolutions = 1;
            FewIndicesSolution();
        }
        // For four or more indices
        else
        {
            currentSolution = 0;
            totalSolutions = Factorial(count - 1) / 2;

            /// Call with length - 1 to keep one element fixed in place. This avoids wasting time
            /// evaluating tours that are identical except for beginning at a different point
            GenerateSolutions(indices, indices.Length - 1);
        }
    }

    // Heap's algoritm for generating all permutations
    private void GenerateSolutions(int[] indices, int n)
    {
        if (n == 1)
        {
            EvaluateSolution();
        } else
        {
            for (int i = 0; i < n; i++)
            {
                GenerateSolutions(indices, n - 1);
                int swapIndex = (n % 2 == 0) ? i : 0;
                (indices[swapIndex], indices[n - 1]) = (indices[n - 1], indices[swapIndex]);
            }
        }
    }

    private void EvaluateSolution()
    {
        // Ignore solutions which are just the reverse of another solution
        if (indices[0] < indices[indices.Length - 2])
        {
            // Add to the current solution
            currentSolution++;

            // Calculate length of the path (including returning to start point)
            float tourDst = 0;
            for (int i = 0; i < indices.Length; i++)
            {
                // Starts from 0 --> 1, ends at indices.Length --> 0
                int nextIndex = (i + 1) % indices.Length;
                tourDst += LookUpDistance(indices[i], indices[nextIndex]);
            }

            // Save the path indices if this is the best solution found so far
            if (tourDst < bestTourDst)
            {
                bestTourDst = tourDst;
                
                // Copy over one array to another
                System.Array.Copy(indices, bestTourIndices, indices.Length);
            }
        }

        UpdateText();
    }

    private void FewIndicesSolution()
    {
        float tourDst = 0;
        for (int i = 0; i < indices.Length; i++)
        {
            // Starts from 0 --> 1, ends at indices.Length --> 0
            int nextIndex = (i + 1) % indices.Length;
            tourDst += LookUpDistance(indices[i], indices[nextIndex]);
        }
        bestTourDst = tourDst;
        System.Array.Copy(indices, bestTourIndices, indices.Length);

        UpdateText();
    }

    private float LookUpDistance(int index1, int index2)
    {
        float dx = gameObjectIndices[index1].transform.position.x - gameObjectIndices[index2].transform.position.x;
        float dy = gameObjectIndices[index1].transform.position.y - gameObjectIndices[index2].transform.position.y;
        return Mathf.Sqrt(Mathf.Pow(dx, 2) + Mathf.Pow(dy, 2));
    }

    private int Factorial(int n)
    {
        if (n == 1)
        {
            return 1;
        }
        return n * Factorial(n - 1);
    }

    private void UpdateText()
    {   
        completionText.text = currentSolution + " / " + totalSolutions + "\n"
                            + Math.Round(currentSolution / totalSolutions * 100, 2) + " % \n"
                            + Math.Round(Time.time - startTime, 2) + " seconds \n"
                            + Math.Round(bestTourDst, 2) + " km";
    }
}
