using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Threading;
using ThreadPriority = System.Threading.ThreadPriority;

namespace InfiniteValue
{
    /// EditorWindow inheriting class in charge of drawing the unit tests window.
    class UnitTestsWindow : EditorWindow
    {
        enum Mode
        {
            Cast,
            Parse,
            ToString,
            Arithmetic,
            Functions,
        }

        // consts
        static Dictionary<Mode, AUnitTest> testsRef => new Dictionary<Mode, AUnitTest>()
            {
                { Mode.Cast, new CastTest() },
                { Mode.Parse, new ParseTest() },
                { Mode.ToString, new ToStringTest() },
                { Mode.Arithmetic, new ArithmeticTest() },
                { Mode.Functions, new FunctionsTest() },
            };

        const ThreadPriority threadPriority = ThreadPriority.AboveNormal;

        const string windowTitle = "Infinite Value Unit Tests";
        static readonly Vector2 minWindowSize = new Vector2(600, 300);

        const string parametersTitle = "Parameters";
        const string resultsTitle = "Results";

        const string testButtonText = "Test";
        const string cancelButtonText = "Cancel";
        const string processingFormat = "Processing... ({0:##0.00} %)";

        const double timeAtProcessOver = 0.25f;

        static readonly Color loadBarBgColor = Color.gray;
        static readonly Color loadBarFrontColor = Color.green;

        const double successRatio = 0.9;

        static Color successColor => (EditorGUIUtility.isProSkin ? Color.cyan : (Color)(new Color32(0, 50, 230, byte.MaxValue)));

        // private fields
        Mode mode = default;
        Vector2 scrollPos = Vector2.zero;
        [NonSerialized] GUIStyle bigButtonStyle = null;
        [NonSerialized] GUIStyle wrapLabelStyle = null;
        [NonSerialized] GUIStyle arrayStyle = null;
        
        Dictionary<Mode, AUnitTest> tests = null;

        Thread processThread = null;
        float threadProgressRatio = 0;
        double threadEndTime = -1;

        bool gottaProcess = false;
        TestResult lastResult = null;

        // unity messages
        void OnGUI()
        {
            // init styles
            if (bigButtonStyle == null)
            {
                bigButtonStyle = new GUIStyle(GUI.skin.button);
                bigButtonStyle.fixedHeight = EditorGUIUtility.singleLineHeight * 2f;
                bigButtonStyle.fontSize = (int)(bigButtonStyle.fontSize * 1.75f);
            }
            if (wrapLabelStyle == null)
            {
                wrapLabelStyle = new GUIStyle(EditorStyles.label);
                wrapLabelStyle.wordWrap = true;
                wrapLabelStyle.richText = true;
            }
            if (arrayStyle == null)
            {
                arrayStyle = new GUIStyle(EditorStyles.textField);
                arrayStyle.alignment = TextAnchor.MiddleCenter;
                arrayStyle.richText = true;
                arrayStyle.wordWrap = false;
            }

            // init tests
            if (tests == null)
                tests = testsRef;

            // mode
            GUI.enabled = (processThread == null);

            EditorGUI.BeginChangeCheck();
            mode = (Mode)GUILayout.Toolbar((int)mode, Enum.GetNames(typeof(Mode)).Select((s) => new GUIContent(ObjectNames.NicifyVariableName(s))).ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                GUI.FocusControl(null);
                lastResult = null;
            }

            EditorGUILayout.Space();

            // create or delete process thread
            if (Event.current.type == EventType.Layout)
            {
                if (processThread == null && gottaProcess)
                {
                    gottaProcess = false;

                    threadProgressRatio = 0;
                    processThread = new Thread(() => lastResult = tests[mode].Process(ref threadProgressRatio));
                    processThread.Priority = threadPriority;
                    processThread.Start();

                    threadEndTime = -1;
                }
                else if (processThread != null && !processThread.IsAlive)
                {
                    if (threadEndTime < 0)
                        threadEndTime = EditorApplication.timeSinceStartup;
                    else if (EditorApplication.timeSinceStartup - threadEndTime >= timeAtProcessOver)
                        processThread = null;
                }
            }

            // processing draw
            if (processThread != null)
            {
                GUI.enabled = true;

                EditorGUILayout.LabelField(string.Format(processingFormat, threadProgressRatio * 100));

                Rect rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight * 2);
                EditorGUI.DrawRect(rect, loadBarBgColor);
                rect.width *= threadProgressRatio;
                EditorGUI.DrawRect(rect, loadBarFrontColor);

                EditorGUILayout.Space();

                if (GUILayout.Button(cancelButtonText, bigButtonStyle))
                {
                    processThread.Abort();
                    threadEndTime = 0;
                    lastResult = null;
                }
            }
            // normal draw
            else
            {
                // draw description
                EditorGUILayout.LabelField(tests[mode].description, wrapLabelStyle);

                EditorGUILayout.Space();

                // begin scroll
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

                // draw parameters
                EditorGUILayout.LabelField(parametersTitle, EditorStyles.boldLabel);

                tests[mode].DrawParameters();

                // draw test button
                EditorGUILayout.Space();

                if (GUILayout.Button(testButtonText, bigButtonStyle))
                    gottaProcess = true;

                // draw results
                if (lastResult != null)
                {
                    (List<OneFailedResult> failedResultsList, long extraFailedResults, long usedIterations, double perFailCharSuccess) = lastResult;

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(resultsTitle, EditorStyles.boldLabel);

                    GUI.enabled = false;
                    GUI.color = new Color(1f, 1f, 1f, 2f);

                    EditorGUILayout.LabelField(new GUIContent($"Not ignored iterations: {usedIterations}",
                        "This is the number of iterations we actually used (ignoring those that we skipped)."), GUILayout.Width(400));
                    EditorGUILayout.Space();

                    if (failedResultsList.Count > 0)
                    {
                        double perfectFail = ((double)((failedResultsList.Count - 1) + extraFailedResults) / usedIterations);

                        EditorGUILayout.LabelField(new GUIContent($"Perfect Success Rate: {FailPercent(1 - perfectFail)}",
                            "This is the number of iterations that returned the exact same result."), wrapLabelStyle, GUILayout.Width(400));
                        EditorGUILayout.LabelField(new GUIContent($"Per Character Success Rate: " +
                            $"{FailPercent((1 - perfectFail) + (perFailCharSuccess / (failedResultsList.Count - 1)) * perfectFail)}",
                            "This is the number of characters that stayed the same extrapolated from the results in the array."), wrapLabelStyle, GUILayout.Width(400));

                        EditorGUILayout.Space();

                        Rect rect = EditorGUILayout.GetControlRect(false, failedResultsList.Count * EditorGUIUtility.singleLineHeight);
                        rect.height = EditorGUIUtility.singleLineHeight;
                        rect.width /= 2;

                        foreach (OneFailedResult res in failedResultsList)
                        {
                            EditorGUI.LabelField(rect, res.primitiveResult, arrayStyle);

                            rect.x += rect.width;

                            EditorGUI.LabelField(rect, res.infValResult, arrayStyle);

                            rect.x -= rect.width;
                            rect.y += rect.height;
                        }

                        if (extraFailedResults > 0)
                        {
                            EditorGUILayout.Space();

                            EditorGUILayout.LongField(new GUIContent("Not Listed Fail Results",
                                $"Extra fails count that were not shown in the array (array size is limited to {TestsCommon.maxNumberOfFailedResults})."), extraFailedResults);
                        }
                    }
                    else
                        EditorGUILayout.LabelField($"No Fail: <color=#{ColorUtility.ToHtmlStringRGB(successColor)}>100</color> %", wrapLabelStyle);

                    GUI.color = new Color(1f, 1f, 1f, 1f);
                    GUI.enabled = true;
                }

                // end scroll
                EditorGUILayout.EndScrollView();

                // local function
                string FailPercent(double ratio)
                {
                    Color color = ratio < successRatio
                        ? Color.Lerp(TestsCommon.failColor, EditorStyles.label.normal.textColor, (float)(ratio / successRatio)) 
                        : Color.Lerp(EditorStyles.label.normal.textColor, successColor, (float)((ratio - successRatio) / (1 - successRatio)));

                    return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{(ratio * 100).ToString("##0.00")}</color> %";
                }

            }
        }

        void OnInspectorUpdate()
        {
            if (processThread != null)
                Repaint();
        }

        void Awake()
        {
            // set window properties
            titleContent = new GUIContent(windowTitle);
            minSize = minWindowSize;
        }

        void OnDestroy()
        {
            processThread?.Abort();
        }
    }
}