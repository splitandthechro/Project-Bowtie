using System;
using nginz;
using Newtonsoft.Json;

namespace ProjectBowtie
{
	public class EnemyConfiguration
	{
		[JsonIgnore]
		public Texture2D Texture;

		public string Name;
		public float LifePoints;
		public float Strength;
		public float Speed;

		public string TexturePath;

		public EnemyConfiguration () {
			LifePoints = 0;
			Strength = 0;
			Speed = 0;
			Name = string.Empty;
		}

		public EnemyConfiguration (Texture2D tex, float lp, float strength, float speed) {
			Texture = tex;
			LifePoints = lp;
			Strength = strength;
			Speed = speed;
		}

		public void LoadTexture () {
			Texture = UIController.Instance.Game.Content.Load<Texture2D> (TexturePath);
		}
	}
}

