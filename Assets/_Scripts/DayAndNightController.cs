using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;
using UnityEngine.Rendering.Universal;

public class DayAndNightController : MonoBehaviour
{
	public static DayAndNightController Instance;

	public bool IsNight { get; private set; } = false;

	[SerializeField] private float switchNightProgressDuration = 3;
	[SerializeField] private Color seaNightColor;
	[SerializeField] private Volume nighVolume;
	[SerializeField] private Light2D globalLight;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			var seq = DOTween.Sequence();

			seq.Append
			(
				DOVirtual.Float(0, 1, switchNightProgressDuration, t =>
				{
					nighVolume.weight = t;
					globalLight.intensity = 1 - t / 2;



					if (!IsNight && t >= .85f)
					{
						IsNight = true;
						var lights = GameObject.FindGameObjectsWithTag("Light");
						foreach (var l in lights)
						{
							if (l.TryGetComponent<Light2D>(out var lightComponent))
							{
								lightComponent.enabled = true;
							}
						}
					}
				})
			);

			seq.Join
			(
				DOVirtual.Color(Camera.main.backgroundColor, seaNightColor, switchNightProgressDuration,
					t => { Camera.main.backgroundColor = t; })
			);
		}
	}
}