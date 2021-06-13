using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

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

        for (float y = 15f; y < targetHeight - 5f; y += 1f)
        {
            var newItem = ObjectPoolManager.Instance.SpawnObject(PrefabManager.Instance.GetPrefab(200));
            newItem.transform.position = new Vector3(Random.Range(-4f, 4f), y, 0f);
        }
    }
}
