using UnityEngine;

namespace MyAssets
{
    public interface ISlimeRotation
    {
        void DoLookOnTarget(Transform target);
    }

    [System.Serializable]
    public class SlimeRotation : IRotation,ISlimeRotation, ICharacterComponent<ISlimeSetup>
    {
        [SerializeField]
        private Transform thisTransform;

        [SerializeField]
        private float rotationSpeed;

        public void DoSetup(ISlimeSetup slime)
        {
            thisTransform = slime.gameObject.transform;
        }

        public void DoUpdate(){}

        public void DoFixedUpdate(){}


        public void DoLookOnTarget(Transform target)
        {
            // 対象への方向を計算
            Vector3 direction = target.position - thisTransform.position;

            // 現在の回転から目標の回転を計算
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // 現在の回転から目標の回転へスムーズに補間
            thisTransform.rotation = Quaternion.Slerp(
                thisTransform.rotation, // 現在の回転
                targetRotation,     // 目標の回転
                Time.deltaTime * rotationSpeed // 回転速度を調整
            );
        }

        public void DoFreeMode(){}
    }
}
