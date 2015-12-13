using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using nginz;
using nginz.Common;
using OpenTK;
using OpenTK.Graphics;

namespace ProjectBowtie
{
	public class DevConsole : IUpdatable, IDrawable2D, ICanLog
	{
		public bool Visible;
		public int Width;
		public int Height;
		Dictionary<string, Action> Actions;
		Texture2D Texture;
		StringBuilder Buffer;
		Label Text;

		public DevConsole () {
			Visible = false;
			Buffer = new StringBuilder ();
			Actions = new Dictionary<string, Action> ();
			LoadContent ();
		}

		public void RegisterCommand (string command, Action action) {
			Actions.Add (command.ToUpperInvariant (), action);
		}

		void LoadContent () {
			var game = UIController.Instance.Game;
			Texture = game.Content.Load<Texture2D> ("background_console.png");
			Width = game.Bounds.Width;
			Height = 30;
			Text = new Label (Width, Height, UIController.Instance.Fonts ["Roboto Regular"]) {
				X = 0,
				Y = 0,
				CenterText = false,
				ForegroundColor = Color4.White,
				Text = string.Empty,
				FontSize = 10.25f,
			};
		}

		void ProcessCommand () {
			string command = Buffer.ToString ();
			if (Actions.ContainsKey (command)) {
				// TODO: Play sound
				Actions [command] ();
			}
		}

		#region IUpdatable implementation

		public void Update (GameTime time) {
			var game = UIController.Instance.Game;
			if (game.Keyboard.IsKeyTyped (OpenTK.Input.Key.Tab))
				Visible = !Visible;
			if (!Visible)
				return;
			if (game.Keyboard.IsAnyAlphanumericKeyTyped ()) {
				foreach (var kvp in game.Keyboard.Alphanumeric) {
					if (game.Keyboard.IsKeyTyped (kvp.Key)) {
						Buffer.Append (kvp.Value);
						Text.Text = Buffer.ToString ();
					}
				}
			}
			if (game.Keyboard.IsKeyTyped (OpenTK.Input.Key.BackSpace)) {
				if (Buffer.Length > 0)
					Buffer.Length--;
				Text.Text = Buffer.ToString ();
			}
			if (game.Keyboard.IsKeyTyped (OpenTK.Input.Key.Enter)) {
				ProcessCommand ();
				Buffer.Clear ();
				Text.Text = string.Empty;
				Visible = false;
			}
			Text.Update (time);
		}

		#endregion

		#region IDrawable2D implementation

		public void Draw (GameTime time, SpriteBatch batch) {
			if (!Visible)
				return;
			batch.Draw (Texture, Texture.Bounds, new Rectangle (0, 0, Width, Height), Color4.White);
			Text.Draw (time, batch);
		}

		#endregion
	}
}

