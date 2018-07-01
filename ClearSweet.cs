/***
  *Title:""��Ŀ
  *Description:
  *		����:
  *Author:D
  *Data:2018.03.18
  *
  *
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearSweet : MonoBehaviour
{
    public AnimationClip clearAnimation;
    public AudioClip clearClip;

    private bool isClearing;

    public bool IsClearing
    {
        get
        {
            return isClearing;
        }
      
    }

    protected GameSweet sweet;
    private void Awake()
    {
        sweet = GetComponent<GameSweet>();
    }
    //开启清除协程,
    public virtual void Clear()
    {
        isClearing = true;
        StartCoroutine(ClearCoroutine());
    }

    private IEnumerator ClearCoroutine()
    {
        Animator animator = GetComponent<Animator>();

        if (animator != null)
        {
            animator.Play(clearAnimation.name);
            //玩家得分,播放声音
            GameManager.Instance.playerScore++;
            AudioSource.PlayClipAtPoint(clearClip, transform.position);
            yield return new WaitForSeconds(clearAnimation.length);
            Destroy(gameObject);
        }


    }
}

