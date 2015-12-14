using System;
using nginz;
using OpenTK;
using OpenTK.Graphics;

namespace ProjectBowtie
{
	public class AfterMatchScreen : UIScene
	{
		readonly Texture2D Background;
		const float timeout = 2000;
		float delta = 0;

		public AfterMatchScreen () : base ("after_match_screen") {
			Background = UIController.Instance.Game.Content.Load<Texture2D> ("neutral_screen.png");
		}

		public override void Update (GameTime time) {
			delta += (float) time.Elapsed.TotalMilliseconds;
			if (delta > timeout && UIController.Instance.Game.Mouse.IsButtonDown (OpenTK.Input.MouseButton.Left))
				UIController.Instance.Game.Exit ();
			base.Update (time);
		}

		public override void Draw (GameTime time, SpriteBatch batch) {
			batch.Draw (Background, Vector2.Zero, Color4.White);
			base.Draw (time, batch);
		}
	}
}

