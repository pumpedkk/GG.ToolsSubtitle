using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


namespace GGTools.Subtitle
{
    [CustomEditor(typeof(Subtitle))]
    public class SubtitleEditor : Editor
    {
        SerializedProperty listnameProp;

        SerializedProperty subtitleTypeProp;

        SerializedProperty dialogueScriptProp;
        SerializedProperty subtitleProp;
        #region TextReferences
        SerializedProperty subtitleTextProp;
        SerializedProperty maxCharactersProp;
        SerializedProperty typewriterEffectTimeProp;
        #endregion

        #region NameReferences
        SerializedProperty nameTextProp;
        SerializedProperty hasTextBoxProp;
        SerializedProperty splitCharacterProp;
        #endregion

        #region PortraitReferences
        SerializedProperty portraitProp;
        SerializedProperty canSetNativeSizePortraitProp;
        SerializedProperty setNativeTypePortraitProp;
        SerializedProperty multiplyFactorPortraitProp;
        SerializedProperty divideFactorPortraitProp;
        #endregion

        #region PoseReferences
        SerializedProperty characterPoseProp;
        SerializedProperty canSetNativeSizePoseProp;
        SerializedProperty setNativeTypePoseProp;
        SerializedProperty multiplyFactorPoseProp;
        SerializedProperty divideFactorPoseProp;
        #endregion

        #region AudioReferences
        SerializedProperty audioSourceProp;
        SerializedProperty nextSubtitleOnItEndProp;
        #endregion

        SerializedProperty scriptSpeechProp;

        #region AdvancedSettings
        SerializedProperty advancedSettingsProp;
        SerializedProperty csvSplitterProp;
        SerializedProperty characterNamePositionProp;
        SerializedProperty characterSpeechPositionProp;
        #endregion

        void OnEnable()
        {
            listnameProp = serializedObject.FindProperty("listName");

            dialogueScriptProp = serializedObject.FindProperty("dialogueScript");
            subtitleTypeProp = serializedObject.FindProperty("subititleType");
            subtitleProp = serializedObject.FindProperty("subtitle");
            subtitleTextProp = serializedObject.FindProperty("subtitleText");
            nameTextProp = serializedObject.FindProperty("nameText");
            hasTextBoxProp = serializedObject.FindProperty("hasTextBox");
            portraitProp = serializedObject.FindProperty("portrait");
            canSetNativeSizePortraitProp = serializedObject.FindProperty("canSetNativeSizePortrait");
            setNativeTypePortraitProp = serializedObject.FindProperty("setNativeTypePortrait");
            multiplyFactorPortraitProp = serializedObject.FindProperty("multiplyFactorPortrait");
            divideFactorPortraitProp = serializedObject.FindProperty("divideFactorPortrait");
            splitCharacterProp = serializedObject.FindProperty("splitCharacter");
            characterPoseProp = serializedObject.FindProperty("characterPose");
            canSetNativeSizePoseProp = serializedObject.FindProperty("canSetNativeSizePose");
            setNativeTypePoseProp = serializedObject.FindProperty("setNativeTypePose");
            multiplyFactorPoseProp = serializedObject.FindProperty("multiplyFactorPose");
            divideFactorPoseProp = serializedObject.FindProperty("divideFactorPose");
            typewriterEffectTimeProp = serializedObject.FindProperty("typewriterEffectTime");
            scriptSpeechProp = serializedObject.FindProperty("scriptSpeeches");
            audioSourceProp = serializedObject.FindProperty("audioSource");
            nextSubtitleOnItEndProp = serializedObject.FindProperty("nextSubtitleOnItEnd");
            maxCharactersProp = serializedObject.FindProperty("maxCharacters");

            #region AdvancedSettings
                        advancedSettingsProp = serializedObject.FindProperty("advancedSettings");
            csvSplitterProp = serializedObject.FindProperty("csvSplitter");
            characterNamePositionProp = serializedObject.FindProperty("characterNamePosition");
            characterSpeechPositionProp = serializedObject.FindProperty("characterSpeechPosition");
            #endregion
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Subtitle subtitle = (Subtitle)target;

            EditorGUILayout.LabelField("Your Dialogue Script File", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(dialogueScriptProp, new GUIContent("Dialogue Script"));

            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            if (dialogueScriptProp.objectReferenceValue != null)
            {
                if (GUILayout.Button("Read Script"))
                {
                    subtitle.CreateDialogueEditor();
                }
                EditorGUILayout.Space();

                if (GUILayout.Button("Clear Script"))
                {
                    subtitle.ClearScriptEditor();
                }
                EditorGUILayout.Space();
            }
            EditorGUILayout.LabelField("Subtitle Object", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(subtitleProp, new GUIContent(""));

            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Subtitle Type", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();

            var currentValue = (SubtitleType)subtitleTypeProp.intValue;

            EditorGUI.indentLevel++;

            var newValue = (SubtitleType)EditorGUILayout.EnumFlagsField("", currentValue);

            EditorGUI.indentLevel--;

            if (EditorGUI.EndChangeCheck())
            {
                subtitleTypeProp.intValue = (int)newValue;

            }

            EditorGUILayout.Space();

            DrawSubtitleTypeFields(newValue);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(scriptSpeechProp, new GUIContent(listnameProp.stringValue), true);

            EditorGUILayout.Space();

            advancedSettingsProp.boolValue = BoldToggle("Advance Settings", advancedSettingsProp.boolValue);

            EditorGUILayout.Space();

            if (advancedSettingsProp.boolValue)
            {
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(csvSplitterProp, new GUIContent("CSV Spliter"));
                EditorGUILayout.PropertyField(characterNamePositionProp, new GUIContent("Character Name Position"));
                EditorGUILayout.PropertyField(characterSpeechPositionProp, new GUIContent("Character Speech Position"));

            }

            serializedObject.ApplyModifiedProperties();
        }

        void DrawSubtitleTypeFields(SubtitleType type)
        {

            if (type != 0)
            {
                EditorGUILayout.LabelField("Subtitle Config", EditorStyles.boldLabel);
            }

            EditorGUI.indentLevel++;

            if (type.HasFlag(SubtitleType.Subtitle))
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Subtitle TextBox Config", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(subtitleTextProp, new GUIContent("Subtitle TextBox"));
                EditorGUILayout.PropertyField(maxCharactersProp, new GUIContent("Max Characters"));
                EditorGUILayout.Space();
                if (type.HasFlag(SubtitleType.TypewriterEffect))
                {
                    EditorGUILayout.PropertyField(typewriterEffectTimeProp, new GUIContent("The Time Between Letters"));
                }
                EditorGUI.indentLevel--;
            }
            if (type.HasFlag(SubtitleType.Name))
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Name TextBox Config", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(hasTextBoxProp, new GUIContent("Has Text Box"));
                EditorGUILayout.Space();
                if (hasTextBoxProp.boolValue) 
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(nameTextProp, new GUIContent("Name TextBox"));

                }
                if (type.HasFlag(SubtitleType.CharacterPose))
                {
                    EditorGUILayout.PropertyField(splitCharacterProp, new GUIContent("Char to Split Name And Pose"));
                }
                EditorGUI.indentLevel--;
            }
            if (type.HasFlag(SubtitleType.Portrait))
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Portrait Config", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(portraitProp, new GUIContent("Display Portrait Image"));
                EditorGUILayout.PropertyField(canSetNativeSizePortraitProp, new GUIContent("Set Native Size"));
                if (canSetNativeSizePortraitProp.boolValue)
                {
                    EditorGUI.BeginChangeCheck();

                    var currentValue = (SetNativeType)setNativeTypePortraitProp.intValue;
                    var portraitValue = (SetNativeType)EditorGUILayout.EnumPopup("Scale Type", currentValue);

                    if (EditorGUI.EndChangeCheck())
                    {
                        setNativeTypePortraitProp.intValue = (int)portraitValue;

                    }
                    if (portraitValue == SetNativeType.Multiply)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(multiplyFactorPortraitProp, new GUIContent("Multiply Factor"));
                        EditorGUI.indentLevel--;
                    }
                    else if (portraitValue == SetNativeType.Divide)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(divideFactorPortraitProp, new GUIContent("Divide Factor"));
                        EditorGUI.indentLevel--;
                    }
                }
                EditorGUI.indentLevel--;
            }
            if (type.HasFlag(SubtitleType.CharacterPose))
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Character Pose Config", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(characterPoseProp, new GUIContent("Display Character Pose Image"));
                EditorGUILayout.PropertyField(canSetNativeSizePoseProp, new GUIContent("Set Native Size"));
                if (canSetNativeSizePoseProp.boolValue)
                {
                    EditorGUI.BeginChangeCheck();

                    var currentPoseValue = (SetNativeType)setNativeTypePoseProp.intValue;
                    var poseValue = (SetNativeType)EditorGUILayout.EnumPopup("Scale Type", currentPoseValue);

                    if (EditorGUI.EndChangeCheck())
                    {
                        setNativeTypePoseProp.intValue = (int)poseValue;

                    }
                    if (poseValue == SetNativeType.Multiply)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(multiplyFactorPoseProp, new GUIContent("Multiply Factor"));
                        EditorGUI.indentLevel--;
                    }
                    else if (poseValue == SetNativeType.Divide)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(divideFactorPoseProp, new GUIContent("Divide Factor"));
                        EditorGUI.indentLevel--;
                    }
                }
                EditorGUI.indentLevel--;

            }

            if (type.HasFlag(SubtitleType.Audio))
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Audio Config", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(audioSourceProp, new GUIContent("Audio Source"));
                EditorGUILayout.PropertyField(nextSubtitleOnItEndProp, new GUIContent("Next Subtitle On It End"));


                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;

        }

        public static bool BoldToggle(string label, bool value)
        {
            Rect rect = GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight);

            GUIStyle bold = new GUIStyle(EditorStyles.label);
            bold.fontStyle = FontStyle.Bold;

            Rect labelRect = EditorGUI.PrefixLabel(rect, new GUIContent(label), bold);

            return EditorGUI.Toggle(labelRect, value);
        }

    }
    [CustomPropertyDrawer(typeof(CharacterSpeech))]
    public class CharacterSpeechDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var nameProp = property.FindPropertyRelative("name");
            var textProp = property.FindPropertyRelative("text");
            var timeToNextProp = property.FindPropertyRelative("timeToNext");
            var speechProp = property.FindPropertyRelative("speech");
            var whatToDoNextProp = property.FindPropertyRelative("nextType");
            var customAudioTimeProp = property.FindPropertyRelative("customAudioTime");
            var startEventsProp = property.FindPropertyRelative("startEvent");
            var endEventsProp = property.FindPropertyRelative("endEvents");

            float height = 0;

            height += EditorGUIUtility.singleLineHeight + 4;
            height += EditorGUIUtility.singleLineHeight + 4;
            height += EditorGUIUtility.singleLineHeight + 4;
            height += EditorGUIUtility.singleLineHeight + 4;
            height += EditorGUIUtility.singleLineHeight + 4;
            height += EditorGUIUtility.singleLineHeight + 4;
            height += EditorGUI.GetPropertyHeight(startEventsProp, true) + 6;
            height += EditorGUI.GetPropertyHeight(endEventsProp, true) + 10;
            height += EditorGUIUtility.singleLineHeight + 8;
            height += EditorGUIUtility.singleLineHeight;

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var nameProp = property.FindPropertyRelative("name");
            var textProp = property.FindPropertyRelative("text");
            var timeToNextProp = property.FindPropertyRelative("timeToNext");
            var speechProp = property.FindPropertyRelative("speech");
            var whatToDoNextProp = property.FindPropertyRelative("nextType");
            var endEventsProp = property.FindPropertyRelative("endEvents");
            var startEventsProp = property.FindPropertyRelative("startEvent");
            var hasNameProp = property.FindPropertyRelative("hasName");
            var hasTextProp = property.FindPropertyRelative("hasText");
            var customAudioTimeProp = property.FindPropertyRelative("customAudioTime");

            var lineHeight = EditorGUIUtility.singleLineHeight;
            var y = position.y;


            if (hasNameProp.boolValue)
            {
                y += lineHeight + 4;
                EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), nameProp);
                y += lineHeight + 4;
            }
            if (hasTextProp.boolValue)
            {
                EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), textProp);
                y += lineHeight + 4;
            }
            if (customAudioTimeProp.boolValue)
            {
                EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), timeToNextProp);
                y += lineHeight + 4;
            }

            EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), whatToDoNextProp);
            y += lineHeight + 4;

            float eventsHeight = EditorGUI.GetPropertyHeight(startEventsProp, true);

            EditorGUI.PropertyField(new Rect(position.x, y + lineHeight + 4, position.width, eventsHeight), startEventsProp, true);

            y += lineHeight + eventsHeight;

            eventsHeight = EditorGUI.GetPropertyHeight(endEventsProp, true);

            EditorGUI.PropertyField(new Rect(position.x, y + lineHeight + 4, position.width, eventsHeight), endEventsProp, true);

            y += lineHeight + eventsHeight;


            EditorGUI.EndProperty();
        }
    }

}
