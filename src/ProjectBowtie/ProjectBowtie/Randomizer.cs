using System;

namespace ProjectBowtie
{
	public static class Randomizer
	{
		public static readonly Random rng;

		static Randomizer () {
			rng = new Random ();
		}

		public static int Next (int a, int b) {
			return rng.Next (a, b);
		}
	}
}

