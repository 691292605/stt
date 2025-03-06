using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WhisperUI : MonoBehaviour
{
    public Button startButton;
    public Button stopButton;
    public Text resultText;

    private WhisperAPI whisperAPI;
    private AudioClip recordedClip;
    
    void Start()
    {
        // 获取 WhisperAPI 脚本组件
        whisperAPI = GetComponent<WhisperAPI>();
        
        // 为按钮添加点击事件
        startButton.onClick.AddListener(StartRecording);
        stopButton.onClick.AddListener(StopRecording);
        
        // 隐藏停止按钮，直到开始录音
        stopButton.gameObject.SetActive(false);
    }

    // 开始录音
    void StartRecording()
    {
        recordedClip = whisperAPI.StartRecording();
        resultText.text = "正在录制...";
        
        // 显示停止按钮，隐藏开始按钮
        startButton.gameObject.SetActive(false);
        stopButton.gameObject.SetActive(true);
    }

    // 停止录音并发送到API进行识别
    void StopRecording()
    {
        whisperAPI.StopRecording(UpdateResultText);
        
        // 显示开始按钮，隐藏停止按钮
        startButton.gameObject.SetActive(true);
        stopButton.gameObject.SetActive(false);
    }

    // 更新识别结果
    public void UpdateResultText(string text)
    {
        resultText.text = text;
    }
}