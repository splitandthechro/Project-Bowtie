using System;
using nginz;

namespace ProjectBowtie
{
	class MainClass
	{
		public static void Main (string[] args) {
			var config = new GameConfiguration {
				Width = 832,
				Height = 624,
				FixedFramerate = false,
				FixedWindow = true,
				Vsync = VsyncMode.Adaptive,
				Fullscreen = false,
				WindowTitle = "Project Bowtie"
			};
			using (var game = new MainGame (config))
				game.Run ();
		}
	}
}
