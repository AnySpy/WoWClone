using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Inventory Items")]
    [SerializeField] private List<Item> inventory;

    [Header("Inventory Settings")]
    [SerializeField] private static int maxInventorySize = 100;

    public void Start()
    {
        inventory = new List<Item>();
    }

    public void AddItem(Item item)
    {
        if (inventory.Count < maxInventorySize)
        {
            inventory.Add(item);
            Debug.Log($"Added Item {item.itemName}");
        }
        else
        {
            Debug.Log("Inventory is full!");
        }
    }

    public bool RemoveItem(Item item)
    {
        if (inventory.Contains(item))
        {
            inventory.Remove(item);
            return true;
        }
        else
        {
            Debug.Log("Item not found in inventory!");
            return false;
        }
    }

    public List<Item> GetInventoryItems()
    {
        return inventory;
    }

    public bool ContainsItems(Item item)
    {
        return inventory.Contains(item);
    }
}
