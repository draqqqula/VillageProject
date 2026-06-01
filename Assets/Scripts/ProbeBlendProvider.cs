using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class ProbeBlendProvider : MonoBehaviour, IProbeBlendProvider
{
    [Header("Target")]
    [SerializeField] private Transform _target;

    [Header("Grid")]
    [SerializeField] private Vector3 _gridDimensions = new Vector3(1f, 1f, 1f);

    [Tooltip("Если false, Y берётся из target.position.y без snap к grid.")]
    [SerializeField] private bool _snapY = false;

    [Header("Blend")]
    [SerializeField, Min(0.001f)] private float _blendDuration = 0.5f;

    [Tooltip("Ускоряет blend при быстром движении камеры.")]
    [SerializeField] private bool _speedBasedBlend = true;

    [Tooltip("Ограничение deltaTime, как в коде автора.")]
    [SerializeField] private float _maxDeltaTime = 0.1f;

    [Tooltip("Время сглаживания скорости камеры.")]
    [SerializeField] private float _speedSmoothingTime = 0.05f;

    public Vector3 ProbeA { get; private set; }
    public Vector3 ProbeB { get; private set; }
    public float Blend { get; private set; } = 1f;

    public bool IsInitialized { get; private set; }
    public bool IsBlending { get; private set; }

    /// <summary>
    /// true только в кадр, когда была выбрана новая ProbeB.
    /// В этот кадр обычно нужно перерендерить Pong cubemap.
    /// </summary>
    public bool TriggeredThisFrame { get; private set; }

    public Vector3 TargetPosition => Target.position;

    private Transform Target
    {
        get
        {
            if (_target != null)
                return _target;

            return transform;
        }
    }

    private Vector3 _lastTargetPosition;
    private bool _hasLastTargetPosition;
    private float _smoothedSpeed;

    private void Reset()
    {
        _target = Camera.main != null ? Camera.main.transform : transform;
    }

    private void Awake()
    {
        Initialize(TargetPosition);
    }

    private void LateUpdate()
    {
        Tick(Time.deltaTime);
    }

    public void Tick(float deltaTime)
    {
        TriggeredThisFrame = false;

        Vector3 position = TargetPosition;
        deltaTime = Mathf.Min(deltaTime, _maxDeltaTime);

        if (!IsInitialized)
        {
            Initialize(position);
            return;
        }

        UpdateSmoothedSpeed(position, deltaTime);

        Vector3 snappedProbe = SnapProbe(position);

        if (!IsBlending)
        {
            if (snappedProbe != ProbeB)
            {
                ProbeA = ProbeB;
                ProbeB = snappedProbe;

                Blend = 0f;
                IsBlending = true;
                TriggeredThisFrame = true;
            }
        }
        else
        {
            float baseRate = 1f / Mathf.Max(_blendDuration, 0.001f);
            float blendRate = baseRate;

            if (_speedBasedBlend)
            {
                float gridStep = GetMainGridStep();
                float velocityRate = _smoothedSpeed / gridStep;
                blendRate = Mathf.Max(baseRate, velocityRate);
            }

            Blend += blendRate * deltaTime;

            if (Blend >= 1f)
            {
                Blend = 1f;
                IsBlending = false;

                // Логически старый probe теперь совпадает с новым.
                // Если ты делаешь физический Ping/Pong swap текстур — делай его в этот момент.
                ProbeA = ProbeB;
            }
        }
    }

    public void ForceReset(Vector3 position)
    {
        Initialize(position);
    }

    public void ForceResetToTarget()
    {
        Initialize(TargetPosition);
    }

    private void Initialize(Vector3 position)
    {
        Vector3 probe = SnapProbe(position);

        ProbeA = probe;
        ProbeB = probe;
        Blend = 1f;

        IsInitialized = true;
        IsBlending = false;
        TriggeredThisFrame = true;

        _lastTargetPosition = position;
        _hasLastTargetPosition = true;
        _smoothedSpeed = 0f;
    }

    private Vector3 SnapProbe(Vector3 position)
    {
        float x = SnapAxis(position.x, _gridDimensions.x);

        float y = _snapY
            ? SnapAxis(position.y, _gridDimensions.y)
            : position.y;

        float z = SnapAxis(position.z, _gridDimensions.z);

        return new Vector3(x, y, z);
    }

    private static float SnapAxis(float value, float step)
    {
        step = Mathf.Max(Mathf.Abs(step), 0.0001f);
        return Mathf.Round(value / step) * step;
    }

    private void UpdateSmoothedSpeed(Vector3 position, float deltaTime)
    {
        if (_hasLastTargetPosition && deltaTime > 0f)
        {
            float instantSpeed = Vector3.Distance(position, _lastTargetPosition) / deltaTime;

            float smoothingTime = Mathf.Max(_speedSmoothingTime, 0.0001f);
            float alpha = 1f - Mathf.Exp(-deltaTime / smoothingTime);

            _smoothedSpeed = Mathf.Lerp(_smoothedSpeed, instantSpeed, alpha);
        }

        _lastTargetPosition = position;
        _hasLastTargetPosition = true;
    }

    private float GetMainGridStep()
    {
        float x = Mathf.Abs(_gridDimensions.x);
        float y = Mathf.Abs(_gridDimensions.y);
        float z = Mathf.Abs(_gridDimensions.z);

        if (_snapY)
            return Mathf.Max(x, y, z, 0.0001f);

        return Mathf.Max(x, z, 0.0001f);
    }
}