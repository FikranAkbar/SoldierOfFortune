using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class GuideLevelOne : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI guide1;
    [SerializeField] private bool isG1FadeIn = false;
    [SerializeField] private TextMeshProUGUI guide2;
    [SerializeField] private bool isG2FadeIn = false;

    [SerializeField] private PlayerController player;
    [SerializeField] private Enemy enemy;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        enemy = FindObjectOfType<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.gameStart && !isG1FadeIn)
        {
            isG1FadeIn = true;
            StartCoroutine(ShowGuide1());
        }

        if (enemy.isShakedTheEnemy && enemy.isAwake && !isG2FadeIn)
        {
            isG2FadeIn = true;
            StartCoroutine(ShowGuide2());
        }

        print(guide1.color);
    }

    private IEnumerator ShowGuide1()
    {
        yield return new WaitForSeconds(1f);
        guide1.DOFade(1f, 1f);
        yield return new WaitForSeconds(4f);
        guide1.DOFade(0f, 1f);
    }

    private IEnumerator ShowGuide2()
    {
        yield return new WaitForSeconds(1f);
        guide2.DOFade(1f, 1f);
        yield return new WaitForSeconds(4f);
        guide2.DOFade(0f, 1f);
    }
}
