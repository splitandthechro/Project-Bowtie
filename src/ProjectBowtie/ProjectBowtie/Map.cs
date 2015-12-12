﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;
using nginz;
using nginz.Common;
using OpenTK;
using OpenTK.Graphics;

namespace ProjectBowtie
{
	public class Map : IAsset, IDrawable2D, ICanLog
	{
		public static Map Dummy;
		static Map () {
			Dummy = new Map ("dummy");
			Dummy.Collisions.Add (new Rectangle (0, 0, 20, 20));
			Dummy.Collisions.Add (new Rectangle (13, 37, 13, 37));
		}

		[JsonIgnore]
		public Texture2D Texture;

		[JsonIgnore]
		public Vector2 Position;

		public string Name;
		public float X { get { return Position.X; } set { Position.X = value; } }
		public float Y { get { return Position.Y; } set { Position.Y = value; } }
		public List<Rectangle> Collisions;

		public Map () {
			Name = string.Empty;
			Position = Vector2.Zero;
			Collisions = new List<Rectangle> ();
		}

		public Map (string name) : this () {
			Name = name;
		}

		public void Save (string path) {
			var json = JsonConvert.SerializeObject (this, Formatting.Indented);
			using (var file = File.Open (path, FileMode.Create, FileAccess.Write, FileShare.Read))
			using (var writer = new StreamWriter (file))
				writer.Write (json);
		}

		public static Map Load (string path) {
			var map = JsonConvert.DeserializeObject<Map> (File.ReadAllText (path));
			map.Texture = UIController.Instance.Game.Content.Load<Texture2D> (string.Format ("map_{0}.png", map.Name));
			map.Log ("Loaded map: {0}", map.Name);
			foreach (var collision in map.Collisions) {
				map.Log ("Registered collider: {0}", collision);
			}
			return map;
		}

		#region IDrawable2D implementation

		public void Draw (GameTime time, SpriteBatch batch) {
			batch.Draw (Texture, Texture.Bounds, UIController.Instance.Game.Bounds, Color4.White);
		}

		#endregion
	}
}

