using System;
using nginz;
using Newtonsoft.Json;
using nginz.Common;
using System.IO;

namespace ProjectBowtie
{
	public class EnemyConfiguration : IAsset, ICanLog
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
		public float Speed;
		public float BaseDamage;
		public float DamageMultiplicator;
		public float Health;
		public int Frames;
		public int IdleFrame;
		public int AttackFrameStart;
		public int AttackFrameCount;
		public int WalkFrameStart;
		public int WalkFrameCount;

		public string TexturePath;

		public EnemyConfiguration () {
			Speed = 0;
			Frames = 0;
			IdleFrame = 0;
			AttackFrameStart = 0;
			AttackFrameCount = 1;
			WalkFrameStart = 0;
			WalkFrameCount = 1;
			BaseDamage = 0;
			DamageMultiplicator = 1;
			Health = 0;
			Name = string.Empty;
		}

		public void Save (string path) {
			var json = JsonConvert.SerializeObject (this, Formatting.Indented);
			using (var file = File.Open (path, FileMode.Create, FileAccess.Write, FileShare.Read))
			using (var writer = new StreamWriter (file))
				writer.Write (json);
		}

		public static EnemyConfiguration Load (string path) {
			var conf = JsonConvert.DeserializeObject<EnemyConfiguration> (File.ReadAllText (path));
			return conf;
		}

		public void LoadTexture () {
			if (cachedTexture != null)
				return;
			this.Log ("Loading texture for enemy type '{0}': {1}", Name, TexturePath);
			cachedTexture = UIController.Instance.Game.Content.Load<Texture2D> (TexturePath);
		}
	}
}

