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

		Map ActiveMap;

		public MainGameScene () : base ("main_game") {
			Player = new Player ();
			Maps = new List<Map> ();
			LoadMaps ();
			MakeMapActive ("forest");
		}

		public void MakeMapActive (string map) {
			ActiveMap = Maps.FirstOrDefault (m => m.Name == map);
			Player.MapColliders = ActiveMap.Collisions;
		}

		public void LoadMaps () {
			var game = UIController.Instance.Game;
			Maps.Add (game.Content.Load<Map> ("forest"));
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
			base.Update (time);
		}

		public override void Draw (GameTime time, SpriteBatch batch) {
			if (ActiveMap != default (Map))
				ActiveMap.Draw (time, batch);
			Player.Draw (time, batch);
			base.Draw (time, batch);
		}
	}
}

