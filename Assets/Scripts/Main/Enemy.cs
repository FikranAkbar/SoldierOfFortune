using DG.Tweening;
using FMODUnity;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    public bool isAwake = false;
    public bool isDeath = false;
    [SerializeField] private Vector3 startingPoint;
    [SerializeField] public GameObject enemyRisenSFX;
    [SerializeField] public GameObject enemyAwakenSFX;
    [SerializeField] private GameObject enemyDieSFX;
    [SerializeField] public AILerp aiLerp;
    [SerializeField] private LayerMask trapLayer;
    [SerializeField] private PlayerController player;
    [SerializeField] private SpriteRenderer enemySprite;
    [SerializeField] public Animator anim;
    [SerializeField] private float stopRange;
    [SerializeField] private float waitTime;
    [SerializeField] public bool isShakedTheEnemy;
    [SerializeField] public bool isPlayerEscaped = false;
    [SerializeField] private bool isAssigned = false;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        aiLerp = GetComponent<AILerp>();
        transform.GetChild(1).GetComponent<SpriteRenderer>().DOFade(0f, 0.1f);
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (player.gameStart)
        {
            //if (isPlayerEscaped)
            //{
            //    aiLerp.canMove = false;
            //    aiLerp.canSearch = false;
            //}
            //else
            {
                if (player.isFlashOn && isAwake && !isDeath)
                {
                    if (!isAssigned)
                    {
                        AssignStartingPoint(new Vector3(transform.position.x, transform.position.y));
                        aiLerp.canMove = true;
                        aiLerp.canSearch = true;
                    }
                    CheckWhereToFace();
                }
                else if (!player.isFlashOn && isAwake && !isDeath)
                {
                    StopOnTheNearestNode();
                    isAssigned = false;
                }

                if (enemyDieSFX.GetComponent<StudioEventEmitter>().IsPlaying())
                {
                    print("enemyDIE");
                    StopOnTheNearestNode();
                    if (Mathf.Abs(transform.position.x) % 1 <= stopRange && Mathf.Abs(transform.position.y) % 1 <= stopRange)
                    {
                        isAwake = false;
                    }
                }
            }
        }
        else if (player.gameFinished)
        {
            aiLerp.canSearch = false;
            aiLerp.canMove = false;
        }
    }

    private void StopOnTheNearestNode()
    {
        if(Vector3.Distance(transform.position, startingPoint) > 0.9f)
        {
            if (transform.position.x != startingPoint.x)
            {
                if (Mathf.Abs(transform.position.x) % 1 <= stopRange)
                {
                    print("Modulus X = " + transform.position.x % 1);
                    print("Modulus Y = " + transform.position.y % 1);
                    aiLerp.canMove = false;
                    aiLerp.canSearch = false;
                }
            }
            else if (transform.position.y != startingPoint.y)
            {
                if (Mathf.Abs(transform.position.y) % 1 <= stopRange)
                {
                    print("Modulus X = " + transform.position.x % 1);
                    print("Modulus Y = " + transform.position.y % 1);
                    aiLerp.canMove = false;
                    aiLerp.canSearch = false;
                }
            }
        }
    }

    public void EnemyIsDeath()
    {
        gameObject.GetComponent<Collider2D>().enabled = false;
        isDeath = true;
        enemyDieSFX.SetActive(true);
        anim.SetTrigger("death");        
    }

    private void CheckWhereToFace()
    {
        if (player != null)
        {
            if (player.transform.position.x - transform.position.x >= 0)
            {
                enemySprite.flipX = true;
            }
            else
            {
                enemySprite.flipX = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player)
        {
            print("Player terkena jebakan");
            player.PlayerDiesOfEnemy();
            Destroy(player);
            StartCoroutine(ReloadScene());
        }
    }

    private IEnumerator ReloadScene()
    {
        yield return new WaitForSeconds (1f);
        FindObjectOfType<LevelOpenerAndCloser>().EndLevel(waitTime * 2f / 3f);
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator DestroyAfterSecs()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    private void AssignStartingPoint(Vector3 position)
    {
        isAssigned = true;
        startingPoint = position;
    }
}
