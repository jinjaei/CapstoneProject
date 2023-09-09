using InfiniteValue;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/*
 * Upgrades shop.
 * 
 */ 
namespace IV_Demo
{
    public class UpgradesDisplay : MonoBehaviour
    {
        // editor
        public string priceFormat = "$ {0: 3}";
        public string typePowerFormat = "TP x {0: 3}";
        public string moneyPerGameFormat = "$/G x {0: 3}";
        public string incomeFormat = "{1} x {0: 3}";
        [Space]
        public Transform listParent;

        // private custom types
        class AnUpgrade
        {
            string name;
            GameObject mainObj;
            GameObject ownedObj;
            Button button;

            int countCondition;
            string upgradeCondition;
            bool cheatOnly;

            public void Refresh()
            {
                if (Inventory.upgrades.Contains(name))
                {
                    mainObj.SetActive(true);

                    ownedObj.SetActive(true);
                    button.interactable = true;
                    button.enabled = false;
                    button.targetGraphic.color = Color.clear;
                }
                else
                {
                    Upgrade up = DataBase.upgrades[name];
                    if (cheatOnly)
                        mainObj.SetActive(SaveAndLoad.isCheated);
                    else if (up.targetType == Upgrade.TargetType.Income)
                        mainObj.SetActive(Inventory.incomes.ContainsKey(up.incomeTarget) && Inventory.incomes[up.incomeTarget] >= countCondition);
                    else
                        mainObj.SetActive(string.IsNullOrEmpty(upgradeCondition) || Inventory.upgrades.Contains(upgradeCondition));

                    if (mainObj.activeSelf)
                    {
                        ownedObj.SetActive(false);
                        button.interactable = Inventory.CanBuyUpgrade(name);
                        button.enabled = true;
                        button.targetGraphic.color = Color.white;
                    }
                }
            }

            public AnUpgrade(string name, UpgradesDisplay main, GameObject refObj, string upgradeCondition = null)
            {
                // instantiate
                GameObject inst = Instantiate(refObj, main.listParent);
                inst.transform.name = name;

                // set private fields
                this.name = name;
                mainObj = inst;
                ownedObj = inst.transform.Find("Content/Owned").gameObject;
                button = inst.transform.Find("Button").GetComponent<Button>();

                // set display
                Upgrade up = DataBase.upgrades[name];
                inst.transform.Find("Content/Icon").GetComponent<Image>().sprite = up.image;
                inst.transform.Find("Content/Text/Title").GetComponent<Text>().text = up.name;
                string statFormat = (up.targetType == Upgrade.TargetType.TypePower ? main.typePowerFormat :
                                    (up.targetType == Upgrade.TargetType.MoneyPerGame ? main.moneyPerGameFormat :
                                     main.incomeFormat));
                inst.transform.Find("Content/Text/Stat + Price/Stat").GetComponent<Text>().text = string.Format(statFormat, (InfVal)up.increaseRatio, ShortName(up.incomeTarget));
                inst.transform.Find("Content/Text/Stat + Price/Price").GetComponent<Text>().text = string.Format(main.priceFormat, up.price);
                inst.GetComponent<ToolTip>().text = up.description;

                // set condition
                if (up.cheatOnly)
                    cheatOnly = true;
                else if (up.targetType == Upgrade.TargetType.Income)
                    countCondition = up.countForPossibleBuy;
                else
                    this.upgradeCondition = upgradeCondition;

                // set button
                button.onClick.AddListener(() => Inventory.BuyUpgrade(name));
            }
        }

        // private fields
        List<AnUpgrade> upgradeDisplays = new List<AnUpgrade>();

        // unity
        void Start()
        {
            GameObject refObj = listParent.Find("Reference").gameObject;

            List<string> upgrades = DataBase.upgrades.Keys.ToList();
            upgrades.Sort((a, b) => DataBase.upgrades[a].price.CompareTo(DataBase.upgrades[b].price));

            string prevTypePower = null;
            string prevMoneyPerGame = null;

            foreach (string up in upgrades)
            {
                switch (DataBase.upgrades[up].targetType)
                {
                    case Upgrade.TargetType.TypePower:
                        upgradeDisplays.Add(new AnUpgrade(up, this, refObj, prevTypePower));
                        prevTypePower = up;
                        break;
                    case Upgrade.TargetType.MoneyPerGame:
                        upgradeDisplays.Add(new AnUpgrade(up, this, refObj, prevMoneyPerGame));
                        prevMoneyPerGame = up;
                        break;
                    case Upgrade.TargetType.Income:
                        upgradeDisplays.Add(new AnUpgrade(up, this, refObj));
                        break;
                }
            }

            refObj.SetActive(false);

            GetComponent<ScrollRect>().normalizedPosition = Vector2.zero;
        }

        void Update()
        {
            foreach (AnUpgrade u in upgradeDisplays)
                u.Refresh();
        }

        // private methods
        static string ShortName(string longName)
        {
            if (longName == string.Empty)
                return string.Empty;

            string[] splits = longName.ToLower().Split(' ');

            switch (splits.Length)
            {
                case 1: return $"{splits[0][0]}";
                case 2: return $"{splits[0][0]}.{splits[1][0]}";
                case 3: return $"{splits[0][0]}.{splits[1][0]}.{splits[2][0]}";
                case 4: return $"{splits[0][0]}.{splits[1][0]}.{splits[2][0]}.{splits[3][0]}";
                case 5: return $"{splits[0][0]}.{splits[1][0]}.{splits[2][0]}.{splits[3][0]}.{splits[4][0]}";
            }

            return string.Empty;
        }
    }
}