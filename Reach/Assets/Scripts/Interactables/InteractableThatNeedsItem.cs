﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractableThatNeedsItem : Interactable
{
    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    [Header("Item")]
    public string ItemSaveProperty;
    public Item ItemNeeded;

    private bool _hasItem = false;

    [Header("Change Sprite")]
    public bool changeSprite;
    public Sprite sprite;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (SaveHandler.GetValueByProperty(SceneManager.GetActiveScene().name, name, ItemSaveProperty, out bool hasItem))
        {
            _hasItem = hasItem;
            AddItem();
        }
    }

    private void AddItem()
    {
        if (changeSprite)
        {
            _spriteRenderer.sprite = sprite;
        } else
        {
            _spriteRenderer.enabled = true;
        }
    }

    public override bool Interact(Item item)
    {
        bool succesfulInteraction = false;

        if (!_hasItem && item && item == ItemNeeded)
        {
            succesfulInteraction = true;
            _hasItem = true;
   
            AddItem();
            SaveHandler.SaveLevel(this.name, ItemSaveProperty, true);
        }

        return succesfulInteraction;
    }
}
