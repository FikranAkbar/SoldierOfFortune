using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Trap : MonoBehaviour
{
    [SerializeField] private float dieAfterSeconds;
    [SerializeField] private float waitTime;
    [SerializeField] private GameObject[] allMusics;
    private bool playerDie;
    LevelOpenerAndCloser blackBG;
    // Start is called before the first frame update
    void Start()
    {
        blackBG = FindObjectOfType<LevelOpenerAndCloser>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy && !enemy.isDeath)
        {
            StartCoroutine(DestroyEnemyWithinSeconds(enemy));
        }

        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player && !playerDie)
        {
            playerDie = true;
            print("Player terkena jebakan");
            player.PlayerDies();
            StartCoroutine(ReloadScene());
        }
    }

    private IEnumerator DestroyEnemyWithinSeconds(Enemy enemy)
    {
        yield return new WaitForSeconds(dieAfterSeconds);
        enemy.EnemyIsDeath();
    }

    private IEnumerator ReloadScene()
    {
        yield return new WaitForSeconds(1f);
        blackBG.EndLevel(waitTime * 2f / 3f);
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
