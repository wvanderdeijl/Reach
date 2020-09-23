﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : Interactable
{
    private LevelLoader _levelLoader;

    [Header("Load Next Level")]
    public string LevelName;

    [Header("Unlock Door")]
    public bool IsDoorLocked;
    public string KeyGameobject;
    public string KeyProperty;

    [Header("Play Cutscene")]
    public bool PlayCutscene;

    [Header("Audio")]
    public string Audio;

    private void Awake()
    {
        _levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
    }

    public override bool Interact(Item item)
    {
        bool succesfullInteraction = false;

        if (IsDoorLocked)
        {
            if (SaveHandler.GetValueByProperty(SceneManager.GetActiveScene().name, KeyGameobject, KeyProperty, out bool isUnlocked))
            {
                if (LevelName != string.Empty)
                {
                    FindObjectOfType<AudioManager>().PlaySound("Unlocked");

                    _levelLoader.LoadNextLevel(LevelName, PlayCutscene, Audio);
                    succesfullInteraction = true;
                } else
                {
                    FindObjectOfType<AudioManager>().PlaySound("Locked");
                }
            }
        }
        else
        {
            if (LevelName != string.Empty)
            {
                _levelLoader.LoadNextLevel(LevelName, PlayCutscene, Audio);
                succesfullInteraction = true;
            }
        }

        return succesfullInteraction;
    }
}
