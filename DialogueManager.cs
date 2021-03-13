using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private Queue<string> sentences;
    private GameObject textDisplay;

    public Text nameText;
    public Text dialogue;

    public void Init()
    {
        sentences = new Queue<string>();
        textDisplay = GameObject.Find("Text Display");
        textDisplay.SetActive(false);
    }

    public void StartDialogue(Dialogue dialogue)
    {
        // Debug.Log("Showing text from " + dialogue.name);
        nameText.text = dialogue.name;

        sentences.Clear();

        textDisplay.SetActive(true);
        textDisplay.transform.SetAsLastSibling();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if(sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        Debug.Log(sentence);
        dialogue.text = sentence;
    }

    void EndDialogue()
    {
        textDisplay.SetActive(false);
        // Debug.Log("end of dialogue");
    }
}
