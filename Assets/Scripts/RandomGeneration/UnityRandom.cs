using UnityEngine;

public class UnityRandom : IRandomGenerator {
    public int GetInt(int begin, int end) {
        return Random.Range(begin, end);
    }
}