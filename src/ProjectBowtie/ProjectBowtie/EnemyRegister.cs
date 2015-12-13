using System;
using System.Collections.Generic;

namespace ProjectBowtie
{
	public class EnemyRegister
	{
		public Dictionary<string, EnemyConfiguration> EnemyTypes;

		public EnemyConfiguration this [string name] {
			get { return EnemyTypes [name]; }
		}

		public EnemyRegister () {
			EnemyTypes = new Dictionary<string, EnemyConfiguration> ();
		}

		public void AddEnemyType (string name, EnemyConfiguration configuration) {
			if (!EnemyTypes.ContainsKey (name)) {
				configuration.Name = name;
				EnemyTypes.Add (name, configuration);
			}
		}
	}
}

