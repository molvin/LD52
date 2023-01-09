using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    private Queue<AudioSource> m_AudioQueue = new Queue<AudioSource>();
    private List<AudioSource> m_ActiveAudios = new List<AudioSource>();
    public AudioMixerGroup MasterGroup;

    // Start is called before the first frame update
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            for(int i = 0 ; i < 10; i++)
                m_AudioQueue.Enqueue(AddAudioSource());
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
      for(int i = m_ActiveAudios.Count - 1; i >= 0; i--)
      {
        if(!m_ActiveAudios[i].isPlaying)
        {
            AudioSource audioSource = m_ActiveAudios[i];
            m_AudioQueue.Enqueue(audioSource);
            audioSource.spatialBlend = 0.5f;
            audioSource.loop = false;
            m_ActiveAudios.RemoveAt(i);
        }  
      }
    }

    public void PlayAudio(AudioClip clip)
    {
        PlayAudio(clip, Camera.main.transform.position);
    }

    public void PlayAudio(AudioClip clip, Vector3 pos)
    {
        if(!clip)
            return;

        if(m_AudioQueue.Count == 0)
            m_AudioQueue.Enqueue(AddAudioSource());

        AudioSource audioSource = m_AudioQueue.Dequeue();
        audioSource.clip = clip;
        audioSource.Play();
        audioSource.transform.position = pos;

        audioSource.pitch = Random.Range(0.95f, 1.05f);
        m_ActiveAudios.Add(audioSource);
    }

    public void PlayMusic(AudioClip input_clip)
    {
        if(!input_clip)
            return;

        foreach(AudioSource audio_source in m_ActiveAudios)
        {
            if(audio_source.clip == input_clip)
               return;
        }

        if(m_AudioQueue.Count == 0)
            m_AudioQueue.Enqueue(AddAudioSource());

        AudioSource audioSource = m_AudioQueue.Dequeue();

        audioSource.pitch = 1;
        audioSource.clip = input_clip;
        audioSource.loop = true;
        audioSource.transform.position = Camera.main.transform.position;
        audioSource.spatialBlend = 0;

        StartCoroutine(FadeMusic(audioSource, 2.5f, true));

        m_ActiveAudios.Add(audioSource);
    }

    public void StopMusic(AudioClip clip)
    {
        foreach(AudioSource audio_source in m_ActiveAudios)
        {
            if(audio_source.clip == clip)
                StartCoroutine(FadeMusic(audio_source, 2, false));
        }
    }

    IEnumerator FadeMusic(AudioSource audio_source, float fade_time, bool fade_in)
    {
        float timer = fade_in ? 0 : fade_time;

        if(fade_in)
        {
            audio_source.Play();
            audio_source.volume = 0f;
        }

        while(fade_in && timer < fade_time || !fade_in && timer > 0f)
        {
            timer += fade_in ? Time.deltaTime : -Time.deltaTime;
            
            audio_source.volume =  timer / fade_time;
            yield return 0;
        }

        if(!fade_in)
            audio_source.Stop();

        yield return 0;
    }

    AudioSource AddAudioSource()
    {
        GameObject new_game_object = new GameObject();
        DontDestroyOnLoad(new_game_object);
        AudioSource audioSource = new_game_object.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0.5f;
        audioSource.maxDistance = 1000f;
        audioSource.minDistance = 10f;
        audioSource.outputAudioMixerGroup = MasterGroup;
        return audioSource;
    }

}
