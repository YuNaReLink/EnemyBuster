using UnityEngine;

namespace MyAssets
{
    /*
     * スライムの制御クラス
     */
    public class SlimeController : CharacterBaseController,ISlimeSetup
    {
        [SerializeField]
        private SlimeStatusProperty         property;
        public IBaseStatus                  BaseStatus => property;

        [SerializeField]
        private Movement                    movement;
        public IMovement                    Movement => movement;
        
        private StepClimberJudgment         stepClimberJudgment;
        public IStepClimberJudgment         StepClimberJudgment => stepClimberJudgment;
        
        [SerializeField]
        private SlimeRotation               rotation;
        public IRotation                    Rotation => rotation;
        public ISlimeRotation               SlimeRotation => rotation;

        [SerializeField]
        private SlimeAnimator               animator;
        public IEnemyAnimator               EnemyAnimator => animator;
        public ISlimeAnimator               SlimeAnimator => animator;

        private FieldOfView                 fieldOfView;
        public IFieldOfView                 FieldOfView =>  fieldOfView;

        [SerializeField]
        private GuardTrigger                guardTrigger;
        public IGuardTrigger                GuardTrigger => guardTrigger;

        private SlimeEffectHandler          effectHandler;
        public SlimeEffectHandler           EffectHandler => effectHandler;

        private SEHandler                   seHandler;
        public SEHandler                    SEHandler => seHandler;

        [SerializeField]
        private SlimeBodyAttackController   attackObject;
        public SlimeBodyAttackController    AttackObject => attackObject;

        [SerializeField]
        private StateMachine<string>        stateMachine;
        public IStateMachine                StateMachine => stateMachine;

        [SerializeField]
        private string                      defaultStateKey;

        [SerializeField]
        private SlimeIdleState              idleState;


        [SerializeField]
        private SlimePatrolState            patrolState;

        [SerializeField]
        private ChaseState                  chaseState;

        [SerializeField]
        private ReadySlimeAttackState       readyAttackState;

        [SerializeField]
        private SlimeAttackState            attackState;

        [SerializeField]
        private SlimeDamageState            damageState;

        [SerializeField]
        private SlimeDeathState             deathState;

        ISlimeState<string>[]               states;


        public override CharacterType       CharaType => CharacterType.Enemy;
        protected override void Awake()
        {
            fieldOfView = GetComponent<FieldOfView>();
            attackObject = GetComponentInChildren<SlimeBodyAttackController>();

            effectHandler = GetComponent<SlimeEffectHandler>();
            seHandler = GetComponent<SEHandler>();

            animator.DoSetup(this);
            velocity.DoSetup(this);
            movement.DoSetup(this);
            rotation.DoSetup(this);
            damageContainer.DoSetup(this);
            damagement.DoSetup(this);

            property.Setup();

            states = new ISlimeState<string>[]
            {
                idleState,
                patrolState,
                chaseState,
                readyAttackState,
                attackState,
                damageState,
                deathState
            };
            stateMachine.DoSetup(states);
            foreach (var state in states)
            {
                state.DoSetup(this);
            }
            stateMachine.ChangeState(defaultStateKey);

            groundCheck.SetTransform(transform);

            if (animator == null)
            {
                Debug.LogError("Animatorがアタッチされていません");
            }
        }

        protected override void Update()
        {
            if (SystemManager.IsPause) { return; }
            float t = Time.deltaTime;
            property.DoUpdate(t);
            groundCheck.DoUpdate();
            stateMachine.DoUpdate(t);
            fieldOfView.DoUpdate();
        }

        protected override void FixedUpdate()
        {
            stateMachine.DoFixedUpdate(Time.deltaTime);
        }

        private void OnAnimatorIK()
        {
            stateMachine.DoAnimatorIKUpdate();
        }

        private void OnDestroy()
        {
            stateMachine.Dispose();
        }

        protected override void OnTriggerEnter(Collider other)
        {
            stateMachine.DoTriggerEnter(gameObject,other);
        }

        protected override void OnTriggerStay(Collider other)
        {
            stateMachine.DoTriggerStay(gameObject,other);
        }

        protected override void OnTriggerExit(Collider other)
        {
            stateMachine.DoTriggerExit(gameObject,other);
        }

        public void RunDestroy()
        {
            effectHandler.EffectLedger.SetPosAndRotCreate((int)SlimeEffectType.Death,transform.position,transform.rotation);
            Destroy(gameObject);
        }
    }
}
