using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using static WavUtility;

public class WhisperAPI : MonoBehaviour
{
    [SerializeField] private string m_ServerSetting = "http://localhost:9000";
    [SerializeField] private string m_TaskType = "transcribe";
    [SerializeField] private OutputType m_OutputType = OutputType.json;

    private string m_SpeechRecognizeURL;
    private AudioClip audioClip;

    private void Awake()
    {
        m_SpeechRecognizeURL = GetPostUrl();
    }

    private string GetPostUrl()
    {
        return string.Format("{0}/asr?task={1}&encode=true&output={2}&language=zh", m_ServerSetting, m_TaskType, m_OutputType);    
    }

    public AudioClip StartRecording()
    {
        audioClip = Microphone.Start(null, false, 10, 16000);
        return audioClip;
    }

    public void StopRecording(Action<string> callback)
    {
        if (Microphone.IsRecording(null))
        {
            Microphone.End(null);
            StartCoroutine(SendAudioToAPI(audioClip, callback));
        }
        else
        {
            Debug.LogError("No recording found!");
        }
    }

    private IEnumerator SendAudioToAPI(AudioClip clip, Action<string> callback)
    {
        byte[] audioData = WavUtility.FromAudioClip(clip);
        WWWForm form = new WWWForm();
        form.AddBinaryData("audio_file", audioData, "test.mp3", "audio/mpeg");

        UnityWebRequest www = UnityWebRequest.Post(m_SpeechRecognizeURL, form);
        www.SetRequestHeader("accept", "application/json");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error sending audio file: " + www.error);
        }
        else
        {
            string responseText = www.downloadHandler.text;
            Response response = JsonUtility.FromJson<Response>(responseText);
            Debug.Log("Transcription: " + response.text);
            callback?.Invoke(response.text);
        }
    }
}

[Serializable]
public class Response
{
    [SerializeField] public string text = string.Empty;
    [SerializeField] public string language = string.Empty;
}

public enum OutputType
{
    txt,
    json,
    vtt,
    srt,
    tsv
}