using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager> {

    // Audio players components
    public AudioSource EffectsSource;
    public AudioSource MusicSource;

    // Random pitch adjustment range
    public float LowPitchRange = 0.95f;
    public float HighPitchRange = 1.05f;

    // Play a single clip through the sound effects source
    public void Play(AudioClip clip) {
        EffectsSource.clip = clip;
        EffectsSource.Play();
    }
    public void Play(AudioClip clip, float volume) {
        EffectsSource.clip = clip;
        EffectsSource.volume = volume;
        EffectsSource.Play();
    }
    // Play a single clip through the music source
    public void PlayMusic(AudioClip clip) {
        MusicSource.clip = clip;
        MusicSource.Play();
    }
    public void PlayMusic(AudioClip clip, float volume) {
        MusicSource.clip = clip;
        MusicSource.volume = volume;
        MusicSource.Play();
    }
    // Pause a single clip through the sound effects source
    public void Pause() {
        EffectsSource.Pause();
    }
    // Play a single clip through the music source
    public void PauseMusic() {
        MusicSource.Pause();
    }
    // Stop a single clip through the sound effects source
    public void Stop() {
        EffectsSource.Stop();
    }
    // Stop a single clip through the music source
    public void StopMusic() {
        MusicSource.Stop();
    }

    // Play a random clip from an array, and randomize the pitch slightly
    public void RandomSoundEffect(params AudioClip[] clips) {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(LowPitchRange, HighPitchRange);

        EffectsSource.pitch = randomPitch;
        EffectsSource.clip = clips[randomIndex];
        EffectsSource.Play();
    }


}
