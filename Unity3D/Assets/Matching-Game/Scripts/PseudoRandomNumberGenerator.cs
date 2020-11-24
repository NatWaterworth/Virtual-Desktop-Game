using UnityEngine;

//A random number generator for creating reusable seeds
public class PseudoRandomNumberGenerator : MonoBehaviour
{
    #region Singleton

    public static PseudoRandomNumberGenerator instance;

    private void Awake()
    {
        instance = this;
        ResetSequence();
    }

    #endregion

    [Tooltip("output = seed value * multiply value + offset value (mod modN) (for first iteration)")]
    const int offsetValue = 1;
    [Tooltip("output = seed value * multiply value + offset value (mod modN) (for first iteration)")]
    const int multiplyValue = 16807;
    [Tooltip("the upper bound of the ouput values")]
    const int modN = 2147483647, modN2 = 123532523, modN3 = 1235332, modN4 = 21121;
    [Tooltip("the number that determines variation of the sequence")]
    [SerializeField]
    int seed;

    int index; // check how far along sequence I have iterated.

    int currentNumber;

    /*Calculate next number in sequence*/
    int GetRandomSeededNumber()
    {
        currentNumber = ((currentNumber * multiplyValue) + offsetValue) % modN;
        //randomise even and oddness by modulus with different sizes of modN
        //applying modulus will be dependant on the size of current number ensuring odd and even isn't alternating
        currentNumber = currentNumber % (modN2);
        currentNumber = currentNumber % (modN3);
        currentNumber = currentNumber % (modN4);
        return currentNumber;
    }

    /*Reset random number generator to the start of a seed sequence*/
    public void ResetSequence()
    {
        index = 0;

        if (seed != 0)
            currentNumber = seed;
        else
        {
            Debug.LogWarning("Map Generation Note: If seed = 0 in Inspector a random seed will be chosen. Any other value will be used.");
            currentNumber = Random.Range(0, 1000);
        }

        GetRandomNumber();
    }

    /*Returns a seeded number between 0 and max int value*/
    public int GetRandomNumber()
    {
        GetRandomSeededNumber();
        //Debug.Log("random number:" + currentNumber);
        //Debug.Log("random number % 2:" + currentNumber%2);
        index++;
        return Mathf.Abs(currentNumber);
    }

    //Set the seed value for a random number generated sequence
    public void SetSeed(int newSeed)
    {
        seed = newSeed;
    }
}

