using System.Collections.Generic;
using UnityEngine;

public static class Randomiser
{
    private static Queue<int> preGeneratedValues = new Queue<int>();
    private static Queue<int> randomValuesQueue = new Queue<int>();

    public static int Value
    {
        get
        {
            preGeneratedValues.Enqueue(randomValuesQueue.Dequeue());
            return preGeneratedValues.Peek();
        }
    }

    public static void Instantiate(int seed, int numberOfValuesToGenerate)
    {
        randomValuesQueue = new Queue<int>();
        GenerateRandomValues(seed, numberOfValuesToGenerate);
    }

    public static void UndoValue()
    {
        randomValuesQueue.Enqueue(preGeneratedValues.Dequeue());
    }

    // Regenerate random values with a specified seed
    public static void RegenerateRandomValues(int seed, int numberOfValuesToGenerate)
    {
        randomValuesQueue.Clear();
        preGeneratedValues.Clear();
        GenerateRandomValues(seed, numberOfValuesToGenerate);
    }

    // Generate a set of random values based on the seed
    private static void GenerateRandomValues(int seed, int numberOfValuesToGenerate)
    {
        System.Random generator = new System.Random(seed);

        for (int i = 0; i < numberOfValuesToGenerate; i++)
        {
            int randomValue = generator.Next();
            randomValuesQueue.Enqueue(randomValue);
        }
    }
}
