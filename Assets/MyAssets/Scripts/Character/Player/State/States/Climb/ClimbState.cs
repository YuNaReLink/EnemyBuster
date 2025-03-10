using System.Collections.Generic;

namespace MyAssets
{
    /*
     * プレイヤーの登り状態
     */
    [System.Serializable]
    public class ClimbState : PlayerStateBase
    {

        private IPlayerAnimator         animator;

        private IClimb                  climb;

        public static readonly string   StateKey = "Climb";
        public override string          Key => StateKey;

        public override List<ICharacterStateTransition<string>> CreateTransitionList(IPlayerSetup actor)
        {
            List<ICharacterStateTransition<string>> re = new List<ICharacterStateTransition<string>>();
            if (StateChanger.IsContain(MoveState.StateKey)) { re.Add(new IsEndClimbTransition(actor, StateChanger, MoveState.StateKey)); }
            if (StateChanger.IsContain(PlayerIdleState.StateKey)) { re.Add(new IsEndClimbTransition(actor, StateChanger, PlayerIdleState.StateKey)); }
            if (StateChanger.IsContain(PlayerDamageState.StateKey)) { re.Add(new IsDamageTransition(actor, StateChanger, PlayerDamageState.StateKey)); }
            if (StateChanger.IsContain(PlayerDeathState.StateKey)) { re.Add(new IsDeathTransition(actor, StateChanger, PlayerDeathState.StateKey)); }
            return re;
        }

        public override void DoSetup(IPlayerSetup actor)
        {
            base.DoSetup(actor);
            animator = actor.PlayerAnimator;
            climb = actor.Climb;
        }

        public override void DoStart()
        {
            base.DoStart();
            animator.Animator.SetBool(animator.ClimbAnimationID, true);
            climb.DoClimbStart();
        }

        public override void DoUpdate(float time)
        {
            base.DoUpdate(time);
            climb.DoClimb();
        }

        public override void DoFixedUpdate(float time)
        {
            base.DoFixedUpdate(time);
        }

        public override void DoExit()
        {
            base.DoExit();
            climb.DoClimbExit();
            animator.Animator.SetBool(animator.ClimbAnimationID, false);
        }
    }
}
