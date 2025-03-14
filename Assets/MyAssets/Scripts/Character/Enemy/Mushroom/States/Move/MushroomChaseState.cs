using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    /*
     マッシュルームの追跡状態*/
    [System.Serializable]
    public class MushroomChaseState : MushroomStateBase
    {
        private IMovement               movement;
        private IVelocityComponent      velocity;
        private Transform               thisTransform;
        private FieldOfView             fieldOfView;

        private IMushroomAnimator       animator;

        private Timer                   currentSearchinTimer = new Timer();
        private GameObject              targetObject;
        private Vector3                 targetLastPoint;

        [SerializeField]
        private float                   moveSpeed = 4;
        [SerializeField]
        private float                   rotationSpeed = 8;
        [SerializeField]
        private float                   moveSpeedChangeRate = 8;

        [SerializeField]
        private float                   searchingTime = 1.0f;

        [SerializeField]
        private float                   minChaseDistance = 2.5f;

        [SerializeField]
        private float                   gravityMultiply;

        public static readonly string   StateKey = "Chase";
        public override string          Key => StateKey;

        public override List<ICharacterStateTransition<string>> CreateTransitionList(IMushroomSetup enemy)
        {
            List<ICharacterStateTransition<string>> re = new List<ICharacterStateTransition<string>>();
            if (StateChanger.IsContain(MushroomIdleState.StateKey)) { re.Add(new IsNoTargetInViewTransition(enemy, StateChanger, MushroomIdleState.StateKey)); }
            if (StateChanger.IsContain(MushroomDamageState.StateKey)) { re.Add(new IsEnemyDamageTransition(enemy, StateChanger, MushroomDamageState.StateKey)); }
            if (StateChanger.IsContain(MushroomAttackState.StateKey)) { re.Add(new IsMushroomAttackTransition(enemy, StateChanger, MushroomAttackState.StateKey)); }
            if (StateChanger.IsContain(MushroomDeathState.StateKey)) { re.Add(new IsDeathTransition(enemy, StateChanger, MushroomDeathState.StateKey)); }
            return re;
        }

        public override void DoSetup(IMushroomSetup actor)
        {
            base.DoSetup(actor);
            movement = actor.Movement;
            velocity = actor.Velocity;
            thisTransform = actor.gameObject.transform;
            fieldOfView = actor.gameObject.GetComponent<FieldOfView>();
            animator = actor.MushroomAnimator;
        }

        public override void DoStart()
        {
            if (fieldOfView.TryGetFirstObject(out targetObject))
            {
                targetLastPoint = targetObject.transform.position;
            }
            currentSearchinTimer.End();
        }

        void TargetUpdate()
        {
            //今追っかけてるオブジェクトが見える
            if (fieldOfView.IsInside(targetObject))
            {
                targetLastPoint = targetObject.transform.position;
                currentSearchinTimer.End();
                return;
            }

            //見えないなら

            //新しいオブジェクトが見えたらそちらを追いかけるように切り替える
            if (fieldOfView.TryGetFirstObject(out var obj))
            {
                targetObject = obj;
                targetLastPoint = targetObject.transform.position;
                currentSearchinTimer.End();
                return;
            }

            //新しいオブジェクトもいないなら,一定時間で終了するためタイマースタート
            if (currentSearchinTimer.IsEnd())
            {
                currentSearchinTimer.Start(searchingTime);
            }
        }

        public override void DoFixedUpdate(float time)
        {
            base.DoFixedUpdate(time);
            TargetUpdate();
            currentSearchinTimer.Update(time);

            Vector3 targetVec = targetLastPoint - thisTransform.position;
            targetVec.y = 0.0f;
            float targetDistance = targetVec.magnitude;
            if (targetDistance < minChaseDistance)
            {
                animator.Animator.SetInteger(animator.MoveAnimationID, Define.Zero);
                animator.Animator.SetInteger(animator.AttackAnimationID, 0);
                movement.Stop();
            }
            else
            {
                animator.Animator.SetInteger(animator.MoveAnimationID, 1);
                movement.MoveTo(targetLastPoint, moveSpeed, moveSpeedChangeRate, rotationSpeed, time);
            }
            velocity.Rigidbody.velocity += Physics.gravity * gravityMultiply * time;
        }

        public override void DoExit()
        {
            base.DoExit();
            animator.Animator.SetInteger(animator.MoveAnimationID, Define.Zero);
            movement.Stop();
        }
    }
}
