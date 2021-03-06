﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveHandler : MonoBehaviour
{
    private void Awake()
    {
        SaveCurrentSceneName();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            PlayerPrefs.DeleteAll();
            print("Deleted playerprefs");
        }
    }

    public static void SavePlayerData(PlayerData playerData)
    {
        if (playerData == null)
        {
            return;
        }

        string playerDataJson = JsonConvert.SerializeObject(playerData);
        PlayerPrefs.SetString("PlayerData", playerDataJson);
    }

    public static PlayerData GetPlayerData()
    {
        string playerDataJson = PlayerPrefs.GetString("PlayerData");
        if (string.IsNullOrEmpty(playerDataJson))
        {
            return null;
        }

        PlayerData playerData = JsonConvert.DeserializeObject<PlayerData>(playerDataJson);
        return playerData;
    }

    private void SaveCurrentSceneName()
    {
        if (SceneManager.GetActiveScene().name.ToLower() == "startmenu")
        {
            return;
        }

        PlayerPrefs.SetString("LastActiveSceneName", SceneManager.GetActiveScene().name);
    }

    public static string GetLastActiveSceneName()
    {
        string lastActiveSceneName = PlayerPrefs.GetString("LastActiveSceneName");
        if (string.IsNullOrEmpty(lastActiveSceneName))
        {
            lastActiveSceneName = SceneManager.GetActiveScene().name;
        }

        return lastActiveSceneName;
    }

    public static void SaveInventory()
    {
        IEnumerable<string> currentInventoryItemNames = Inventory.GetCurrentItems().Select(i => i.name);
        if (currentInventoryItemNames != null && currentInventoryItemNames.Count() > 0)
        {
            PlayerPrefs.SetString("Inventory_Items", JsonConvert.SerializeObject(currentInventoryItemNames));
        }
        else
        {
            PlayerPrefs.DeleteKey("Inventory_Items");
        }
    }

    public static List<Item> GetSavedItemsForInventory()
    {
        List<Item> listInvItemsFromSave = new List<Item>();

        string inventoryItemNamesAsJson = PlayerPrefs.GetString("Inventory_Items");
        if (!string.IsNullOrEmpty(inventoryItemNamesAsJson))
        {
            List<string> listItemNames = new List<string>();
            listItemNames = JsonConvert.DeserializeObject<List<string>>(inventoryItemNamesAsJson);

            if (listItemNames != null)
            {
                foreach (string itemName in listItemNames)
                {
                    Item item = Resources.Load<Item>($"ScriptableObjects/{itemName}");
                    if (item)
                    {
                        listInvItemsFromSave.Add(item);
                    }
                }
            }
        }

        return listInvItemsFromSave;
    }

    public static void SaveLevel(string gameObjectNameOfInteractable, string property, object valueToSafe)
    {
        string keyToSafe = gameObjectNameOfInteractable + "_" + property;

        string currentSceneName = SceneManager.GetActiveScene().name;
        string savedDataJson = PlayerPrefs.GetString(currentSceneName);
        if (!string.IsNullOrEmpty(savedDataJson))//There is already a safe for this scene
        {
            Dictionary<string, object> dicSavedDataOfScene = JsonConvert.DeserializeObject<Dictionary<string, object>>(savedDataJson);
            if (!dicSavedDataOfScene.ContainsKey(keyToSafe))
            {
                dicSavedDataOfScene.Add(keyToSafe, valueToSafe);
            }
            else
            {
                dicSavedDataOfScene[keyToSafe] = valueToSafe;
            }

            PlayerPrefs.SetString(currentSceneName, JsonConvert.SerializeObject(dicSavedDataOfScene));
        }
        else//There is no safe for this scene
        {
            Dictionary<string, object> dicToSafe = new Dictionary<string, object>()
            {
                { keyToSafe, valueToSafe }
            };

            string dataToSafeJson = JsonConvert.SerializeObject(dicToSafe);
            PlayerPrefs.SetString(currentSceneName, dataToSafeJson);
        }
    }

    public static bool GetValueByProperty<T>(string sceneName, string gameObjectNameOfInteractable, string property, out T value)
    {
        value = default;

        string keyOfValueToRetrieve = gameObjectNameOfInteractable + "_" + property;
        string savedDataJson = PlayerPrefs.GetString(sceneName);

        bool valueExists = false;
        if (!string.IsNullOrEmpty(savedDataJson))
        {
            //(T)Convert.ChangeType(currentValue,typeof(T));
            Dictionary<string, object> dicSavedDataForLevel = JsonConvert.DeserializeObject<Dictionary<string, object>>(savedDataJson);

            if (dicSavedDataForLevel.ContainsKey(keyOfValueToRetrieve))
            {
                value = (T)dicSavedDataForLevel[keyOfValueToRetrieve];
                valueExists = true;
            }
        }

        return valueExists;
    }
}