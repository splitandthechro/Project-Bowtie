using System;
using nginz;

namespace ProjectBowtie
{
	public class WaveSpawnConfiguration
	{
		public Enemy EnemyInstance;
		public int SpawnCount;
		public int SpawnIndex;
		public int SpawnSpeed;

		float delta;

		public WaveSpawnConfiguration () {
			EnemyInstance = null;
			SpawnCount = 0;
			SpawnIndex = 0;
		}

		public WaveSpawnConfiguration (Enemy enemyType, int count) {
			EnemyInstance = enemyType;
			SpawnCount = count;
			SpawnIndex = 0;
		}
	}
}

