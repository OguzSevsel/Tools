using UnityEngine;
using Tools.DialogueSystem;
using System.Collections.Generic;

public class DialogManager : MonoBehaviour
{
    [SerializeField] private List<DSGraphSO> dialogues;

    public static DialogManager Instance { get; private set; }
    private DialogueElement dialogUI;
    private DialogueRunner runner;
    private DSGraphSO currentGraph;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        dialogUI = GameObject.FindGameObjectWithTag("Dialogue Element").GetComponent<DialogueElement>();
        StartDialogueByName("New Dialogue");
    }

    public void StartDialogueByName(string name)
    {
        currentGraph = null;
        currentGraph = dialogues.Find(d => d.name == name);
        if (currentGraph == null)
            return;
        runner = new DialogueRunner(currentGraph);
        runner.OnDialogueEvent += DialogueEventHandler;
        runner.StartDialogue();
    }

    public void StartDialogue(DSGraphSO graph)
    {
        currentGraph = null;
        currentGraph = dialogues.Find(d => d == graph);
        if (currentGraph == null)
            return;
        runner = new DialogueRunner(currentGraph);
        runner.OnDialogueEvent += DialogueEventHandler;
        runner.StartDialogue();
    }

    private void DialogueEventHandler(IDialogueEvent e)
    {
        switch (e)
        {
            case MessageEvent:
                MessageEvent messageEvent = (MessageEvent)e;
                dialogUI.ShowDialogText(messageEvent.DialogueText, messageEvent.ActorName, messageEvent.ActorSprite, (e) => messageEvent.Advance());
                break;
            case ChoiceEvent:
                ChoiceEvent choiceEvent = (ChoiceEvent)e;
                dialogUI.ShowChoicesText(choiceEvent.Choices, (evt) => choiceEvent.Advance(evt));
                break;
            case EndEvent:
                EndEvent endEvent = (EndEvent)e;
                runner.OnDialogueEvent -= DialogueEventHandler;
                StartDialogueByName(endEvent.DialogueId);
                if (currentGraph == null)
                {
                    dialogUI.EndDialog();
                }
                break;
            default:
                break;
        }
    }
}
