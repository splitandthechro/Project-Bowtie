using System;
using System.Collections.Generic;
using nginz.Common;
using nginz;
using OpenTK;

namespace ProjectBowtie
{
	public class Wave : ICanLog, IUpdatable, IDrawable2D
	{
		public int SpawnCount;
		public float SpawnSpeed;
		public List<EnemyConfiguration> EnemyTypes;

		Random rng;
		float spawnTimeDelta;
		List<Enemy> enemies;

		public Wave () {
			SpawnCount = 0;
			SpawnSpeed = 0;
			spawnTimeDelta = 0;
			EnemyTypes = new List<EnemyConfiguration> ();
			enemies = new List<Enemy> ();
			rng = new Random ();
		}

		public Wave (int count, int speed) : this () {
			SpawnCount = count;
			SpawnSpeed = speed;
		}

		public void AddEnemyType (EnemyConfiguration configuration) {
			this.Log ("Added enemy type to wave: {0}", configuration.Name);
			EnemyTypes.Add (configuration);
		}

		void SpawnRandomEnemy () {
			var conf = EnemyTypes [rng.Next (0, EnemyTypes.Count)];
			var left = rng.Next (0, 2) == 0;
			var top = rng.Next (0, 2) == 0;
			var pos = new Vector2 (
				x: left
				? -conf.Texture.Width
				: UIController.Instance.Game.Bounds.Width,
				y: top
				? -conf.Texture.Height
				: UIController.Instance.Game.Bounds.Height
			);
			var enemy = new Enemy (conf, pos);
		}

		#region IUpdatable implementation

		public void Update (GameTime time) {
			spawnTimeDelta += (float) time.Elapsed.TotalSeconds;
			if (spawnTimeDelta > 1f) {
				SpawnRandomEnemy ();
				spawnTimeDelta -= 1f;
			}
		}

		#endregion

		#region IDrawable2D implementation

		public void Draw (GameTime time, SpriteBatch batch) {
		}

		#endregion
	}
}

