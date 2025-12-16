using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class endDialogue : MonoBehaviour
{
    [Header("Sprites")]
    public GameObject idleSprite;
    public GameObject activeSprite;

    public TextMeshProUGUI textComponent;
    public GameObject dialogueBox;
    public GameObject skipBox;
    public Button exitButton;

    public string[] lines;
    public float textSpeed;
    private int index;

    [Header("Chirp Sounds")]
    public AudioSource chirp1;
    public AudioSource chirp2;
    public AudioSource chirp3;

    private Coroutine blinkRoutine;
    private Coroutine chirpRoutine;
    private Coroutine typeRoutine;

    private bool isTyping = false;

    void Start()
    {
        textComponent.text = string.Empty;

        if (dialogueBox != null) dialogueBox.SetActive(true);
        if (skipBox != null) skipBox.SetActive(true);
        if (exitButton != null) exitButton.gameObject.SetActive(false);

        StartDialogue();
    }

    void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        // If we're still typing, clicking should instantly finish THIS line.
        if (isTyping)
        {
            FinishLineInstant();
            return;
        }

        // If line is already fully shown, clicking advances
        if (textComponent.text == lines[index])
        {
            NextLine();
        }
    }

    void StartDialogue()
    {
        index = 0;
        StartTypingCurrentLine();
    }

    void StartTypingCurrentLine()
    {
        textComponent.text = string.Empty;

        if (dialogueBox != null) dialogueBox.SetActive(true);
        if (skipBox != null) skipBox.SetActive(true);

        typeRoutine = StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        isTyping = true;

        blinkRoutine = StartCoroutine(BlinkSprites());
        chirpRoutine = StartCoroutine(PlayRandomChirps());

        foreach (char c in lines[index])
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        StopBlinkAndChirp();

        isTyping = false;

        
        if (index == lines.Length - 1)
        {
            yield return new WaitForSeconds(2f);
            HideDialogueUI();
        }
    }

    void FinishLineInstant()
    {

        if (typeRoutine != null) StopCoroutine(typeRoutine);
        typeRoutine = null;

        StopBlinkAndChirp();

        isTyping = false;

        textComponent.text = lines[index];


        if (index == lines.Length - 1)
        {
            HideDialogueUI();
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            StartTypingCurrentLine();
        }
        // else: do nothing (we already hide when final line completes)
    }

    void HideDialogueUI()
    {
        if (dialogueBox != null) dialogueBox.SetActive(false);
        if (skipBox != null) skipBox.SetActive(false);

        if (exitButton != null) exitButton.gameObject.SetActive(true);
    }

    void StopBlinkAndChirp()
    {
        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
            blinkRoutine = null;
        }

        if (chirpRoutine != null)
        {
            StopCoroutine(chirpRoutine);
            chirpRoutine = null;
        }

        if (activeSprite != null) activeSprite.SetActive(false);
        if (idleSprite != null) idleSprite.SetActive(true);
    }

    IEnumerator BlinkSprites()
    {
        while (true)
        {
            if (idleSprite != null) idleSprite.SetActive(false);
            if (activeSprite != null) activeSprite.SetActive(true);
            yield return new WaitForSeconds(0.2f);

            if (idleSprite != null) idleSprite.SetActive(true);
            if (activeSprite != null) activeSprite.SetActive(false);
            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator PlayRandomChirps()
    {
        AudioSource[] chirps = { chirp1, chirp2, chirp3 };

        while (true)
        {
            AudioSource c = chirps[Random.Range(0, chirps.Length)];
            if (c != null) c.Play();

            yield return new WaitForSeconds(Random.Range(0.08f, 0.15f));
        }
    }

    public void LoadLevelSelect()
    {
        SceneManager.LoadScene("LevelSelect");
    }
}
