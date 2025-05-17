using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;

public class WandMenuController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject wandMenuUI;           // Main wand editor panel
    public Transform snippetListPanel;      // Panel that holds all draggable snippet buttons
    public Transform spellEditorRootSlot;   // Initial SpellSlot in the editor

    public GameObject snippetButtonPrefab;  // Prefab for buttons the user drags
    public GameObject spellNodePrefab;      // Prefab to spawn into SpellSlots

    [Header("Snippets")]
    public TextAsset jsonFile;              // JSON file containing snippet definitions

    private SpellSnippet[] availableSnippets;
    private bool menuOpen = false;

    public GameObject deathScreen;

    void Start()
    {
        LoadSnippetsFromJson();
        GenerateButtons();
		BuildInitialSpell();
        wandMenuUI.SetActive(false);
    }

    void LoadSnippetsFromJson()
    {
        if (jsonFile == null)
        {
            Debug.LogError("No JSON spell file assigned!");
            return;
        }

        string wrapped = "{\"items\":" + jsonFile.text + "}";
        SpellSnippetListWrapper wrapper = JsonUtility.FromJson<SpellSnippetListWrapper>(wrapped);
        availableSnippets = wrapper.items;
    }

    void GenerateButtons()
    {
        foreach (SpellSnippet snippet in availableSnippets)
        {
            GameObject btn = Instantiate(snippetButtonPrefab, snippetListPanel);
            TMP_Text label = btn.GetComponentInChildren<TMP_Text>();
            if (label != null) label.text = snippet.label;

            DraggableSpellNode draggable = btn.GetComponent<DraggableSpellNode>();
            if (draggable != null)
            {
                draggable.snippet = snippet;
                draggable.prefabToSpawn = spellNodePrefab;
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (deathScreen != null && deathScreen.activeSelf)
                return;

            if (menuOpen)
                CloseMenu();
            else
                OpenMenu();
        }
    }

    void OpenMenu()
    {
        wandMenuUI.SetActive(true);
        Time.timeScale = 0f;
        menuOpen = true;
    }

    void CloseMenu()
    {
        wandMenuUI.SetActive(false);
        Time.timeScale = 1f;
        menuOpen = false;
    }

    public string GetSpellAssembledString()
    {
        SpellNode rootNode = spellEditorRootSlot.GetComponentInChildren<SpellNode>();
        return rootNode != null ? rootNode.GetSpellString() : "[empty]";
    }

	public void ResetSpell()
	{
		SpellSlot slot = spellEditorRootSlot.GetComponent<SpellSlot>();
		if (slot != null)
		{
			slot.ResetSlot();
		}

		foreach (Transform child in snippetListPanel)
		{
			Destroy(child.gameObject);
		}

		GenerateButtons();
	}

	void BuildInitialSpell()
	{
		// 1. Find the snippets needed for the expression
		SpellSnippet root = FindSnippet("target.take_damage");
		SpellSnippet arg0 = FindSnippet("const(1)");

		if (root == null || arg0 == null)
		{
			Debug.LogError("Failed to find required snippets for initial spell.");
			return;
		}

		// 2. Get the root SpellSlot from the editor
		SpellSlot rootSlot = spellEditorRootSlot.GetComponent<SpellSlot>();
		if (rootSlot == null)
		{
			Debug.LogError("spellEditorRootSlot does not contain a SpellSlot component.");
			return;
		}

		// 3. Instantiate the root SpellNode into the slot's contentParent
		GameObject rootNode = Instantiate(spellNodePrefab, rootSlot.contentParent);
		SpellNode rootScript = rootNode.GetComponent<SpellNode>();
		rootScript.Initialize(root);

		// 4. Mark the root slot as filled (to hide border etc.)
		rootSlot.SetFilled(true);

		// 5. Find the first slot inside the root node to insert const(1)
		SpellSlot[] childSlots = rootNode.GetComponentsInChildren<SpellSlot>();
		if (childSlots.Length > 0)
		{
			GameObject argNode = Instantiate(spellNodePrefab, childSlots[0].contentParent);
			SpellNode argScript = argNode.GetComponent<SpellNode>();
			argScript.Initialize(arg0);

			childSlots[0].SetFilled(true);
		}
	}

	SpellSnippet FindSnippet(string functionName)
	{
		foreach (var s in availableSnippets)
		{
			if (s.functionName == functionName)
				return s;
		}
		return null;
	}

	public SpellNode GetRootSpellNode()
	{
		return spellEditorRootSlot.GetComponentInChildren<SpellNode>();
	}
}

[System.Serializable]
public class SpellSnippetListWrapper
{
    public SpellSnippet[] items;
}

