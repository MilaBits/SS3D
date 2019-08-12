﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Mirror;
using UnityEngine;

public class HumanInventory : NetworkBehaviour
{
    private int ActiveHandIndex = 0;

    [SerializeField]
    private HumanInventoryUI inventoryUI;

    [SerializeField]
    private NetworkIdentity networkIdentity;

    [SerializeField]
    private NetworkTransformChild LeftHandNetworkTransformChild;

    [SerializeField]
    private NetworkTransformChild RightHandNetworkTransformChild;

    private void Start()
    {
        if (!isLocalPlayer)
        {
            Destroy(this);
            Destroy(inventoryUI.gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("SwapActive"))
        {
            ActiveHandIndex = ActiveHandIndex == 1 ? 0 : 1;
            inventoryUI.SwitchActiveHand(ActiveHandIndex);
        }

        if (Input.GetButtonDown("DropActive"))
        {
            Drop();
        }

        if (Input.GetButtonDown("Click"))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100))
            {
                Debug.Log(hit.collider.gameObject.name);
                Item item = hit.collider.GetComponent<Item>();
                if (item != null) PickUp(item.gameObject);
            }
        }
    }

    private void PickUp(GameObject itemObject)
    {
        Item item = itemObject.GetComponent<Item>();
        if (!item.Held && item.compatibleSlots.HasFlag(SlotTypes.Hand))
        {
            if (ActiveHandIndex == 0)
            {
                if (!inventoryUI.GetSlots().slotLeftHand.uiItem)
                    inventoryUI.SetSlotUiItem(inventoryUI.GetSlots().slotLeftHand, item);
            }
            else
            {
                if (!inventoryUI.GetSlots().slotRightHand.uiItem)
                    inventoryUI.SetSlotUiItem(inventoryUI.GetSlots().slotRightHand, item);
            }
        }
    }

    private void Drop()
    {
        Item item = null;
        if (ActiveHandIndex == 0)
        {
            item = inventoryUI.GetSlots().slotLeftHand.uiItem.Item;

            inventoryUI.ClearSlotUiItem(inventoryUI.GetSlots().slotLeftHand);
            item.CmdRelease();
        }
        else
        {
            item = inventoryUI.GetSlots().slotRightHand.uiItem.Item;
            inventoryUI.ClearSlotUiItem(inventoryUI.GetSlots().slotRightHand);
            item.CmdRelease();
        }
    }
}