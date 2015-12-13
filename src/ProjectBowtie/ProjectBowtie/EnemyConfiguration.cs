using System;
using nginz;
using Newtonsoft.Json;
using nginz.Common;

namespace ProjectBowtie
{
	public class EnemyConfiguration : ICanLog
	{
		[JsonIgnore]
		public Texture2D Texture {
			get {
				if (cachedTexture == null)
					LoadTexture ();
				return cachedTexture;
			}
		}
		Texture2D cachedTexture;

		public string Name;
		public float LifePoints;
		public float Strength;
		public float Speed;
		public int Frames;
		public int IdleFrame;
		public int AttackFrameStart;
		public int AttackFrameCount;

		public string TexturePath;

		public EnemyConfiguration () {
			LifePoints = 0;
			Strength = 0;
			Speed = 0;
			Frames = 0;
			IdleFrame = 0;
			AttackFrameStart = 0;
			AttackFrameCount = 0;
			Name = string.Empty;
		}

		public EnemyConfiguration (string texturePath, float lp, float strength, float speed) {
			TexturePath = texturePath;
			LifePoints = lp;
			Strength = strength;
			Speed = speed;
		}

		public void LoadTexture () {
			this.Log ("Loading texture for enemy type '{0}': {1}", Name, TexturePath);
			cachedTexture = UIController.Instance.Game.Content.Load<Texture2D> (TexturePath);
		}
	}
}

