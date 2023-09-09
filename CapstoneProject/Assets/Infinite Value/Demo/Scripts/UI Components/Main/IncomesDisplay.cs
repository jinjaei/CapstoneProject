using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/*
 * Incomes shop.
 * 
 */ 
namespace IV_Demo
{
    public class IncomesDisplay : MonoBehaviour
    {
        // editor
        public string currentCountFormat = "{0} ({1: 3} G/s | {2: 3} %)";
        public string priceFormat = "$ {0: 3}";
        public string powerFormat = "{0: 3} G/s";
        [Space]
        public Transform listParent;

        // private custom types
        class AnIncome
        {
            IncomesDisplay main;

            string name;
            GameObject mainObj;
            Text countText;
            Text priceText;
            Button button;
            string incomeCondToShow;

            public void Refresh()
            {
                priceText.text = string.Format(main.priceFormat, Inventory.IncomePrice(name));
                button.interactable = Inventory.CanBuyIncome(name);

                if (Inventory.incomes.ContainsKey(name) && Inventory.incomes[name] > 0)
                {
                    mainObj.SetActive(true);

                    countText.gameObject.SetActive(true);
                    countText.text = string.Format(main.currentCountFormat, Inventory.incomes[name], Inventory.IncomePower(name),
                        (Inventory.IncomePower(name).ToExponent(-2) * 100 / Inventory.gamesPerSec).ToExponent(-1));
                }
                else
                {
                    mainObj.SetActive(incomeCondToShow == null || 
                        (Inventory.incomes.ContainsKey(incomeCondToShow) && Inventory.incomes[incomeCondToShow] > 0));

                    if (mainObj.activeSelf)
                        countText.gameObject.SetActive(false);
                }
            }

            public AnIncome(string name, IncomesDisplay main, GameObject refObj, string incomeCondToShow)
            {
                // instantiate
                GameObject inst = Instantiate(refObj, main.listParent);
                inst.transform.name = name;

                // set private fields
                this.main = main;
                this.name = name;
                mainObj = inst;
                countText = inst.transform.Find("Content/Top/Text/Count").GetComponent<Text>();
                priceText = inst.transform.Find("Content/Bot/Stat + Price/Price").GetComponent<Text>();
                button = inst.transform.Find("Button").GetComponent<Button>();
                this.incomeCondToShow = incomeCondToShow;

                // set display
                Income inc = DataBase.incomes[name];
                inst.transform.Find("Content/Top/Icon").GetComponent<Image>().sprite = inc.image;
                inst.transform.Find("Content/Top/Text/Title").GetComponent<Text>().text = inc.name;
                inst.transform.Find("Content/Bot/Stat + Price/Stat").GetComponent<Text>().text = string.Format(main.powerFormat, inc.gamesPerSec);
                inst.GetComponent<ToolTip>().text = inc.description;

                // set button
                button.onClick.AddListener(() => Inventory.BuyIncome(name));
            }
        }

        // private fields
        List<AnIncome> incomesDisplay = new List<AnIncome>();

        // unity
        void Start()
        {
            GameObject refObj = listParent.Find("Reference").gameObject;

            List<string> incomes = DataBase.incomes.Keys.ToList();
            incomes.Sort((a, b) => DataBase.incomes[a].basePrice.CompareTo(DataBase.incomes[b].basePrice));

            for (int i = 0; i < incomes.Count; i++)
                incomesDisplay.Add(new AnIncome(incomes[i], this, refObj, (i == 0 ? null : incomes[i - 1])));

            refObj.SetActive(false);

            GetComponent<ScrollRect>().normalizedPosition = Vector2.zero;
        }

        void Update()
        {
            foreach (AnIncome u in incomesDisplay)
                u.Refresh();
        }
    }
}