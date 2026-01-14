using UnityEditor;
using UnityEngine;

/// <summary>
/// Preenche automaticamente os campos timeToNext dos blocos de fala
/// do sistema de legendas do GGTools, usando uma aproximação baseada
/// no tamanho do texto em relação à duração total do áudio.
///
/// Ideia:
/// - Para cada entrada em scriptSpeeches:
///   - Usa o AudioClip em "speech" (por linha).
///   - Soma o número de caracteres de todos os "characterSpeechs[x].text".
///   - Calcula para cada bloco uma duração proporcional:
///       timeToNext ~= (len(textoBloco) / somaTotalCaracteres) * clip.length
/// - Isso evita ter que ouvir manualmente cada áudio.
///
/// Uso:
/// - Selecione o prefab (ou vários) com o componente de legendas do GGTools.
/// - Menu: Tools/Subtitles/Preencher timeToNext (GGTools - estimado)
/// </summary>
public static class SubtitleGGToolsTimeAutoFiller
{
    private const string MenuPath = "Tools/Subtitles/Preencher timeToNext (GGTools - estimado)";

    [MenuItem(MenuPath)]
    public static void FillTimeToNextForSelection()
    {
        var selection = Selection.gameObjects;
        if (selection == null || selection.Length == 0)
        {
            Debug.LogWarning("[SubtitleGGToolsTimeAutoFiller] Nenhum GameObject selecionado.");
            return;
        }

        int processedComponents = 0;

        foreach (var go in selection)
        {
            if (go == null) continue;

            var monoBehaviours = go.GetComponentsInChildren<MonoBehaviour>(true);
            foreach (var mb in monoBehaviours)
            {
                if (mb == null) continue;

                var so = new SerializedObject(mb);
                var scriptSpeechesProp = so.FindProperty("scriptSpeeches");
                if (scriptSpeechesProp == null || !scriptSpeechesProp.isArray)
                    continue; // não é o componente esperado

                bool changed = false;

                for (int i = 0; i < scriptSpeechesProp.arraySize; i++)
                {
                    var lineProp = scriptSpeechesProp.GetArrayElementAtIndex(i);
                    if (lineProp == null) continue;

                    var speechClipProp = lineProp.FindPropertyRelative("speech");
                    var characterSpeechsProp = lineProp.FindPropertyRelative("characterSpeechs");

                    if (speechClipProp == null || characterSpeechsProp == null || !characterSpeechsProp.isArray)
                        continue;

                    var clip = speechClipProp.objectReferenceValue as AudioClip;
                    if (clip == null || clip.length <= 0f)
                        continue;

                    // Soma total de caracteres de todos os trechos de fala desta linha
                    float totalChars = 0f;
                    for (int j = 0; j < characterSpeechsProp.arraySize; j++)
                    {
                        var charElem = characterSpeechsProp.GetArrayElementAtIndex(j);
                        if (charElem == null) continue;

                        var textProp = charElem.FindPropertyRelative("text");
                        if (textProp == null) continue;

                        var text = textProp.stringValue ?? string.Empty;
                        totalChars += text.Length;
                    }

                    if (totalChars <= 0f)
                        continue;

                    // Define o timeToNext de cada bloco proporcionalmente ao tamanho do texto
                    for (int j = 0; j < characterSpeechsProp.arraySize; j++)
                    {
                        var charElem = characterSpeechsProp.GetArrayElementAtIndex(j);
                        if (charElem == null) continue;

                        var textProp = charElem.FindPropertyRelative("text");
                        var timeToNextProp = charElem.FindPropertyRelative("timeToNext");
                        if (textProp == null || timeToNextProp == null)
                            continue;

                        var text = textProp.stringValue ?? string.Empty;
                        float charCount = Mathf.Max(1, text.Length); // evita zero

                        float estimatedDuration = (charCount / totalChars) * clip.length;
                        timeToNextProp.floatValue = estimatedDuration;
                        changed = true;
                    }
                }

                if (changed)
                {
                    so.ApplyModifiedProperties();
                    EditorUtility.SetDirty(mb);
                    processedComponents++;
                }
            }
        }

        if (processedComponents > 0)
        {
            AssetDatabase.SaveAssets();
            Debug.Log($"[SubtitleGGToolsTimeAutoFiller] timeToNext preenchido em {processedComponents} componente(s).");
        }
        else
        {
            Debug.LogWarning("[SubtitleGGToolsTimeAutoFiller] Nenhum componente compatível (com 'scriptSpeeches') encontrado na seleção.");
        }
    }
}

