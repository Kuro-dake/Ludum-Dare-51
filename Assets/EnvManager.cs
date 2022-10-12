using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
using IEnumRunner;
using IEnumRunner.Transitions;

public class EnvManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static EnvManager inst;
    [SerializeField] private SpriteMask game_mask;
    void Start()
    {
        SC.Initialize();
        inst = this;
        //StartGame();
        Player.onFall += OnFall;
        Player.inst.transform.position = current_environment.bunni_point.position;
        Game.onDestinationReached += OnDestinationReached;
        environments.ForEach(e=>e.gameObject.SetActive(false));
        current_environment.gameObject.SetActive(true);
        Bird.inst.ResetBird();
    }

    void OnDestinationReached(object sender, object args)
    {
        current_environment_index++;
        Bird.inst.transform.position = new Vector2(13.189f, 8.59f);
        EndGame();
    }
    
    private void OnFall(object sender, object args)
    {
        if (Player.inst.out_of_hp)
        {
            EndGame();
        }
    }

    [SerializeField] private CinemachineVirtualCamera vcam, vcam_follow;
    private bool camFollowPlayer;

    public static bool cam_follow_player
    {
        get => inst.camFollowPlayer;
        set
        {
            inst.camFollowPlayer = value;
            inst.vcam.Priority = value ? 0 : 1;
            inst.vcam_follow.Priority = value ? 1 : 0;
        }
    }
    private Player player => Player.inst;

    private Coroutine switch_game_routine;
    
    public void StartGame()
    {
        if (switch_game_routine != null)
        {
            return;
        }

        switch_game_routine = StartCoroutine(StartGameStep());
    }

    IEnumerator StartGameStep()
    {
        //Game.music_player.PlayOnly("slow_base;fast_drums;slow_oboe;magic;");
        // 3f
        yield return Make.The(player).In(1f).MoveTo((Vector2)current_environment.bunni_point.transform.position + Vector2.up * 4.3f ).Execute();
        //yield return Make.The(player).In(1f).MoveTo((Vector2)Camera.main.transform.position + Vector2.up * .4f ).Execute();
        // .5f
        Make.The(player).In(.3f).FixedTransition().MoveBy(Vector2.up * .4f).Happen();
        game_env.SetActive(true);
        Game.inst.StartGame();
        
        yield return new WaitForSeconds(.3f);
        yield return Make.The(game_mask).In(.4f).ScaleTo(36f).Execute();
        current_environment.gameObject.SetActive(false);
        //yield return new WaitForSeconds(1f);
        // .4f
        yield return Make.The(player).In(.2f).MoveTo((Vector2)Game.inst.transform.position ).Execute();
        //yield return new WaitForSeconds(.2f);
        
        cam_follow_player = true;
        Player.inst.hp = 3;
        
        Game.inst.SetStartCountdown();
        
        
        Player.inst.waddler.blobber = false;
        switch_game_routine = null;
        game_started = true;

    }

    public void EndGame()
    {
        Bird.inst.ResetBird();
        if (switch_game_routine != null)
        {
            return;
        }

        StartCoroutine(EndGameStep());
    }

    public static LandEnvironment current_environment => inst.environments[inst.current_environment_index];
    [SerializeField] public List<LandEnvironment> environments;
    private int current_environment_index = 4;
    
    [SerializeField] private GameObject game_env;
    IEnumerator EndGameStep()
    {
        Game.inst.EndGame();
        Player.inst.rotation_right = true;
        Game.music_player.PlayOnly("slow_base;fast_drums;slow_oboe;magic");
        cam_follow_player = false;
        Player.inst.waddler.blobber = true;
        Player.inst.hp = -1;
        current_environment.transform.position = (Vector2)Game.inst.transform.position;
        current_environment.gameObject.SetActive(true);
        yield return Make.The(player).In(3f).MoveTo((Vector2)current_environment.bunni_point.transform.position + Vector2.up * 4.3f ).Execute();
        game_mask.transform.position = Player.inst.transform.position;
        yield return Make.The(game_mask).In(.4f).ScaleTo(0f).Execute();
        Game.game_started = false;

        yield return Make.The(player).In(1f).MoveTo((Vector2)current_environment.bunni_point.transform.position ).Execute();
        game_env.SetActive(false);
        switch_game_routine = null;
        game_started = false;
        Game.music_player.PlayOnly("slow_base;slow_bass;slow_drums;slow_oboe;slow_squeak;magic");
        yield return new WaitForSeconds(1f);
        dialogues.StartDialogue(current_environment.next_dialogue);

    }
    
    private bool game_started = false;
    // Update is called once per frame
    void Update()
    {
        return;
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!game_started)
            {
                StartGame();
            }
            else
            {
                EndGame();
            }
            
        }
    }

    [SerializeField] private Dialogues dialogues;
    public void StartEnvironment()
    {
        Bird.inst.ResetBird();
        dialogues.StartDialogue(current_environment.next_dialogue);
    }
    
}
