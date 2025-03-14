using System.Collections.Generic;
using UnityEngine;

namespace MyAssets
{
    public enum SpawnEffectType
    {
        Spawn
    }


    /*
     * 敵を生成するだけのクラス
     */
    public class SpawnEnemy : MonoBehaviour
    {
        [SerializeField]
        private EnemyLedger         enemyLedger;

        [SerializeField]
        private EffectLedger        effectLedger;

        [SerializeField]
        private SpawnPoint[]        spawnPoint;



        private ItemCreateMachine   itemCreateMachine;

        private void Awake()
        {
            spawnPoint = GetComponentsInChildren<SpawnPoint>();
            itemCreateMachine = FindObjectOfType<ItemCreateMachine>();
        }

        public void SetActive(bool a)
        {
            gameObject.SetActive(a);
        }
        private void Update()
        {
            if (!GameManager.Instance.Debug)
            {
                DoUpdate();
            }
            else
            {
                DebugUpdate();
            }

        }

        private void DoUpdate()
        {
            if (GameModeController.Instance.AbstractGameMode.IsEnd) { return; }
            Spawn();
        }

        private int SelectSpawnPoint()
        {
            // 値を格納するリストを作成
            List<int> numbers = new List<int>();
            if (GameModeController.Instance.AbstractGameMode.SpawnLimit)
            {
                // 指定された範囲の数値をリストに追加
                for (int i = 0; i < spawnPoint.Length; i++)
                {
                    if (!spawnPoint[i].IsUse) // 除外値をスキップ
                    {
                        numbers.Add(i);
                    }
                }
            }
            else
            {
                // 指定された範囲の数値をリストに追加
                for (int i = 0; i < spawnPoint.Length; i++)
                {
                    numbers.Add(i);
                }
            }

            // リストからランダムな値を選択
            int randomIndex = Random.Range(0, numbers.Count);
            return numbers[randomIndex];
        }

        private void Spawn()
        {
            if (!GameModeController.Instance.AbstractGameMode.WaveChange) { return; }
            int enemyIndex = Random.Range(0,(int)EnemyTag.Count);
            //スポーンポイントを設定
            int spawnIndex = SelectSpawnPoint();
            Vector3 pos = spawnPoint[spawnIndex].SpawnPositionOutput();
            //スポーン
            CharacterBaseController enemy = Instantiate(enemyLedger[enemyIndex], pos, spawnPoint[spawnIndex].transform.rotation);
            Instantiate(effectLedger[(int)SpawnEffectType.Spawn], pos, spawnPoint[spawnIndex].transform.rotation);
            //カウント
            SpawnCount count = enemy.gameObject.AddComponent<SpawnCount>();
            count.SetSpawnPoint(spawnPoint[spawnIndex]);

            if(itemCreateMachine != null)
            {
                count.SetItemCreateMachine(itemCreateMachine);
            }
        }
        //シーンに存在する敵をすべて削除する
        public void AllEnemyDestroy()
        {
            SpawnCount[] spawnEnemys = FindObjectsOfType<SpawnCount>();
            for (int i = 0; i < spawnEnemys.Length; i++)
            {
                Destroy(spawnEnemys[i].gameObject);
            }
            Destroy(gameObject);
        }
        //ここからしたはデバッグ処理↓
        [SerializeField]
        private EnemyTag    enemyTag;

        [SerializeField]
        private bool        debugSpawn = false;

        private void DebugUpdate()
        {
            if (!debugSpawn) { return; }
            DebugSpawn();
        }
        private void DebugSpawn()
        {
            int enemyIndex = (int)enemyTag;
            int spawnIndex = SelectSpawnPoint();
            CharacterBaseController enemy = Instantiate(enemyLedger[enemyIndex], spawnPoint[spawnIndex].transform.position, spawnPoint[spawnIndex].transform.rotation);
            SpawnCount count = enemy.gameObject.AddComponent<SpawnCount>();
            count.SetSpawnPoint(spawnPoint[spawnIndex]);
            debugSpawn = false;
        }
    }
}
