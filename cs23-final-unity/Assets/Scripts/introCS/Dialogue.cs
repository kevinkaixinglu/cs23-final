using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Dialogue : MonoBehaviour
{
    [Header("Sprites")]
    public GameObject idleSprite;
    public GameObject activeSprite;

    public TextMeshProUGUI textComponent;
    public GameObject dialogueBox;   // panel that holds the text / portraits

    public string[] lines;
    public float textSpeed;
    private int index;

    [Header("Chirp Sounds")]
    public AudioSource chirp1;
    public AudioSource chirp2;
    public AudioSource chirp3;

    [Header("UI")]
    public GameObject continueButton; // button to show after line 7

    private Coroutine blinkRoutine;
    private Coroutine chirpRoutine;
    
    // NEW: are we waiting for the button instead of mouse click?
    private bool waitingForContinueButton = false;

    void Start()
    {
        textComponent.text = string.Empty;

        if (continueButton != null) continueButton.SetActive(false);
        if (dialogueBox != null) dialogueBox.SetActive(true);

        startDialogue();
    }

    void Update()
    {
        // Don't advance with mouse if we're waiting for the continue button
        if (waitingForContinueButton)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();

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

                activeSprite.SetActive(false);
                idleSprite.SetActive(true);

                textComponent.text = lines[index];
            }
        }
    }

    void startDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        // Whenever we start typing, we’re not waiting for a button
        waitingForContinueButton = false;

        if (continueButton != null) continueButton.SetActive(false);
        if (dialogueBox != null) dialogueBox.SetActive(true);

        blinkRoutine = StartCoroutine(BlinkSprites());
        chirpRoutine = StartCoroutine(PlayRandomChirps());

        foreach (char c in lines[index])
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

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

        idleSprite.SetActive(true);
        activeSprite.SetActive(false);

        // === SPECIAL CASE: AFTER LINE 7 (index 6) ===
        // Only if there ARE more lines to show after this one.
        if (index == 11 && index < lines.Length - 1)
        {
            waitingForContinueButton = true;

            if (dialogueBox != null) dialogueBox.SetActive(false);
            if (continueButton != null) continueButton.SetActive(true);
        }
    }

    IEnumerator BlinkSprites()
    {
        while (true)
        {
            idleSprite.SetActive(false);
            activeSprite.SetActive(true);
            yield return new WaitForSeconds(0.2f);

            idleSprite.SetActive(true);
            activeSprite.SetActive(false);
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

    // Called by your UI button's OnClick
    public void OnContinueButtonPressed()
    {
        if (!waitingForContinueButton)
            return;

        waitingForContinueButton = false;

        if (continueButton != null) continueButton.SetActive(false);
        if (dialogueBox != null) dialogueBox.SetActive(true);

        // Move to next line and start typing
        index++;
        textComponent.text = string.Empty;
        StartCoroutine(TypeLine());
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            // End of dialogue
            StartCoroutine(LoadNextScene());
        }
    }

    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(0.3f);
        SceneManager.LoadScene("LevelSelect");
    }
}
