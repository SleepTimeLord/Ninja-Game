using System.Collections.Generic;

public class JStateMachine
{
    public readonly JState Root;
    public readonly JTransitionSequencer Sequencer;
    bool started;

    public JStateMachine(JState root)
    {
        Root = root;
        Sequencer = new JTransitionSequencer(this);
    }

    public void Start()
    {
        if (started) return;
        started = true;
        Root.Enter();
    }

    public void Tick(float deltaTime)
    {
        if (!started) Start();
        InternalTick(deltaTime);
    }

    internal void InternalTick(float deltaTime) => Root.Update(deltaTime);

    public void ChangeState(JState from, JState to)
    {
        if (from == to || from == null || to == null) return;

        JState lca = JTransitionSequencer.Lca(from, to);

        if (from == lca)
        {
            // to is a descendant of from retract from's current
            // active branch, but from itself stays active.
            if (from.ActiveChild != null) from.ActiveChild.Exit();
            from.ActiveChild = null;
        }
        else
        {
            // fully exit froms own active subtree once,
            // then walk up to but dont include the lca, exiting each level
            // without re-cascading into a branch already exited.
            from.Exit();
            for (JState s = from.Parent; s != lca; s = s.Parent)
            {
                s.ExitSelf();
            }
        }

        if (to == lca)
        {
            // to is an ancestor thats already active. Nothing to
            // enter, just drop the stale reference to the branch we left.
            to.ActiveChild = null;
            return;
        }

        var stack = new Stack<JState>();
        for (JState s = to; s != lca; s = s.Parent) stack.Push(s);
        while (stack.Count > 0) stack.Pop().Enter();
    }
}