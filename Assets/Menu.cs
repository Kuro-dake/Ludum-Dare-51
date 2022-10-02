using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IEnumRunner;
using IEnumRunner.Transitions;
using UnityEngine;

using TMPro;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public bool active
    {
        get => gameObject.activeSelf;
        set => gameObject.SetActive(value);
    }

    [SerializeField] private CanvasGroup panel, text;
    // Start is called before the first frame update
    private bool ready = false;
    void Start()
    {
        text.alpha = 0f;
        panel.alpha = 1f;
        if (dev_skip)
        {
            active = false;
            EnvManager.inst.StartEnvironment();
        }
        Sequence s = Sequence.New();
        s +=Make.The(panel).ThenWait(1f).In(.5f).CGAlphaTo(0f);
        s +=Make.The(text).ThenWait(.5f).In(.5f).CGAlphaTo(1f).then.MakeHappen(()=>ready=true);

        s.Run();
        inst = this;
    }

    private static Menu inst;
    
    [SerializeField] private bool dev_skip = true;
    // Update is called once per frame

    private bool end = false;

    public static void End()
    {
        inst.EndInst();
    }

    void EndInst()
    {
        end = true;
        active = true;
        RectTransform rt = text.transform.Find("title").GetComponent<RectTransform>();
        rt.GetComponent<TextMeshProUGUI>().text = "The End!";
         
        rt = text.transform.Find("psts").GetComponent<RectTransform>();
         
        rt.GetComponent<TextMeshProUGUI>().text = "\n\n\n\nCreated by kuro@dizztal for Ludum Dare 51 compo\n\n 1. - 3. 10. 2022\n\nThank you for playing!";

        StartCoroutine(EndStep());

    }

    IEnumerator EndStep()
    {
        panel.alpha = 0f;
        text.alpha = 0f;
        yield return Make.The(panel).In(2f).CGAlphaTo(1f).Execute();
        yield return new WaitForSeconds(.5f);
        
        yield return Make.The(text).In(2f).CGAlphaTo(1f).Execute();

        yield return new WaitForSeconds(15f);
        Game.music_player.PlayOnly("");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(0);
    }
    void Update()
    {
        
        RectTransform rt = text.transform.Find("title").GetComponent<RectTransform>();
        float pos = Mathf.Sin(Time.time * .25f) * 50f + 207.3f;
        
        rt.anchoredPosition = Vector2.up * pos;
        
        RectTransform pst = text.transform.Find("psts").GetComponent<RectTransform>();
        pst.anchoredPosition = Vector2.up * (Mathf.Sin(Time.time * .5f) * 25f - 4.5f);

        if (end)
        {
            return;
        }
        
        if (!ready)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Make.The(text).In(.5f).CGAlphaTo(0f).then.MakeHappen(() => active = false).Happen();
            EnvManager.inst.StartEnvironment();
        }

        
        
        
    }
}
