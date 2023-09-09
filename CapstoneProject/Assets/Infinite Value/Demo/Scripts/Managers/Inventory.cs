using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InfiniteValue;
using System.Text;
using InspectorAttribute;
using System;
using System.Linq;

/*
 * Store the player current state.
 * This manager stores the player score, money, incomes and upgrades.
 * It also define the player current statistics.
 * 
 */
namespace IV_Demo
{
    public class Inventory : AManager<Inventory>
    {
        const int baseTypePower = 1;
        const float baseMoneyPerGame = 4;

        const int enforcedPrecision = 16;

        //editor
        public float tickGamePerSecDelay = 0.1f;
        [Space]
        [SerializeField, ConditionalHide(HideCondition.IsntPlaying, HideType.HideOrReadonly)] string gamesStr;
        [SerializeField, ConditionalHide(HideCondition.IsntPlaying, HideType.HideOrReadonly)] string moneyStr;
        [SerializeField, ConditionalHide(HideCondition.IsntPlaying, HideType.HideOrReadonly)] string upgradesStr;
        [SerializeField, ConditionalHide(HideCondition.IsntPlaying, HideType.HideOrReadonly)] string incomesStr;

        // public access
        public static InfVal games { get; private set; } = new InfVal(0, enforcedPrecision);
        public static InfVal money { get; private set; } = new InfVal(0, enforcedPrecision);
        public static HashSet<string> upgrades { get; private set; } = new HashSet<string>();
        public static Dictionary<string, int> incomes { get; private set; } = new Dictionary<string, int>();

        public static InfVal gamesMadeWhileAway { get; private set; }

        public static InfVal IncomePower(string income)
        {
            if (incomePowerDico.ContainsKey(income))
                return incomePowerDico[income];
            return 0;
        }

        public static bool CanBuyUpgrade(string upgrade) => (money >= DataBase.upgrades[upgrade].price && !upgrades.Contains(upgrade));

        public static void BuyUpgrade(string upgrade)
        {
            if (!CanBuyUpgrade(upgrade))
                return;

            money -= DataBase.upgrades[upgrade].price;
            upgrades.Add(upgrade);

            RefreshStats();
        }

        public static InfVal IncomePrice(string income) => incomePrices[income];

        public static bool CanBuyIncome(string income) => (money >= IncomePrice(income));

        public static void BuyIncome(string income)
        {
            if (!CanBuyIncome(income))
                return;

            money -= IncomePrice(income);
            if (!incomes.ContainsKey(income))
                incomes[income] = 0;
            ++incomes[income];

            incomePrices[income] *= DataBase.incomes[income].priceIncreasePerUnit;

            RefreshStats();
        }

        public static void SetFromSave((InfVal games, InfVal money, HashSet<string> upgrades, Dictionary<string, int> incomes, DateTime? time) saveTuple)
        {
            games = saveTuple.games.ToPrecision(enforcedPrecision);
            money = saveTuple.money.ToPrecision(enforcedPrecision);
            upgrades = new HashSet<string>((saveTuple.upgrades ?? new HashSet<string>()).Where((s) => DataBase.upgrades.ContainsKey(s)).ToList());
            incomes = new Dictionary<string, int>(saveTuple.incomes ?? new Dictionary<string, int>()).Where((k) => DataBase.incomes.ContainsKey(k.Key)).ToDictionary(i => i.Key, i => i.Value);

            foreach (KeyValuePair<string, Income> kv in DataBase.incomes)
                incomePrices[kv.Key] = (kv.Value.basePrice *
                    (!incomes.ContainsKey(kv.Key) ? 1 : MathInfVal.Pow(kv.Value.priceIncreasePerUnit, incomes[kv.Key])));

            RefreshStats();

            if (saveTuple.time != null)
            {
                double elapsedSeconds = (DateTime.Now - saveTuple.time.Value).TotalSeconds;

                gamesMadeWhileAway = MathInfVal.Truncate(elapsedSeconds * gamesPerSec);
                AddToGames(gamesMadeWhileAway);
            }
        }

        public static void AddToGames(InfVal val)
        {
            val = MathInfVal.Truncate(val);

            games += val;
            money += val * moneyPerGame * moneyRatio;
        }

        public static void CheatAddMoney(InfVal val)
        {
            val = MathInfVal.Truncate(val);

            money += val;
        }

        public static InfVal typePower { get; private set; }
        public static InfVal moneyPerGame { get; private set; }
        public static InfVal gamesPerSec { get; private set; }

        public static InfVal moneyRatio = 1; // cheat

        // internal static logic
        static Dictionary<string, InfVal> incomePowerDico = new Dictionary<string, InfVal>();
        static Dictionary<string, InfVal> incomePrices = new Dictionary<string, InfVal>();

        static void RefreshStats()
        {
            typePower = baseTypePower;
            moneyPerGame = baseMoneyPerGame;
            gamesPerSec = 0;

            foreach (string u in upgrades)
            {
                if (DataBase.upgrades[u].targetType == Upgrade.TargetType.TypePower)
                    typePower *= DataBase.upgrades[u].increaseRatio;
                else if (DataBase.upgrades[u].targetType == Upgrade.TargetType.MoneyPerGame)
                    moneyPerGame *= DataBase.upgrades[u].increaseRatio;
            }

            incomePowerDico.Clear();
            foreach (KeyValuePair<string, int> i in incomes)
            {
                InfVal thisIncomeGamesPerSec = DataBase.incomes[i.Key].gamesPerSec * i.Value;

                foreach (string u in upgrades)
                    if (DataBase.upgrades[u].targetType == Upgrade.TargetType.Income && DataBase.upgrades[u].incomeTarget == i.Key)
                        thisIncomeGamesPerSec *= DataBase.upgrades[u].increaseRatio;

                incomePowerDico[i.Key] = thisIncomeGamesPerSec;
                gamesPerSec += thisIncomeGamesPerSec;
            }
        }

        // internal instance logic
        InfVal leftOverGamesPerSec = default;
        float lastTickTime;

        StringBuilder strBuilder = new StringBuilder();

        void Start()
        {
            foreach (KeyValuePair<string, Income> kv in DataBase.incomes)
                incomePrices[kv.Key] = kv.Value.basePrice;

#if UNITY_WEBGL
            Time.maximumDeltaTime = 10f;
#endif

            StartCoroutine(GamesPerSecTickRoutine());

            RefreshStats();
        }

        IEnumerator GamesPerSecTickRoutine()
        {
            while (true)
            {
                float timeElapsed = Time.time - lastTickTime;

                leftOverGamesPerSec += gamesPerSec * timeElapsed;

                AddToGames(leftOverGamesPerSec);

                leftOverGamesPerSec = MathInfVal.DecimalPart(leftOverGamesPerSec);
                lastTickTime = Time.time;
                yield return new WaitForSeconds(tickGamePerSecDelay);
            }
        }

#if UNITY_EDITOR
        void Update()
        {
            gamesStr = games.ToString();
            moneyStr = games.ToString();

            strBuilder.Clear();
            foreach (string u in upgrades)
            {
                if (strBuilder.Length > 0)
                    strBuilder.Append(", ");
                strBuilder.Append(u);
            }
            upgradesStr = strBuilder.ToString();

            strBuilder.Clear();
            foreach (KeyValuePair<string, int> i in incomes)
            {
                if (strBuilder.Length > 0)
                    strBuilder.Append(", ");
                strBuilder.Append($"{i.Key}: {i.Value}");
            }
            incomesStr = strBuilder.ToString();
        }
#endif
    }
}