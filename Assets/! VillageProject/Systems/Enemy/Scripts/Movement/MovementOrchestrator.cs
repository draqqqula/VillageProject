using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Makes sure only one <see cref="MovementAgentBase"/> is active at the time
/// </summary>
public class MovementOrchestrator : MonoBehaviour
{
    private MovementAgentBase _activeAgent;
    private bool _isNeutral;
    public UnityEvent OnNeutralEnter;
    public UnityEvent OnNeutralExit;
    [SerializeField] private List<MovementAgentBase> _agents;

    public bool IsNeutral => _activeAgent == null;
    public MovementAgentBase ActiveAgent
    {
        get
        {
            return _activeAgent;
        }
        private set
        {
            if (!ReferenceEquals(_activeAgent, value))
            {
                if (_activeAgent == null)
                {
                    OnNeutralExit?.Invoke();
                }
                else
                {
                    if (value == null)
                    {
                        OnNeutralEnter?.Invoke();
                    }
                    else
                    {
                        DisableAgent(_activeAgent);
                    }
                }
                _activeAgent = value;
            }
        }
    }

    #region EditorLogic

    [ContextMenu("Bind Agents")]
    public void BindAgents()
    {
        _agents = new List<MovementAgentBase>();
        GetComponents(_agents);
        SubscribeToEvents();
    }

    private void Reset()
    {
        BindAgents();
    }

    private void SubscribeToEvents()
    {
        foreach (var agent in _agents)
        {
            AddCall(agent, agent.PathAssigned, new UnityAction<MovementAgentBase>(HandlePathAssigned));
            AddCall(agent, agent.DestinationReached, new UnityAction<MovementAgentBase>(HandleDestinationReached));
            agent.enabled = false;
        }
    }

    private void AddCall(MovementAgentBase agent, UnityEvent unityEvent, UnityAction<MovementAgentBase> call)
    {
        UnityEventTools.RemovePersistentListener(unityEvent, call);
        UnityEventTools.AddObjectPersistentListener(unityEvent, call, agent);
    }

    #endregion

    public void HandlePathAssigned(MovementAgentBase agent)
    {
        ActiveAgent = agent;
        agent.enabled = true;
    }

    public void HandleDestinationReached(MovementAgentBase agent)
    {
        ActiveAgent = null;
        agent.enabled = false;
    }

    private void DisableAgent(MovementAgentBase agent)
    {
        agent.HandleCancellation();
        agent.enabled = false;
    }
}
