using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "AudioManager", menuName = "Manager/AudioManager", order = 0)]
public class AudioManager : Service
{

    public override void GameStartInitialize()
    {
        base.GameStartInitialize();
        DontDestroyOnLoad(service_transform);
        music_track_name = null;
    }

    public void OnCharacterHit(object sender, System.EventArgs args)
    {
        PlayResource("hit", 1f, new FloatRange(1f, 1.2f));
    }


    public List<NamedAudioClip> clips = new List<NamedAudioClip>();
    AudioSource music_track;
    public string music_track_name { get; protected set; }
    public bool already_playing_music => music_track != null && music_track.isPlaying;
    public AudioSource PlayMusic(string _music_track_name = "", float volume = 1f, float fadeout_duration = 2f)
    {

        if (already_playing_music)
        {
            if (_music_track_name == music_track_name)
            {
                return music_track;
            }
            else
            {
                FadeOutSource(music_track, fadeout_duration);
            }

        }
        if (_music_track_name != "")
        {
            music_track_name = _music_track_name;
        }
        music_track = PlayResource("Music/" + music_track_name, volume);

        if (music_track != null)
        {
            music_track.loop = true;
            music_track.priority = 255;
            if (already_playing_music) { FadeInSource(music_track, fadeout_duration, volume); }
        }

        return music_track;
    }

    public void StopMusic(float fadeout_duration)
    {
        if (music_track == null)
        {
            return;
        }
        FadeOutSource(music_track, fadeout_duration);
        music_track_name = "";
    }


    List<Pair<string, List<int>>> random_resource_queue = new List<Pair<string, List<int>>>();
    public AudioSource PlayRandomResource(string resource_name, IntRange numbers = null, float volume = .3f, FloatRange pitch = null, string format = "{0}{1}")
    {

        Pair<string, List<int>> res_q = random_resource_queue.Find(delegate (Pair<string, List<int>> rq) { return rq.first == resource_name; });

        if (res_q == null)
        {
            res_q = new Pair<string, List<int>>(resource_name, new List<int>());
            random_resource_queue.Add(res_q);
        }
        if (res_q.second.Count == 0)
        {
            for (int i = numbers.min; i <= numbers.max; i++)
            {
                res_q.second.Add(i);
            }
        }
        int clip_num = res_q.second[Random.Range(0, res_q.second.Count)];

        res_q.second.Remove(clip_num);
        resource_name = string.Format(format, resource_name, clip_num.ToString());
        //Debug.Log(resource_name);
        return PlayResource(resource_name, volume, pitch);
    }

    public AudioSource PlayResource(string resource_name, float volume = .3f, FloatRange pitch = null)
    {
        AudioClip ac = Resources.Load<AudioClip>("Audio/" + resource_name);
        return PlayTrack(ac, volume, pitch);
    }
    public AudioSource PlaySound(string sound_name, float volume = .3f, FloatRange pitch = null)
    {
        return PlayTrack(clips.Find(delegate (NamedAudioClip ac)
        {
            return ac.first == sound_name;
        }).second, volume * .5f, pitch);
    }

    public int max_sources = 5;

    public AudioSource PlayTrack(AudioClip ac, float volume = 1f, FloatRange pitch = null)
    {
        if (!active) { return null; }

        pitch = pitch == null ? new FloatRange(1f, 1f) : pitch;

        AudioSource c_as = null;
        foreach (AudioSource asc in service_transform.GetComponents<AudioSource>())
        {
            if (!asc.isPlaying)
            {
                Destroy(asc);
            }
        }
        if (c_as == null && service_transform.GetComponents<AudioSource>().Length < max_sources)
        {
            c_as = service_transform.gameObject.AddComponent<AudioSource>();
        }
        if (c_as != null)
        {
            c_as.clip = ac;
            c_as.pitch = pitch;
            c_as.time = 0f;
            c_as.loop = false;
            c_as.volume = volume;
            c_as.Play();
            return c_as;
        }
        else
        {
            //Debug.Log("Too many clips playing");
            return null;
            //throw new UnityException("Too many AudioSources.");
        }
    }

    public Coroutine FadeOutSource(AudioSource asc, float duration = 1f)
    {
        return StartCoroutine(FadeOutSourceStep(asc, duration));
    }

    public Coroutine FadeInSource(AudioSource asc, float duration = 1f, float volume = 1f)
    {
        if (!asc.isPlaying)
        {
            asc.Play();
        }
        asc.volume = 0f;
        return StartCoroutine(FadeOutSourceStep(asc, duration, volume));
    }

    IEnumerator FadeOutSourceStep(AudioSource asc, float duration, float target = 0f)
    {

        float starting_volume = asc.volume;

        float current = 0f;
        while (current < 1f)
        {
            current += Time.deltaTime / duration;
            if (asc == null)
            {
                yield break;
            }
            asc.volume = Mathf.Lerp(starting_volume, target, current);
            yield return null;
        }

        if (asc != null && target == 0f)
        {
            asc.Stop();
        }
    }


    

}
