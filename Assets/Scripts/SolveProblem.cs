using System;
using System.Collections;
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
    private int[] bestTourIndices;
    private float bestTourDst;

    // The ratio of completed solutions compared to the total
    private long currentSolution;
    private long totalSolutions;

    // Calculations before update
    [SerializeField] private int calculcationsBeforeUpdate = 500;

    // Keeping track of the time
    private float startTime;

    // The Line Controller for drawing the line
    [SerializeField] private LineRenderer lineRenderer;
    private LineController lc;

    void Start()
    {
        // Components
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        lc = lineRenderer.GetComponent<LineController>();
    }

    public void Solve() // Connected to the Solve-button, using variables from GameManager
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

        // Assigning the start time
        startTime = Time.time;

        if (count <= 3) // Exception for three of fewer indices
        {
            currentSolution = 1;
            totalSolutions = 1;
            EvaluateSolution();
        } else // For four or more indices
        {
            currentSolution = 0;
            totalSolutions = Factorial(count - 1) / 2;
            
            // Using coroutine to be able to update every frame. Length - 1 to not move the first point
            StartCoroutine(LexicographicSolutions(indices.Length - 1));
        }
    }

    /// Lexicographic Order
    /// Using the lexicographic properties of the array to find all possible combinations
    /// Based on: https://www.quora.com/How-would-you-explain-an-algorithm-that-generates-permutations-using-lexicographic-ordering
    private IEnumerator LexicographicSolutions(int n)
    {
        while (currentSolution < totalSolutions && !Input.GetKeyDown(KeyCode.Escape)) // Can use escape to cancel
        {
            // Debug.Log((currentSolution + 1) + ": [" + string.Join(",", indices) + "].");

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

            // Update the program every calculcationsBeforeUpdate frames
            if (currentSolution % calculcationsBeforeUpdate == 0)
            {
                UpdateVisualisation();
                yield return null;
            }
        }

        // Update the program in the end
        UpdateVisualisation();
    }

    private void Reverse(int[] array, int index) // Reverse all elements from index to the end in an array
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
        if (indices.Length <= 3) // For 3 indices
        {
            // Calculate distance, guaranteed to be the best distance
            bestTourDst = CalculateTourDistance();
            System.Array.Copy(indices, bestTourIndices, indices.Length);

            // Update the program
            UpdateVisualisation();
            
        } else // For 4 or more indices
        {
            // Ignore solutions which are just the reverse of another solution
            if (indices[0] < indices[indices.Length - 2])
            {
                // Keep track of the current solution
                currentSolution++;

                // Calculate distance
                float tourDst = CalculateTourDistance();

                // Save the path indices if this is the best solution found so far
                if (tourDst < bestTourDst)
                {
                    bestTourDst = tourDst;

                    // Keep track of the best tour
                    System.Array.Copy(indices, bestTourIndices, indices.Length);
                }
            }
        }
    }

    private float CalculateTourDistance() // Distance of the current tour
    {
        float tourDst = 0;
        
        // Calculate length of the path (including returning to start point)
        for (int i = 0; i < indices.Length; i++)
        {
            // Starts from 0 --> 1, ends at indices.Length --> 0
            int nextIndex = (i + 1) % indices.Length;
            tourDst += LookUpDistance(gameObjectIndices, indices[i], indices[nextIndex]);
        }

        return tourDst;
    }

    private float LookUpDistance(GameObject[] array, int index1, int index2) // Distance between two points in an array
    {
        if (gameObjectIndices[index1] != null && gameObjectIndices[index2] != null)
        {
            float dx = array[index1].transform.position.x - array[index2].transform.position.x;
            float dy = array[index1].transform.position.y - array[index2].transform.position.y;
            return Mathf.Sqrt(Mathf.Pow(dx, 2) + Mathf.Pow(dy, 2));
        }

        return 0;
    }

    private long Factorial(int n) // Calculating n!
    {
        if (n == 1)
        {
            return 1;
        }
        return n * Factorial(n - 1);
    }

    public void StopSolving() // Used to prevent errors when indices are removed when the code is running
    {
        StopAllCoroutines();
    }

    private void UpdateVisualisation() // Updating the text showing all stats and the line for the best tour
    {   
        completionText.text = currentSolution + " / " + totalSolutions + "\n"
                            + Math.Round(((double)currentSolution / totalSolutions * 100), 4) + " % \n"
                            + Math.Round(Time.time - startTime, 2) + " seconds \n"
                            + Math.Round(bestTourDst, 2) + " km";

        lc.UpdateLine(bestTourIndices);
    }
}