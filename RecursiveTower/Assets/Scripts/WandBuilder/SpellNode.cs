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

	public SpellRuntimeNode ToRuntimeNode()
	{
		switch (snippet.functionName)
		{
			case "const(1)":
				return new ConstNode(1);

			case "const(5)":
				return new ConstNode(5);

			case "player.get_health()":
				return new PlayerGetHealthNode();

			case "add":
				return new AddNode(
					GetChildRuntimeNodeAt(0),
					GetChildRuntimeNodeAt(1)
				);

			case "target.take_damage":
				return new TargetTakeDamageNode(GetChildRuntimeNodeAt(0));

			case "player.heal":
				return new HealNode(GetChildRuntimeNodeAt(0));

			case "player":
				return new PlayerNode();

			case "target":
			    return new TargetNode();

			case "target.neighbor()":
			    return new GetTargetNeighborNode();

			case "player.get_distance":
				return new GetDistanceNode(GetChildRuntimeNodeAt(0));

			case "player.take_damage":
				return new PlayerTakeDamageNode(GetChildRuntimeNodeAt(0));

			case "player.neighbor()":
				return new PlayerNeighborNode();

			case "target.get_distance":
				return new TargetGetDistanceNode(GetChildRuntimeNodeAt(0));

			case "if":
				return new IfNode(
					GetChildRuntimeNodeAt(0),
					GetChildRuntimeNodeAt(1)
				);

			case "if_else":
				return new IfElseNode(
					GetChildRuntimeNodeAt(0),
					GetChildRuntimeNodeAt(1),
					GetChildRuntimeNodeAt(2)
				);

			case "equals":
				return new EqualsNode(
					GetChildRuntimeNodeAt(0),
					GetChildRuntimeNodeAt(1)
				);

			case "greater_than":
				return new GreaterThanNode(
					GetChildRuntimeNodeAt(0),
					GetChildRuntimeNodeAt(1)
				);

			default:
				Debug.LogError("Unknown spell function: " + snippet.functionName);
				return new ConstNode(0);
		}
	}

	private SpellRuntimeNode GetChildRuntimeNodeAt(int index)
	{
		Debug.Log($"[DEBUG] Resolving argument {index} for function {snippet.functionName}");

		Transform row = slotParent.GetChild(index);
		if (row == null) {
			Debug.LogError($"Row {index} not found in slotParent of {gameObject.name}");
			return new ConstNode(0);
		}

		Transform slotContainer = row.Find("SlotContainer");
		if (slotContainer == null) {
			Debug.LogError($"SlotContainer not found in row {index} of {gameObject.name}");
			return new ConstNode(0);
		}

		SpellSlot slot = slotContainer.GetComponentInChildren<SpellSlot>(true);
		if (slot == null) {
			Debug.LogError($"No SpellSlot found in SlotContainer at index {index} of {gameObject.name}");
			return new ConstNode(0);
		}

		SpellNode childNode = slot.contentParent.GetComponentInChildren<SpellNode>(true);
		if (childNode == null) {
			Debug.LogError($"No SpellNode found in slot.contentParent of argument {index} for {snippet.functionName}");
			return new ConstNode(0);
		}

		return childNode.ToRuntimeNode();

	}

}

