using System;
using System.Collections.Generic;
using System.Drawing;
using nginz;
using nginz.Common;
using OpenTK;
using OpenTK.Graphics;

namespace ProjectBowtie
{
	public class Enemy : LivingEntity, IUpdatable, IDrawable2D, ICanLog
	{
		public EnemyConfiguration Conf;
		public Texture2D Texture;
		public Vector2 PlayerPositionOrigin;
		public bool Defeated;

		SpriteSheet2D Sprites;
		Animator HitAnimation;
		Animator WalkAnimation;
		EnemyMovement Movement;
		bool EnteringMap;
		bool IsInsideMap;
		bool GotHitByDash;
		bool Attacked;
		float AttackDelta;
		float AttackAnimationDelta;
		const float AttackAnimationTimeout = 200f;
		Vector2 InitialMapTargetPoint;

		public Enemy (EnemyConfiguration conf, Vector2 pos) {
			Conf = conf;
			Texture = conf.Texture;
			Position = pos;
			CurrentX = Position.X;
			CurrentY = Position.Y;
			Colliders = new List<Rectangle> ();
			Sprites = new SpriteSheet2D (Texture, Conf.Frames, 1);
			Width = Sprites.TileWidth;
			Height = Sprites.TileHeight;
			HitAnimation = new Animator (Sprites, Conf.AttackFrameCount, Conf.AttackFrameStart);
			WalkAnimation = new Animator (Sprites, Conf.WalkFrameCount, Conf.WalkFrameStart);
			PlayerPositionOrigin = Vector2.Zero;
			Movement = EnemyMovement.Idle;
			IsInsideMap = false;
			Defeated = false;
			Speed = conf.Speed;
		}

		#region IUpdatable implementation

		public void Update (GameTime time) {
			var game = UIController.Instance.Game;

			// Initial target destination
			if (!EnteringMap) {
				HitAnimation.DurationInMilliseconds = Conf.AttackAnimationDuration;
				WalkAnimation.DurationInMilliseconds = Conf.WalkAnimationDuration;
				InitialMapTargetPoint = GetOptimalStartingLocation ();
				MoveTo (InitialMapTargetPoint);
				EnteringMap = true;
				return;
			}

			// Update pathing to reach initial target destination
			if (!IsInsideMap) {
				if (UpdatePathing (time))
					return;
				IsInsideMap = true;
			}

			GotHitByDash &= GlobalObjects.Player.Dashing;

			if (PlayerInCloseProximity () && !GotHitByDash && GlobalObjects.Player.Dashing) {
				GlobalObjects.Shaker.IntenseShake ();
				GlobalObjects.Player.Conf.Attack (Conf);
				Defeated |= Conf.Health < 0;
				if (Defeated)
					this.Log ("'{0}' was defeated", Conf.Name);
				GotHitByDash = true;
			}

			else if (PlayerInCloseProximity () && !GotHitByDash) {
				if (!Attacked) {
					// Shake violently
					GlobalObjects.Shaker.Shake (2f + (MathHelper.Clamp ((200f - GlobalObjects.Player.Conf.Health) / 2, 0, 50)), 100f, 2f);
					Conf.Attack (GlobalObjects.Player.Conf);
					AttackAnimationDelta = AttackAnimationTimeout;
					Attacked = true;
					AttackDelta = 0;
				}
			}

			AttackDelta += (float) time.Elapsed.TotalMilliseconds;
			Attacked &= AttackDelta < Conf.AttackSpeed * 1000f;

			// Update pathing till the last move is done
			bool updatedPathing;
			if (updatedPathing = UpdatePathing (time)) {
				Movement = EnemyMovement.Walk;
				WalkAnimation.Position = Position;
				WalkAnimation.Update (time);
				HitAnimation.Position = Position;
				HitAnimation.Update (time);
			}
			Movement = EnemyMovement.Idle;
			if (AttackAnimationDelta > float.Epsilon) {
				AttackAnimationDelta -= (float)time.Elapsed.TotalMilliseconds;
				Movement = EnemyMovement.Attack;
			}
			if (updatedPathing)
				return;

			// Basic AI
			const int maxdistance = 32;
			switch (Randomizer.Next (0, 10)) {
			case 0:
				// Move left by a certain amount
				if (Position.X > maxdistance + 32)
					MoveTo (new Vector2 (Position.X - Randomizer.Next (0, maxdistance), Position.Y));
				break;
			case 1:
				// Move right by a certain amount
				if (Position.X < 832 - Sprites.TileWidth - maxdistance - 32)
					MoveTo (new Vector2 (Position.X + Randomizer.Next (0, maxdistance), Position.Y));
				break;
			case 2:
				// Move up by a certain amount
				if (Position.Y > maxdistance + 32)
					MoveTo (new Vector2 (Position.X, Position.Y - Randomizer.Next (0, maxdistance)));
				break;
			case 3:
				// Move down by a certain amount
				if (Position.Y < 624 - Sprites.TileHeight - maxdistance - 32)
					MoveTo (new Vector2 (Position.X, Position.Y + Randomizer.Next (0, maxdistance)));
				break;
			case 4:
			case 5:
			case 6:
				// Move towards the player
				var targetX = Randomizer.Next (0, maxdistance);
				var targetY = Randomizer.Next (0, maxdistance);
				if (PlayerPositionOrigin.X < CurrentX)
					targetX *= -1;
				if (PlayerPositionOrigin.Y < CurrentY)
					targetY *= -1;
				MoveTo (new Vector2 (Position.X + targetX, Position.Y + targetY));
				break;
			}
		}

		#endregion

		bool PlayerInCloseProximity (int inflateX = 0, int inflateY = 0) {
			var bigPlayerBounds = GlobalObjects.Player.CollisionBounds;
			if (inflateX != 0 || inflateY != 0)
				bigPlayerBounds.Inflate (Width, Height);
			return bigPlayerBounds.IntersectsWith (CollisionBounds);
		}

		Vector2 GetOptimalStartingLocation () {
			var vec = new Vector2 ();
			vec.X = Position.X < 0
				? 32
				: Position.X >= 832
				? 832 - Width - 32
				: 32;
			vec.Y = Position.Y;
			return vec;
		}

		#region IDrawable2D implementation

		public void Draw (GameTime time, SpriteBatch batch) {
			switch (Movement) {
			case EnemyMovement.Idle:
				batch.Draw (
					texture: Sprites.Texture,
					sourceRect: Sprites [Conf.IdleFrame],
					position: Position,
					color: Color4.White,
					scale: Vector2.One,
					rotation: Rotation,
					origin: new Vector2 (Width / 2f, Height / 2f),
					depth: 0
				);
				break;
			case EnemyMovement.Walk:
				WalkAnimation.Draw (time, batch);
				break;
			case EnemyMovement.Attack:
				HitAnimation.Draw (time, batch);
				break;
			}
			if (DevSettings.VisualizeCollision)
				batch.Draw (DevSettings.CollisionTexture, DevSettings.CollisionTexture.Bounds, CollisionBounds, Color4.White);
			if (DevSettings.VisualizeCoordinates) {
				var font = UIController.Instance.Fonts ["Roboto Regular"];
				font.DrawString (batch,
					string.Format ("X: {0} Y: {1}", (int) CurrentX, (int) CurrentY),
					new Vector2 (CurrentX + (Width / 2) + 16, CurrentY - (Height / 2)), Color4.White);
			}
		}

		#endregion
	}
}

