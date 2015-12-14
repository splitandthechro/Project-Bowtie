using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;
using nginz;
using nginz.Common;
using OpenTK;
using OpenTK.Graphics;
using System.Linq;

namespace ProjectBowtie
{
	public class Map : IAsset, IUpdatable, IDrawable2D, ICanLog
	{
		public static Map Dummy;
		static Map () {
			Dummy = new Map ("dummy");
			Dummy.Collisions.Add (new Rectangle (0, 0, 20, 20));
			Dummy.Collisions.Add (new Rectangle (13, 37, 13, 37));
			Dummy.EnemyRegister.AddEnemyType ("slime", new EnemyConfiguration { Health = 100, Speed = 40, BaseDamage = 20, TexturePath = "slime.png" });
			var wave = new Wave (10, 50);
			wave.AddEnemyType (Dummy.EnemyRegister ["slime"]);
			Dummy.Waves.Add (wave);
		}

		[JsonIgnore]
		public Texture2D Texture;
		public string Name;
		public List<Rectangle> Collisions;
		public List<Wave> Waves;
		public int WaveIndex;
		public Wave ActiveWave;

		EnemyRegister EnemyRegister;

		public Map () {
			Name = string.Empty;
			Collisions = new List<Rectangle> ();
			Waves = new List<Wave> ();
			WaveIndex = 0;
			EnemyRegister = new EnemyRegister ();
		}

		public Map (string name) : this () {
			Name = name;
		}

		public void Initialize () {
			ActiveWave = Waves [WaveIndex];
			ActiveWave.Index = 1;
		}

		public void UpdateWaveEnemyPathing (Vector2 PlayerPositionOrigin) {
			ActiveWave.UpdateEnemyPathing (PlayerPositionOrigin);
		}

		public void Save (string path) {
			var json = JsonConvert.SerializeObject (this, Formatting.Indented);
			using (var file = File.Open (path, FileMode.Create, FileAccess.Write, FileShare.Read))
			using (var writer = new StreamWriter (file))
				writer.Write (json);
		}

		public static Map Load (string path) {
			var map = JsonConvert.DeserializeObject<Map> (File.ReadAllText (path));
			map.Texture = UIController.Instance.Game.Content.Load<Texture2D> (string.Format ("map_{0}.png", map.Name));
			map.Log ("Loaded map: {0}", map.Name);
			foreach (var collision in map.Collisions) {
				map.Log ("Registered collider: {0}", collision);
			}
			map.Initialize ();
			return map;
		}

		#region IUpdatable implementation

		public void Update (GameTime time) {
			ActiveWave.Update (time);
			if (ActiveWave.WaveEnded) {
				WaveIndex++;
				if (WaveIndex < Waves.Count) {
					this.Log ("Loading next wave");
					ActiveWave = Waves [WaveIndex];
					ActiveWave.Index = WaveIndex + 1;
				} else
					UIController.Instance.SwitchScene ("after_match_screen");
			}
		}

		#endregion

		#region IDrawable2D implementation

		public void Draw (GameTime time, SpriteBatch batch) {
			batch.Draw (Texture, Texture.Bounds, UIController.Instance.Game.Bounds, Color4.White);
			ActiveWave.Draw (time, batch);
		}

		#endregion
	}
}

