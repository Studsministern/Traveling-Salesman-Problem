# Travelling-Salesman-Problem
Project created with Unity 2020.03

Program for finding the best solution to the Travelling Salesman Problem (TSP) for a certain number of points (indices). This is done by testing permutations made with Lexicographic ordering and evaluating the distance of each one to find the shortest path.

(*The program also has code for Heap's algorithm. This code is however commented out and moved to the end of the class. In order to use this, some code needs to be moved to Solve(), and the methods needs to be reintroduced. Despite the calculations with Heap's algoritm being seemingly faster, it does all calculations before displaying the shortest path. Drawing lines between points has not been implemented with Heap's algorithm.*)

# Links
Concept and Heap's algorithm based on Sebastian Lague's video: [Coding Adventure: Ant and Slime Simulations](https://www.youtube.com/watch?v=X-iSQQgOd1A).

Lexicographic ordering based on The Coding Train's video:      [Live Stream #57 - Traveling Salesperson](https://www.youtube.com/watch?v=r_SpBy9fQuo&ab_channel=TheCodingTrain).
