using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using InspectorAttribute;
using System;

/*
 * Game data loader.
 * This manager will load incomes, upgrades and text files using the Resources class.
 * It will then parse the text files to generate multiples small texts to type in the play zone.
 * Those can then be accessed from other objects.
 * 
 */
namespace IV_Demo
{
    public class DataBase : AManager<DataBase>
    {
        // custom types
        public enum TextType
        {
            Code,
            GameDesignDoc,
            Facebook,
            Cmd,
            StackOverflow,
        }

        public struct TypedText
        {
            public TextType type { get; private set; }
            public string title { get; private set; }
            public string text { get; private set; }

            public TypedText(TextType type, string title, string text) { this.type = type; this.title = title; this.text = text; }
        }

        // editor
        [ConditionalHide(HideCondition.IsPlaying, HideType.Hide), SerializeField] string codeTextsName = "Texts/Code";
        [ConditionalHide(HideCondition.IsPlaying, HideType.Hide), SerializeField] string gddTextsName = "Texts/Game Design Documents";
        [ConditionalHide(HideCondition.IsPlaying, HideType.Hide), SerializeField] string fbTextsName = "Texts/Facebook";
        [ConditionalHide(HideCondition.IsPlaying, HideType.Hide), SerializeField] string soTextsName = "Texts/Stack Overflow";
        [ConditionalHide(HideCondition.IsPlaying, HideType.Hide), SerializeField] string cmdTextsName = "Texts/CMD";
        [ConditionalHide(HideCondition.IsPlaying, HideType.Hide), SerializeField] string textSeparator = "###";

        [ConditionalHide(HideCondition.IsntPlaying, HideType.HideOrReadonly), SerializeField] string loadedUpgrades;
        [ConditionalHide(HideCondition.IsntPlaying, HideType.HideOrReadonly), SerializeField] string loadedIncomes;
        [ConditionalHide(HideCondition.IsntPlaying, HideType.HideOrReadonly), SerializeField] string loadedTexts;

        // public access
        public static IReadOnlyDictionary<string, Upgrade> upgrades => instance._upgrades;
        public static IReadOnlyDictionary<string, Income> incomes => instance._incomes;

        public static IReadOnlyList<TypedText> GetTextList(TextType type) => instance.textsDico[type];

        public static int textsCount => instance.fullTypedTextList.Count;
        public static TypedText GetRandomText(float[] ponderations, out int resultIndex)
        {
            TypedText ret = CustomRandom.Ponderated(instance.fullTypedTextList.ToArray(), ponderations);
            resultIndex = instance.fullTypedTextList.IndexOf(ret);
            return ret;
        }

        // internal logic
        Dictionary<string, Upgrade> _upgrades = new Dictionary<string, Upgrade>();
        Dictionary<string, Income> _incomes = new Dictionary<string, Income>();

        Dictionary<TextType, List<TypedText>> textsDico = new Dictionary<TextType, List<TypedText>>();
        List<TypedText> fullTypedTextList = new List<TypedText>();

        void Awake()
        {
            // load upgrades
            Upgrade[] upgradesList = Resources.LoadAll<Upgrade>("");
            foreach (Upgrade u in upgradesList)
                _upgrades[u.name] = u;

            // load incomes
            Income[] incomesList = Resources.LoadAll<Income>("");
            foreach (Income i in incomesList)
                _incomes[i.name] = i;

            // load texts
            SetTypedTextsList(TextType.Code, codeTextsName);
            SetTypedTextsList(TextType.GameDesignDoc, gddTextsName);
            SetTypedTextsList(TextType.Facebook, fbTextsName);
            SetTypedTextsList(TextType.Cmd, cmdTextsName);
            SetTypedTextsList(TextType.StackOverflow, soTextsName);

#if UNITY_EDITOR
            // update inspector
            loadedUpgrades = $"{_upgrades.Count}: {string.Join(", ", _upgrades.Keys.ToArray())}";
            loadedIncomes = $"{_incomes.Count}: {string.Join(", ", _incomes.Keys.ToArray())}";
            loadedTexts = $"C:{textsDico[TextType.Code].Count} " +
                          $"GDD:{textsDico[TextType.GameDesignDoc].Count} " +
                          $"FB:{textsDico[TextType.Facebook].Count} " +
                          $"CMD:{textsDico[TextType.Cmd].Count} " +
                          $"SO:{textsDico[TextType.StackOverflow].Count}";
#endif
        }

        // private methods
        void SetTypedTextsList(TextType type, string assetName)
        {
            textsDico[type] = new List<TypedText>();

            // load the asset
            TextAsset asset = Resources.Load<TextAsset>(assetName);

            if (asset == null)
                return;

            string loadedText = asset.text;

            // split into different texts based on the separator
            string[] texts = loadedText.Split(new string[] { textSeparator }, StringSplitOptions.None);

            foreach (string t in texts)
            {
                // split into lines
                List<string> lines = new List<string>(t.Split(new string[] { Environment.NewLine }, StringSplitOptions.None));

                // get title
                string title = null;
                if (!string.IsNullOrWhiteSpace(lines[0]))
                {
                    title = lines[0].Substring(1);
                    lines.RemoveAt(0);
                }

                // remove empty lines at beggining and end
                while (lines.Count > 0 && string.IsNullOrWhiteSpace(lines[0]))
                    lines.RemoveAt(0);

                while (lines.Count > 0 && string.IsNullOrWhiteSpace(lines[lines.Count - 1]))
                    lines.RemoveAt(lines.Count - 1);

                // ignore empty texts
                if (lines.Count <= 0)
                    continue;

                // add to the list
                string text = string.Join(Environment.NewLine, lines).Replace("\\n", Environment.NewLine);
                textsDico[type].Add(new TypedText(type, title, text));

                // add to full list
                fullTypedTextList.Add(new TypedText(type, title, text));
            }

            Resources.UnloadAsset(asset);
        }
    }
}