using System;
using Unity.Behavior;
using UnityEngine;
using Composite = Unity.Behavior.Composite;
using Unity.Properties;
using System.Collections.Generic;

public abstract class BinaryStateSequenceBase : Composite
{
    [SerializeReference] public Node True;
    [SerializeReference] public Node False;
    private Node m_currentNode;

    protected override Status OnStart()
    {
        Subscribe(HandleValueChanged);

        if (GetValue())
        {
            SwitchBranchTo(True);
        }
        else
        {
            SwitchBranchTo(False);
        }

        return Status.Waiting;
    }

    protected override Status OnUpdate()
    {
        return m_currentNode.CurrentStatus;
    }

    protected override void OnEnd()
    {
        Unsubscribe(HandleValueChanged);
    }

    private void HandleValueChanged()
    {
        if (GetValue())
        {
            SwitchBranchTo(True);
            DisconnectBranch(False);
        }
        else
        {
            SwitchBranchTo(False);
            DisconnectBranch(True);
        }
    }

    private void SwitchBranchTo(Node node)
    {
        m_currentNode = node;
        StartNode(node);
    }

    private void DisconnectBranch(Node node)
    {
        EndNode(node);
    }

    private bool IsRunning(Node node)
    {
        return node.CurrentStatus == Status.Running || node.CurrentStatus == Status.Waiting;
    }

    protected abstract bool GetValue();
    protected abstract void Subscribe(System.Action handler);
    protected abstract void Unsubscribe(System.Action handler);
}

