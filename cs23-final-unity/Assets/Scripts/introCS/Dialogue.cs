using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    [Header("Sprites")]
    public GameObject idleSprite;
    public GameObject activeSprite;

    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;
    private int index;
    public AudioSource chirp1;
    public AudioSource chirp2;
    public AudioSource chirp3;
    

    private Coroutine blinkRoutine;
    private Coroutine chirpRoutine;

    void Start()
    {
        textComponent.text = string.Empty;
        startDialogue();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                
                // stop blinking if skipping
                if (blinkRoutine != null)
                {
                    StopCoroutine(blinkRoutine);
                    blinkRoutine = null;
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
            gameObject.SetActive(false);
        }
    }

    IEnumerator PlayRandomChirps()
{
    AudioSource[] chirps = { chirp1, chirp2, chirp3 };

    while (true)
    {
        AudioSource c = chirps[Random.Range(0, chirps.Length)];
        if (c != null) c.Play();

        // small randomness to feel natural
        yield return new WaitForSeconds(Random.Range(0.08f, 0.15f));
    }
}
}
