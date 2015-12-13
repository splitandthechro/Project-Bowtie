using System;
using nginz;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;

namespace ProjectBowtie
{
	public class Enemy : IUpdatable, IDrawable2D
	{
		public Texture2D Texture;
		public Vector2 Position;
		public float Damage;
		public float Speed;

		public Rectangle Bounds {
			get {
				var rect = new Rectangle (
					x: (int) Position.X - (Texture.Width / 2),
					y: (int) Position.Y - (Texture.Height / 2),
					width: Texture.Width,
					height: Texture.Height
				);
				rect.Inflate (-20, -20);
				return rect;
			}
		}

		public Enemy () {
			Position = Vector2.Zero;
		}

		public Enemy (EnemyConfiguration conf, Vector2 pos) {
			Texture = conf.Texture;
			Position = pos;
		}

		#region IUpdatable implementation

		public void Update (GameTime time) {
		}

		#endregion

		#region IDrawable2D implementation

		public void Draw (GameTime time, SpriteBatch batch) {
			batch.Draw (Texture, Texture.Bounds, Position, Color4.White);
		}

		#endregion
	}
}

