using System;
using System.Collections.Generic;
using nginz.Common;

namespace ProjectBowtie
{
	public class Wave : ICanLog
	{
		public int SpawnCount;
		public float SpawnSpeed;

		public List<EnemyConfiguration> EnemyTypes;

		public Wave () {
			SpawnCount = 0;
			SpawnSpeed = 0;
			EnemyTypes = new List<EnemyConfiguration> ();
		}

		public Wave (int count, int speed) : this () {
			SpawnCount = count;
			SpawnSpeed = speed;
		}

		public void AddEnemyType (EnemyConfiguration configuration) {
			this.Log ("Added enemy type to wave: {0}", configuration.Name);
			EnemyTypes.Add (configuration);
		}
	}
}

