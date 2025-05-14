using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class SpellSlot : MonoBehaviour, IDropHandler
{
    public Transform contentParent; // Where inserted node goes
    private bool filled = false;

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop called on SpellSlot");
        if (filled) return;

        var dragged = eventData.pointerDrag;
        var node = dragged.GetComponent<DraggableSpellNode>();
        if (node != null)
        {
            GameObject newNode = Instantiate(node.prefabToSpawn, contentParent);
            newNode.GetComponent<SpellNode>().Initialize(node.snippet);
            filled = true;
            Destroy(dragged); // remove from source

            var image = GetComponent<Image>();
            if (image != null)
                image.enabled = false;
        }
    }

    public void ResetSlot()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        filled = false;

        var image = GetComponent<Image>();
        if (image != null) image.enabled = true;
    }

    public void SetFilled(bool value)
    {
        filled = value;

        var image = GetComponent<Image>();
        if (image != null)
            image.enabled = !filled;
    }
}

