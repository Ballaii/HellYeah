using UnityEngine;

namespace Eliot.Utility
{
    /// <summary>
    /// Utility methods to handle some of the probability related calculations.
    /// </summary>
    public static class EliotRandom
    {
        /// <summary>
        /// Return a boolean with a given probability.
        /// </summary>
        /// <param name="probabilityInPercents"></param>
        /// <returns></returns>
        public static bool TrueWithProbability(float probabilityInPercents)
        {
            if (probabilityInPercents >= 100f)
                return true;
            if (probabilityInPercents <= 0f)
                return false;

            var randomNumber = Random.Range(0f, 100f);
            if (randomNumber <= probabilityInPercents)
            {
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// Return a boolean with a given probability (with margins).
        /// </summary>
        /// <param name="minProbability"></param>
        /// <param name="maxProbability"></param>
        /// <returns></returns>
        public static bool TrueWithProbability(float minProbability, float maxProbability)
        {
            if (minProbability == maxProbability)
            {
                return TrueWithProbability(minProbability);
            }
            else
            {
                return TrueWithProbability(Random.Range(minProbability, maxProbability));
            }
        }
    }
}