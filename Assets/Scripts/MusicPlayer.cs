using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MusicPlayer : MonoBehaviour {
	[SerializeField]
	List<AudioClip> clips = new List<AudioClip>();
	List<AudioSource> tracks = new List<AudioSource>();

	public float fade_in_speed = 10f;
	public float fade_in_duration => BeatTracker.beat_time * 4; 
	float fis_inv, fid_inv;
	public float max_volume;
	public int running = 0;
	private Dictionary<string, AudioSource> a_sources_indexed = new Dictionary<string, AudioSource>();
	private Dictionary<string, float> clips_volumes = new Dictionary<string, float>();
	public float track_time => tracks[0].time;
	void Start(){
		fis_inv = 1f / fade_in_speed;
		fid_inv = 1f / fade_in_duration;
		foreach (AudioClip c in clips) {
			GameObject g = new GameObject ($"track {c.name}", new System.Type[]{ typeof(AudioSource) });
			g.transform.SetParent (transform);
			AudioSource t = g.GetComponent<AudioSource> ();
			t.clip = c;
			t.Stop ();
			t.time = 0f;
			t.loop = true;
			t.volume = 0f;
			t.pitch = 1f;
			t.Play ();
			tracks.Add (t);
			
			a_sources_indexed.Add(c.name, t);
			clips_volumes.Add(c.name, 1f);

		}
		
		
		
		StartCoroutine (SyncTracks ());
		//music_volume += Time.deltaTime * .2f * (alive && movement_speed > music_stars_start_speed ? 1 : -1);
		//music_volume = Mathf.Clamp (music_volume, 0f, 1f);
	}
	[SerializeField]
	float no_game_pitch = .05f, pitch_speed = .2f;
	IEnumerator SyncTracks()
	{
		yield break;
		while(true){
			yield return new WaitForSeconds (16f);
			float ftime = tracks [0].time;
			tracks.ForEach (delegate(AudioSource obj) {
				obj.time = ftime;	
			});
		}

	}
	void Update(){
		/*for (int i = 0; i < tracks.Count; i++) {
			this[i] = Mathf.MoveTowards(this[i], running > i || i == 0 ? max_volume : 0f, Time.deltaTime * fis_inv );
			this[i] = Mathf.Clamp (this[i], 0f, 1f);
		}*/
		tracks.ForEach(delegate(AudioSource obj) {
			obj.pitch = Mathf.MoveTowards(tracks[0].pitch, running == 0 ? no_game_pitch : 1f, Time.deltaTime * pitch_speed);
		} );
		foreach (KeyValuePair<string, float> kv in clips_volumes)
		{
			if (!a_sources_indexed.ContainsKey(kv.Key))
			{
				Debug.LogError($"missing track '{kv.Key}'");
			}
			a_sources_indexed[kv.Key].volume =
				Mathf.MoveTowards(a_sources_indexed[kv.Key].volume, kv.Value, Time.deltaTime * fid_inv);
		}
	}

	public void PlayOnly(string clips)
	{
		clips_volumes.ToList().ForEach(kv=>clips_volumes[kv.Key] = 0f);
		foreach (string clipname in clips.Split(';'))
		{
			if (clipname == "")
			{
				continue;
			}
			clips_volumes[clipname] = max_volume;
		}
	}
	
	float this[int i]{
		get{
			return tracks [i].volume;
		}
		set{
			tracks [i].volume = value;
		}
	}

	float this[string name]
	{
		get
		{
			return clips_volumes[name];
		}
		set
		{
			clips_volumes[name] = value;
		}
	}
	
	/*AudioSource _music;
	AudioSource music {
		get{
			if (_music == null) {
				_music = GameObject.Find ("music").GetComponent<AudioSource>();
			}
			return _music;
		}
	}
	float music_volume{
		get {
			return music.volume;
		}
		set{
			music.volume = value;
		}
	}*/


}