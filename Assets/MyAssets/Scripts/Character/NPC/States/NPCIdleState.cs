using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    /*
     * NPCの待機状態
     */
    [System.Serializable]
    public class NPCIdleState : NPCStateBase
    {
        private IMovement               movement;

        [SerializeField]
        private float                   moveSpeed;

        public static readonly string   StateKey = "Idle";
        public override string          Key => StateKey;

        public override List<ICharacterStateTransition<string>> CreateTransitionList(INPCSetup actor)
        {
            List<ICharacterStateTransition<string>> re = new List<ICharacterStateTransition<string>>();
            if (StateChanger.IsContain(NPCMoveState.StateKey)) { re.Add(new IsNPCMoveTransition(actor, StateChanger, NPCMoveState.StateKey)); }
            return re;
        }

        public override void DoSetup(INPCSetup actor)
        {
            base.DoSetup(actor);
            movement = actor.Movement;
        }

        public override void DoStart()
        {
            base.DoStart();
        }

        public override void DoUpdate(float time)
        {
            base.DoUpdate(time);
        }

        public override void DoFixedUpdate(float time)
        {
            base.DoFixedUpdate(time);
            movement.Move(moveSpeed);
        }

        public override void DoExit()
        {
            base.DoExit();
            
        }
    }
}
