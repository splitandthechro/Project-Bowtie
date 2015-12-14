using System;
using System.Collections.Generic;
using nginz.Common;
using nginz;
using OpenTK;
using System.Linq;

namespace ProjectBowtie
{
	public class Wave : ICanLog, IUpdatable, IDrawable2D
	{
		public int SpawnCount;
		public float SpawnSpeed;
		public List<EnemyConfiguration> EnemyTypes;
		public bool WaveEnded;

		float spawnTimeDelta;
		int spawnedCount;
		readonly List<Enemy> enemies;

		public Wave () {
			SpawnCount = 0;
			SpawnSpeed = 0;
			spawnTimeDelta = 0;
			spawnedCount = 0;
			EnemyTypes = new List<EnemyConfiguration> ();
			enemies = new List<Enemy> ();
			WaveEnded = false;
		}

		public Wave (int count, int speed) : this () {
			SpawnCount = count;
			SpawnSpeed = speed;
		}

		public void UpdateEnemyPathing (Vector2 PlayerPositionOrigin) {
			for (var i = 0; i < enemies.Count; i++)
				enemies [i].PlayerPositionOrigin = PlayerPositionOrigin;
		}

		public void AddEnemyType (EnemyConfiguration configuration) {
			this.Log ("Added enemy type to wave: {0}", configuration.Name);
			EnemyTypes.Add (configuration);
		}

		void SpawnRandomEnemy () {
			var conf = EnemyTypes [Randomizer.Next (0, EnemyTypes.Count)];
			var left = Randomizer.Next (0, 2) == 0;
			var pos = new Vector2 (
				x: left
				? -conf.Texture.Width
				: 832,
				y: Randomizer.Next (-conf.Texture.Height, UIController.Instance.Game.Bounds.Height)
			);
			this.Log ("Spawning enemy of type '{0}' at position {1}", conf.Name, pos);
			var enemy = new Enemy (conf, pos);
			enemies.Add (enemy);
		}

		#region IUpdatable implementation

		public void Update (GameTime time) {
			for (var i = 0; i < EnemyTypes.Count; i++)
				EnemyTypes [i].LoadTexture ();
			if (SpawnCount > spawnedCount) {
				spawnTimeDelta += (float)time.Elapsed.TotalSeconds;
				if (spawnTimeDelta > 1f / SpawnSpeed) {
					SpawnRandomEnemy ();
					spawnedCount++;
					spawnTimeDelta -= (1f / SpawnSpeed);
				}
			}
			WaveEnded |= spawnedCount == SpawnCount && enemies.All (e => e.Defeated);
			for (var i = 0; i < enemies.Count; i++)
				if (!enemies [i].Defeated)
					enemies [i].Update (time);
		}

		#endregion

		#region IDrawable2D implementation

		public void Draw (GameTime time, SpriteBatch batch) {
			for (var i = 0; i < enemies.Count; i++)
				if (!enemies [i].Defeated)
					enemies [i].Draw (time, batch);
		}

		#endregion
	}
}

