using System.Collections.Generic;
using System.Reflection;

public class JStateMachineBuilder
{
    readonly JState root;

    public JStateMachineBuilder(JState root)
    {
        this.root = root;
    }

    public JStateMachine Build()
    {
        var m = new JStateMachine(root);
        Wire(root, m, new HashSet<JState>());
        return m;
    }

    void Wire(JState s, JStateMachine m, HashSet<JState> visited)
    {
        if (s == null) return;
        if (!visited.Add(s)) return;

        var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
        var machineField = typeof(JState).GetField("Machine", flags);
        if (machineField != null) machineField.SetValue(s, m);

        foreach (var fld in s.GetType().GetFields(flags))
        {
            if (!typeof(JState).IsAssignableFrom(fld.FieldType)) continue;
            if (fld.Name == "Parent") continue;

            var child = (JState)fld.GetValue(s);
            if (child == null) continue;
            if (!ReferenceEquals(child.Parent, s)) continue;

            Wire(child, m, visited);
        }
    }
}