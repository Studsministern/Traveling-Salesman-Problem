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
    private long currentSolution;
    private long totalSolutions;
    [SerializeField] private int calculcationsBeforeUpdate = 500;

    // The time for starting solve
    private float startTime;

    // The Line Controller for drawing the line
    [SerializeField] private LineRenderer lineRenderer;
    private LineController lc;

    void Start()
    {
        // Finding the game manager and Line Controller
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        lc = lineRenderer.GetComponent<LineController>();
    }

    public void Solve()
    {
        // Intialize variables
        int count = gameManager.Indices.Count;
        indices = new int[count];
        bestTourIndices = new int[count];
        bestTourDst = float.MaxValue;

        // Assign value to indices, bestTourIndices and gameObjectIndices
        for (int i = 0; i < count; i++)
        {
            indices[i] = i;
            bestTourIndices[i] = i;
        }

        gameObjectIndices = new GameObject[count];
        gameObjectIndices = gameManager.Indices.ToArray();
        lc.SetUpLine(gameObjectIndices);

        // The time for starting
        startTime = Time.time;

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

            /// Lexicographic Order
            /// Using the lexicographic properties of the array to find all possible combinations
            StartCoroutine(LexicographicSolutions(indices.Length - 1));
        }
    }

    private IEnumerator LexicographicSolutions(int n)
    {
        while (currentSolution < totalSolutions && !Input.GetKeyDown(KeyCode.Escape))
        {
            //Debug.Log((currentSolution + 1) + ": [" + string.Join(",", indices) + "].");

            // STEP 1: Find largest x where P[x] < P[x+1]
            int largestX = -1;
            for (int x = n - 1; x >= 0; x--)
            {
                if (indices[x] < indices[x + 1])
                {
                    largestX = x;
                    break;
                }
            }

            if (largestX == -1)
            {
                Debug.Log("A problem has occured with largestX");
                break;
            }

            // STEP 2: Find largest y where P[x] < P[y]
            int largestY = -1;
            for (int y = n; y >= 0; y--)
            {
                if (indices[largestX] < indices[y])
                {
                    largestY = y;
                    break;
                }
            }

            if (largestY == -1)
            {
                Debug.Log("A problem has occured with largestY");
                break;
            }

            // STEP 3: Swap P[x] with P[y]
            (indices[largestX], indices[largestY]) = (indices[largestY], indices[largestX]);

            // STEP 4: Reverse from P[x+1] to n
            Reverse(indices, largestX + 1);

            // Calculate the length of the solution
            EvaluateSolution();

            // Update the program
            if(currentSolution % calculcationsBeforeUpdate == 0)
            {
                lc.UpdateLine(bestTourIndices);
                yield return null;
            }
        }

        // Update drawing of the line after everything has been completed
        lc.UpdateLine(bestTourIndices);
    }

    private void Reverse(int[] array, int index)
    {
        // Temporary array to store the inverted numbers
        int[] tempArray = new int[array.Length - index];
        int tempArrayIndex = 0;

        // Store the numbers in inverted order
        for(int i = array.Length - 1; i >= index; i--)
        {
            tempArray[tempArrayIndex] = array[i];
            tempArrayIndex++;
        }

        // Reverse the first array
        for (int i = array.Length - 1; i >= index; i--)
        {
            array[i] = tempArray[tempArrayIndex - 1];
            tempArrayIndex--;
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

            UpdateText();
        }
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
        lc.UpdateLine(bestTourIndices);
    }

    private float LookUpDistance(int index1, int index2)
    {
        if (gameObjectIndices[index1] != null && gameObjectIndices[index2] != null)
        {
            float dx = gameObjectIndices[index1].transform.position.x - gameObjectIndices[index2].transform.position.x;
            float dy = gameObjectIndices[index1].transform.position.y - gameObjectIndices[index2].transform.position.y;
            return Mathf.Sqrt(Mathf.Pow(dx, 2) + Mathf.Pow(dy, 2));
        }

        return 0;
    }

    private long Factorial(int n)
    {
        if (n == 1)
        {
            return 1;
        }
        return n * Factorial(n - 1);
    }

    public void StopSolving()
    {
        StopAllCoroutines();
    }

    private void UpdateText()
    {   
        completionText.text = currentSolution + " / " + totalSolutions + "\n"
                            + Math.Round(((double)currentSolution / totalSolutions * 100), 4) + " % \n"
                            + Math.Round(Time.time - startTime, 2) + " seconds \n"
                            + Math.Round(bestTourDst, 2) + " km";
    }
}



/// Heap's algoritm
/// Call with length - 1 to keep one element fixed in place. This avoids wasting time
/// evaluating tours that are identical except for beginning at a different point
//totalSolutions /= 2;
//GenerateSolutions(indices.Length - 1);

/* private void GenerateSolutions(int n)
    {
        if (n == 1)
        {
            EvaluateSolutionHeap();
        } else
        {
            for (int i = 0; i < n; i++)
            {
                GenerateSolutions(n - 1);
                int swapIndex = (n % 2 == 0) ? i : 0;
                (indices[swapIndex], indices[n - 1]) = (indices[n - 1], indices[swapIndex]);
            }
        }
    } */
