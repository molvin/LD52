using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    private Queue<AudioSource> m_AudioQueue = new Queue<AudioSource>();
    private List<AudioSource> m_ActiveAudios = new List<AudioSource>();
 
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

        audioSource.pitch = Random.Range(0.8f, 1.2f);
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
        audioSource.Play();
        audioSource.loop = true;
        audioSource.transform.position = Camera.main.transform.position;
        audioSource.spatialBlend = 0;
        m_ActiveAudios.Add(audioSource);
    }

    public void StopMusic(AudioClip clip)
    {
        foreach(AudioSource audio_source in m_ActiveAudios)
        {
            if(audio_source.clip == clip)
                audio_source.Stop();
        }
    }

    AudioSource AddAudioSource()
    {
        GameObject new_game_object = new GameObject();
        DontDestroyOnLoad(new_game_object);
        AudioSource audioSource = new_game_object.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0.5f;
        audioSource.maxDistance = 1000f;
        audioSource.minDistance = 10f;
        return audioSource;
    }

}
