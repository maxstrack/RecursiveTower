using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpellNode : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform slotParent;

    private SpellSnippet snippet;

	public TMP_Text labelPrefix;
	public TMP_Text labelSuffix;
	public TMP_Text commaLabelPrefab;

	public GameObject indentedSlotRowPrefab; // assign in inspector

	public void Initialize(SpellSnippet data)
	{
		snippet = data;

		if (data.isTerminal)
		{
			// Simple one-liner
			labelPrefix.text = data.functionName;
			labelSuffix.gameObject.SetActive(false); // Hide the suffix label if not needed
			slotParent.gameObject.SetActive(false);  // Hide slot section entirely
		}
		else
		{
			// Multiline layout
			labelPrefix.text = data.functionName + "(";
			labelSuffix.text = ")";
			labelSuffix.gameObject.SetActive(true);
			slotParent.gameObject.SetActive(true);

			for (int i = 0; i < data.argCount; i++)
			{
				GameObject row = Instantiate(indentedSlotRowPrefab, slotParent);
				Transform slotContainer = row.transform.Find("SlotContainer");

				if (slotContainer != null)
				{
					Instantiate(slotPrefab, slotContainer);
				}
			}
		}
	}

    public string GetSpellString()
    {
        if (snippet.isTerminal) return snippet.functionName;

        string[] args = new string[slotParent.childCount];
        for (int i = 0; i < slotParent.childCount; i++)
        {
            SpellSlot slot = slotParent.GetChild(i).GetComponent<SpellSlot>();
            SpellNode child = slot.contentParent.GetComponentInChildren<SpellNode>();
            args[i] = child != null ? child.GetSpellString() : "[empty]";
        }

        return $"{snippet.functionName}({string.Join(", ", args)})";
    }
}

