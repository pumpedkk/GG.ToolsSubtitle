using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GGTools.FileReaders;
using GGTools.TMProUltilitis;
using GGTools.SpriteUltilitis;
using System.Collections.Generic;
using UnityEngine.Events;

namespace GGTools.Subtitle
{
    public class Subtitle : MonoBehaviour
    {
        private static Subtitle _inst;
        private int subtitleIndex;
        private bool pause;

        [HideInInspector][SerializeField] private string listName = "List of Character Speech";

        #region References

        [SerializeField] private GameObject subtitle;
        [SerializeField] private SubtitleType subititleType = SubtitleType.Subtitle;
        [SerializeField] private TextAsset dialogueScript;

        #region TextReferences
        [SerializeField] private TextMeshProUGUI subtitleText;
        [SerializeField] private int maxCharacters;
        #endregion

        #region NameReferences
        [SerializeField] private TextMeshProUGUI nameText;
        #endregion

        #region PortraitReferences
        [SerializeField] private Image portrait;
        [SerializeField] private bool canSetNativeSizePortrait;
        [SerializeField] private SetNativeType setNativeTypePortrait = SetNativeType.None;
        [SerializeField] private float multiplyFactorPortrait;
        [SerializeField] private float divideFactorPortrait;
        [SerializeField] private char splitCharacter = '.';
        #endregion

        #region CharacterPoseReferences
        [SerializeField] private Image characterPose;
        [SerializeField] private float typewriterEffectTime;
        [SerializeField] private bool canSetNativeSizePose;
        [SerializeField] private SetNativeType setNativeTypePose = SetNativeType.None;
        [SerializeField] private float multiplyFactorPose;
        [SerializeField] private float divideFactorPose;
        #endregion

        #region AudioReferences
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private bool nextSubtitleOnItEnd;


        #endregion

        [SerializeField] private List<CharacterSpeech> characterSpeech = new List<CharacterSpeech>();
        #endregion

        #region AdvancedSettings
        [SerializeField] private bool advancedSettings;
        [SerializeField] private char csvSplitter = ';';
        [SerializeField] private int characterNamePosition = 0;
        [SerializeField] private int characterSpeechPosition = 1;
        #endregion

        public static int SubtitleIndex => _inst._SubtitleIndex;
        private int _SubtitleIndex
        {
            get => subtitleIndex;
            set => subtitleIndex = value;
        }

        void Awake()
        {
            if (_inst != null && _inst != this)
            {
                Destroy(gameObject);
                return;
            }
            _inst = this;
            if (audioSource == null)
            {
                if (!TryGetComponent<AudioSource>(out audioSource))
                {
                    audioSource = this.gameObject.AddComponent<AudioSource>();
                }
            }
        }

        /// <summary>
        /// Begins the subtitle sequence from index 0 and schedules the next subtitle
        /// based on audio length or custom timing.
        /// </summary>
        public static void StartSubtitle() => _inst._StartSubtitle();
        private void _StartSubtitle()
        {
            SubtitleInstance(0);
            subtitleIndex = 0;

            if (characterSpeech[subtitleIndex].speech == null)
            {
                Invoke("_NextSubtitle", characterSpeech[subtitleIndex].timeToNext);
            }
            else
            {
                Invoke("_NextSubtitle", characterSpeech[subtitleIndex].speech.length);
            }


        }

        /// <summary>
        /// Clears all previously loaded CharacterSpeech entries and resets the list name.
        /// </summary>

        public static void ClearScript() => _inst._ClearScript();


        /// <summary>
        /// Advances to the next subtitle in the sequence, handling stop conditions
        /// and scheduling the following entry.
        /// </summary>
        public static void NextSubtitle() => _inst._NextSubtitle();
        private void _NextSubtitle()
        {
            if (characterSpeech.Count <= subtitleIndex)
            {
                Debug.LogError("No More Speech");
                return;
            }
            if (characterSpeech[subtitleIndex].nextType == WhatToDoNext.Stop)
            {
                Invoke("StopSubtitle", 1f);
                return;
            }

            subtitleIndex++;
            SubtitleInstance(subtitleIndex);

            if (characterSpeech[subtitleIndex].speech == null)
            {
                Invoke("_NextSubtitle", characterSpeech[subtitleIndex].timeToNext);
            }
            else
            {
                Invoke("_NextSubtitle", characterSpeech[subtitleIndex].speech.length);
            }





        }

        public static void Stop() => _inst._StopSubtitle(true);
        public static void Pause() => _inst._StopSubtitle(false);
        public void StopSubtitle() => _StopSubtitle();
        private void _StopSubtitle(bool force = true)
        {
            pause = !force;
            subtitle.SetActive(!force);
            if (force)
            {
                pause = false;
                audioSource.Stop();
            }
            else
            {
                audioSource.Pause();
            }

        }
        private void _ResumeSubtitle()
        {
            if (pause)
            {
                subtitle.SetActive(true);
                pause = false;
                audioSource.Play();
            }
        }
        /// <summary>
        /// Jumps directly to a specific subtitle index and plays it immediately.
        /// </summary>
        /// <param name="jumpIndex">Index to jump to (1-based).</param>
        public static void JumpSubtitle(int jumpIndex) => _inst._JumpSubtitle(jumpIndex);
        private void _JumpSubtitle(int jumpIndex)
        {
            subtitleIndex = --jumpIndex;
            _NextSubtitle();
        }
        /// <summary>
        /// Displays a subtitle entry based on its index, updating text, name, portraits,
        /// character poses, and audio according to the active SubtitleType flags.
        /// </summary>
        /// <param name="subIndex">Index of the subtitle to display.</param>
        private void SubtitleInstance(int subIndex)
        {
            if (pause)
            {
                _ResumeSubtitle();
                return;
            }
            float timeWriterAux = 0;
            if (characterSpeech.Count <= 0)
            {
                _CreateDialogueScript();
            }
            if (characterSpeech.Count <= 0)
            {
                Debug.LogError("No Speech");
            }
            else
            {
                subtitle.SetActive(true);
                if (subititleType.HasFlag(SubtitleType.TypewriterEffect))
                {
                    timeWriterAux = typewriterEffectTime;
                }
                if (subititleType.HasFlag(SubtitleType.Subtitle))
                {
                    subtitleText.Type(this, characterSpeech[subIndex].text, timeWriterAux);
                }
                if (subititleType.HasFlag(SubtitleType.Name))
                {
                    if (subititleType.HasFlag(SubtitleType.CharacterPose))
                    {
                        nameText.text = characterSpeech[subIndex].name.Split(splitCharacter)[0];
                    }
                    else
                    {
                        nameText.text = characterSpeech[subIndex].name;
                    }
                }
                if (subititleType.HasFlag(SubtitleType.Portrait) && subititleType.HasFlag(SubtitleType.CharacterPose))
                {
                    portrait.sprite = LoadPortrait(characterSpeech[subIndex].name);

                    if (portrait.sprite == null)
                    {
                        portrait.gameObject.SetActive(false);
                    }
                    else
                    {
                        portrait.gameObject.SetActive(true);
                    }

                    if (canSetNativeSizePortrait)
                    {
                        portrait.SetNativeSize();
                        if (setNativeTypePortrait == SetNativeType.Multiply)
                        {
                            portrait.rectTransform.sizeDelta *= multiplyFactorPortrait;
                        }
                        else if (setNativeTypePortrait == SetNativeType.Divide)
                        {
                            portrait.rectTransform.sizeDelta /= divideFactorPortrait;
                        }
                    }
                    characterPose.sprite = SpriteLoader.LoadSprite(characterSpeech[subIndex].name);

                    if (characterPose.sprite == null)
                    {
                        characterPose.gameObject.SetActive(false);
                    }
                    else
                    {
                        characterPose.gameObject.SetActive(true);
                    }

                    if (canSetNativeSizePose)
                    {
                        characterPose.SetNativeSize();
                        if (setNativeTypePose == SetNativeType.Multiply)
                        {
                            characterPose.rectTransform.sizeDelta *= multiplyFactorPose;
                        }
                        else if (setNativeTypePose == SetNativeType.Divide)
                        {
                            characterPose.rectTransform.sizeDelta /= divideFactorPose;
                        }
                    }
                }
                else
                {
                    if (subititleType.HasFlag(SubtitleType.Portrait))
                    {
                        portrait.sprite = SpriteLoader.LoadSprite(characterSpeech[subIndex].name);

                        if (portrait.sprite == null)
                        {
                            portrait.gameObject.SetActive(false);
                        }
                        else
                        {
                            portrait.gameObject.SetActive(true);
                        }

                        if (canSetNativeSizePortrait)
                        {
                            portrait.SetNativeSize();
                            if (setNativeTypePortrait == SetNativeType.Multiply)
                            {
                                portrait.rectTransform.sizeDelta *= multiplyFactorPortrait;
                            }
                            else if (setNativeTypePortrait == SetNativeType.Divide)
                            {
                                portrait.rectTransform.sizeDelta /= divideFactorPortrait;
                            }
                        }
                    }
                    if (subititleType.HasFlag(SubtitleType.CharacterPose))
                    {
                        characterPose.sprite = SpriteLoader.LoadSprite(characterSpeech[subIndex].name);

                        if (characterPose.sprite == null)
                        {
                            characterPose.gameObject.SetActive(false);
                        }
                        else
                        {
                            characterPose.gameObject.SetActive(true);
                        }

                        if (canSetNativeSizePose)
                        {
                            characterPose.SetNativeSize();
                            if (setNativeTypePose == SetNativeType.Multiply)
                            {
                                portrait.rectTransform.sizeDelta *= multiplyFactorPose;
                            }
                            else if (setNativeTypePose == SetNativeType.Divide)
                            {
                                portrait.rectTransform.sizeDelta /= divideFactorPose;
                            }
                        }
                    }
                }
                if (subititleType.HasFlag(SubtitleType.Audio))
                {
                    audioSource.PlayOneShot(characterSpeech[subIndex].speech);
                }
            }
        }
        private void _ClearScript()
        {
            listName = "List of Character Speech";
            characterSpeech.Clear();
        }
        public void ClearScriptEditor() => _ClearScript();

        /// <summary>
        /// Reads the assigned dialogue script (CSV or TXT), splits its lines using the
        /// configured delimiter, and converts each row into CharacterSpeech entries.
        /// </summary>
        public static void CreateDialogueScript() => _inst._CreateDialogueScript();
        private void _CreateDialogueScript()
        {
            _ClearScript();
            listName = dialogueScript.name;
            string[][] file = dialogueScript.ReadLines().SplitCSV(csvSplitter);

            foreach (string[] s in file)
            {
                CreateCharacterSpeech(s);
            }
        }
        /// <summary>
        /// Converts a CSV line into a CharacterSpeech object, handling automatic text pagination
        /// when text exceeds the maximum character limit.
        /// </summary>
        /// <param name="line">The parsed CSV values for a dialogue row.</param>
        private void CreateCharacterSpeech(string[] line, bool page = false, float pageIndex = 0)
        {
            CharacterSpeech aux = new CharacterSpeech();
            if (subititleType.HasFlag(SubtitleType.Subtitle))
            {
                aux.hasText = true;
            }
            if (subititleType.HasFlag(SubtitleType.Audio))
            {
                aux.hasAudio = true;
                if (nextSubtitleOnItEnd)
                {
                    aux.customAudioTime = false;

                }
            }
            if (subititleType.HasFlag(SubtitleType.Name) || subititleType.HasFlag(SubtitleType.Portrait) || subititleType.HasFlag(SubtitleType.CharacterPose))
            {
                aux.hasName = true;
            }

            aux.name = line[characterNamePosition];

            if (line[characterSpeechPosition].Length > maxCharacters)
            {
                string[] textPages = line[characterSpeechPosition].CreatePage(maxCharacters);
                int pageAux = 0;
                foreach (string p in textPages)
                {
                    string[] pageSpeech = new string[characterNamePosition + characterSpeechPosition + 1];
                    pageSpeech[characterNamePosition] = aux.name;
                    pageSpeech[characterSpeechPosition] = p;
                    if (p != "" && p != " " && p != " ")
                    {
                        CreateCharacterSpeech(pageSpeech, true, pageAux);
                    }
                    pageAux++;
                }
            }
            else
            {
                aux.text = line[characterSpeechPosition];
                characterSpeech.Add(aux);
            }

        }
        public void CreateDialogueEditor() => _CreateDialogueScript();

        /// <summary>
        /// Loads a portrait sprite based on the character's name,
        /// trimming using the configured split character.
        /// </summary>
        /// <param name="name">Full character name string.</param>
        /// <returns>The corresponding portrait sprite.</returns>

        private Sprite LoadPortrait(string name)
        {
            return SpriteLoader.LoadSprite(name.Split(splitCharacter)[0]);
        }

    }

    /// <summary>
    /// Represents a single dialogue entry containing text, audio, name,
    /// event callbacks, timing settings, and pagination information.
    /// </summary>

    [System.Serializable]
    public class CharacterSpeech
    {
        public string name;
        public string text;
        public AudioClip speech;

        public WhatToDoNext nextType = WhatToDoNext.NextSubtitle;

        public UnityEvent startEvent;

        public UnityEvent endEvents;

        public float timeToNext;

        public bool hasName;
        public bool hasText;
        public bool hasAudio;
        public bool customAudioTime = true;
        public bool page;
        public int pageIndex;
    }

    /// <summary>
    /// Flags defining which subtitle features are enabled when displaying each entry.
    /// </summary>
    [System.Flags]
    public enum SubtitleType
    {
        Subtitle = 1 << 0,
        Name = 1 << 1,
        Portrait = 1 << 2,
        CharacterPose = 1 << 3,
        TypewriterEffect = 1 << 4,
        Audio = 1 << 5,
        HaveChoices = 1 << 6,
    }
    /// <summary>
    /// Defines what action should occur after a subtitle finishes (continue or stop).
    /// </summary>
    public enum WhatToDoNext
    {
        NextSubtitle = 1 << 0,
        Stop = 1 << 1,
    }
    /// <summary>
    /// Specifies how images should be resized after calling SetNativeSize().
    /// </summary>
    public enum SetNativeType
    {
        None = 0,
        Multiply = 1,
        Divide = 2
    }
}