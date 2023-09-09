using UnityEngine;
using UnityEditor;

namespace IV_Demo
{
    [CustomEditor(typeof(Audio))]
    public class AudioDrawer : Editor
    {
        // const params
        const string musicTitle = "Music";
        const string soundsTitle = "Sounds";
        const string paramsTitle = "Parameters";
        const string testTitle = "Test";

        const string ponderationLabel = "Weight";

        const string playButtonStr = "Play";
        const string addButtonStr = " +";
        const string removeButtonStr = " -";

        const float smallButtonWidth = 18;
        const float smallButtonHeight = 16;

        static Color[] backgroundColors => EditorGUIUtility.isProSkin ?
            new Color[2] { new Color32(41, 41, 41, 255), new Color32(61, 61, 61, 255) } : // base color: Color32(56, 56, 56, 255)
            new Color[2] { new Color32(179, 179, 179, 255), new Color32(209, 209, 209, 255) }; // base color: Color32(194, 194, 194, 255)

        // fields
        int testSoundIndex = 0;

        public override void OnInspectorGUI()
        {
            // start
            serializedObject.Update();
            Audio audio = target as Audio;

            // get vars
            SerializedProperty musicProp = serializedObject.FindProperty("music");
            SerializedProperty musicVolumeRatioProp = serializedObject.FindProperty("musicVolumeRatio");
            SerializedProperty sound_ParamsArrayProp = serializedObject.FindProperty("sounds_ParamsArray");
            SerializedProperty maxAudioSourcesProp = serializedObject.FindProperty("maxAudioSources");
            
            // music
            EditorGUILayout.LabelField(musicTitle, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(musicProp);
            EditorGUILayout.PropertyField(musicVolumeRatioProp);

            // sounds
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(soundsTitle, EditorStyles.boldLabel);
            DrawOneSoundParamsArray(sound_ParamsArrayProp);

            // parameters
            EditorGUILayout.LabelField(paramsTitle, EditorStyles.boldLabel);

            if (Application.isPlaying)
                GUI.enabled = false;
            EditorGUILayout.PropertyField(maxAudioSourcesProp);
            GUI.enabled = true;

            EditorGUILayout.Space();

            // test
            EditorGUILayout.LabelField(testTitle, EditorStyles.boldLabel);

            testSoundIndex = EditorGUILayout.Popup(testSoundIndex, System.Enum.GetNames(typeof(Audio.Sound)));

            if (GUILayout.Button(playButtonStr))
                Audio.PlaySound((Audio.Sound)testSoundIndex);

            // end
            serializedObject.ApplyModifiedProperties();
        }

        void DrawOneSoundParamsArray(SerializedProperty arrayProp)
        {
            for (int i = 0; i < arrayProp.arraySize; i++)
            {
                SerializedProperty soundNameProp = arrayProp.GetArrayElementAtIndex(i).FindPropertyRelative("soundName");
                SerializedProperty soundClipsArrayProp = arrayProp.GetArrayElementAtIndex(i).FindPropertyRelative("soundClipsArray");
                SerializedProperty ponderationsArrayProp = arrayProp.GetArrayElementAtIndex(i).FindPropertyRelative("ponderationsArray");

                float ponderationSum = 0;
                for (int j = 0; j < ponderationsArrayProp.arraySize; j++)
                    ponderationSum += ponderationsArrayProp.GetArrayElementAtIndex(j).floatValue;

                EditorGUILayout.BeginHorizontal();
                {
                    // sound name
                    EditorGUILayout.LabelField(soundNameProp.stringValue, GUILayout.MaxWidth(EditorGUIUtility.labelWidth - 4));

                    // ponderation sums
                    GUI.enabled = false;
                    EditorGUILayout.FloatField(ponderationSum);
                    GUI.enabled = true;

                    // add button
                    if (GUILayout.Button(addButtonStr, GUILayout.MaxWidth(smallButtonWidth), GUILayout.MaxHeight(smallButtonHeight)))
                    {
                        soundClipsArrayProp.arraySize += 1;
                        ponderationsArrayProp.arraySize += 1;

                        // default value
                        ponderationsArrayProp.GetArrayElementAtIndex(soundClipsArrayProp.arraySize - 1).floatValue = 1;

                        soundClipsArrayProp.GetArrayElementAtIndex(soundClipsArrayProp.arraySize - 1).FindPropertyRelative("volumeRatio").floatValue = 1;
                        soundClipsArrayProp.GetArrayElementAtIndex(soundClipsArrayProp.arraySize - 1).FindPropertyRelative("pitchBounds").FindPropertyRelative("_min").floatValue = 1;
                        soundClipsArrayProp.GetArrayElementAtIndex(soundClipsArrayProp.arraySize - 1).FindPropertyRelative("pitchBounds").FindPropertyRelative("_max").floatValue = 1;
                    }
                }
                EditorGUILayout.EndHorizontal();

                // soundClips
                for (int j = 0; j < soundClipsArrayProp.arraySize; j++)
                {
                    SerializedProperty ponderationProp = ponderationsArrayProp.GetArrayElementAtIndex(j);

                    SerializedProperty clipProp = soundClipsArrayProp.GetArrayElementAtIndex(j).FindPropertyRelative("clip");
                    SerializedProperty volumeProp = soundClipsArrayProp.GetArrayElementAtIndex(j).FindPropertyRelative("volumeRatio");
                    SerializedProperty pitchBoundsProp = soundClipsArrayProp.GetArrayElementAtIndex(j).FindPropertyRelative("pitchBounds");
                    SerializedProperty priorityProp = soundClipsArrayProp.GetArrayElementAtIndex(j).FindPropertyRelative("priority");

                    EditorGUI.indentLevel += 1;

                    Rect elemRect = EditorGUILayout.BeginHorizontal();
                    elemRect.x += 11;
                    elemRect.width -= 9;
                    EditorGUI.DrawRect(elemRect, backgroundColors[j % 2]);
                    {
                        EditorGUILayout.BeginVertical();
                        {
                            // ponderation
                            EditorGUILayout.PropertyField(ponderationProp, new GUIContent(ponderationLabel));

                            // elem params
                            EditorGUILayout.PropertyField(clipProp);
                            EditorGUILayout.PropertyField(volumeProp);
                            EditorGUILayout.PropertyField(pitchBoundsProp);
                            EditorGUILayout.PropertyField(priorityProp);

                            // small space
                            EditorGUILayout.LabelField("", GUILayout.MaxHeight(2));
                        }
                        EditorGUILayout.EndVertical();

                        EditorGUILayout.BeginVertical(GUILayout.MaxWidth(smallButtonWidth));
                        {
                            // remove button
                            EditorGUILayout.LabelField("", GUILayout.MaxWidth(smallButtonWidth), GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 2f + 1));
                            if (GUILayout.Button(removeButtonStr, GUILayout.MaxWidth(smallButtonWidth), GUILayout.MaxHeight(smallButtonHeight)))
                            {
                                soundClipsArrayProp.DeleteArrayElementAtIndex(j);
                                ponderationsArrayProp.DeleteArrayElementAtIndex(j);
                            }
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUI.indentLevel -= 1;
                }

                EditorGUILayout.Space();
            }
        }
    }
}