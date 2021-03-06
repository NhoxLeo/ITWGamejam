﻿using UnityEngine;
using System.Collections;
using DG;

public class Heavy : ICharacter
{
    [SerializeField]
    private float skillMoveTime;
    [SerializeField]
    private float skillMoveDistance;
    [SerializeField]
    private GameObject explosionPrefab;

    private Coroutine skillCoroutine = null;
    protected override void Awake()
    {
        base.Awake();
        CharacterType = CharacterType.Heavy;
    }

    protected override bool CanMove()
    {
        if(State == CharacterState.SkillActivated)
        {
            return false;
        }
        return base.CanMove();
    }

    protected override void Charge()
    {
        base.Charge();
        chargeEffect.transform.localScale *= 1.5f;
    }
    protected override void UseSkill()
    {
        base.UseSkill();
        animator.Play("skill", 0);
        IsInvincible = true;
        State = CharacterState.SkillActivated;
        SetCollidable(false);
        skillCoroutine = StartCoroutine(SkillProcess());
    }

    private IEnumerator SkillProcess()
    {
        float elapsedTime = 0f;
        Vector2 startPos = transform.position;
        Vector2 endPos = startPos + (prevMovedDirection * skillMoveDistance);

        bool sfxPlayed = false;
        while(elapsedTime < skillMoveTime)
        {
            yield return null;
            animator.enabled = false;
            elapsedTime += Time.deltaTime;
            transform.position = AnimationHelper.Arc(startPos, endPos, 0.5f, elapsedTime / skillMoveTime);
            if(elapsedTime / skillMoveTime > 0.3f)
            {
                animator.enabled = true;
            }
            if(elapsedTime / skillMoveTime > 0.6f && sfxPlayed == false)
            {
                sfxPlayed = true;
                SfxManager.Instance.Play(SfxType.Heavy_Skill);
            }
        }
        transform.position = endPos;

        Explosion newExplosion = Instantiate(explosionPrefab).GetComponent<Explosion>();
        newExplosion.SetOwner(this);
        newExplosion.gameObject.layer = LayerMask.NameToLayer(LayerNames.Team + Player.TeamNumber.ToString());
        newExplosion.transform.position = this.transform.position;


        if(State == CharacterState.SkillActivated)
        {
            OnSkillEnd();
        }
    }

    protected override void OnSkillEnd()
    {
        if(skillCoroutine != null)
        {
            StopCoroutine(skillCoroutine);
            skillCoroutine = null;
        }
        SetCollidable(true);
        animator.enabled = true;
        State = CharacterState.Idle;
        IsInvincible = false;
    }

    protected override void Dodge()
    {
        base.Dodge();
        SetCollidable(true);
    }
    protected override IEnumerator DodgeProcess()
    {
        IsDodgeCoolTime = true;
        EffectController.Instance.ShowEffect(EffectType.Heavy_Evade, new Vector2(0f, 0.1f), this.transform);
        yield return new WaitForSeconds(dodgeDuration);

        if(State == CharacterState.Dodge)
        {
            OnDodgeEnd();
        }

        yield return new WaitForSeconds(dodgeCoolTime - dodgeDuration);

        IsDodgeCoolTime = false;
    }

    protected override void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag(TagNames.Character) == true)
        {
            ICharacter otherCharacter = other.gameObject.GetComponent<ICharacter>();
            if(otherCharacter.State == CharacterState.Flying
                && this.State == CharacterState.Dodge)
            {
                EffectController.Instance.ShowEffect(EffectType.Heavy_Counter, Vector2.zero, this.transform);
                SfxManager.Instance.Play(SfxType.Heavy_Counter);
                otherCharacter.OnHit(this, 1, true);
                return;
            }
        }
        else if(other.gameObject.CompareTag(TagNames.Map) == true)
        {
            if(State == CharacterState.SkillActivated)
            {
                OnSkillEnd();
                return;
            }
        }
        base.OnCollisionEnter2D(other);
    }
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
    }

    protected override void Launch()
    {
        base.Launch();
        SfxManager.Instance.Play(SfxType.Heavy_Fly);
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        SfxManager.Instance.Play(SfxType.Heavy_Dead);
    }
}