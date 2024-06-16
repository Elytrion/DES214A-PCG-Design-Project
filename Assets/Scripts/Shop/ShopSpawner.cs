using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct ShopItem
    {
        public GameObject ItemPrefab;
        public int ItemCost;
        public string DisplayText;
    }

    public List<ShopItem> AllShopItems;

    public Transform[] ItemSpawnpoints;

    public Entity VendingMachine;

    public List<ItemPickup> SpawnedItems;
    public int itemsInStock = 0;

    public int CostMultiplierPerItem = 3;

    // Start is called before the first frame update
    void Start()
    {
        SpawnShopItems();
    }

    // Update is called once per frame
    void Update()
    {
        if (itemsInStock <= 0)
        {
            VendingMachine.NoDamage = true;
        }

        if (VendingMachine.Health <= 0)
        {
            foreach (var item in SpawnedItems)
            {
                if (item == null)
                    continue;

                Destroy(item.gameObject);
            }
            foreach (var sps in ItemSpawnpoints)
            {
                if (sps == null)
                    continue;

                sps.gameObject.SetActive(false);
            }
            itemsInStock = 0;
            SpawnedItems = new List<ItemPickup>();
        }
    }

    private void SpawnShopItems()
    {
        List<ShopItem> itemsToSpawn = new List<ShopItem>(AllShopItems);

        for (int i = 0; i < ItemSpawnpoints.Length; i++)
        {
            int randomItemIndex = Random.Range(0, AllShopItems.Count);
            ShopItem randomShopItem = AllShopItems[randomItemIndex];
            AllShopItems.RemoveAt(randomItemIndex);
            GameObject item = Instantiate(randomShopItem.ItemPrefab, ItemSpawnpoints[i].position, Quaternion.identity);
            item.transform.parent = ItemSpawnpoints[i];
            var pickupLogic = item.GetComponent<ItemPickup>();
            var promptLogic = item.GetComponent<PickupPrompt>();
            pickupLogic.InstantPickup = false;
            pickupLogic.OnlyPlayerPickup = true;         
            pickupLogic.RequiresCost = true;
            pickupLogic.ItemCost = randomShopItem.ItemCost;
            pickupLogic.OnItemPickup.AddListener(OnItemPickedUp);
            SpawnedItems.Add(item.GetComponent<ItemPickup>());
            itemsInStock++;

            if (promptLogic != null)
            {
                promptLogic.PromptText.text = "Buy " +randomShopItem.DisplayText + " for " +  pickupLogic.ItemCost + " coins (E)";
            }
        }
    }

    private void OnItemPickedUp()
    {
        itemsInStock--;
    }
}
