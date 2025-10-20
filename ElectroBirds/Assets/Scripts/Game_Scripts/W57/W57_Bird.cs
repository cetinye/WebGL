using System.Collections;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Game_Scripts.W57;
using UnityEngine;
using UnityEngine.UI;
using W57;
using Random = UnityEngine.Random;

public class W57_Bird : MonoBehaviour
{
    private bool isCollidable = false;
    private bool flying = true;
    private static readonly int isFliying = Animator.StringToHash("isFlying");
    private static readonly int isIdling = Animator.StringToHash("isIdling");
    private static readonly string electricFlowName = "ElectricFlowContainer";

    public W57_LevelManager levelManager;
    public Animator playerAnimator;
    public GameObject electrocutionContainer;
    public GameObject feedbackContainer;
    private Transform destinationTransform;
    public bool birdElectrocuted;
    public W57_Cable cable;
    public W57_Cable.LandingPosition landingPos;

    private void Start()
    {
        StartCoroutine(CheckBirdPosition());
    }

    public void OnBirdClicked()
    {
        if (flying) return;
        flying = true;
        levelManager.PlaySound(eW57FxSoundStates.JUMP);
        transform.DOMove(transform.position + new Vector3(0, 6, 0), 0.25f)
            .OnStart(Fly)
            .OnComplete(() =>
            {
                transform.DOMove(transform.position + new Vector3(0, -6, 0), 0.25f)
                    .SetDelay(0.25f) // stay in the air
                    .OnComplete(Idle);
            });
    }

    private void Idle()
    {
        playerAnimator.SetBool(isFliying, false);
        playerAnimator.SetBool(isIdling, true);
        flying = false;
        isCollidable = true;
    }

    private void Fly()
    {
        playerAnimator.SetBool(isIdling, false);
        playerAnimator.SetBool(isFliying, true);
    }

    public TweenerCore<Vector3, Vector3, VectorOptions> GoToDestination(Transform tr)
    {
        destinationTransform = tr;

        var tween = transform.DOMove(tr.position, Random.Range(2.5f, 4.5f))
            .OnStart(Fly)
            .OnComplete(Idle);

        return tween;
    }

    public void SetStandingCable(W57_Cable cable, W57_Cable.LandingPosition landingPos)
    {
        this.cable = cable;
        this.landingPos = landingPos;
    }

    public W57_Cable GetStandingCable()
    {
        return cable;
    }

    public void SetLandingPosState(bool state)
    {
        landingPos.isOccupied = state;
    }

    private IEnumerator CheckBirdPosition()
    {
        if (!flying)
        {
            transform.position = destinationTransform.position;
        }

        yield return new WaitForSeconds(0.3f);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (isCollidable && col.gameObject.name == electricFlowName)
        {
            OnElectrocuted();
        }
    }

    private void OnElectrocuted()
    {
        Fly();
        levelManager.PlaySound(eW57FxSoundStates.ELECTROCUTE);

        birdElectrocuted = true;
        electrocutionContainer.SetActive(true);
        feedbackContainer.SetActive(true);
        levelManager.BirdElectrocuted();

        Invoke(nameof(Reset), 1.5f);
    }

    private void Reset()
    {
        Idle();
        electrocutionContainer.SetActive(false);
        feedbackContainer.SetActive(false);
    }

    public void OverflowingStarted()
    {
        birdElectrocuted = false;
    }

    public void OverflowingCompleted()
    {
        if (!birdElectrocuted && levelManager.electricSentToBirds)
        {
            levelManager.BirdSaved();
        }
    }
}