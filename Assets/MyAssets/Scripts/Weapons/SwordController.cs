using UnityEngine;

namespace MyAssets
{
    public enum SwordSEType
    {
        Slash1,
        Slash2,
        Succession,
        Hit1,
        Hit2
    }
    /*
     * プレイヤーが所持してる剣のクラス
     * 表示・非表示やコライダーの設定、当たり判定の区別を行っている
     */
    //RequireComponentでそのほかのコンポーネントも自動でアタッチ
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(SwordEffectHandler))]
    public class SwordController : BaseAttackController
    {

        [SerializeField]
        private IPlayerAnimator         animator;

        private IPlayerSetup            actor;

        [SerializeField]
        private new CapsuleCollider     collider;


        //保存用のcenter・radius・height
        private Vector3                 center;

        private float                   radius;

        private float                   height;


        private int                     attackCount;

        private int                     hitCount;

        private SwordEffectHandler      swordEffectHandler;

        
        private float                   ratioPower = 1.0f;

        public void SetAttackType(AttackType type,SwordSEType seType)
        {
            attackType = type;
            attackCount = (int)seType;
            if(type == AttackType.Normal)
            {
                hitCount = (int)SwordSEType.Hit1;
            }
            else if(type == AttackType.Charge)
            {
                hitCount = (int)SwordSEType.Hit2;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            PlayerController controller = GetComponentInParent<PlayerController>();

            if(controller != null)
            {
                actor = controller.GetComponent<IPlayerSetup>();
                animator = controller.PlayerAnimator;
            }

            collider = GetComponent<CapsuleCollider>();

            swordEffectHandler = GetComponent<SwordEffectHandler>();

            center = collider.center;
            radius = collider.radius;
            height = collider.height;
        }

        private void NoActivateCollider()
        {
            collider.center = Vector3.zero;
            collider.radius = 0.0f;
            collider.height = 0.0f;
        }

        private void ActivateCollider()
        {
            collider.center = center;
            collider.radius = radius;
            collider.height = height;
        }

        public void Slash()
        {
            seHandler.Play(attackCount);
        }

        public void SpinSlash()
        {
            seHandler.OnPlay((int)SwordSEType.Succession);
        }

        private void Start()
        {
            collider.enabled = false;
        }

        public void EnabledCollider(float start,float end,bool all)
        {
            if (all)
            {
                collider.enabled = true;
                swordEffectHandler.ActivateSlachEffect(true);
                ActivateCollider();
            }
            else
            {
                AnimatorStateInfo animInfo = animator.Animator.GetCurrentAnimatorStateInfo(0);
                if(animInfo.normalizedTime >= start &&animInfo.normalizedTime <= end)
                {
                    collider.enabled = true;
                    ActivateCollider();
                    swordEffectHandler.ActivateSlachEffect(true);
                }
                else
                {
                    NoActivateCollider();
                    swordEffectHandler.ActivateSlachEffect(false);
                }
            }
        }
        public void NotEnabledCollider()
        {
            NoActivateCollider();
            swordEffectHandler.ActivateSlachEffect(false);
            collider.enabled = false;
        }

        public float GetRatioPower()
        {
            return ratioPower;
        }
        public void SetRatioPower(float r)
        {
            ratioPower = r;
        }

        //基礎ダメージと武器のダメージ分
        private float GetPower()
        {
            return (attackObject.Power + actor.Stauts.BasePower) * GetRatioPower();
        }

        private void OnTriggerEnter(Collider other)
        {
            if(attackType == AttackType.Succession) { return; }
            if (other.gameObject.layer != 8) { return; }
            ICharacterSetup characterSetup = other.GetComponentInChildren<ICharacterSetup>();
            if (characterSetup == null) { return; }
            IDamageContainer damageContainer = characterSetup.DamageContainer;
            if (damageContainer == null) { return; }
            if (damageContainer.IsDeath) { return; }
            swordEffectHandler.EffectLedger.SetPosAndRotCreate((int)SwordEffectType.Hit, other.ClosestPoint(transform.position), other.transform.rotation);
            seHandler.Play(hitCount);
            damageContainer.GiveDamage((int)GetPower(), attackObject.KnockBack, attackObject.Type, transform,actor.CharaType);
        }

        private void OnTriggerStay(Collider other)
        {
            if(attackType != AttackType.Succession) { return; }
            if (other.gameObject.layer != 8) { return; }
            ICharacterSetup characterSetup = other.GetComponentInChildren<ICharacterSetup>();
            if (characterSetup == null) { return; }
            IDamageContainer damageContainer = characterSetup.DamageContainer;
            if (damageContainer == null){return;}
            if (damageContainer.IsDeath) { return; }
            swordEffectHandler.EffectLedger.SetPosAndRotCreate((int)SwordEffectType.Hit, other.ClosestPoint(transform.position),other.transform.rotation);
            seHandler.Play(hitCount);
            damageContainer.GiveDamage((int)GetPower(), attackObject.KnockBack, attackObject.Type, transform, actor.CharaType);
        }

    }
}
