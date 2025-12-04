using UnityEngine;
using UnityEngine.Formats.Alembic.Importer;

public class AlembicLoopController : MonoBehaviour
{
    [Header("Alembic Player Reference")]
    [Tooltip("Reference to the AlembicStreamPlayer component. Will auto-assign from this GameObject if not set.")]
    [SerializeField] private AlembicStreamPlayer alembicPlayer;

    [Header("Playback Settings")]
    [Tooltip("Start playing automatically when the scene loads.")]
    [SerializeField] private bool autoPlay = true;
    [Tooltip("Loop the animation continuously between start and end time.")]
    [SerializeField] private bool loop = true;
    [Tooltip("Speed multiplier for playback. Use 1 for normal speed, 0.5 for half speed, 2 for double speed, -1 for reverse.")]
    [SerializeField] private float playbackSpeed = 1f;

    [Header("Time Range (Auto-fetched from AlembicStreamPlayer)")]
    [Tooltip("Override the automatic start/end time from AlembicStreamPlayer with custom values.")]
    [SerializeField] private bool useCustomRange = false;
    [Tooltip("Custom start time in seconds. Only used when Use Custom Range is enabled.")]
    [SerializeField] private float customStartTime = 0f;
    [Tooltip("Custom end time in seconds. Only used when Use Custom Range is enabled.")]
    [SerializeField] private float customEndTime = 1f;

    [Header("Runtime Info")]
    [Tooltip("Current playback time in seconds (read-only).")]
    [SerializeField] private float currentPlaybackTime;
    [Tooltip("Whether the animation is currently playing (read-only).")]
    [SerializeField] private bool isPlaying;

    private float startTime;
    private float endTime;

    void Start()
    {
        if (alembicPlayer == null)
        {
            alembicPlayer = GetComponent<AlembicStreamPlayer>();
        }

        if (alembicPlayer == null)
        {
            Debug.LogError($"AlembicStreamPlayer not found on {gameObject.name}. Please assign it in the Inspector.");
            enabled = false;
            return;
        }

        InitializeTimeRange();

        if (autoPlay)
        {
            Play();
        }
    }

    void Update()
    {
        if (!isPlaying || alembicPlayer == null)
            return;

        currentPlaybackTime += Time.deltaTime * playbackSpeed;

        if (loop)
        {
            if (playbackSpeed > 0 && currentPlaybackTime > endTime)
            {
                currentPlaybackTime = startTime;
            }
            else if (playbackSpeed < 0 && currentPlaybackTime < startTime)
            {
                currentPlaybackTime = endTime;
            }
        }
        else
        {
            currentPlaybackTime = Mathf.Clamp(currentPlaybackTime, startTime, endTime);

            if ((playbackSpeed > 0 && currentPlaybackTime >= endTime) ||
                (playbackSpeed < 0 && currentPlaybackTime <= startTime))
            {
                Stop();
            }
        }

        alembicPlayer.CurrentTime = currentPlaybackTime;
    }

    private void InitializeTimeRange()
    {
        if (useCustomRange)
        {
            startTime = customStartTime;
            endTime = customEndTime;
        }
        else
        {
            startTime = alembicPlayer.StartTime;
            endTime = alembicPlayer.EndTime;
        }

        currentPlaybackTime = startTime;
        alembicPlayer.CurrentTime = currentPlaybackTime;

        Debug.Log($"Alembic playback initialized: Start={startTime:F3}s, End={endTime:F3}s, Duration={endTime - startTime:F3}s");
    }

    public void Play()
    {
        isPlaying = true;
    }

    public void Pause()
    {
        isPlaying = false;
    }

    public void Stop()
    {
        isPlaying = false;
        currentPlaybackTime = startTime;
        alembicPlayer.CurrentTime = currentPlaybackTime;
    }

    public void Restart()
    {
        currentPlaybackTime = startTime;
        alembicPlayer.CurrentTime = currentPlaybackTime;
        Play();
    }

    public void SetTime(float time)
    {
        currentPlaybackTime = Mathf.Clamp(time, startTime, endTime);
        alembicPlayer.CurrentTime = currentPlaybackTime;
    }

    public void SetTimeNormalized(float normalizedTime)
    {
        float time = Mathf.Lerp(startTime, endTime, Mathf.Clamp01(normalizedTime));
        SetTime(time);
    }

    private void OnValidate()
    {
        if (alembicPlayer != null && Application.isPlaying)
        {
            InitializeTimeRange();
        }
    }
}

/* Docs:
 
✨ Features
    Auto-fetches Start/End Time from the AlembicStreamPlayer component
    Looping - Seamlessly loops between start and end
    Custom Range - Option to override with custom start/end times
    Playback Speed - Control speed (negative for reverse)
    Playback Controls - Play, Pause, Stop, Restart methods
    Runtime Info - Inspector shows current playback state

🎮 How to Use
    1. Add to RopeSkipper GameObject:
    Select your RopeSkipper GameObject
    Add Component → Search for AlembicLoopController
    The script will auto-detect the AlembicStreamPlayer

2. Inspector Settings:
    Auto Play: ✓ (starts playing on scene load)
    Loop: ✓ (loops continuously)
    Playback Speed: 1.0 (normal speed, try 0.5 for slow-mo, -1 for reverse)
    Use Custom Range: ☐ (uses AlembicStreamPlayer's StartTime/EndTime)

3. Control from Other Scripts:
    AlembicLoopController controller = GetComponent<AlembicLoopController>();

    controller.Play();
    controller.Pause();
    controller.Stop();
    controller.Restart();

    controller.SetTime(1.5f);
    controller.SetTimeNormalized(0.5f);

🔧 Advanced: Custom Range
    If you want to play only a portion of the animation:

    Check Use Custom Range
    Set Custom Start Time: 0.5 (start at 0.5 seconds)
    Set Custom End Time: 2.0 (end at 2.0 seconds)

📊 What You'll See
    Based on your current Alembic settings:

    Start Time: 0.0078125 seconds (~0.008s)
    End Time: 2.5859375 seconds (~2.586s)
    Duration: ~2.578 seconds
    The script will automatically loop this animation smoothly!
 
 */