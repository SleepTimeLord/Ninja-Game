using System.Collections.Generic;

public abstract class JState
{
    public readonly JStateMachine Machine;
    public readonly JState Parent;
    public JState ActiveChild;

    public JState(JStateMachine machine, JState parent = null)
    {
        Machine = machine;
        Parent = parent;
    }

    protected virtual JState GetInitialState () => null;
    protected virtual JState GetTransition() => null;

    protected virtual void OnEnter() { }
    protected virtual void OnExit() { }
    protected virtual void OnUpdate(float deltaTime) { }

    internal void Enter()
    {
        if (Parent != null) Parent.ActiveChild = this;
        OnEnter();
        JState init = GetInitialState();
        if (init != null) init.Enter();
    }
    internal void Exit()
    {
        if (ActiveChild != null) ActiveChild.Exit();
        ActiveChild = null;
        OnExit();
    }

    internal void ExitSelf()
    {
        ActiveChild = null;
        OnExit();
    }

    internal void Update(float deltaTime)
    {
        JState t = GetTransition();
        if (t != null)
        {
            Machine.Sequencer.RequestTransition(this, t);
            return;
        }

        if (ActiveChild != null) ActiveChild.Update(deltaTime);
        OnUpdate(deltaTime);
    }

    /// <summary>
    /// Returns the deepest currently active descendent state
    /// the leaf of the active path
    /// </summary>
    public JState Leaf()
    {
        JState s = this;
        while (s.ActiveChild != null) s = s.ActiveChild;
        return s;
    }

    /// <summary>
    /// yields this state and then each ancestor up to the root
    /// </summary>
    public IEnumerable<JState> PathToRoot()
    {
        for (JState s = this; s != null; s = s.Parent) yield return s;
    }
}
