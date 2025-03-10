using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    /*
     * プレイヤーのダメージ状態
     */
    [System.Serializable]
    public class PlayerDamageState : PlayerStateBase
    {
        private Transform               thisTransform;

        private IVelocityComponent      velocity;

        private IPlayerAnimator         animator;

        private IDamageContainer        damageContainer;

        private IDamagement             damageMove;

        private IGroundCheck            groundCheck;

        private SEHandler               seHandler;

        private Timer                   damageTimer = new Timer();

        private IPlayerStatus           stauts;

        [SerializeField]
        private float                   decreaseForce = 0.9f;

        [SerializeField]
        private float                   damageGravityMultiply = 2.0f;

        public static readonly string   StateKey = "Damage";
        public override string          Key => StateKey;

        public override List<ICharacterStateTransition<string>> CreateTransitionList(IPlayerSetup actor)
        {
            List<ICharacterStateTransition<string>> re = new List<ICharacterStateTransition<string>>();
            if (StateChanger.IsContain(GetUpState.StateKey)) { re.Add(new IsPlayerDamageToGetUpTransition(actor, damageTimer, StateChanger, GetUpState.StateKey)); }
            if (StateChanger.IsContain(MoveState.StateKey)) { re.Add(new IsNotPlayerDamageToTransition(actor,damageTimer, StateChanger, MoveState.StateKey)); }
            if (StateChanger.IsContain(PlayerIdleState.StateKey)) { re.Add(new IsNotPlayerDamageToTransition(actor, damageTimer, StateChanger, PlayerIdleState.StateKey)); }
            if (StateChanger.IsContain(BattleIdleState.StateKey)) { re.Add(new IsNotPlayerDamageToBattleTransition(actor, damageTimer, StateChanger, BattleIdleState.StateKey)); }
            if (StateChanger.IsContain(BattleMoveState.StateKey)) { re.Add(new IsNotPlayerDamageToBattleTransition(actor, damageTimer, StateChanger, BattleMoveState.StateKey)); }
            if (StateChanger.IsContain(PlayerDeathState.StateKey)) { re.Add(new IsDeathTransition(actor, StateChanger, PlayerDeathState.StateKey)); }
            return re;
        }
        public override void DoSetup(IPlayerSetup actor)
        {
            base.DoSetup(actor);
            thisTransform = actor.gameObject.transform;
            velocity = actor.Velocity;
            animator = actor.PlayerAnimator;
            groundCheck = actor.GroundCheck;
            damageContainer = actor.DamageContainer;
            damageMove = actor.Damagement;
            seHandler = actor.SEHandler;
            stauts = actor.Stauts;
        }

        public override void DoStart()
        {
            base.DoStart();

            seHandler.Play((int)PlayerSETag.Damage);

            velocity.Rigidbody.velocity = Vector3.zero;

            DamageType type = damageContainer.AttackType;
            int damageType = -1;
            if (groundCheck.Landing)
            {
                switch (type)
                {
                    case DamageType.Small:
                        damageType = -1;
                        Debug.Log("小ダメージ通った");
                        break;
                    case DamageType.Middle:
                        damageType = 0;
                        damageMove.AddForceMove(thisTransform.position, damageContainer.Attacker.position, damageContainer.KnockBack * 1.5f);
                        damageTimer.Start(1.0f);
                        break;
                    case DamageType.Big:
                        damageType = 1;
                        damageMove.AddForceMove(thisTransform.position, damageContainer.Attacker.position, damageContainer.KnockBack * 4.0f);
                        damageTimer.Start(1.5f);
                        break;
                }
            }
            else
            {
                damageType = 1;
                damageMove.AddForceMove(thisTransform.position, damageContainer.Attacker.position, damageContainer.KnockBack * 2.0f);
                damageTimer.Start(1.5f);
            }
            animator.Animator.SetInteger(animator.ImpactAnimationID, damageType);
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
            damageContainer.ClearDamage();
            animator.Animator.SetInteger(animator.ImpactAnimationID, -1);
            stauts.ClearStoredDamage();
        }
    }
}
