using UnityEngine;

namespace MyAssets
{
    /*
     * 剣と盾などの装備の位置を変えるなどの処理をまとめて行うクラス
     */
    public class WeaponController : MonoBehaviour,IEquipment
    {
        [SerializeField]
        private Transform[]         weaponTransforms = new Transform[(int)WeaponPositionTag.Count];

        [SerializeField]
        private Transform           haveWeapon;

        public GameObject           HaveWeapon => haveWeapon.gameObject;

        [SerializeField]
        private ShieldController    shieldTool;
        public ShieldController     ShieldTool => shieldTool;

        private void Awake()
        {
            WeaponPosition[] weaponPosition = GetComponentsInChildren<WeaponPosition>();
            for(int i = 0; i < weaponTransforms.Length; i++)
            {
                if (weaponPosition[i].Tag == WeaponPositionTag.Hand)
                {
                    weaponTransforms[(int)WeaponPositionTag.Hand] = weaponPosition[i].ThisObject.transform;
                }
                else if (weaponPosition[i].Tag == WeaponPositionTag.Receipt)
                {
                    weaponTransforms[(int)WeaponPositionTag.Receipt] = weaponPosition[i].ThisObject.transform;
                }
            }

            shieldTool = GetComponentInChildren<ShieldController>();
            ICharacterSetup character = GetComponent<ICharacterSetup>();
            if(character != null)
            {
                shieldTool.Setup(GetComponent<ICharacterSetup>());
            }
        }

        public void SetInWeapon()
        {
            SetTransform(weaponTransforms[(int)WeaponPositionTag.Receipt]);
        }

        public void SetOutWeapon()
        {
            SetTransform(weaponTransforms[(int)WeaponPositionTag.Hand]);
        }

        private void SetTransform(Transform nextTransform)
        {
            haveWeapon.SetParent(null);
            haveWeapon.SetParent(nextTransform);
            haveWeapon.position = nextTransform.position;
            haveWeapon.rotation = nextTransform.rotation;
        }
    }
}
