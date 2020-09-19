using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWarning : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] public bool isEnemyWarningSFXOn = false;
    [SerializeField] private StudioEventEmitter ambienceHorrorSFX;
    [SerializeField] private StudioEventEmitter enemyWarningSFX;
    [SerializeField] private StudioEventEmitter metallicHorrorSFX;
    [SerializeField] private StudioEventEmitter slowHeartBeat;
    [SerializeField] private StudioEventEmitter mediumHeartBeat;
    [SerializeField] private StudioEventEmitter fastHeartBeat;
    [SerializeField] private StudioEventEmitter levelFinishedSFX;
    [SerializeField] private bool alreadyRun;
    public Enemy[] enemies;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        enemies = FindObjectsOfType<Enemy>();
        Debug.Log("Total Enemy: " + enemies.Length);
    }

    // Update is called once per frame
    void Update()
    {
        if (player.gameStart)
        {
            //if (IsThereAnyMovingEnemy())
            //{
            //    if (!enemyWarningSFX.IsPlaying())
            //    {

            //    }
            //}
            //else if (!IsThereAnyMovingEnemy())
            //{
            //    if (!ambienceHorrorSFX.IsPlaying() && metallicHorrorSFX.IsPlaying())
            //    {

            //    }
            //}

            //print("Awaken Enemy : " + IsThereAnyAwakenEnemy());
            //print("Moving Enemy : " + IsThereAnyMovingEnemy());
            //print("Fast Heart Beat : " + fastHeartBeat.IsPlaying());
            //print("Medium Heart Beat : " + mediumHeartBeat.IsPlaying());
            if (IsThereAnyMovingEnemy() && player.isFlashOn)
            {
                if (!fastHeartBeat.IsPlaying())
                {
                    ambienceHorrorSFX.Stop();
                    enemyWarningSFX.Play();
                    metallicHorrorSFX.Stop();
                    slowHeartBeat.Stop();
                    mediumHeartBeat.Stop();
                    fastHeartBeat.Play();
                }
            }
            else if (IsThereAnyAwakenEnemy())
            {
                if (!mediumHeartBeat.IsPlaying())
                {
                    ambienceHorrorSFX.Play();
                    enemyWarningSFX.Stop();
                    metallicHorrorSFX.Play();
                    slowHeartBeat.Stop();
                    mediumHeartBeat.Play();
                    fastHeartBeat.Stop();
                }
            }
            else
            {
                if (!slowHeartBeat.IsPlaying())
                {
                    ambienceHorrorSFX.Play();
                    enemyWarningSFX.Stop();
                    metallicHorrorSFX.Play();
                    slowHeartBeat.Play();
                    mediumHeartBeat.Stop();
                    fastHeartBeat.Stop();
                }
            }
        }
        else if (!player.gameStart && player.gameFinished && !alreadyRun)
        {
            alreadyRun = true;
            ambienceHorrorSFX.Stop();
            enemyWarningSFX.Stop();
            metallicHorrorSFX.Stop();
            slowHeartBeat.Stop();
            mediumHeartBeat.Stop();
            fastHeartBeat.Stop();
            levelFinishedSFX.Play();
            Destroy(gameObject, 4.6f);
        }
    }
        

    private bool IsThereAnyMovingEnemy()
    {
        foreach(var enemy in enemies)
        {
            if (enemy.aiLerp.canMove && !enemy.isDeath)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsThereAnyAwakenEnemy()
    {
        foreach(var enemy in enemies)
        {
            if (enemy.isAwake)
            {
                return true;
            }
        }
        return false;
    }
}
