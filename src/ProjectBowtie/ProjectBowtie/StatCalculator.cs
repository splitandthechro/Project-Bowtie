using System;
using nginz.Common;

namespace ProjectBowtie
{
	public static class StatCalculator
	{
		public static void Attack (this EnemyConfiguration attacker, EnemyConfiguration target) {
			var damage = attacker.BaseDamage * attacker.DamageMultiplicator;
			target.Health -= damage;
			LogExtensions.LogStatic ("Target '{0}' got attacked by '{1}' and lost {2} health", target.Name, attacker.Name, damage);
		}
	}
}

