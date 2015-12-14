using System;
using System.Collections.Generic;
using System.Linq;
using nginz;

namespace ProjectBowtie
{
	public class MainGameScene : UIScene
	{
		readonly Player Player;
		readonly List<Map> Maps;
		readonly DevConsole DevConsole;

		Map ActiveMap;

		public MainGameScene () : base ("main_game") {
			Player = new Player ();
			GlobalObjects.Player = Player;
			Maps = new List<Map> ();
			DevConsole = new DevConsole ();
			DevConsole.RegisterCommand ("bounds 1", () => DevSettings.VisualizeCollision = true);
			DevConsole.RegisterCommand ("bounds 0", () => DevSettings.VisualizeCollision = false);
			DevConsole.RegisterCommand ("prox 1", () => DevSettings.VisualizeProximity = true);
			DevConsole.RegisterCommand ("prox 0", () => DevSettings.VisualizeProximity = false);
			DevConsole.RegisterCommand ("coords 1", () => DevSettings.VisualizeCoordinates = true);
			DevConsole.RegisterCommand ("coords 0", () => DevSettings.VisualizeCoordinates = false);
			LoadContent ();
			MakeMapActive ("forest");
		}

		public void MakeMapActive (string map) {
			ActiveMap = Maps.FirstOrDefault (m => m.Name == map);
			Player.Colliders = ActiveMap.Collisions;
		}

		public void LoadContent () {
			var game = UIController.Instance.Game;
			var map = game.Content.Load<Map> ("forest");
			DevSettings.CollisionTexture = game.Content.Load<Texture2D> ("collider.png");
			Player.Conf = game.Content.Load<EnemyConfiguration> ("player");
			Maps.Add (map);
		}

		public override void OnSceneSwitch () {
			var game = UIController.Instance.Game;
			game.Mouse.CursorVisible = false;
		}

		public override void Update (GameTime time) {
			var game = UIController.Instance.Game;
			if (game.Keyboard.IsKeyTyped (OpenTK.Input.Key.Escape))
				UIController.Instance.SwitchScene ("main_menu");
			Player.Update (time);
			ActiveMap.Update (time);
			ActiveMap.UpdateWaveEnemyPathing (new OpenTK.Vector2 (Player.CollisionBounds.X, Player.CollisionBounds.Y));
			DevConsole.Update (time);
			base.Update (time);
		}

		public override void Draw (GameTime time, SpriteBatch batch) {
			if (ActiveMap != default (Map))
				ActiveMap.Draw (time, batch);
			Player.Draw (time, batch);
			DevConsole.Draw (time, batch);
			base.Draw (time, batch);
		}
	}
}

