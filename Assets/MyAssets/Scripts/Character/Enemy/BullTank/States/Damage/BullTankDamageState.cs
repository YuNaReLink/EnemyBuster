using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    /*
     * ブルタンクのダメージ状態
     */
    [System.Serializable]
    public class BullTankDamageState : BullTankStateBase
    {
        private IBaseStatus             stauts;

        private Transform               thisTransform;

        private IVelocityComponent      velocity;

        private IBullTankAnimator       animator;

        private IDamageContainer        damageContainer;

        private IDamagement             damageMove;

        private FieldOfView             fieldOfView;

        private Timer                   damageTimer = new Timer();


        [SerializeField]
        private float                   decreaseForce = 0.9f;

        [SerializeField]
        private float                   damageGravityMultiply = 2.0f;

        [SerializeField]
        private float                   damageIdleCount = 0.5f;

        public static readonly string   StateKey = "Damage";
        public override string          Key => StateKey;

        public override List<ICharacterStateTransition<string>> CreateTransitionList(IBullTankSetup actor)
        {
            List<ICharacterStateTransition<string>> re = new List<ICharacterStateTransition<string>>();
            if (StateChanger.IsContain(BullTankIdleState.StateKey)) { re.Add(new IsNotDamageToTransition(actor, damageTimer, StateChanger, BullTankIdleState.StateKey)); }
            if (StateChanger.IsContain(ChaseState.StateKey)) { re.Add(new IsTimerTargetInViewTransition(actor, damageTimer, StateChanger, ChaseState.StateKey)); }
            if (StateChanger.IsContain(BullTankDeathState.StateKey)) { re.Add(new IsDeathTransition(actor, StateChanger, BullTankDeathState.StateKey)); }
            return re;
        }
        public override void DoSetup(IBullTankSetup actor)
        {
            base.DoSetup(actor);
            stauts = actor.BaseStatus;
            thisTransform = actor.gameObject.transform;
            velocity = actor.Velocity;
            animator = actor.BullTankAnimator;
            damageContainer = actor.DamageContainer;
            damageMove = actor.Damagement;
            fieldOfView = actor.gameObject.GetComponent<FieldOfView>();
        }

        public override void DoStart()
        {
            base.DoStart();
            FoundTarget();
            velocity.Rigidbody.velocity = Vector3.zero;
            DamageType type = damageContainer.AttackType;
            int damageType = 0;
            damageMove.AddForceMove(thisTransform.position, damageContainer.Attacker.position, damageContainer.KnockBack * 1.0f);
            damageTimer.Start(damageIdleCount);
            animator.Animator.SetInteger(animator.ImpactAnimationID, damageType);
        }

        private void FoundTarget()
        {
            fieldOfView.SetAllSearch(true);
            Vector3 target = damageContainer.Attacker.position;
            target = new Vector3(target.x, thisTransform.position.y, target.z);
            thisTransform.LookAt(target);
        }

        public override void DoUpdate(float time)
        {
            base.DoUpdate(time);
            damageTimer.Update(time);
        }

        public override void DoFixedUpdate(float time)
        {
            base.DoFixedUpdate(time);
            //AddForceで与えた分を少しずつ減少
            Vector3 v = velocity.Rigidbody.velocity;
            velocity.Rigidbody.velocity = v * decreaseForce;
            velocity.Rigidbody.velocity += Physics.gravity * damageGravityMultiply * time;
        }

        public override void DoExit()
        {
            base.DoExit();
            fieldOfView.SetAllSearch(false);
            damageContainer.ClearDamage();
            animator.Animator.SetInteger(animator.ImpactAnimationID, -1);
            stauts.ClearStoredDamage();
        }
    }
}
