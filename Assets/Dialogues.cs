using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IEnumRunner.Transitions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Dialogues : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        panel_rt.localScale = hidden_scale;

    }
    Vector2 hidden_scale = Vector2.zero;
    private RectTransform panel_rt => panel.GetComponent<RectTransform>();
    
    // Update is called once per frame
    void Update()
    {
        
    }

    private bool running = false;
    public void StartDialogue(string filename)
    {
        if (running)
        {
            throw new Exception("Trying to run second dialogue");
        }
        string text = Resources.Load<TextAsset>("text/"+filename).text;

        List<string> lines = text.Split('\n').ToList();
        lines.RemoveAll(l => l.Trim() == "");
        
        StartCoroutine(DialogueStep(lines));
        
    }

    [SerializeField] private TextMeshProUGUI tmpro;
    [SerializeField] private Image panel;

    [SerializeField] private float change_time = .1f;

    void ShowBubbleTip(string speaker)
    {
        panel.transform.Find("bubbletips").GetComponentsInChildren<Image>().ToList().ForEach(i=>i.gameObject.SetActive(false));
        panel.transform.Find("bubbletips").Find(speaker)?.gameObject.SetActive(true);
    }

    [SerializeField] private bool dev_skip;
    IEnumerator DialogueStep(List<string> lines)
    {
        running = true;

        yield return new WaitForSeconds(1f);

        if (!dev_skip)
        {
            foreach (DialogueCommand line in lines.ConvertAll((l)=>new DialogueCommand(l)))
            {

                tmpro.text = line.text;

                panel_rt.sizeDelta = line.size;
                panel_rt.anchoredPosition = line.pos; 
                ShowBubbleTip(line.speaker);

                if (line.is_command)
                {
                    yield return line.ExecuteCommand();
                    continue;
                }
            
                yield return Make.The(panel_rt).In(change_time * .5f).ScaleTo(1f).Execute();
            
                yield return Common.WaitForKeyDown(KeyCode.Space);
                yield return null;
            
                yield return Make.The(panel_rt).In(change_time * .5f).ScaleTo(hidden_scale).Execute();
            }
        }
        
        running = false;

        yield return new WaitForSeconds(1f);
        
        EnvManager.inst.StartGame();
        

    }

    class DialogueCommand
    {
        public const float default_width = 573.7f;
        public const float default_height = 229.1f;

        public const float default_x = 0f;
        public const float default_y = 119.6f;
        
        public string text { get; protected set; }
        public string speaker { get; protected set; } = "";
        public float width { get; protected set; } = default_width;
        public float height { get; protected set; } = default_height;
        public float x { get; protected set; } = default_x;
        public float y { get; protected set; } = default_y;
        public Vector2 size => new Vector2(width, height);
        public Vector2 pos => new Vector2(x, y);
        private string[] parts;

        public IEnumerator ExecuteCommand()
        {
            string command = parts[0].Substring(1);
            string[] parameters = parts.ToList().GetRange(1, parts.Length-1).ToArray();
            switch (command)
            {
                case "music":
                    Game.music_player.PlayOnly(parameters[0]);
                    break;
                case "end":
                    Menu.End();
                    while (true)
                    {
                        yield return null;
                    }
            }
        }
        public bool is_command { get; protected set; }
        public DialogueCommand(string line)
        {
            parts = line.Split('|');
            if (parts[0][0] == '!')
            {
                is_command = true;
                return;
            }
            
            ApplyOnIndex((i)=>text=parts[i].Replace("\\n", "\n"), 0);
            ApplyOnIndex((i)=>speaker=parts[i], 1);
            ApplyOnIndex((i) =>{
                width = float.Parse(parts[i]);
            },2);
            
            ApplyOnIndex((i) =>{
                height = float.Parse(parts[i]);
            },3);
            ApplyOnIndex((i) =>{
                x = float.Parse(parts[i]);
            },4);
            
            ApplyOnIndex((i) =>{
                y = float.Parse(parts[i]);
            },5);
            
        }

        void ApplyOnIndex(System.Action<int> call, int index)
        {
            if (parts.Length > index && parts[index].Trim().Length > 0)
            {
                call(index);
            }
        }
        
        
    }
    
    
}
