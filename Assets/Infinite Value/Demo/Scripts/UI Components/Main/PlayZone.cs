using System;
using UnityEngine;
using UnityEngine.UI;
using InspectorAttribute;
using System.Text;
using System.Linq;
using InfiniteValue;

/*
 * Main menu where the user can type to create games.
 * 
 */ 
namespace IV_Demo
{
    public class PlayZone : MonoBehaviour
    {
        // consts
        int maxMouseButtonCount = 3;

        // private types
        [Serializable] class TypeZonesParam
        {
            [ReadOnly] public string name = default;
            public GameObject mainObj = default;
            public ScrollRect scroll = default;
            public Text title = default;
            public Text text = default;
        }

        // editor fields
        [Header("First time experience")]
        public int enoughGamesToSkipFirstForcedText = 30;
        public int forcedTypedText = 9;
        public int maxKeyStrokesToType = 50;
        [Header("Components per type")]
        [SerializeField] TypeZonesParam[] typeZonesParams;
        [Header("Code syntax colors")]
        public Color codeSpecialKeyWordColor = Color.blue;
        public string[] codeKeyWords;
        public Color codeCommentaryColor = Color.green;
        public Color codeStringColor = Color.yellow;
        [Header("FB fonts")]
        public Font normalFont;
        public Font asciiFont;
        [Header("Cmd")]
        public string cmdNameFormat = "/user/{0}>";
        [Header("Extra Type Power")]
        public float timeToAddOneBuffered = 1f;
        [Space]
        public float maxTextPartPerKeyPress = 0.1f;

        // public fields
        [HideInInspector] public int autoTypeRate = 0;

        // private fields
        int currentCharCount;
        bool isOver;

        TypeZonesParam currentZoneParam;
        DataBase.TextType currentType;
        string toType;
        string cmdInsert;

        StringBuilder strBuilder = new StringBuilder();

        float[] ponderations;
        int firstTimeCount = 0;

        float bufferedCharsToAdd = 0;
        float bufferedCharTime = 0;

        int filteredToTypeLength;
        InfVal extraChars = 0;

        float autoTypeTime = 0;

        // unity
        void Update()
        {
            bool updateText = false;

            // auto type
            if (autoTypeRate > 0)
            {
                autoTypeTime += Time.deltaTime;

                while (autoTypeTime >= 1f / autoTypeRate)
                {
                    DoOneTypePress();

                    updateText = true;

                    autoTypeTime -= 1f / autoTypeRate;
                }
            }
            // on key press
            else
            {
                autoTypeTime = 0;

                if (AnyKeyboardKeyDown())
                {
                    DoOneTypePress();

                    updateText = true;
                }
            }

            // add buffered characters over time
            if (!isOver)
            {
                int buffered = (int)bufferedCharsToAdd;

                if (buffered > 0)
                    bufferedCharTime += Time.deltaTime;
                else
                    bufferedCharTime = 0;

                while (buffered > 0 && bufferedCharTime >= timeToAddOneBuffered / buffered)
                {
                    bufferedCharTime -= timeToAddOneBuffered / buffered;
                    ++currentCharCount;
                    --bufferedCharsToAdd;

                    updateText = true;
                }
            }

            // update the text if needed
            if (updateText)
                UpdateText();
        }

        void Start()
        {
            ponderations = new float[DataBase.textsCount];
            for (int i = 0; i < ponderations.Length; i++)
                ponderations[i] = 1;

            foreach (TypeZonesParam p in typeZonesParams)
                p.mainObj.SetActive(false);

            GetNewTextToType();
        }

        void OnValidate()
        {
            typeZonesParams = OnValidateUtility.EnforceConstantEnumArray<DataBase.TextType, TypeZonesParam>(
                typeZonesParams,
                (x) => x.name,
                (x, str) => { x.name = str; return x; });
        }

        // private methods
        void DoOneTypePress()
        {
            if (isOver)
            {
                if (autoTypeRate == 0)
                    Audio.PlaySound(Audio.Sound.TypingEnd);

                // add unnused buffer chars to extra chars
                extraChars += bufferedCharsToAdd;
                bufferedCharsToAdd = 0;

                // add to games 1 + 1 more for every time we could have typed the text with the extra chars
                if (extraChars > 0)
                {
                    Inventory.AddToGames(1 + (extraChars / filteredToTypeLength));
                    extraChars %= filteredToTypeLength;
                }
                else
                    Inventory.AddToGames(1);

                // get a new text to type
                GetNewTextToType();
            }
            else
            {
                // get the number of char we're typing
                InfVal typedChar = Inventory.typePower;
                float maxTypedChar = filteredToTypeLength * maxTextPartPerKeyPress;

                // split between the ones we will type now or buffer and the one we add to the extra list
                float charToTypeNow = (float)MathInfVal.Min(typedChar, maxTypedChar);
                extraChars += typedChar - charToTypeNow;

                // add at least one character
                ++currentCharCount;
                --charToTypeNow;

                // add the rest to the buffered chars
                bufferedCharsToAdd += charToTypeNow;
            }
        }

        void GetNewTextToType()
        {
            // reset current state
            currentCharCount = 0;
            isOver = false;
            currentZoneParam?.mainObj.SetActive(false);

            // get random text
            DataBase.TypedText typedText;
            if (firstTimeCount < forcedTypedText && Inventory.games < enoughGamesToSkipFirstForcedText)
            {
                DataBase.TextType textType = (DataBase.TextType)(firstTimeCount % Enum.GetValues(typeof(DataBase.TextType)).Length);

                typedText = DataBase.GetTextList(textType)[firstTimeCount / Enum.GetValues(typeof(DataBase.TextType)).Length];
                filteredToTypeLength = new string(typedText.text.Where((c) => !char.IsWhiteSpace(c)).ToArray()).Length;

                ++firstTimeCount;
            }
            else
            {
                int resultIndex = -1;
                do
                {
                    typedText = DataBase.GetRandomText(ponderations, out resultIndex);
                    filteredToTypeLength = new string(typedText.text.Where((c) => !char.IsWhiteSpace(c)).ToArray()).Length;
                }
                while ((float)filteredToTypeLength / maxKeyStrokesToType > Inventory.typePower);

                for (int i = 0; i < ponderations.Length; i++)
                    ponderations[i] = Mathf.Min(1, ponderations[i] + 1f / ponderations.Length);
                ponderations[resultIndex] = 0;
            }

            // use result
            TypeZonesParam param = typeZonesParams[(int)typedText.type];

            // set current
            currentType = typedText.type;
            toType = typedText.text;

            // apply to param
            currentZoneParam = param;

            currentZoneParam.mainObj.SetActive(true);
            if (currentType == DataBase.TextType.Cmd)
                cmdInsert = string.Format(cmdNameFormat, typedText.title);
            else if (currentZoneParam.title != null)
                currentZoneParam.title.text = typedText.title;
            currentZoneParam.text.text = string.Empty;

            // set font
            if (currentType == DataBase.TextType.Facebook)
            {
                if (typedText.title == "A")
                {
#if !UNITY_WEBGL || UNITY_EDITOR
                    currentZoneParam.text.font = asciiFont;
#endif
                    currentZoneParam.text.horizontalOverflow = HorizontalWrapMode.Overflow;
                }
                else
                {
#if !UNITY_WEBGL || UNITY_EDITOR
                    currentZoneParam.text.font = normalFont;
#endif
                    currentZoneParam.text.horizontalOverflow = HorizontalWrapMode.Wrap;
                }
            }

            // apply
            UpdateText();
        }
        
        void UpdateText()
        {
            bool isInComment = false;
            bool isInString = false;
            int keywordLength = 0;
            int insertCmdNameIndex = -1;

            // update text to show + set isOver
            strBuilder.Clear();
            if (currentType == DataBase.TextType.Cmd)
                strBuilder.Append(cmdInsert);

            int usedCharCount = currentCharCount;
            int i;
            float maxCharW = 0;
            float currentCharW = 0;
            for (i = 0; i < toType.Length && usedCharCount > 0; i++)
            {
                // check if we are currently starting a new line string
                bool isTypingNewLine = AreWeWriting(Environment.NewLine, false);

                if (isTypingNewLine)
                {
                    if (currentCharW > maxCharW)
                        maxCharW = currentCharW;

                    currentCharW = -Environment.NewLine.Length + 1;
                }

                // cmd only
                if (currentType == DataBase.TextType.Cmd)
                {
                    --insertCmdNameIndex;

                    // add name at the beggining of every line
                    if (insertCmdNameIndex == 0)
                        strBuilder.Append(cmdInsert);

                    // set index if typing new line
                    if (isTypingNewLine)
                        insertCmdNameIndex = Environment.NewLine.Length;
                }

                // add the character
                strBuilder.Append(toType[i]);
                ++currentCharW;

                bool writingChar = !char.IsWhiteSpace(toType[i]);

                if (writingChar)
                    --usedCharCount;

                // code only
                if (currentType == DataBase.TextType.Code)
                {
                    if (writingChar)
                    {
                        // add comment color
                        if (!isInComment && !isInString && toType[i] == '/' && i < toType.Length - 1 && toType[i + 1] == '/')
                        {
                            strBuilder.Remove(strBuilder.Length - 1, 1);
                            strBuilder.Append($"<color=#{ColorUtility.ToHtmlStringRGB(codeCommentaryColor)}>/");
                            isInComment = true;
                        }

                        // add keyword color
                        if (!isInComment && !isInString && keywordLength == 0)
                        {
                            foreach (string kw in codeKeyWords)
                                if (AreWeWriting(kw, true))
                                {
                                    strBuilder.Remove(strBuilder.Length - 1, 1);
                                    strBuilder.Append($"<color=#{ColorUtility.ToHtmlStringRGB(codeSpecialKeyWordColor)}>{kw[0]}");
                                    keywordLength = kw.Length;
                                    break;
                                }
                        }

                        // end keyword color
                        if (keywordLength > 0)
                        {
                            --keywordLength;
                            if (keywordLength == 0)
                                strBuilder.Append("</color>");
                        }

                        if (!isInComment && toType[i] == '"')
                        {
                            // add string color
                            if (!isInString)
                            {
                                strBuilder.Remove(strBuilder.Length - 1, 1);
                                strBuilder.Append($"<color=#{ColorUtility.ToHtmlStringRGB(codeStringColor)}>\"");
                            }
                            // end string color
                            else
                                strBuilder.Append("</color>");

                            isInString = !isInString;
                        }
                    }
                    // end comments
                    else if (isInComment && isTypingNewLine)
                    {
                        strBuilder.Append("</color>");
                        isInComment = false;
                    }
                }
            }
            if (isInString || isInComment || keywordLength > 0)
                strBuilder.Append("</color>");
            isOver = (i == toType.Length || usedCharCount > 0 || EndsWithWhiteSpaces());

            // apply to text component
            currentZoneParam.text.text = strBuilder.ToString();

            // set correct scroll
            if (currentType == DataBase.TextType.Code)
            {
                RectTransform scrollRect = currentZoneParam.scroll.transform as RectTransform;
                RectTransform contentRect = currentZoneParam.scroll.content;

                if (contentRect.rect.width <= scrollRect.rect.width)
                    currentZoneParam.scroll.normalizedPosition = Vector2.zero;
                else
                {
                    maxCharW = Mathf.Max(currentCharW, maxCharW, 1);

                    int charBeforeHorizontalScroll = (int)(maxCharW * scrollRect.rect.width / contentRect.rect.width);

                    float widthRatio = currentCharW < charBeforeHorizontalScroll ? 0 : 
                        (currentCharW - charBeforeHorizontalScroll + 3) / (maxCharW - charBeforeHorizontalScroll);

                    currentZoneParam.scroll.normalizedPosition = new Vector2(widthRatio, 0);
                }
            }
            else
                currentZoneParam.scroll.normalizedPosition = Vector2.zero;

            // local function
            bool AreWeWriting(string word, bool followedBySeparation)
            {
                int index = 0;
                for (; index < word.Length; index++)
                    if (toType[i + index] != word[index])
                        return false;

                if (!followedBySeparation || toType.Length == i + index)
                    return true;

                char nextChar = toType[i + index];
                return (char.IsWhiteSpace(nextChar) || nextChar == ';');
            }

            bool EndsWithWhiteSpaces()
            {
                for (int index = i; index < toType.Length; index++)
                    if (!char.IsWhiteSpace(toType[index]))
                        return false;
                return true;
            }
        }

        bool AnyKeyboardKeyDown()
        {
            if (!Input.anyKeyDown)
                return false;

            for (int i = 0; i < maxMouseButtonCount; i++)
                if (Input.GetMouseButtonDown(i))
                    return false;

            return true;
        }
    }
}