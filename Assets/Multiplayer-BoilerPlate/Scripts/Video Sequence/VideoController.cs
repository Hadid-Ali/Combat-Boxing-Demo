using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public float initialWaiting = 5f;
    
    private WaitForSeconds m_TimeDelay;
    private WaitForEndOfFrame m_FrameDelay = new();
    
    private bool isPlaying = true;
    private GameEvent m_OnVideoFinished = new();

    private void Awake()
    {
        m_TimeDelay = new(initialWaiting);
    }

    public void Initialize(Action action)
    {
        m_OnVideoFinished.Register(action);
    }

    void OnEnable()
    {
        videoPlayer.Play(); 
        StartCoroutine(this.CheckVideoUpdate());
    }
    
    IEnumerator CheckVideoUpdate()
    {
        yield return m_TimeDelay;

        while (this.videoPlayer.isPlaying)
        {
            yield return m_FrameDelay;
        }
        m_OnVideoFinished.Raise();
    }
}
