using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    /*
     * ラッシュアタック開始状態
     */
    [System.Serializable]
    public class RushAttackStartState : BullTankStateBase
    {
        private IMovement                       movement;
        private IVelocityComponent              velocity;

        private IBullTankAnimator               animator;

        private BullTankHeadAttackController    headWeapon;

        [SerializeField]
        private float                           gravityMultiply;

        public static readonly string           StateKey = "RushStart";
        public override string                  Key => StateKey;

        public override List<ICharacterStateTransition<string>> CreateTransitionList(IBullTankSetup actor)
        {
            List<ICharacterStateTransition<string>> re = new List<ICharacterStateTransition<string>>();
            if (StateChanger.IsContain(RushAttackLoopState.StateKey)) { re.Add(new IsNotBullTankAttackTransition(actor, "Attack02_Run_Start", StateChanger, RushAttackLoopState.StateKey)); }
            return re;
        }

        public override void DoSetup(IBullTankSetup actor)
        {
            base.DoSetup(actor);
            movement = actor.Movement;
            velocity = actor.Velocity;
            animator = actor.BullTankAnimator;
            headWeapon = actor.HeadAttackObject;
        }

        public override void DoStart()
        {
            base.DoStart();
            movement.Stop();
            animator.Animator.SetInteger(animator.AttackAnimationID, 1);
            headWeapon.EnabledCollider(0,0,true);
        }

        public override void DoFixedUpdate(float time)
        {
            base.DoFixedUpdate(time);
            velocity.Rigidbody.velocity += Physics.gravity * gravityMultiply * time;
        }

        public override void DoExit()
        {
            base.DoExit();
            animator.Animator.SetInteger(animator.AttackAnimationID, -1);
            movement.Stop();
        }
    }
}
