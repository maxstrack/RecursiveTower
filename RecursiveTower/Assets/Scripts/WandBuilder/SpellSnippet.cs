using UnityEngine;
public enum SpellOperation {
    Const,
    PlayerGetHealth,
    Add,
    DealDamage,
    // more ops later
}

[System.Serializable]
public class SpellSnippet
{
    public string label;         // UI label, e.g., target.deal_damage(...)
    public string functionName;  // Internal name, e.g., deal_damage
    public SpellOperation operation;
    public int argCount;         // How many argument slots this node needs
    public bool isTerminal;      // True if it's a leaf node (no slots)
    public int constValue;  // used for Const
	public string description;
}
