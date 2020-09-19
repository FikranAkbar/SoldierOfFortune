using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
    [SerializeField] private float waitTime;
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
        //Enemy[] enemies = FindObjectsOfType<Enemy>();
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player)
        {
            //foreach (Enemy enemy in enemies)
            //{
            //    enemy.isPlayerEscaped = true;
            //}

            player.gameStart = false;
            player.gameFinished = true;
            if (SceneManager.GetActiveScene().name == "Level 6")
            {
                Destroy(player);
                blackBG.EndLevel(waitTime * 2f/3f);
                StartCoroutine(LoadCurrentScene("Outro", waitTime));
            }
            else
            {   
                Destroy(player);
                blackBG.EndLevel(waitTime * 2f/3f);
                StartCoroutine(LoadCurrentScene(SceneManager.GetActiveScene().buildIndex + 1, waitTime));
            }
        }
    }

    private IEnumerator LoadCurrentScene(int index, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(index);
    }

    private IEnumerator LoadCurrentScene(string name, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(name);
    }
}
