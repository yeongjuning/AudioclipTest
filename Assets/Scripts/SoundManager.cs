using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public enum FormatType
    {
        PCM_MP3,
        Vorbis_MP3,
        ADPCM_MP3,
        PCM_OGG,
        Vorbis_OGG,
        ADPCM_OGG,
    }

    public enum ButtonType
    {
        Play,
        Pause,
        AfterFiveMinute,
        PreFiveMinute
    }

    [SerializeField] AudioSource[] audioSources;
    [SerializeField] Slider sliderPlayState;
    [SerializeField] Button[] btnControl;
    [SerializeField] Text goPlayText;

    AudioSource curSelectedAudioSource = null;

    bool isPlaying = false;
    bool isPause = false;

    void Awake()
    {
        Init();
    }

    private void Start()
    {
        
    }

    public void Init()
    {
        for (int i = 0; i < audioSources.Length; i++) audioSources[i].playOnAwake = false;
        for (int i = 0; i < btnControl.Length; i++) btnControl[i].gameObject.SetActive(true);

        btnControl[(int)ButtonType.Pause].gameObject.SetActive(false);
        sliderPlayState.value = 0f;
        goPlayText.text = "";
    }

    IEnumerator Co_PlayAudio()
    {
        var clip = curSelectedAudioSource.clip;
        while ((isPlaying || isPause) && (sliderPlayState.value < sliderPlayState.maxValue))
        {
            if (isPause) 
                yield return Co_Pause();

            var clipPlayTime = clip.length - (clip.length - curSelectedAudioSource.time);
            sliderPlayState.value = clipPlayTime;

            yield return null;
        }

        Debug.Log("Play Complate");
        isPlaying = false;
        isPause = false;

        sliderPlayState.value = 0f;
        PlayOrPauseForActive(false);
    }

    IEnumerator Co_Pause()
    {
        curSelectedAudioSource.Pause();
        while (isPause)
        {
            if (isPlaying)
            {
                curSelectedAudioSource.UnPause();
                isPause = !isPause;
                yield return Co_PlayAudio();
            } 
            yield return null;
        }
    }

    #region On Click Event

    public void OnClickSelected(int index)
    {
        Debug.Log($"Selected Index : {index}");
        if (!isPlaying)
        {
            curSelectedAudioSource = audioSources[index];
            goPlayText.text = curSelectedAudioSource.gameObject.name;
        }
    }

    public void OnClickPlayAudio()
    {
        if (curSelectedAudioSource == null) return;

        sliderPlayState.value = curSelectedAudioSource.time;
        sliderPlayState.maxValue = curSelectedAudioSource.clip.length;

        isPlaying = true;
        PlayOrPauseForActive(isPlaying);
        curSelectedAudioSource.Play();
        goPlayText.text = curSelectedAudioSource.gameObject.name;

        StartCoroutine(Co_PlayAudio());
    }

    public void OnClickPause()
    {
        isPause = true;
        isPlaying = false;
        PlayOrPauseForActive(false);
    }

    public void OnClickStop()
    {
        curSelectedAudioSource.Stop();
        sliderPlayState.value = 0f;
        isPlaying = false;
        if (isPause) isPause = false;
        PlayOrPauseForActive(isPlaying);
    }

    public void OnClickPreFiveMinute()
    {
        sliderPlayState.value -= 1f;
        curSelectedAudioSource.time = sliderPlayState.value;
    }

    public void OnClickAfterFiveMinute()
    {
        sliderPlayState.value += 1f;
        curSelectedAudioSource.time = sliderPlayState.value;
    }
    #endregion

    void PlayOrPauseForActive(bool isPlaying)
    {
        btnControl[(int)ButtonType.Play].gameObject.SetActive(!isPlaying);
        btnControl[(int)ButtonType.Pause].gameObject.SetActive(isPlaying);
    }

    public void OnValueChangedSlider()
    {
        curSelectedAudioSource.time = sliderPlayState.value;
    }
}
