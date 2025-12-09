using System.Collections;
using UnityEngine;

public class GymAmbientSounds : MonoBehaviour
{
    [Header("Gym Ambient Sound Effects")]
    [SerializeField] private AudioClip[] punchBagSounds;
    [SerializeField] private AudioClip[] dumbbellSounds;
    [SerializeField] private AudioClip[] crowdChatterSounds;
    [SerializeField] private AudioClip[] boxingRingSounds;
    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField] private AudioClip[] gruntsAndBreathingSounds;

    [Header("Ambient Settings")]
    [SerializeField] private float minTimeBetweenSounds = 0.5f;
    [SerializeField] private float maxTimeBetweenSounds = 3f;
    [SerializeField] private float ambientVolume = 0.6f;
    [SerializeField] private int maxSimultaneousSounds = 5;

    private AudioSource ambientSource;
    private int currentPlayingCount = 0;

    private void Awake()
    {
        ambientSource = gameObject.AddComponent<AudioSource>();
        ambientSource.playOnAwake = false;
        ambientSource.volume = ambientVolume;
    }

    private void Start()
    {
        StartCoroutine(PlayRandomGymAmbience());
    }

    private IEnumerator PlayRandomGymAmbience()
    {
        while (true)
        {
            float waitTime = Random.Range(minTimeBetweenSounds, maxTimeBetweenSounds);
            yield return new WaitForSeconds(waitTime);

            if (currentPlayingCount < maxSimultaneousSounds)
            {
                PlayRandomGymSound();
            }
        }
    }

    private void PlayRandomGymSound()
    {
        AudioClip clipToPlay = GetRandomClipFromAllCategories();

        if (clipToPlay != null)
        {
            float randomVolume = Random.Range(ambientVolume * 0.7f, ambientVolume);
            ambientSource.PlayOneShot(clipToPlay, randomVolume);

            StartCoroutine(TrackSoundDuration(clipToPlay.length));
        }
    }

    private AudioClip GetRandomClipFromAllCategories()
    {
        int categoryCount = 6;
        int randomCategory = Random.Range(0, categoryCount);

        switch (randomCategory)
        {
            case 0:
                return GetRandomClip(punchBagSounds);
            case 1:
                return GetRandomClip(dumbbellSounds);
            case 2:
                return GetRandomClip(crowdChatterSounds);
            case 3:
                return GetRandomClip(boxingRingSounds);
            case 4:
                return GetRandomClip(footstepSounds);
            case 5:
                return GetRandomClip(gruntsAndBreathingSounds);
            default:
                return null;
        }
    }

    private AudioClip GetRandomClip(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0)
            return null;

        return clips[Random.Range(0, clips.Length)];
    }

    private IEnumerator TrackSoundDuration(float duration)
    {
        currentPlayingCount++;
        yield return new WaitForSeconds(duration);
        currentPlayingCount--;
    }
}

/* How It Works:
    One AudioSource plays all the sounds using PlayOneShot() which allows overlapping
    Random intervals between 0.5-3 seconds (configurable)
    Random selection from all your gym sound categories
    Random volume variation to make it feel more natural
    Limits simultaneous sounds to prevent audio chaos (max 5 by default)

Setup in Unity:
    Create a GameObject called "GymAmbient"
    Attach this script
    In the Inspector, expand each array and add your sound clips:
    Punch Bag Sounds: multiple punch/hit variations
    Dumbbell Sounds: metal clanking, weights dropping
    Crowd Chatter: people talking, mumbling
    Boxing Ring Sounds: bell, rope movement
    Footstep Sounds: gym floor footsteps
    Grunts and Breathing: workout efforts

Tips:
    More clips = better variety (add 3-5 variations per category)
    Adjust timing for your desired crowd density
    Lower volume (0.4-0.6) so it doesn't overpower game sounds
    This uses one AudioSource and works perfectly!
 
 
 */

/* 
    * Punching bag: https://pixabay.com/users/freesound_community-46691455/?utm_source=link-attribution&utm_medium=referral&utm_campaign=music&utm_content=38481
    * Dumbbell: https://pixabay.com/users/freesound_community-46691455/?utm_source=link-attribution&utm_medium=referral&utm_campaign=music&utm_content=105592
    * Card Select: https://pixabay.com/users/freesound_community-46691455/?utm_source=link-attribution&utm_medium=referral&utm_campaign=music&utm_content=35956
 */