using InfiniteValue;
using UnityEngine;
using InspectorAttribute;

/*
 * Represent upgrades that will be loaded by the DataBase manager.
 * Define the upgrade target, power and price.
 * 
 */
namespace IV_Demo
{
    //[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Infinite Value Demo/Upgrade", order = 1)]
    public class Upgrade : ADisplayedScriptableObject
    {
        // custom type
        public enum TargetType
        {
            TypePower,
            MoneyPerGame,
            Income,
        }

        // public fields
        [Header("Values")]
        public TargetType targetType = default;
        [ConditionalHide("!targetType", (int)TargetType.Income, HideType.Hide)] public string incomeTarget = "";
        [ConditionalHide("!targetType", (int)TargetType.Income, HideType.Hide)] public int countForPossibleBuy = default;
        public float increaseRatio = 2;
        [Space]
        public InfVal price = 0;
        public bool cheatOnly = false;

        // unity
        protected override void OnValidate()
        {
            base.OnValidate();

            if (increaseRatio < 1)
                increaseRatio = 1;
            if (price < 0)
                price = 0;
        }
    }
}