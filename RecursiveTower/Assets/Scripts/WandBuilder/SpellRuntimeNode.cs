using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public abstract class SpellRuntimeNode {
    public abstract object Evaluate(SpellContext context);
}

public class SpellContext {
    public PlayerHealth player;
    public Enemy target;

    public SpellContext(PlayerHealth player, Enemy target) {
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

public class TargetTakeDamageNode : SpellRuntimeNode {
    public SpellRuntimeNode damageAmount;

    public TargetTakeDamageNode(SpellRuntimeNode damageAmount) {
        this.damageAmount = damageAmount;
    }

    public override object Evaluate(SpellContext context) {
        int dmg = Convert.ToInt32(damageAmount.Evaluate(context));
        context.target.TakeDamage(dmg);
        return null;
    }
}

public class HealNode : SpellRuntimeNode {
    public SpellRuntimeNode amount;

    public HealNode(SpellRuntimeNode amount) {
        this.amount = amount;
    }

    public override object Evaluate(SpellContext context) {
        int amt = Convert.ToInt32(amount.Evaluate(context));
        context.player.Heal(amt);
        return null;
    }
}

public class PlayerNode : SpellRuntimeNode {
    public override object Evaluate(SpellContext context) {
        return context.player;
    }
}

public class TargetNode : SpellRuntimeNode {
    public override object Evaluate(SpellContext context) {
        return context.target;
    }
}

public class GetDistanceNode : SpellRuntimeNode {
    public SpellRuntimeNode otherEntity;

    public GetDistanceNode(SpellRuntimeNode otherEntity) {
        this.otherEntity = otherEntity;
    }

    public override object Evaluate(SpellContext context) {
        object other = otherEntity.Evaluate(context);

        if (other is MonoBehaviour mb) {
            float distance = Vector3.Distance(
                context.player.transform.position,
                mb.transform.position
            );
            return distance;
        }

        Debug.LogWarning("GetDistanceNode: argument is not a valid entity.");
        return 0f;
    }
}

public class GetTargetNeighborNode : SpellRuntimeNode {
    public override object Evaluate(SpellContext context) {
        if (context.target == null) {
            Debug.LogWarning("GetTargetNeighborNode: Target is null.");
            return null;
        }

        Vector3 origin = context.target.transform.position;

        Enemy[] allEnemies = GameObject.FindObjectsOfType<Enemy>();

        Enemy closest = allEnemies
            .Where(e => e != context.target)
            .OrderBy(e => Vector3.Distance(origin, e.transform.position))
            .FirstOrDefault();

        if (closest != null)
            return closest;

        Debug.Log("No nearby enemy found.");
        return null;
    }
}

public class PlayerTakeDamageNode : SpellRuntimeNode {
    public SpellRuntimeNode amount;

    public PlayerTakeDamageNode(SpellRuntimeNode amount) {
        this.amount = amount;
    }

    public override object Evaluate(SpellContext context) {
        int dmg = Convert.ToInt32(amount.Evaluate(context));
        context.player.TakeDamage(dmg);
        return null;
    }
}

public class PlayerNeighborNode : SpellRuntimeNode {
    public override object Evaluate(SpellContext context) {
        if (context.player == null)
            return null;

        Vector3 origin = context.player.transform.position;
        Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();

        return enemies
            .OrderBy(e => Vector3.Distance(origin, e.transform.position))
            .FirstOrDefault();
    }
}

public class TargetGetDistanceNode : SpellRuntimeNode {
    public SpellRuntimeNode other;

    public TargetGetDistanceNode(SpellRuntimeNode other) {
        this.other = other;
    }

    public override object Evaluate(SpellContext context) {
        var entity = other.Evaluate(context) as MonoBehaviour;
        if (entity == null || context.target == null) return 0f;

        return Vector3.Distance(context.target.transform.position, entity.transform.position);
    }
}
public class IfNode : SpellRuntimeNode {
    public SpellRuntimeNode condition;
    public SpellRuntimeNode action;

    public IfNode(SpellRuntimeNode condition, SpellRuntimeNode action) {
        this.condition = condition;
        this.action = action;
    }

    public override object Evaluate(SpellContext context) {
        bool result = Convert.ToBoolean(condition.Evaluate(context));
        if (result) {
            return action.Evaluate(context);
        }
        return null;
    }
}

public class IfElseNode : SpellRuntimeNode {
    public SpellRuntimeNode condition;
    public SpellRuntimeNode thenBranch;
    public SpellRuntimeNode elseBranch;

    public IfElseNode(SpellRuntimeNode condition, SpellRuntimeNode thenBranch, SpellRuntimeNode elseBranch) {
        this.condition = condition;
        this.thenBranch = thenBranch;
        this.elseBranch = elseBranch;
    }

    public override object Evaluate(SpellContext context) {
        bool result = Convert.ToBoolean(condition.Evaluate(context));
        return result ? thenBranch.Evaluate(context) : elseBranch.Evaluate(context);
    }
}

public class EqualsNode : SpellRuntimeNode {
    public SpellRuntimeNode left;
    public SpellRuntimeNode right;

    public EqualsNode(SpellRuntimeNode left, SpellRuntimeNode right) {
        this.left = left;
        this.right = right;
    }

    public override object Evaluate(SpellContext context) {
        var a = left.Evaluate(context);
        var b = right.Evaluate(context);
        return a != null && b != null && a.Equals(b);
    }
}

public class GreaterThanNode : SpellRuntimeNode {
    public SpellRuntimeNode left;
    public SpellRuntimeNode right;

    public GreaterThanNode(SpellRuntimeNode left, SpellRuntimeNode right) {
        this.left = left;
        this.right = right;
    }

    public override object Evaluate(SpellContext context) {
        double a = Convert.ToDouble(left.Evaluate(context));
        double b = Convert.ToDouble(right.Evaluate(context));
        return a > b;
    }
}

