using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpTrigger : MonoBehaviour
{
    [SerializeField] private float _speedMultiplier;
    private const float EFFECT_DURATION = 4f;
    
    private List<GameObject> _speededEnemies = new List<GameObject>();

    [SerializeField] private GameObject _speedAuraPrefab;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (_speededEnemies.Contains(other.gameObject)) return;
            
            var speedComponent = other.GetComponent<Speed>();
            if (speedComponent != null)
            {
                var aura = Instantiate(_speedAuraPrefab, other.transform.position, Quaternion.identity, other.transform);
                speedComponent.StartCoroutine(SpeedUpRoutine(speedComponent, aura));
                _speededEnemies.Add(other.gameObject);
            }
        }
    }

    private IEnumerator SpeedUpRoutine(Speed speedComponent, GameObject aura)
    {
        speedComponent.Value.Value *= _speedMultiplier;
        yield return new WaitForSeconds(EFFECT_DURATION);
        speedComponent.ReturnToDefault();
        _speededEnemies.Remove(speedComponent.gameObject);
        Destroy(aura);
    }
}