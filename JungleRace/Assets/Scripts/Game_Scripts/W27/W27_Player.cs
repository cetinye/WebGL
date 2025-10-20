using System;
using DG.Tweening;
using JungleRace;
using UnityEngine;
using Random = UnityEngine.Random;

public class W27_Player : MonoBehaviour
{
    public bool isStunned = false;
    public W27_MazeGenerator mg;
    public Animator playerAnimator;
    public SpriteRenderer playerSp;

    public bool left, right, up, down;

    public BoxCollider2D bc;
    public ParticleSystem dust, stunStars;
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.name == "Target")
        {
            transform.DOPunchScale(new Vector3(1.05f, 1.05f), 0.4f, 1).OnStart(() => bc.isTrigger = true).OnComplete(() => bc.isTrigger = false);

            mg.levelDone = true;
            mg.moving = false;
            mg.clickable = false;
            setAnimSpeed(0);
        }
        else
        {
            dust.transform.position = col.transform.position;
            mg.playerHitWall = true;
            isStunned = true;
        }
    }

    private void Start()
    {
        stunStars.Stop();
        dust.Stop();
        // resetMotion();
        playerAnimator.SetBool("rabbitBack", true);
    }

    private void Update()
    {
        calculatePlayerRotation();
    }

    void calculatePlayerRotation()
    {
        if (mg.cameraRotation == 0)
        {
            if (left)
            {
                playSideAnimation(true);
            }
            else if (right)
            {
                playSideAnimation(false);
            }
            else if (up)
            {
                playBackAnimation();
            }
            else if (down)
            {
                playFrontAnimation();
            }
        }
        else if (mg.cameraRotation == 90)
        {
            if (left)
            {
                playBackAnimation();
            }
            else if (right)
            {
                playFrontAnimation();
            }
            else if (up)
            {
                playSideAnimation(false);
            }
            else if (down)
            {
                playSideAnimation(true);
            }
        }
        else if (mg.cameraRotation == -90)
        {
            if (left)
            {
                playFrontAnimation();
            }
            else if (right)
            {
                playBackAnimation();
            }
            else if (up)
            {
                playSideAnimation(true);
            }
            else if (down)
            {
                playSideAnimation(false);
            }
        }
        else if (mg.cameraRotation == 180)
        {
            if (left)
            {
                playSideAnimation(false);
            }
            else if (right)
            {
                playSideAnimation(true);
            }
            else if (up)
            {
                playFrontAnimation();
            }
            else if (down)
            {
                playBackAnimation();
            }
        }
    }

    private void ResetStun()
    {
        isStunned = false;
        stunStars.Stop();
        dust.Stop();
    }

    void OnTriggerExit2D(Collider2D col)
    {
        mg.playerHitWall = false;
        CancelInvoke(nameof(ResetStun));
        Invoke(nameof(ResetStun), W27_GameController.LevelSO.cooldown);
    }

    public void playBackAnimation()
    {
        // resetMotion();
        // playerAnimator.SetBool("rabbitBack", true);

        playerSp.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }
    public void playFrontAnimation()
    {
        // resetMotion();
        // playerAnimator.SetBool("rabbitFront", true);

        playerSp.transform.localRotation = Quaternion.Euler(0, 0, 180);
    }
    public void playSideAnimation(bool mirror)
    {
        // resetMotion();
        // playerAnimator.SetBool("rabbitSide", true);

        // var spriteScale = playerAnimator.transform.localScale;
        // playerAnimator.transform.localScale = mirror ? new Vector3(Math.Abs(spriteScale.x) * -1, spriteScale.y, 1f) : new Vector3(Math.Abs(spriteScale.x), spriteScale.y, 1f);

        playerSp.transform.localRotation = Quaternion.Euler(0, 0, mirror ? 90 : -90);
    }

    public void resetMotion()
    {
        foreach (var param in playerAnimator.parameters)
        {
            playerAnimator.SetBool(param.name, false);
        }
    }
    public void setAnimSpeed(float speed)
    {
        playerAnimator.speed = speed;
        //rewind the current animation to the beginning

        if (speed == 0)
        {
            playerAnimator.Play(playerAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0);
        }
    }

    public void resetRotation()
    {
        left = false;
        right = false;
        up = false;
        down = false;
    }

    public void playHitWallAnimation()
    {
        var stunChance = Random.Range(0, 100);
        // if (stunChance > 75)
        // {
        //     stunStars.Simulate(0f, false, true);
        //     stunStars.Play();
        // }

        stunStars.Simulate(0f, false, true);
        stunStars.Play();

        dust.Simulate(0f, false, true);
        dust.Play();
    }
}