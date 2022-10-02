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
    private static EnvManager inst;
    [SerializeField] private SpriteMask game_mask;
    void Start()
    {
        SC.Initialize();
        inst = this;
        //StartGame();
        Player.onFall += OnFall;
        Player.inst.transform.position = current_env.bunni_point.position;
        Game.onDestinationReached += OnDestinationReached;
    }

    void OnDestinationReached(object sender, object args)
    {
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
        Game.music_player.PlayOnly("slow_base;fast_drums;slow_oboe;magic;");
        // 3f
        yield return Make.The(player).In(1f).MoveTo((Vector2)Camera.main.transform.position + Vector2.up * .4f ).Execute();
        // .5f
        Make.The(player).In(.3f).FixedTransition().MoveBy(Vector2.up * .4f).Happen();
        game_env.SetActive(true);
        Game.inst.StartGame();
        
        yield return new WaitForSeconds(.3f);
        yield return Make.The(game_mask).In(.4f).ScaleTo(31f).Execute();
        current_env.gameObject.SetActive(false);
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
        if (switch_game_routine != null)
        {
            return;
        }

        StartCoroutine(EndGameStep());
    }

    public static LandEnvironment current_environment => inst.current_env;
    [SerializeField] private LandEnvironment current_env;
    [SerializeField] private GameObject game_env;
    IEnumerator EndGameStep()
    {
        Game.inst.EndGame();
        Game.music_player.PlayOnly("slow_base;fast_drums;slow_oboe;magic");
        cam_follow_player = false;
        Player.inst.waddler.blobber = true;
        Player.inst.hp = -1;
        current_env.transform.position = (Vector2)Game.inst.transform.position;
        current_env.gameObject.SetActive(true);
        yield return Make.The(player).In(3f).MoveTo((Vector2)current_env.bunni_point.transform.position + Vector2.up * 4.3f ).Execute();
        yield return Make.The(game_mask).In(.4f).ScaleTo(0f).Execute();
        Game.game_started = false;

        yield return Make.The(player).In(1f).MoveTo((Vector2)current_env.bunni_point.transform.position ).Execute();
        game_env.SetActive(false);
        switch_game_routine = null;
        game_started = false;
        Game.music_player.PlayOnly("slow_base;slow_drums;slow_oboe;slow_squeak;magic");
    }
    
    private bool game_started = false;
    // Update is called once per frame
    void Update()
    {
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
    
}
