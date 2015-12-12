using System;
using nginz;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace ProjectBowtie
{
	public class MainGame : Game
	{
		MainMenuScene Scene_MainMenu;
		MainGameScene Scene_MainGame;

		public MainGame (GameConfiguration conf)
			: base (conf) { }

		protected override void Initialize () {

			// Set content root
			ContentRoot = "assets";

			// Bind UI Controller
			UIController.Instance.Bind (this);
			UIController.Instance.LoadDefaultFonts ();

			// Create scenes
			Scene_MainMenu = new MainMenuScene ();
			Scene_MainGame = new MainGameScene ();

			// Set active scene
			Scene_MainMenu.MakeActive ();
			base.Initialize ();
		}

		protected override void Update (GameTime time) {

			// Update the UI
			UIController.Instance.Update (time);
			base.Update (time);
		}

		protected override void Draw (GameTime time) {

			// Clear the screen
			GL.ClearColor (Color4.Black);
			GL.Clear (ClearBufferMask.ColorBufferBit);

			// Begin sprite batching
			SpriteBatch.Begin ();

			// Draw the UI
			UIController.Instance.Draw (time, SpriteBatch);

			// End sprite batching
			SpriteBatch.End ();
			base.Draw (time);
		}
	}
}

