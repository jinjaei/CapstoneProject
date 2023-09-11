using System;
using UnityEngine;
using UnityEngine.UI;

/*
 * Display the current statistics of the player.
 * 
 */ 
namespace IV_Demo
{
    public class Stats : MonoBehaviour
    {
        public string typePowerFormat = "Type P: {0:3 <}";
        public Text typePowerText;
        [Space]
        public string moneyPerGameFormat = "$/Game: {0:3 <}";
        public Text moneyPerGameText;
        [Space]
        public string incomeFormat = "Games/s: {0:3 <}";
        public Text incomeText;
        [Space]
        public Text timeText;

        void Update()
        {
            typePowerText.text = string.Format(typePowerFormat, Inventory.typePower);
            moneyPerGameText.text = string.Format(moneyPerGameFormat, Inventory.moneyPerGame);
            incomeText.text = string.Format(incomeFormat, Inventory.gamesPerSec);

            timeText.text = (DateTime.Now - SaveAndLoad.creationTime).ToString(@"%d\:hh\:mm\:ss");
        }
    }
}