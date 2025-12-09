using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour /*MonobehaviourSingleton<SoundManager>*/
{
    public static SoundManager Instance { get; private set; }

    [Header("Sound effects")]
    [SerializeField] List<AudioClip> cardSelectSfx;
    [SerializeField] List<AudioClip> punchSfx;
    [SerializeField] List<AudioClip> dumbbellSfx;

    [Header("Audio Settings")]
    [SerializeField] private float soundVolume = 1f;

   [SerializeField] private AudioSource sfxAudioSource;
    public /*override*/ void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

       /* sfxAudioSource = GetComponent<AudioSource>();

        if (sfxAudioSource == null)
        {
            sfxAudioSource = gameObject.AddComponent<AudioSource>();
        }*/

        sfxAudioSource.playOnAwake = false;
        sfxAudioSource.volume = soundVolume;
    }
    private void Start()
    {
        //InvokeRepeating(nameof(PlayDumbbellSound), 3f, 6f);
    }

    private void OnEnable()
    {
        GameEvents.BoxingDemoGameFlowEvents.CardSelected.Register(OnCardSelected);
    }

    private void OnDisable()
    {
        GameEvents.BoxingDemoGameFlowEvents.CardSelected.UnRegister(OnCardSelected);
    }

    private void OnCardSelected()
    {
        if (cardSelectSfx != null)
            PlaySound(cardSelectSfx[Random.Range(0, cardSelectSfx.Count - 1)]);
    }
    public void PlayPunchSound()
    {
        if (punchSfx != null)
            PlaySound(punchSfx[Random.Range(0, punchSfx.Count - 1)]);
    }
    public void PlayDumbbellSound()
    {
        if (dumbbellSfx != null)
            PlaySound(dumbbellSfx[Random.Range(0, dumbbellSfx.Count - 1)]);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && sfxAudioSource != null)
        {
            sfxAudioSource.PlayOneShot(clip, soundVolume);
        }
    }

}
