using System;
using System.Collections.Generic;
using nginz.Common;
using nginz;
using OpenTK;
using System.Linq;
using OpenTK.Graphics;

namespace ProjectBowtie
{
	public class Wave : ICanLog, IUpdatable, IDrawable2D
	{
		public int Index;
		public int SpawnCount;
		public float SpawnSpeed;
		public List<EnemyConfiguration> EnemyTypes;
		public bool WaveEnded;
		public bool AnnouncementEnded;
		public float AnnouncementTimeout = 6f;
		public float AnnouncementDelta;
		public string Message;

		Font Font;

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
			Message = string.Empty;
			Font = UIController.Instance.Fonts ["Roboto Regular"];
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
			var enemy = new Enemy (conf.Clone () as EnemyConfiguration, pos);
			enemies.Add (enemy);
		}

		#region IUpdatable implementation

		public void Update (GameTime time) {
			if (!AnnouncementEnded) {
				AnnouncementDelta += (float) time.Elapsed.TotalSeconds;
				AnnouncementEnded |= AnnouncementDelta > AnnouncementTimeout;
				return;
			}
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
			if (!AnnouncementEnded) {
				var str = string.Format ("Wave {0} incoming: {1}", Index, Message);
				var measurement = Font.MeasureString (str);
				var vec = new Vector2 ((832f / 2f) - (measurement.X / 2f), (624f / 2f) - (measurement.Y / 2f));
				Color4 color = Color4.White;
				if (AnnouncementDelta < 2f)
					color.A = AnnouncementDelta / 2;
				else if (AnnouncementDelta > 4f)
					color.A = 1f - (AnnouncementDelta - 4) / 2;
				Font.DrawString (batch, str, vec, color);
				return;
			}
			for (var i = 0; i < enemies.Count; i++)
				if (!enemies [i].Defeated)
					enemies [i].Draw (time, batch);
		}

		#endregion
	}
}

