using System;
using System.Collections.Generic;
using System.Drawing;
using nginz;
using OpenTK;
using OpenTK.Graphics;

namespace ProjectBowtie
{
	public class Enemy : MovementController, IUpdatable, IDrawable2D
	{
		public EnemyConfiguration Conf;
		public Texture2D Texture;
		public Vector2 PlayerPositionOrigin;

		Random rng;
		SpriteSheet2D Sprites;
		Animator HitAnimation;
		bool EnteringMap;
		bool IsInsideMap;
		Vector2 InitialMapTargetPoint;

		public Enemy (EnemyConfiguration conf, Vector2 pos) {
			Conf = conf;
			Texture = conf.Texture;
			Position = pos;
			CurrentX = Position.X;
			CurrentY = Position.Y;
			Colliders = new List<Rectangle> ();
			Sprites = new SpriteSheet2D (Texture, Conf.Frames, 1);
			HitAnimation = new Animator (Sprites, Conf.AttackFrameCount, Conf.AttackFrameStart);
			PlayerPositionOrigin = Vector2.Zero;
			rng = new Random ();
			IsInsideMap = false;
			Speed = conf.Speed;
		}

		#region IUpdatable implementation

		public void Update (GameTime time) {
			if (!EnteringMap) {
				InitialMapTargetPoint = GetRandomMapLocation ();
				MoveTo (InitialMapTargetPoint);
				EnteringMap = true;
			}
			if (EnteringMap) {
				IsInsideMap |=
					Math.Abs (Position.X - InitialMapTargetPoint.X) < float.Epsilon
					&& Math.Abs (Position.Y - InitialMapTargetPoint.Y) < float.Epsilon;
				if (!IsInsideMap) {
					UpdatePathing (time);
					return;
				}
			}
			Console.WriteLine ("Enemy '{0}' reached target point!", Conf.Name);
		}

		#endregion

		Vector2 GetRandomMapLocation () {
			return new Vector2 (Randomizer.Next (32, 832 - Width - 32), Randomizer.Next (32, 624 - Height - 32));
		}

		#region IDrawable2D implementation

		public void Draw (GameTime time, SpriteBatch batch) {
			if (Texture	!= null)
				batch.Draw (Texture, Sprites [Conf.IdleFrame], Position, Color4.White);
		}

		#endregion
	}
}

