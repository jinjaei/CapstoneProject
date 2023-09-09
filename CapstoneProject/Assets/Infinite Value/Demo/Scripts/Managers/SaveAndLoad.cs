using UnityEngine;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using InfiniteValue;
using System.Collections.Generic;
using InspectorAttribute;
using System.Globalization;

/*
 * A simple save and load system featuring an autosave and import/export functionality.
 * The data will be stored in a text file using readable JSON.
 * 
 * It features a cheat key system, this way the player can freely modify the JSON representation
 * of the saved data but the game will know about it and mark the game as cheated.
 * 
 */
namespace IV_Demo
{
    public class SaveAndLoad : AManager<SaveAndLoad>
    {
        const string fileName = "Save.txt";

        const string cheatKeyStr = "CheatKey: ";
        const string cheatedStr = "CHEATED";

        const string secretPassword = "SalAmI22"; // this will be used to generate the cheat key

        Exception invalidSaveOnLoadException => new Exception("Save text was in an invalid format.");

        // editor
        [Tooltip("This text will be on top of every save files.")]
        [TextArea(4, 8)]
        public string warningText = "Editing the save can result in corrupted data or unexpected behaviour, proceed with caution.\n" +
                                    "It will also mark your game as cheated forever.\n";
        [Tooltip("Seconds we will wait in between two autosaves.")]
        public float _timeBetweenAutoSaves = 10f;
        [Tooltip("Is this save marked as cheated ?")]
        [SerializeField, ConditionalHide(HideCondition.IsntPlaying, HideType.Hide)] bool _isCheated;
        [Space]
        [Tooltip("Disable saves (useful when creating the game).")]
        [SerializeField, ConditionalHide(HideCondition.IsPlaying)] bool debugDisableSaves = false;
        [Tooltip("Show in the console when saving happens.")]
        public bool verbose = true;

        // public access

        /// <summary>Time it takes to save all the objects that asked for it. A value < 1 disable the auto save.</summary>
        public static float timeBetweenTwoAutoSaves
        {
            get { return instance._timeBetweenAutoSaves; }
            set { instance._timeBetweenAutoSaves = value; }
        }

        /// <summary>Wether a save was modified outside of the game.</summary>
        public static bool isCheated
        {
            get { return instance._isCheated; }
            set { instance._isCheated = value; }
        }

        /// <summary>When was the current save created.</summary>
        public static DateTime creationTime { get; private set; }

        /// <summary>Return where on the device saves are stored.</summary>
        public static string saveLocation
        {
            get
            {
#if UNITY_EDITOR
                string cutAppDataPath = Application.dataPath.Substring(0, Application.dataPath.Length - ("Assets").Length);
                return $"{cutAppDataPath}/{Configuration.folderPath}/Demo/{fileName}";
#else
                return $"{Application.persistentDataPath}/{fileName}";
#endif
            }
        }

        /// <summary>Get the save text as a string.</summary>
        public static string Export()
        {
            instance.Save();
            return instance.CreateSaveText();
        }

        /// <summary>Set the game state from a save text. May throw an Exception.</summary>
        public static void Import(string saveTxt)
        {
            instance.SetFromSaveText(saveTxt);
            instance.Save();
        }

        /// <summary>Reset the game (delete the save and act as if the game was launched for the first time).</summary>
        public static void Reset()
        {
            isCheated = false;
            creationTime = DateTime.Now;
            Inventory.SetFromSave((default, default, default, default, null));
            instance.Save();
        }

        // internal logic
        [Serializable]
        class SaveObj
        {
            [Serializable]
            struct Income
            {
                public string name;
                public int count;

                public Income(string key, int value) { name = key; count = value; }
            }

            [SerializeField] string creationTime;
            [SerializeField] InfVal score;
            [SerializeField] InfVal money;
            [SerializeField] string[] upgrades;
            [SerializeField] Income[] incomes;
            [SerializeField] string saveTime;

            public SaveObj(DateTime creation, InfVal score, InfVal money, HashSet<string> upgrades, Dictionary<string, int> incomes, DateTime time)
            {
                creationTime = creation.ToString(CultureInfo.InvariantCulture);

                this.score = score;
                this.money = money;
                this.upgrades = upgrades.ToArray();

                List<Income> incomesList = new List<Income>();
                foreach (KeyValuePair<string, int> i in incomes)
                    incomesList.Add(new Income(i.Key, i.Value));
                this.incomes = incomesList.ToArray();

                saveTime = time.ToString(CultureInfo.InvariantCulture);
            }

            public void Deconstruct(out DateTime creation, out InfVal score, out InfVal money, out HashSet<string> upgrades,
                out Dictionary<string, int> incomes, out DateTime time)
            {
                creation = DateTime.Parse(creationTime, CultureInfo.InvariantCulture);

                score = this.score;
                money = this.money;
                upgrades = new HashSet<string>(this.upgrades);

                incomes = new Dictionary<string, int>();
                foreach (Income inc in this.incomes)
                    incomes[inc.name] = inc.count;

                time = DateTime.Parse(saveTime, CultureInfo.InvariantCulture);
            }
        }

        float lastSaveTime;

        void Save()
        {
            if (debugDisableSaves)
                return;

#if UNITY_WEBGL && !UNITY_EDITOR
            PlayerPrefs.SetString("Save", CreateSaveText());
            PlayerPrefs.Save();
#else
            File.WriteAllText(saveLocation, CreateSaveText());
#endif

            lastSaveTime = Time.time;
        }

        void Load()
        {
            if (debugDisableSaves)
                return;

            string saveText = null;

#if UNITY_WEBGL && !UNITY_EDITOR
            if (PlayerPrefs.HasKey("Save"))
                saveText = PlayerPrefs.GetString("Save");
#else
            if (File.Exists(saveLocation))
                saveText = File.ReadAllText(saveLocation);
#endif

            if (saveText != null)
            {
                try { SetFromSaveText(saveText); }
                catch { }
            }
        }

        string CreateSaveText()
        {
            SaveObj saveObj = new SaveObj(creationTime, Inventory.games, Inventory.money, Inventory.upgrades, Inventory.incomes, DateTime.Now);

            string json = JsonUtility.ToJson(saveObj, true);
            string cheatLine = '\n' + cheatKeyStr + (_isCheated ? cheatedStr : GenerateCheatKeyFromJson(json));

            return warningText + json + cheatLine;
        }

        void SetFromSaveText(string allTxt)
        {
            if (allTxt == null)
                throw invalidSaveOnLoadException;

            int jsonBeginIndex = allTxt.IndexOf('{');
            int jsonEndIndex = allTxt.LastIndexOf('}');
            if (jsonBeginIndex < 0 || jsonEndIndex <= 0)
                throw invalidSaveOnLoadException;

            if (jsonEndIndex + 2 + cheatKeyStr.Length > allTxt.Length)
                throw invalidSaveOnLoadException;
            string cheatKey = allTxt.Substring(jsonEndIndex + 2 + cheatKeyStr.Length);

            string json = allTxt.Substring(jsonBeginIndex, jsonEndIndex + 1 - jsonBeginIndex);

            SaveObj loadedObj;
            try { loadedObj = JsonUtility.FromJson<SaveObj>(json); }
            catch { throw invalidSaveOnLoadException; }

            if (!_isCheated && cheatKey != GenerateCheatKeyFromJson(json))
                _isCheated = true;

            (DateTime creation, InfVal score, InfVal money, HashSet<string> upgrades, Dictionary<string, int> incomes, DateTime time) = loadedObj;
            creationTime = creation;
            Inventory.SetFromSave((score, money, upgrades, incomes, time));
        }

        string GenerateCheatKeyFromJson(string json)
        {
            HashAlgorithm algorithm = new SHA256Managed();

            json = new string(json.Where(c => !char.IsControl(c)
                && !char.IsSeparator(c) && !char.IsWhiteSpace(c)).ToArray());

            byte[] bytes = Encoding.UTF8.GetBytes(json + secretPassword);
            bytes = algorithm.ComputeHash(bytes);

            return Convert.ToBase64String(bytes);
        }

        // unity
        void Update()
        {
            if (timeBetweenTwoAutoSaves >= 1 && !debugDisableSaves && Time.time - lastSaveTime > timeBetweenTwoAutoSaves)
            {
                if (verbose)
                    Debug.Log("Auto Saving");

                Save();
                lastSaveTime = Time.time;
            }
        }

        void OnApplicationQuit()
        {
            Save();
        }

        void Start()
        {
            creationTime = DateTime.Now;

            Load();
        }
    }
}