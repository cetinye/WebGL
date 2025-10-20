using System;
using UnityEngine;

public class W27_Ghost : MonoBehaviour
{
    public Animator ghostAnimator;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        ghostAnimator.SetBool("tortoiseBack", true);
    }

    public void playBackAnimation()
    {
        // resetMotion();
        // ghostAnimator.SetBool("tortoiseBack", true);

        transform.localRotation = Quaternion.Euler(0, 0, 0);
    }
    public void playFrontAnimation()
    {
        // // resetMotion();
        // transform.localScale = new Vector3(transform.localScale.x, Math.Abs(transform.localScale.y) * -1);

        transform.localRotation = Quaternion.Euler(0, 0, 180);
    }
    public void playSideAnimation(bool mirror)
    {
        // resetMotion();
        // ghostAnimator.SetBool("tortoiseSide", true);

        // transform.localScale = mirror ? new Vector3(Math.Abs(transform.localScale.x) * -1, transform.localScale.y) : new Vector3(Math.Abs(transform.localScale.x), transform.localScale.y);
        transform.localRotation = Quaternion.Euler(0, 0, mirror ? 90 : -90);
    }

    public void resetMotion()
    {
        foreach (var param in ghostAnimator.parameters)
        {
            ghostAnimator.SetBool(param.name, false);
        }
    }
    public void setAnimSpeed(float speed)
    {
        ghostAnimator.speed = speed;
    }
}
