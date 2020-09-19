using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Dialog : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;
    public string[] sentences;
    private int index;
    public float typingSpeed;
    public GameObject spaceToContinue;
    public GameObject typeSound;
    public GameObject blackBG;
    private Coroutine type;
    private int letterCount;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartScene());
    }

    // Update is called once per frame
    void Update()
    {
        if (textDisplay.text == sentences[index] && index != sentences.Length - 1)
        {

            if (Input.GetKeyDown(KeyCode.Space) && spaceToContinue.activeInHierarchy && type == null)
            {
                NextSentence();
            }
        }

        if (textDisplay.text == sentences[sentences.Length - 1])
        {
            if (Input.GetKeyDown(KeyCode.Space) && spaceToContinue.activeInHierarchy && type == null)
            {
                StartCoroutine(LoadNextScene());
            }
        }
    }

    IEnumerator Type()
    {
        typeSound.SetActive(true);
        foreach (char letter in sentences[index].ToCharArray())
        {
            if (Input.GetKey(KeyCode.Space) && letterCount>3)
            {
                textDisplay.text = sentences[index];
                typeSound.SetActive(false);
                yield return new WaitForSeconds(0.5f);
                spaceToContinue.SetActive(true);
                
                break;
            }
            textDisplay.text += letter;
            letterCount++;
            yield return new WaitForSeconds(typingSpeed);
        }
        type = null;
        typeSound.SetActive(false);
        spaceToContinue.SetActive(true);
    }

    public void NextSentence()
    {
        spaceToContinue.SetActive(false);
        if (index < sentences.Length - 1)
        {
            letterCount = 0;
            index++;
            textDisplay.text = "";
            type = StartCoroutine(Type());
        }
        else
        {
            textDisplay.text = "";
        }
    }

    private IEnumerator StartScene()
    {
        Image black = blackBG.GetComponent<Image>();
        black.DOFade(0f, 1f)
            .OnComplete(delegate { StartCoroutine(Type()); });
        yield return null;
    }

    private IEnumerator LoadNextScene()
    {
        string nextScene;
        if (SceneManager.GetActiveScene().name == "Intro")
        {
            nextScene = "Level 1";
        }
        else
        {
            nextScene = "Credit";
        }
        Image black = blackBG.GetComponent<Image>();
        black.DOFade(1f, 1f)
            .OnComplete(delegate { SceneManager.LoadScene(nextScene); });
        yield return null;
    }
}
