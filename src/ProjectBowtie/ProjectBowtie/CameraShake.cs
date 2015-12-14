using System;
using nginz;
using OpenTK;

namespace ProjectBowtie
{
	public class CameraShake
	{
		public const float INTENSITY_ULTRA = 100f;
		public const float INTENSITY_HIGH = 50f;
		public const float INTENSITY_NORMAL = 25f;
		public const float INTENSITY_SUBTLE = 1f;

		public const float AMPLITUDE_ULTRA = 10f;
		public const float AMPLITUDE_HIGH = 5f;
		public const float AMPLITUDE_SUBTLE = 2f;
		public const float AMPLITUDE_VERY_SUBTLE = 1f;

		public const float DURATION_ULTRA = 0.1f;
		public const float DURATION_LONG = 0.5f;
		public const float DURATION_NORMAL = 1f;
		public const float DURATION_SHORT = 2f;
		public const float DURATION_VERY_SHORT = 5f;

		readonly Camera InternalCamera;
		float Amplitude;
		float Intensity;
		float Time;
		float Slope;

		public CameraShake (Camera camera) {
			InternalCamera = camera;
			Amplitude = 0f;
			Intensity = 0f;
			Slope = 0f;
			Time = 0f;
		}

		public void Shake (
			float amplitude = AMPLITUDE_SUBTLE,
			float intensity = INTENSITY_HIGH,
			float duration = DURATION_SHORT,
			bool premultiply_duration = true) {
			Amplitude = amplitude;
			Intensity = intensity;
			Slope = premultiply_duration
				? amplitude * duration
				: duration;
			Time = 0f;
		}

		public void FastShake () {
			Shake (AMPLITUDE_SUBTLE, INTENSITY_SUBTLE, DURATION_VERY_SHORT);
		}

		public void IntenseShake () {
			Shake (AMPLITUDE_HIGH, INTENSITY_HIGH, DURATION_SHORT);
		}

		public void Update (GameTime time) {
			if (Amplitude <= float.Epsilon)
				return;
			Time += (float) time.Elapsed.TotalSeconds;
			Amplitude -= Slope * (float) time.Elapsed.TotalSeconds;
			var amount = Amplitude * (float) Math.Sin (2f * Math.PI * (Time * Intensity));
			var vec3 = new Vector3 (amount, amount, 0);
			InternalCamera.SetAbsolutePosition (vec3);
		}
	}
}

