using System.Collections;
using System.Collections.Generic;
using Manager.Pooling;
using UnityEngine;

public class HunterBase : PoolingObject
{
    public enum State
    {
        Idle,
        Patrol, Track,
        Attack, Defense,
        Dead,
        NONE = 99
    }

    [SerializeField]
    public class Stat
    {
        public float curHP       = 100f, maxHP = 100f;
        public float moveSpeed   = 5f;
        public float attackSpeed = 1.5f;
        public State curState    = State.Idle;
    } [HideInInspector] public Stat hunterStat = new Stat();

    public override string objectName => "Hunter";

    public override sealed void Init()
    {
        hunterStat.curState = State.Idle;
        StartCoroutine(hunterStat.curState.ToString());
        base.Init();
    }

    public override sealed void Release()
    {
        StopCoroutine(hunterStat.curState.ToString());
        base.Release();
    }

    public virtual void HunterIdle() { }

    public virtual void HunterPatrol() { }

    public virtual void HunterTrack() { }

    public virtual void HunterAttack() { }

    public virtual void HunterDefense() { }

    public virtual void HunterDead() { }

    void MoveState()
    {
        if (hunterStat.curState != State.Dead)
            StartCoroutine(hunterStat.curState.ToString());
        else
            Dead();
    }

    IEnumerator Idle()
    {
        while (hunterStat.curState == State.Idle)
        {
            HunterIdle();
            yield return null;
        }
        MoveState();
    }

    IEnumerator Patrol()
    {
        while (hunterStat.curState == State.Patrol)
        {
            HunterPatrol();
            yield return null;
        }
        MoveState();
    }

    IEnumerator Track()
    {
        while (hunterStat.curState == State.Track)
        {
            HunterTrack();
            yield return null;
        }
        MoveState();
    }

    IEnumerator Attack()
    {
        while (hunterStat.curState == State.Attack)
        {
            HunterAttack();
            yield return null;
        }
        MoveState();
    }

    IEnumerator Defense()
    {
        while (hunterStat.curState == State.Defense)
        {
            HunterDefense();
            yield return null;
        }
        MoveState();
    }

    void Dead()
    {
        if (hunterStat.curState == State.Dead)
            Release();
    }
}
