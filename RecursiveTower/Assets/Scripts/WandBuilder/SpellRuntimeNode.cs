using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class SpellRuntimeNode {
    public abstract object Evaluate(SpellContext context);
}

public class SpellContext {
    public PlayerHealth player;
    public EnemyHealth target;

    public SpellContext(PlayerHealth player, EnemyHealth target) {
        this.player = player;
        this.target = target;
    }
}

public class ConstNode : SpellRuntimeNode {
    public int value;

    public ConstNode(int val) {
        value = val;
    }

    public override object Evaluate(SpellContext context) {
        return value;
    }
}

public class PlayerGetHealthNode : SpellRuntimeNode {
    public override object Evaluate(SpellContext context) {
        return context.player.GetHealth();
    }
}

public class AddNode : SpellRuntimeNode {
    public SpellRuntimeNode left;
    public SpellRuntimeNode right;

    public AddNode(SpellRuntimeNode left, SpellRuntimeNode right) {
        this.left = left;
        this.right = right;
    }

    public override object Evaluate(SpellContext context) {
        int a = Convert.ToInt32(left.Evaluate(context));
        int b = Convert.ToInt32(right.Evaluate(context));
        return a + b;
    }
}

public class TakeDamageNode : SpellRuntimeNode {
    public SpellRuntimeNode damageAmount;

    public TakeDamageNode(SpellRuntimeNode damageAmount) {
        this.damageAmount = damageAmount;
    }

    public override object Evaluate(SpellContext context) {
        int dmg = Convert.ToInt32(damageAmount.Evaluate(context));
        context.target.TakeDamage(dmg);
        return null;
    }
}

