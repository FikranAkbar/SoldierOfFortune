using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CreditScreen : MonoBehaviour
{
    [SerializeField] private RectTransform creditText;
    [SerializeField] private TextMeshProUGUI thanksForPlaying;
    [SerializeField] private RawImage amicta;
    [SerializeField] private RawImage blackBg;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private float endPos;
    [SerializeField] private float duration;
    [SerializeField] private float waitDuration;
    [SerializeField] private float fadeInOutDuration_Thanks;
    [SerializeField] private float stayDuration;
    [SerializeField] private float fadeInOutDuration_Amicta;
    Sequence seq;

    // Start is called before the first frame update
    void Start()
    {
        blackBg.DOFade(0f, 0.1f);
        amicta.DOFade(0f, 0.0001f);
        thanksForPlaying.DOFade(0f, 0.0001f);
        description.DOFade(0f, 0.0001f);

        seq = DOTween.Sequence();
        seq
            .Append(creditText.DOAnchorPosY(endPos, duration).SetEase(Ease.Unset))
            .AppendInterval(waitDuration * 1f / 4f)
            .Append(thanksForPlaying.DOFade(1f, fadeInOutDuration_Thanks))
            .AppendInterval(stayDuration)
            .Append(thanksForPlaying.DOFade(0f, fadeInOutDuration_Thanks))
            .AppendInterval(waitDuration * 7f / 4f)
            .Append(amicta.DOFade(1f, fadeInOutDuration_Amicta))
            .Append(description.DOFade(1f, fadeInOutDuration_Amicta))
            .AppendInterval(stayDuration)
            .Append(description.DOFade(0f, 3f))
            .Append(amicta.DOFade(0f, 3f))
            .AppendInterval(2f)
            .AppendCallback(delegate { SceneManager.LoadScene("Menu"); });


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
