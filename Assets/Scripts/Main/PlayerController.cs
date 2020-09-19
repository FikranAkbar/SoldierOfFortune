using DG.Tweening;
using FMOD;
using FMODUnity;
using Pathfinding;
using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    [Header("Torch Properties")]
    [SerializeField] private Light2D fireLightMovePoint;
    [SerializeField] private Light2D fireLightCenter;
    [SerializeField] private GameObject torchOnSFX;
    [SerializeField] private GameObject torchOffSFX;
    [SerializeField] private GameObject torchDust;
    private ParticleSystem torchDustPS;
    [SerializeField] private Ease flashEase;
    [SerializeField] private Ease flashSquashEase;
    [SerializeField] private float flashMoveDuration = 0.3f;
    [SerializeField] private float flashTurnDuration = 0.3f;
    [SerializeField] public bool isFlashOn;
    [SerializeField] private bool turningFlash;

    [Header("Movement Character")]
    [SerializeField] private GameObject playerDieSFX;
    [SerializeField] private GameObject playerDeathByEnemySFX;
    [SerializeField] private GameObject playerSpriteParent;
    [SerializeField] private SpriteRenderer playerSpriteLit;
    [SerializeField] private SpriteRenderer playerSpriteUnLit;
    [SerializeField] private Transform movePoint;
    [SerializeField] private Transform nextPoint;
    [SerializeField] private ParticleSystem dustFootsteps;
    [SerializeField] private Animator anim;
    [SerializeField] private Ease moveEase;
    [SerializeField] private StudioEventEmitter footstepSFX;
    [SerializeField] private float moveDuration = 0.3f;
    [SerializeField] private float minSquashAndStretch;
    [SerializeField] private float maxSquashAndStretch;
    [SerializeField] private bool alreadyMoving;
    [SerializeField] private bool isMoving;
    [SerializeField] private bool canChangeDirection;
    [SerializeField] private bool paralyzeByEnemy;

    [Header("Sensor Detection")]
    [SerializeField] private LayerMask whatStopMovement;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Enemy Awaken And Shaking Camera")]
    [SerializeField] private EnemyWarning enemyWarning;
    [SerializeField] private float awakeDuration = 1f;
    [SerializeField] private float shakingStrength;
    [SerializeField] private int shakingVibrato;
    [SerializeField] private float shakingRandomness;

    [Header("???")]
    //[SerializeField] private EnemyWarning enemyWarning;
    [SerializeField] public bool gameStart = false;
    [SerializeField] public bool gameFinished = false;
    [SerializeField] private LevelOpenerAndCloser blackBG;



    // Start is called before the first frame update
    void Start()
    {
        torchDustPS = torchDust.GetComponent<ParticleSystem>();
        if (GameObject.Find("AudioManager").activeInHierarchy)
            enemyWarning = FindObjectOfType<EnemyWarning>();
        nextPoint.parent = null;
        dustFootsteps.gameObject.transform.parent = null;
        StartCoroutine(TurnOnTorchFirstTime());
        blackBG = FindObjectOfType<LevelOpenerAndCloser>();
        blackBG.StartLevel(1f);
    }

    private void Update()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (gameStart)
        {
            if (!paralyzeByEnemy)
            {
                PlayerMove();
                FireLightControl();
            }
            CheckingEnemyInFront();
        }
    }

    private void PlayerMove()
    {
        if (!Physics2D.OverlapCircle(movePoint.position, 0.2f, whatStopMovement))
        {    
            if (!isMoving && !turningFlash)
            {
                if (movePoint.position - transform.position == Vector3.up &&
                    Input.GetKey(KeyCode.UpArrow) &&
                    Vector3.Distance(nextPoint.position, transform.position) == 0f)
                {
                    anim.SetBool("moving_up", true);
                    nextPoint.position = new Vector3(Mathf.Ceil(movePoint.position.x), Mathf.Ceil(movePoint.position.y), Mathf.Ceil(movePoint.position.z));
                    isMoving = true;
                }
                else if (movePoint.position - transform.position == Vector3.right &&
                    Input.GetKey(KeyCode.RightArrow) &&
                    Vector3.Distance(nextPoint.position, transform.position) == 0f)
                {
                    anim.SetBool("moving_right", true);
                    nextPoint.position = new Vector3(Mathf.Ceil(movePoint.position.x), Mathf.Ceil(movePoint.position.y), Mathf.Ceil(movePoint.position.z));
                    isMoving = true;
                }
                else if (movePoint.position - transform.position == Vector3.down &&
                    Input.GetKey(KeyCode.DownArrow) &&
                    Vector3.Distance(nextPoint.position, transform.position) == 0f)
                {
                    anim.SetBool("moving_down", true);
                    nextPoint.position = new Vector3(Mathf.Ceil(movePoint.position.x), Mathf.Ceil(movePoint.position.y), Mathf.Ceil(movePoint.position.z));
                    isMoving = true;
                }
                else if (movePoint.position - transform.position == Vector3.left &&
                    Input.GetKey(KeyCode.LeftArrow) &&
                    Vector3.Distance(nextPoint.position, transform.position) == 0f)
                {
                    anim.SetBool("moving_left", true);
                    nextPoint.position = new Vector3(Mathf.Ceil(movePoint.position.x), Mathf.Ceil(movePoint.position.y), Mathf.Ceil(movePoint.position.z));
                    isMoving = true;
                }
            }
        }

        if (isMoving && 
            Vector3.Distance(transform.position, nextPoint.position) != 0 &&
            !alreadyMoving)
        {
            alreadyMoving = true;
            SquashAndStretchSprite(moveDuration);
            if (dustFootsteps != null)
            {
                dustFootsteps.transform.position = transform.position;
                dustFootsteps.Play();
            }
            footstepSFX.Play();
            transform.DOMove(nextPoint.position, moveDuration).SetEase(moveEase)
                .OnComplete(delegate
            {
                alreadyMoving = false;
                isMoving = false;
                anim.SetBool("moving_up", false);
                anim.SetBool("moving_right", false);
                anim.SetBool("moving_down", false);
                anim.SetBool("moving_left", false);
            });
        }
        
        if (!isMoving)
        {
            if (canChangeDirection && !turningFlash)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow) )
                {
                    canChangeDirection = false;
                    if (isFlashOn && (!Physics2D.OverlapCircle(movePoint.position, 0.2f, whatStopMovement) ||
                        movePoint.position - transform.position != Vector3.up))
                    {
                        //fireLightMovePoint.gameObject.GetComponent<LightFlickerEffect>().enabled = false;
                        //movePoint.DOScale(transform.localScale.x / 2f, flashMoveDuration / 2f).OnComplete(delegate
                        //{
                        //    movePoint.DOScale(transform.localScale.x, flashMoveDuration / 2f)
                        //    .OnComplete(delegate { fireLightMovePoint.gameObject.GetComponent<LightFlickerEffect>().enabled = true; });
                        //});

                        fireLightMovePoint.gameObject.GetComponent<LightFlickerEffect>().enabled = false;
                        DOTween.To(
                            () => fireLightMovePoint.pointLightOuterRadius,
                            x => fireLightMovePoint.pointLightOuterRadius = x,
                            fireLightMovePoint.pointLightOuterRadius / 2f,
                            flashTurnDuration / 2f).SetEase(flashSquashEase)
                            .OnComplete(delegate {
                                DOTween.To(
                                    () => fireLightMovePoint.pointLightOuterRadius,
                                    x => fireLightMovePoint.pointLightOuterRadius = x,
                                    fireLightMovePoint.pointLightOuterRadius * 2f,
                                    flashTurnDuration / 2f).SetEase(flashSquashEase)
                                    .OnComplete(delegate
                                    {
                                        fireLightMovePoint.gameObject.GetComponent<LightFlickerEffect>().enabled = true;
                                    });
                            });
                    }
                    Vector3 newPos = new Vector3(
                        Mathf.Round(transform.position.x + Vector3.up.x),
                        Mathf.Round(transform.position.y + Vector3.up.y),
                        Mathf.Round(transform.position.z + Vector3.up.z)
                        );
                    movePoint.DOMove(newPos, flashMoveDuration).SetEase(flashEase).OnComplete(() => {
                        torchDust.transform.localPosition = new Vector3(0.286f, 0.2f);
                        canChangeDirection = true;
                        anim.SetBool("face_up", true);
                        anim.SetBool("face_down", false);
                        anim.SetBool("face_right", false);
                        anim.SetBool("face_left", false);
                    });
                    
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    canChangeDirection = false;
                    if (isFlashOn && (!Physics2D.OverlapCircle(movePoint.position, 0.2f, whatStopMovement) ||
                        movePoint.position - transform.position != Vector3.down))
                    {
                        //fireLightMovePoint.gameObject.GetComponent<LightFlickerEffect>().enabled = false;
                        //movePoint.DOScale(transform.localScale.x / 2f, flashMoveDuration / 2f).OnComplete(delegate
                        //{
                        //    movePoint.DOScale(transform.localScale.x, flashMoveDuration / 2f)
                        //    .OnComplete(delegate { fireLightMovePoint.gameObject.GetComponent<LightFlickerEffect>().enabled = true; });
                        //});

                        fireLightMovePoint.gameObject.GetComponent<LightFlickerEffect>().enabled = false;
                        DOTween.To(
                            () => fireLightMovePoint.pointLightOuterRadius,
                            x => fireLightMovePoint.pointLightOuterRadius = x,
                            fireLightMovePoint.pointLightOuterRadius / 2f,
                            flashTurnDuration / 2f).SetEase(flashSquashEase)
                            .OnComplete(delegate {
                                DOTween.To(
                                    () => fireLightMovePoint.pointLightOuterRadius,
                                    x => fireLightMovePoint.pointLightOuterRadius = x,
                                    fireLightMovePoint.pointLightOuterRadius * 2f,
                                    flashTurnDuration / 2f).SetEase(flashSquashEase).OnComplete(delegate
                                    {
                                        fireLightMovePoint.gameObject.GetComponent<LightFlickerEffect>().enabled = true;
                                    });
                            });

                    }
                    Vector3 newPos = new Vector3(
                        Mathf.Round(transform.position.x + Vector3.down.x),
                        Mathf.Round(transform.position.y + Vector3.down.y),
                        Mathf.Round(transform.position.z + Vector3.down.z)
                        );
                    movePoint.DOMove(newPos, flashMoveDuration).SetEase(flashEase).OnComplete(() => {
                        torchDust.transform.localPosition = new Vector3(-0.31f, 0.255f);
                        canChangeDirection = true;
                        anim.SetBool("face_up", false);
                        anim.SetBool("face_down", true);
                        anim.SetBool("face_right", false);
                        anim.SetBool("face_left", false);
                    });
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    canChangeDirection = false;
                    if (isFlashOn && (!Physics2D.OverlapCircle(movePoint.position, 0.2f, whatStopMovement) ||
                        movePoint.position - transform.position != Vector3.right))
                    {
                        //fireLightMovePoint.gameObject.GetComponent<LightFlickerEffect>().enabled = false;
                        //movePoint.DOScale(transform.localScale.x / 2f, flashMoveDuration / 2f).OnComplete(delegate
                        //{
                        //    movePoint.DOScale(transform.localScale.x, flashMoveDuration / 2f)
                        //    .OnComplete(delegate { fireLightMovePoint.gameObject.GetComponent<LightFlickerEffect>().enabled = true; });
                        //});

                        fireLightMovePoint.gameObject.GetComponent<LightFlickerEffect>().enabled = false;
                        DOTween.To(
                            () => fireLightMovePoint.pointLightOuterRadius,
                            x => fireLightMovePoint.pointLightOuterRadius = x,
                            fireLightMovePoint.pointLightOuterRadius / 2f,
                            flashTurnDuration / 2f).SetEase(flashSquashEase)
                            .OnComplete(delegate {
                                DOTween.To(
                                    () => fireLightMovePoint.pointLightOuterRadius,
                                    x => fireLightMovePoint.pointLightOuterRadius = x,
                                    fireLightMovePoint.pointLightOuterRadius * 2f,
                                    flashTurnDuration / 2f).SetEase(flashSquashEase).OnComplete(delegate
                                    {
                                        fireLightMovePoint.gameObject.GetComponent<LightFlickerEffect>().enabled = true;
                                    });
                            });
                    }
                    Vector3 newPos = new Vector3(
                        Mathf.Round(transform.position.x + Vector3.right.x),
                        Mathf.Round(transform.position.y + Vector3.right.y),
                        Mathf.Round(transform.position.z + Vector3.right.z)
                        );
                    movePoint.DOMove(newPos, flashMoveDuration).SetEase(flashEase).OnComplete(() => {
                        torchDust.transform.localPosition = new Vector3(0.208f, 0.149f);
                        canChangeDirection = true;
                        anim.SetBool("face_up", false);
                        anim.SetBool("face_down", false);
                        anim.SetBool("face_right", true);
                        anim.SetBool("face_left", false);
                    });
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    canChangeDirection = false;
                    if (isFlashOn && (!Physics2D.OverlapCircle(movePoint.position, 0.2f, whatStopMovement) ||
                        movePoint.position - transform.position != Vector3.left))
                    {
                        //fireLightMovePoint.gameObject.GetComponent<LightFlickerEffect>().enabled = false;
                        //movePoint.DOScale(transform.localScale.x / 2f, flashMoveDuration / 2f).OnComplete(delegate
                        //{
                        //    movePoint.DOScale(transform.localScale.x, flashMoveDuration / 2f)
                        //    .OnComplete(delegate { fireLightMovePoint.gameObject.GetComponent<LightFlickerEffect>().enabled = true; });
                        //});

                        fireLightMovePoint.gameObject.GetComponent<LightFlickerEffect>().enabled = false;
                        DOTween.To(
                            () => fireLightMovePoint.pointLightOuterRadius,
                            x => fireLightMovePoint.pointLightOuterRadius = x,
                            fireLightMovePoint.pointLightOuterRadius / 2f,
                            flashTurnDuration / 2f).SetEase(flashSquashEase)
                            .OnComplete(delegate {
                                DOTween.To(
                                    () => fireLightMovePoint.pointLightOuterRadius,
                                    x => fireLightMovePoint.pointLightOuterRadius = x,
                                    fireLightMovePoint.pointLightOuterRadius * 2f,
                                    flashTurnDuration / 2f).SetEase(flashSquashEase).OnComplete(delegate
                                    {
                                        fireLightMovePoint.gameObject.GetComponent<LightFlickerEffect>().enabled = true;
                                    });
                            });
                    }
                    Vector3 newPos = new Vector3(
                        Mathf.Round(transform.position.x + Vector3.left.x),
                        Mathf.Round(transform.position.y + Vector3.left.y),
                        Mathf.Round(transform.position.z + Vector3.left.z)
                        );
                    movePoint.DOMove(newPos, flashMoveDuration).SetEase(flashEase).OnComplete(() =>
                    {
                        torchDust.transform.localPosition = new Vector3(-0.321f, 0.18f);
                        canChangeDirection = true;
                        anim.SetBool("face_up", false);
                        anim.SetBool("face_down", false);
                        anim.SetBool("face_right", false);
                        anim.SetBool("face_left", true);
                    });
                }
            }
        }
    }

    private void FireLightControl()
    {
        if (!isMoving && !turningFlash && canChangeDirection)
        {
            if (Input.GetKeyDown(KeyCode.Space) && isFlashOn)
            {
                TurnOffTorch();
            }
            else if (Input.GetKeyDown(KeyCode.Space) && !isFlashOn)
            {
                TurnOnTorch();
            }
        }
    }

    private void CheckingEnemyInFront()
    {
        if (Physics2D.OverlapCircle(movePoint.position, 0.2f, enemyLayer))
        {
            if (isFlashOn)
            {
                Enemy enemy = Physics2D.OverlapCircle(movePoint.position, 0.2f).GetComponent<Enemy>();
                if (enemy && !enemy.isAwake && !enemy.isShakedTheEnemy)
                {
                    enemy.isShakedTheEnemy = true;
                    StartCoroutine(AwakeTheEnemy(awakeDuration, enemy));
                }
            }
        }
    }

    private IEnumerator AwakeTheEnemy(float waitTime, Enemy enemy)
    {
        enemy.anim.SetTrigger("riseUp");
        paralyzeByEnemy = true;
        enemy.enemyRisenSFX.SetActive(true);
        yield return new WaitForSeconds(1.0625f);
        enemy.anim.SetBool("hasAwaken", true);
        enemy.enemyAwakenSFX.SetActive(true);
        ShakeCamera(awakeDuration, shakingStrength, shakingVibrato, shakingRandomness);
        enemy.transform.GetChild(1).GetComponent<SpriteRenderer>().DOFade(1f, waitTime);
        yield return new WaitForSeconds(waitTime);
        paralyzeByEnemy = false;
        yield return new WaitForSeconds(1f);
        enemy.isAwake = true;
    }

    private void TurnOnTorch()
    {
        turningFlash = true;
        torchOnSFX.SetActive(true);
        torchOffSFX.SetActive(false);

        // Tween a float called myFloat to 52 in 1 second
        DOTween.To(() => fireLightMovePoint.intensity, x => fireLightMovePoint.intensity = x, 1f, flashTurnDuration)
            .OnComplete(delegate { isFlashOn = true; fireLightMovePoint.gameObject.GetComponent<LightFlickerEffect>().enabled = true;
             turningFlash = false; torchDustPS.Play(); });
        DOTween.To(() => fireLightCenter.intensity, x => fireLightCenter.intensity = x, 1f, flashTurnDuration)
            .OnComplete(delegate { isFlashOn = true; fireLightCenter.gameObject.GetComponent<LightFlickerEffect>().enabled = true;  });

        playerSpriteLit.DOFade(1f, flashTurnDuration);
        playerSpriteUnLit.DOFade(0f, flashTurnDuration);
    }

    private void TurnOffTorch()
    {
        turningFlash = true;
        // Tween a float called myFloat to 52 in 1 second
        fireLightMovePoint.gameObject.GetComponent<LightFlickerEffect>().enabled = false;
        fireLightCenter.gameObject.GetComponent<LightFlickerEffect>().enabled = false;

        torchOnSFX.SetActive(false);
        torchOffSFX.SetActive(true);

        DOTween.To(() => fireLightMovePoint.intensity, x => fireLightMovePoint.intensity = x, 0f, flashTurnDuration)
            .OnComplete(delegate { isFlashOn = false; turningFlash = false; torchDustPS.Stop(); });
        DOTween.To(() => fireLightCenter.intensity, x => fireLightCenter.intensity = x, 0f, flashTurnDuration)
            .OnComplete(delegate { isFlashOn = false;});

        playerSpriteLit.DOFade(0f, flashTurnDuration);
        playerSpriteUnLit.DOFade(1f, flashTurnDuration);
    }

    private IEnumerator TurnOnTorchFirstTime()
    {
        TurnOnTorch();
        yield return new WaitForSeconds(flashTurnDuration);
        gameStart = true;
        enemyWarning.enabled = true;
    }

    private void ShakeCamera(float awakeDuration, float shakingStrength, float shakingVibrato, float shakingRandomness)
    {
        FindObjectOfType<Camera>().DOShakePosition(awakeDuration, shakingStrength, (int)shakingVibrato, shakingRandomness);
    }

    private void SquashAndStretchSprite(float moveDuration)
    {
        playerSpriteParent.transform.DOScaleX(minSquashAndStretch, moveDuration/3f).SetEase(Ease.Linear)
            .OnComplete(delegate { playerSpriteParent.transform.DOScaleX(maxSquashAndStretch, moveDuration/3f).SetEase(Ease.Linear)
                .OnComplete(delegate { playerSpriteParent.transform.DOScaleX(1f, moveDuration/3f).SetEase(Ease.Linear); }); });
        playerSpriteParent.transform.DOScaleY(maxSquashAndStretch, moveDuration/3f).SetEase(Ease.Linear)
            .OnComplete(delegate { playerSpriteParent.transform.DOScaleY(minSquashAndStretch, moveDuration/3f).SetEase(Ease.Linear)
                .OnComplete(delegate { playerSpriteParent.transform.DOScaleY(1f, moveDuration/3f).SetEase(Ease.Linear); }); });
    }

    public void PlayerDies()
    {
        playerDieSFX.SetActive(true);
        anim.SetBool("death", true);
        torchDustPS.Stop();
        gameStart = false;
        gameFinished = true;
        StartCoroutine(FinishLevelAfterSecs());
    }

    public void PlayerDiesOfEnemy()
    {
        ShakeCamera(awakeDuration/2f, shakingStrength/2f, shakingVibrato/2f, shakingRandomness/2f);
        blackBG.StartFadeDamageColor(awakeDuration / 4f);
        playerDeathByEnemySFX.SetActive(true);
        anim.SetBool("death", true);
        torchDustPS.Stop();
        gameStart = false;
        gameFinished = true;
    }

    private IEnumerator FinishLevelAfterSecs(){
        yield return new WaitForSeconds(1f);
    }
}
