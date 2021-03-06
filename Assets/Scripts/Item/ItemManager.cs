using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

namespace Item
{
    public class GenerateItemMsg : Message
    {
    }


    public class ItemManager : MonoBehaviour
    {
        public List<MyItem> itemList;


        private void Awake()
        {
            Message.AddListener<GenerateItemMsg>(OnGenerateItemMsg);
        }

        private void OnDestroy()
        {
            Message.RemoveListener<GenerateItemMsg>(OnGenerateItemMsg);
        }

        void OnGenerateItemMsg(GenerateItemMsg msg)
        {
            GenerateItem();
        }

        void GenerateItem()
        {
            GameContentModel model = Model.First<GameContentModel>();
            float targetHeight = model.targetHeight;

            float x = Random.Range(-4f, 4f);
            for (float y = 15f; y < targetHeight - 5f; y += 1.5f)
            {
                //TODO : Get Component ????ȭ 
                var newItem =
                    ObjectPoolManager.Instance.SpawnObject(
                        PrefabManager.Instance.GetPrefab(Random.Range(0, 20) == 0 ? 201 : 200)).GetComponent<MyItem>();

                x += Random.Range(-1f, 1f);
                if (x > 4f)
                {
                    x -= 1f;
                }
                else if (x < -4f)
                {
                    x += 1f;
                }

                newItem.transform.position = new Vector3(x, y, 0f);
                newItem.Init();
            }
        }
    }
}