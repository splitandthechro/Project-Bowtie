using System;
using nginz;

namespace ProjectBowtie
{
	class MainClass
	{
		public static void Main (string[] args) {
			var config = new GameConfiguration {
				Width = 640,
				Height = 480,
				FixedFramerate = false,
				Vsync = VsyncMode.Adaptive,
				Fullscreen = false,
				WindowTitle = "Project Bowtie"
			};
			using (var game = new MainGame (config))
				game.Run ();
		}
	}
}
