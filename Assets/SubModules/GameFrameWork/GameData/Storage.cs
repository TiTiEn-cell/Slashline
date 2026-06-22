using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class Storage
{
    public List<StorageItem> ListItems = new List<StorageItem>();
    public int GetItemCountByID(ItemID id)
    {
        var finditem = ListItems.Find(x => x.ID == id);
        if (finditem == null)
        {
            return 0;
        }
        else
        {
            return finditem.Count;
        }
    }

    public void AddItemByID(ItemID id, int value = 1)
    {
        var finditem = ListItems.FindIndex(x => x.ID == id);
        if (finditem == -1)
        {
            ListItems.Add(new StorageItem(id, value));
        }
        else
        {
            ListItems[finditem].Count += value;
        }
    }

    public void UseItemByID(ItemID id, int value = 1)
    {
        bool isActiveSuccess = false;
        var finditem = ListItems.FindIndex(x => x.ID == id);
        if (finditem == -1)
        {
            // return isActiveSuccess;
        }
        else
        {
            isActiveSuccess = ListItems[finditem].Count >= value;
            ListItems[finditem].Count -= value;

            if (ListItems[finditem].Count == 0)
            {
                ListItems.RemoveAt(finditem);
            }
        }
    }
}

[Serializable]
public class StorageItem
{
    public ItemID ID;
    public int Count;
    public StorageItem(ItemID _id, int count)
    {
        ID = _id;
        Count = count;
    }
}
