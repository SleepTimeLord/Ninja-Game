using System.Collections.Generic;

public class JTransitionSequencer
{
    public readonly JStateMachine Machine;

    public JTransitionSequencer(JStateMachine machine)
    {
        Machine = machine;
    }

    public void RequestTransition(JState from, JState to)
    {
        Machine.ChangeState(from, to);
    }

    public static JState Lca(JState a, JState b)
    {
        var ancestorsOfA = new HashSet<JState>();
        for (var s = a; s != null; s = s.Parent) ancestorsOfA.Add(s);

        for (var s = b; s != null; s = s.Parent)
        {
            if (ancestorsOfA.Contains(s)) return s;
        }

        return null;
    }
}