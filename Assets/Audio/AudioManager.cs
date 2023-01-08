using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    private Queue<AudioSource> m_AudioQueue = new Queue<AudioSource>();
    private List<AudioSource> m_ActiveAudios = new List<AudioSource>();
    private 
    // Start is called before the first frame update
    void Start()
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
      for(int i = m_ActiveAudios.Count - 1; i != -1; i--)
      {
        if(!m_ActiveAudios[i].isPlaying)
        {
            m_AudioQueue.Enqueue(m_ActiveAudios[i]);
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
        if(m_AudioQueue.Count == 0)
            m_AudioQueue.Enqueue(AddAudioSource());

        AudioSource audioSource = m_AudioQueue.Dequeue();

        audioSource.PlayOneShot(clip);
        audioSource.transform.position = pos;

        audioSource.pitch = Random.Range(0.8f, 1.2f);
    }

    AudioSource AddAudioSource()
    {
        GameObject new_game_object = new GameObject();
        return new_game_object.AddComponent<AudioSource>();
    }

}
