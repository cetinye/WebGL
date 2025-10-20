using System.Collections.Generic;
using UnityEngine;

public class W44_Passenger : MonoBehaviour
{
    private int startingStationID;
    private int endingStationID;
    public List<int> acceptableEndingStationIndexes = new List<int>();
    public Sprite standingSprite;
    public bool currentlyOnBoard;

    public SpriteRenderer sp;
    public SpriteRenderer Indicator_sp;
    public BoxCollider2D collider;
    public Animator animator;

    public GameObject spriteContainer;
    
    private void Start()
    {
        stopAnim();
    }
    public void setStandingSprite(Sprite standing)
    {
        standingSprite = standing;
        sp.sprite = standingSprite;
    }
    public void setStandingSprite()
    {
        sp.sprite = standingSprite;
    }
    
    public void setSpriteRotation(int side)
    {
        spriteContainer.transform.localScale = new Vector3(side, 1,1);
    }
    
    public void setStartingStationID(int id)
    {
        startingStationID = id;
    }
    public int getStartingStationID()
    {
        return startingStationID;
    }
    public void setEndingStationID(int index)
    {
        endingStationID = index;
    }
    public int getEndingStationID()
    {
        return endingStationID;
    }
    public void setIndicatorSprite(Sprite indicator)
    {
        Indicator_sp.sprite = indicator;
    }
    public void makeVisible()
    {
        sp.sprite = standingSprite;
        collider.enabled = true;
        collider.size = new Vector3(0.2f,0.4f);
    }
    public void makeInvisible()
    {
        stopAnim();
        sp.sprite = null;
        Indicator_sp.sprite = null;
        collider.enabled = false;
    }

    public void setAnimation(RuntimeAnimatorController anim)
    {
        animator.runtimeAnimatorController = anim;
    }

    public void playAnim()
    {
        animator.enabled = true;
    }

    public void stopAnim()
    {
        animator.enabled = false;
    }
}