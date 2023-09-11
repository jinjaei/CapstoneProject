using UnityEngine;
using UnityEngine.UI;

/*
 * Display the current money the player have.
 * 
 */ 
namespace IV_Demo
{
    public class MoneyDisplay : MonoBehaviour
    {
        public string format = "$ {0:5 < Z}";
        public Color cheatedColor = Color.red;

        Text text;

        void Awake()
        {
            text = GetComponentInChildren<Text>();
        }

        void Update()
        {
            if (SaveAndLoad.isCheated)
                text.text = $"<color=#{ColorUtility.ToHtmlStringRGB(cheatedColor)}>{string.Format(format, Inventory.money)}</color>";
            else
                text.text = string.Format(format, Inventory.money);
        }
    }
}