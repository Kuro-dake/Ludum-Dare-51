using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
public class BeatTracker : MonoBehaviour
{
    // Start is called before the first frame update
    private int beat_count = 0;
    private float last_beat_time = 0;
    public const float beat_time = .46875f;
    [SerializeField] private MusicPlayer mp;
    private float source_time => mp.track_time;
    private int total_count;
    public static int total_beats => inst.total_count;

    public static string tempo = "8tempo";
    
    private static BeatTracker inst;
    void Start()
    {
        inst = this;
    }

    public static event System.EventHandler<OnBeatArgs> onBeat; 

    // Update is called once per frame
    void Update()
    {

        if (last_beat_time > source_time)
        {
            beat_count = 0;
            
        }
        
        if (source_time > beat_count * beat_time)
        {
           
            onBeat?.Invoke(this, new OnBeatArgs(total_count));
            beat_count++;
            total_count++;
//            Debug.Log(total_count);
        }

        last_beat_time = source_time;
    }
    
    
}

public class OnBeatArgs : System.EventArgs
{
    protected int beat_number;
    

    public BeatValidator validator { get; protected set; }
    
    public int GetBeatNumber(int every = -1)
    {
        return (every == -1 ? beat_number : beat_number % every);
    }

    
    public OnBeatArgs(int beatNumber)
    {
        beat_number = beatNumber;
        validator = BeatValidator.GetCurrentValidator(beatNumber);
    }
}
