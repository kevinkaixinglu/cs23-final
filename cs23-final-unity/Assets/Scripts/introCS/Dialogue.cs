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

    private Coroutine blinkRoutine;

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
}
