using System.Collections;
using System.Collections.Generic;
using Manager.Pooling;
using UnityEngine.AI;
using UnityEngine;

namespace GhoseHouse.Object.Hunter
{
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

        [System.Serializable]
        public class Stat
        {
            public float curHP = 100f, maxHP = 100f;
            public float moveSpeed = 5f;
            public float attackSpeed = 1.5f;
            [Tooltip("추적 범위")] public float trackRange = 10f;
            [Tooltip("공격/방어 범위")] public float eventRange = 5f;
            [Tooltip("현재 AI 상태")] public State curState = State.Idle;
        }
        [Header("Hunter Base")] [Tooltip("헌터 기본정보")] public Stat hunterStat = new Stat();
        [HideInInspector] protected NavMeshAgent ai;
        [HideInInspector] protected GameObject target;

        public override string objectName => "Hunter";

        void Awake()
        {
            ai = GetComponent<NavMeshAgent>();
            target = GameObject.FindGameObjectWithTag("Player");
        }

        public override sealed void Init()
        {
            base.Init();
            hunterStat.curState = State.Idle;
            StartCoroutine(hunterStat.curState.ToString());
        }

        public override sealed void Release()
        {
            base.Release();
            StopCoroutine(hunterStat.curState.ToString());
        }

        public virtual void HunterIdle() { }

        public virtual void HunterPatrol() { }

        public virtual void HunterTrack() { }

        public virtual void HunterAttack() { }

        public virtual void HunterDefense() { }

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
}