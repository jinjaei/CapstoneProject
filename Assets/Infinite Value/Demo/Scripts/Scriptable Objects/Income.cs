using UnityEngine;
using InfiniteValue;

/*
 * Represent incomes that will be loaded by the DataBase manager.
 * Define the income power and price.
 * 
 */
namespace IV_Demo
{
    //[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Infinite Value Demo/Income", order = 2)]
    public class Income : ADisplayedScriptableObject
    {
        // public fields
        [Header("Values")]
        public InfVal gamesPerSec = 0;
        [Space]
        public InfVal basePrice = 0;
        public float priceIncreasePerUnit = 1.5f;

        // unity
        protected override void OnValidate()
        {
            base.OnValidate();

            if (gamesPerSec < 0)
                gamesPerSec = 0;
            if (basePrice < 0)
                basePrice = 0;
            if (priceIncreasePerUnit < 1)
                priceIncreasePerUnit = 1;
        }
    }
}