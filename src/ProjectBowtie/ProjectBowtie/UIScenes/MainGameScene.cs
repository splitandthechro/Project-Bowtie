using System;
using nginz;

namespace ProjectBowtie
{
	public class MainGameScene : UIScene
	{
		readonly Player Player;

		public MainGameScene () : base ("main_game") {
			Player = new Player ();
		}

		public override void Update (GameTime time) {
			Player.Update (time);
			base.Update (time);
		}

		public override void Draw (GameTime time, SpriteBatch batch) {
			Player.Draw (time, batch);
			base.Draw (time, batch);
		}
	}
}

