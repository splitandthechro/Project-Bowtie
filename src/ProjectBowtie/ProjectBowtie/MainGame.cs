using System;
using nginz;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace ProjectBowtie
{
	public class MainGame : Game
	{
		Camera OrthoCamera;
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

			// Register asset handlers
			Content.RegisterAssetHandler<Map> (typeof (MapHandler));
			Content.RegisterAssetHandler<EnemyConfiguration> (typeof(EnemyConfigurationHandler));
			Content.Save<Map> (Map.Dummy, Map.Dummy.Name);

			// Create scenes
			Scene_MainMenu = new MainMenuScene ();
			Scene_MainGame = new MainGameScene ();

			// Set active scene
			Scene_MainMenu.MakeActive ();

			// Create orthographic camera
			OrthoCamera = new Camera (60f, Resolution, 0, 16, type: ProjectionType.Orthographic);
			GlobalObjects.OrthoCamera = OrthoCamera;
			GlobalObjects.Shaker = new CameraShake (OrthoCamera);
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

			// Update the camera shaker
			GlobalObjects.Shaker.Update (time);

			// Begin sprite batching
			SpriteBatch.Begin (OrthoCamera);

			// Draw the UI
			UIController.Instance.Draw (time, SpriteBatch);

			// End sprite batching
			SpriteBatch.End ();
			base.Draw (time);
		}
	}
}

