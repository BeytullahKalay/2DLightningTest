using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShadowCaster : MonoBehaviour
{
    [SerializeField] private GameObject shadowGameObject;
    [SerializeField] private float detectRadius = 4;
    [SerializeField] private Transform shadowSpawnPosition;
    [SerializeField] private Vector2 minMaxShadowScale = new Vector2(1, 3);
    [SerializeField] private Vector2 minMaxShadowAlpha = new Vector2(.2f, 1);
    [SerializeField] private AnimationCurve shadowOpacityCurve;


    private readonly Dictionary<Collider2D, Transform> _detectedLightSources = new Dictionary<Collider2D, Transform>();


    private void Update()
    {
        if (!DayAndNightController.Instance.IsNight) return;


        var col = Physics2D.OverlapCircleAll(transform.position, detectRadius, LayerMask.NameToLayer("Enemy"));

        if (col.Length > 0)
        {
            foreach (var c in col)
            {
                if (!_detectedLightSources.ContainsKey(c))
                {
                    var shadowObject = Instantiate(shadowGameObject, shadowSpawnPosition.position, Quaternion.identity,
                        transform);
                    shadowObject.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
                    _detectedLightSources.Add(c, shadowObject.transform);
                }

                var dir = (transform.position - c.transform.position).normalized;

                _detectedLightSources[c].up = dir;

                var newScale = Map(detectRadius - Vector2.Distance(transform.position, c.transform.position),
                    0f, detectRadius, minMaxShadowScale.x, minMaxShadowScale.y);

                _detectedLightSources[c].localScale = new Vector2(1, newScale);


                var newAlpha = Map(Vector2.Distance(transform.position, c.transform.position),
                    0, detectRadius, minMaxShadowAlpha.x, minMaxShadowAlpha.y);


                var mappedDistance0To1 = Map(Vector2.Distance(transform.position, c.transform.position), 0,
                    detectRadius, 0.1f, 1);
                var curveValue = shadowOpacityCurve.Evaluate(mappedDistance0To1);
                

                var newColor = _detectedLightSources[c].GetComponent<SpriteRenderer>().material.color;
                newColor.a = newAlpha * curveValue;
                _detectedLightSources[c].GetComponent<SpriteRenderer>().material.color = newColor;
            }
        }

        var colList = col.ToList();
        var listRemoveKeys = new List<Collider2D>();
        foreach (var lightSource in _detectedLightSources.Where(lightSource => !colList.Contains(lightSource.Key)))
        {
            Destroy(lightSource.Value.gameObject);
            listRemoveKeys.Add(lightSource.Key);
        }

        foreach (var removeKey in listRemoveKeys)
        {
            _detectedLightSources.Remove(removeKey);
        }
    }

    private float Map(float input, float inputMin, float inputMax, float min, float max)
    {
        return min + (input - inputMin) * (max - min) / (inputMax - inputMin);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}