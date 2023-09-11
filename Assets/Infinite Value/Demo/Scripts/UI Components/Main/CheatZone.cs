using UnityEngine;
using UnityEngine.UI;
using InfiniteValue;

/*
 * Menu where the player can cheat.
 * If used, it will mark the game as cheated forever.
 * 
 */ 
namespace IV_Demo
{
    public class CheatZone : MonoBehaviour
    {
        public InfValInputField addMoneyField;
        public Button addMoneyButton;
        [Space]
        public Slider typeRateSlider;
        public InputField typeRateField;
        public int maxTypeRate = 300;
        [Space]
        public InfValInputField moneyRatioField;
        [Space]
        public GameObject areYouSureObj;
        public Button areYouSureButton;
        [Space]
        public PlayZone playZone;

        bool ignoreCallbacks = false;

        // unity
        void Start()
        {
            // add money
            addMoneyButton.onClick.AddListener(() =>
            {
                SaveAndLoad.isCheated = true;
                Inventory.CheatAddMoney(addMoneyField.value);
                addMoneyField.value = 0;
            });

            // type rate
            typeRateSlider.maxValue = maxTypeRate;

            typeRateSlider.onValueChanged.AddListener((val) =>
            {
                if (val > 0)
                    SaveAndLoad.isCheated = true;
                playZone.autoTypeRate = (int)val;

                if (!ignoreCallbacks)
                {
                    ignoreCallbacks = true;
                    typeRateField.text = ((int)val).ToString();
                    ignoreCallbacks = false;
                }
            });

            typeRateField.onValueChanged.AddListener((s) =>
            {
                if (!ignoreCallbacks && int.TryParse(s, out int res))
                {
                    ignoreCallbacks = true;
                    typeRateSlider.value = res;
                    ignoreCallbacks = false;
                }
            });

            typeRateField.onValidateInput += (string text, int charIndex, char addedChar) =>
            {
                if (!char.IsDigit(addedChar))
                    return '\0';
                if (addedChar == '-')
                    return '\0';
                return addedChar;
            };

            typeRateField.onEndEdit.AddListener((s) =>
            {
                typeRateField.text = playZone.autoTypeRate.ToString();
            });

            // money ratio
            moneyRatioField.inputField.onEndEdit.AddListener((s) =>
            {
                if (!moneyRatioField.isValid)
                    moneyRatioField.value = Inventory.moneyRatio;
                else
                {
                    if (!moneyRatioField.value.isOne)
                        SaveAndLoad.isCheated = true;
                    Inventory.moneyRatio = moneyRatioField.value;
                }
            });

            // are you sure
            areYouSureButton.onClick.AddListener(() =>
            {
                areYouSureObj.SetActive(false);
            });
        }

        void OnEnable()
        {
            areYouSureObj.SetActive(!SaveAndLoad.isCheated);
        }
    }
}