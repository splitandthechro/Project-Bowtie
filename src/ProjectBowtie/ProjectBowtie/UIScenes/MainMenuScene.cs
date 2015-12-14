using System;
using nginz;
using OpenTK;
using OpenTK.Graphics;

namespace ProjectBowtie
{
	public class MainMenuScene : UIScene
	{
		Sound ThemeMusic;
		Texture2D Background;
		Button BtnNewGame;
		Button BtnLoadGame;
		Button BtnExit;

		public MainMenuScene () : base ("main_menu") {
			LoadContent ();
			CreateLayout ();
			CreateLogic ();
			Controls.Add (BtnNewGame);
			Controls.Add (BtnExit);
		}

		public override void OnSceneSwitch () {
			var game = UIController.Instance.Game;
			game.Mouse.CursorVisible = true;
		}

		void LoadContent () {
			var game = UIController.Instance.Game;
			Background = game.Content.Load<Texture2D> ("background_menu.png");
			ThemeMusic = game.Content.Load<Sound> ("menu_theme.ogg");
			game.SoundManager.LoopMusic = false;
			game.SoundManager.PlayMusic (ThemeMusic);
			game.SoundManager.MusicVolume = 0.01f;
		}

		void CreateLayout () {
			var game = UIController.Instance.Game;
			BtnNewGame = new Button (300, 30, "Roboto Regular") {
				X = (game.Bounds.Width / 2) - 150,
				Y = (game.Bounds.Height / 2) - 30 - 30,
				FontSize = 14.25f,
				ForegroundColor = Color4.White,
				HighlightForegroundColor = Color4.LightGray,
				Text = "New Game",
				UseTexture = false,
			};
			BtnLoadGame = new Button (300, 30, "Roboto Regular") {
				X = (game.Bounds.Width / 2) - 150,
				Y = (game.Bounds.Height / 2) - 30,
				FontSize = 14.25f,
				ForegroundColor = Color4.White,
				HighlightForegroundColor = Color4.LightGray,
				Text = "Load Game",
				UseTexture = false,
			};
			BtnExit = new Button (300, 30, "Roboto Regular") {
				X = (game.Bounds.Width / 2) - 150,
				Y = (game.Bounds.Height / 2) - 30,
				FontSize = 14.25f,
				ForegroundColor = Color4.White,
				HighlightForegroundColor = Color4.LightGray,
				Text = "Exit",
				UseTexture = false,
			};
		}

		void CreateLogic () {
			var game = UIController.Instance.Game;
			BtnNewGame.Click += (sender, e) => UIController.Instance.SwitchScene ("main_game");
			BtnExit.Click += (sender, e) => game.Exit ();
		}

		public override void Draw (GameTime time, SpriteBatch batch) {
			batch.Draw (Background, Background.Bounds, UIController.Instance.Game.Bounds, Color4.White);
			base.Draw (time, batch);
		}
	}
}

