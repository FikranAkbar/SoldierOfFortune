using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using FMODUnity;

public class MenuManager : MonoBehaviour
{

    [SerializeField] private Image midGround;
    [SerializeField] private float speed;
    [SerializeField] private TextMeshProUGUI titleGame;
    [SerializeField] private RectTransform pressSpace;
    [SerializeField] private RectTransform startButton;
    [SerializeField] private RectTransform quitButton;
    [SerializeField] private RectTransform foreGround;
    [SerializeField] private StudioEventEmitter buttonChooseSfx;
    [SerializeField] private StudioEventEmitter buttonSelectSfx;
    [SerializeField] private Image blackBG;
    [SerializeField] private float glowValue;

    private bool canSpace = false;
    private bool canNavigateArrow = false;
    [SerializeField] private int buttonSelected = 0;
    private TextMeshProUGUI startButtonText;
    private TextMeshProUGUI quitButtonText;

    // Start is called before the first frame update
    void Start()
    {
        Sequence seq = DOTween.Sequence();
        seq
            .Append(foreGround.GetComponent<Image>().DOFade(1f, 1f))
            .Append(foreGround.DOAnchorPosY(5, 2f))
            .Append(titleGame.DOFade(1f, 2f))
            .Append(startButton.GetComponent<TextMeshProUGUI>().DOFade(1f, 2f))
            .Join(quitButton.GetComponent<TextMeshProUGUI>().DOFade(1f, 2f))
            .AppendCallback(delegate { canNavigateArrow = true; buttonSelected = 0; canSpace = true; });

        TextMeshProUGUI pressSpaceText = pressSpace.GetComponent<TextMeshProUGUI>();
        float pressSpaceFont = pressSpaceText.fontSize;

        startButtonText = startButton.GetComponent<TextMeshProUGUI>();
        quitButtonText = quitButton.GetComponent<TextMeshProUGUI>();

        startButtonText.DOFade(0f, 0.01f);
        quitButtonText.DOFade(0f, 0.01f);
        titleGame.DOFade(0f, 0.01f);
        foreGround.GetComponent<Image>().DOFade(0f, 0.01f);
        blackBG.DOFade(0f, 0.7f);
        seq.Play();


        //Sequence pressSpaceSeq = DOTween.Sequence();
        //pressSpaceSeq
        //    .Append(DOTween.To(() => pressSpaceText.fontSize, x => pressSpaceText.fontSize = x, 43, 1.5f))
        //    .Append(DOTween.To(() => pressSpaceText.fontSize, x => pressSpaceText.fontSize = x, 38, 1.5f))
        //    .SetLoops(-1);
        //pressSpaceSeq.Play();

        Sequence titleGameSeq = DOTween.Sequence();
        titleGameSeq
            .Append(DOTween.To(() => glowValue, x => glowValue = x, 1f, 2f))
            .Append(DOTween.To(() => glowValue, x => glowValue = x, 0f, 2f))
            .SetLoops(-1);
        titleGameSeq.Play();
    }

    // Update is called once per frame
    void Update()
    {
        midGround.material.mainTextureOffset = new Vector2(Time.time * speed, 0);
        titleGame.fontSharedMaterial.SetFloat(ShaderUtilities.ID_GlowPower, glowValue);

        if (canNavigateArrow)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                canNavigateArrow = false;
                buttonSelected++;
                if (buttonSelected > 1)
                {
                    buttonSelected = 0;
                }
                CheckButtonAnimation();
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                canNavigateArrow = false;
                buttonSelected--;
                if (buttonSelected < 0)
                {
                    buttonSelected = 1;
                }
                CheckButtonAnimation();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Return) && canSpace)
        {
            if (buttonSelected == 0)
            {
                buttonSelectSfx.Play();
                StartCoroutine(LoadIntroScene());
                canSpace = false;
                canNavigateArrow = false;
            }
            else if (buttonSelected == 1)
            {
                buttonSelectSfx.Play();
                Application.Quit();
                canSpace = false;
                canNavigateArrow = false;
            }
        }
    }

    private IEnumerator LoadIntroScene()
    {
        blackBG.DOFade(1f, 2f);
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("Intro");
    }

    private void CheckButtonAnimation()
    {
        if (buttonSelected == 0)
        {
            buttonChooseSfx.Play();
            DOTween.To(() => startButtonText.fontSize, x => startButtonText.fontSize = x, 80, 0.5f);
            DOTween.To(() => quitButtonText.fontSize, x => quitButtonText.fontSize = x, 40, 0.5f)
                .OnComplete(delegate { canNavigateArrow = true; }) ;
        }
        else if (buttonSelected == 1)
        {
            buttonChooseSfx.Play();
            DOTween.To(() => startButtonText.fontSize, x => startButtonText.fontSize = x, 40, 0.5f);
            DOTween.To(() => quitButtonText.fontSize, x => quitButtonText.fontSize = x, 80, 0.5f)
                .OnComplete(delegate { canNavigateArrow = true; });
        }
    }
}
