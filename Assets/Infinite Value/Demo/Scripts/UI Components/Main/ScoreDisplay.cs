using UnityEngine;
using UnityEngine.UI;

/*
 * Display the current score (number of games created).
 * 
 */ 
namespace IV_Demo
{
    public class ScoreDisplay : MonoBehaviour
    {
        public string format = "{0:5 < Z} Games";
        public Color cheatedColor = Color.red;

        Text text;

        void Awake()
        {
            text = GetComponentInChildren<Text>();
        }

        void Update()
        {
            if (SaveAndLoad.isCheated)
                text.text = $"<color=#{ColorUtility.ToHtmlStringRGB(cheatedColor)}>{string.Format(format, Inventory.games)}</color>";
            else
                text.text = string.Format(format, Inventory.games);
        }
    }
}