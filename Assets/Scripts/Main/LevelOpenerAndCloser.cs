using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelOpenerAndCloser : MonoBehaviour
{
    [SerializeField] private RawImage blackBG;
    [SerializeField] private RawImage redDamage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartLevel(float duration)
    {
        blackBG.DOFade(0f, duration);
    }

    public void EndLevel(float duration)
    {
        blackBG.DOFade(1f, duration);
    }

    public void StartFadeDamageColor(float duration)
    {
        redDamage.DOFade(0.7f, duration).OnComplete(
            delegate { redDamage.DOFade(0f, duration); });        
    }
}
