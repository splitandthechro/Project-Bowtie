using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using nginz;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;

namespace ProjectBowtie
{
	public class Player : MovementController, IUpdatable, IDrawable2D
	{
		Texture2D Crosshair;
		Texture2D Collider;
		Texture2D DashEffect;
		SpriteSheet2D PlayerSprites;
		Animator WalkAnimation;
		Animator DashAnimation;
		PlayerMovement Movement;
		bool RightButtonDown;
		bool Dashing;
		bool DashCharging;
		const float DashTimeout = 225;
		const float DashChargeTreshold = 150;
		float DashDelta;
		float DashCharge;
		bool RenderDashEffect;
		bool WasDashing;

		public Player () {
			Movement = PlayerMovement.None;
			Position = Vector2.Zero;
			MoveTo (Position);
			Speed = 200f;
			Colliders = new List<Rectangle> ();
			LoadContent ();
		}

		void LoadContent () {
			var game = UIController.Instance.Game;
			var playertex = game.Content.Load<Texture2D> ("player.png");
			Crosshair = game.Content.Load<Texture2D> ("crosshair.png");
			Collider = game.Content.Load<Texture2D> ("collider.png", TextureConfiguration.Nearest);
			DashEffect = game.Content.Load<Texture2D> ("dash_effect.png");
			PlayerSprites = new SpriteSheet2D (playertex, 5, 1);
			WalkAnimation = new Animator (PlayerSprites, 2, 1);
			WalkAnimation.DurationInMilliseconds = 250;	
			DashAnimation = new Animator (PlayerSprites, 2, 3);
			Width = PlayerSprites.TileWidth;
			Height = PlayerSprites.TileHeight;
			Dashing = false;
		}

		public void Update (GameTime time) {
			var game = UIController.Instance.Game;

			// Update movement
			if (game.Mouse.IsInsideWindow ()) {

				// Movement
				if (!Dashing && game.Mouse.IsButtonDown (MouseButton.Left)) {
					MoveTo (new Vector2 (game.Mouse.X, game.Mouse.Y));
				}

				// Dashing
				if (!RightButtonDown && game.Mouse.IsButtonDown (MouseButton.Right)) {
					RenderDashEffect = true;
					RightButtonDown = true;
					DashCharging = true;
				} else if (RightButtonDown && game.Mouse.IsButtonUp (MouseButton.Right)) {
					RightButtonDown = false;
				}
			}

			// Calculate rotation based on mouse position
			// The player should always face the cursor
			float angleRad = (float) Math.Atan2 (game.Mouse.Y - Position.Y, game.Mouse.X - Position.X);
			float angleDeg = ((180f / (float) Math.PI) * angleRad) + 90;
			Rotation = MathHelper.DegreesToRadians (angleDeg);

			// Update dashing
			Speed = Dashing ? 1000f : 200f;
			if (DashCharging) {
				DashCharge += (float)time.Elapsed.TotalMilliseconds;
				if (DashCharge > DashChargeTreshold) {
					DashCharge = 0;
					DashCharging = false;
					RenderDashEffect = false;
					Dashing = true;
					DisableCollision = true;
					MoveTo (new Vector2 (game.Mouse.X, game.Mouse.Y));
				}
			}
			if (Dashing) {
				DashDelta += (float) time.Elapsed.TotalMilliseconds;
				if (DashDelta > DashTimeout) {
					DashDelta = 0;
					Dashing = false;
					WasDashing = true;
					MoveTo (Position);
				}
			}

			// Update pathing
			var collides = CollidesWithAny ();
			if (!collides && WasDashing) {
				DisableCollision = false;
				WasDashing = false;
			}
			if (UpdatePathing (time) && !CollidesWithAny ()) {
				Movement = Dashing ? PlayerMovement.Dash : PlayerMovement.Walk;
			} else
				Movement = PlayerMovement.None;

			// Update animation positions
			WalkAnimation.Position = Position;
			DashAnimation.Position = Position;

			// Update animations
			WalkAnimation.RotationX = Rotation;
			WalkAnimation.Update (time);
			DashAnimation.RotationX = Rotation;
			DashAnimation.Update (time);
		}

		public void Draw (GameTime time, SpriteBatch batch) {
			var game = UIController.Instance.Game;

			if (RenderDashEffect)
				batch.Draw (
					texture: DashEffect,
					sourceRect: DashEffect.Bounds,
					position: new Vector2 (Position.X - ((DashEffect.Width * 2) / 2f), Position.Y - ((DashEffect.Height * 2) / 2f)),
					color: new Color4 (1, 1, 1, (DashCharge / DashChargeTreshold) / 2f),
					scale: new Vector2 (2, 2),
					depth: 0
				);

			switch (Movement) {
			case PlayerMovement.None:
				batch.Draw (
					texture: PlayerSprites.Texture,
					sourceRect: PlayerSprites [0],
					position: Position,
					color: Color4.White,
					scale: Vector2.One,
					rotation: Rotation,
					origin: new Vector2 (Width / 2f, Height / 2f),
					depth: 0
				);
				break;
			case PlayerMovement.Walk:
				WalkAnimation.Draw (time, batch);
				break;
			case PlayerMovement.Dash:
				DashAnimation.Draw (time, batch);
				break;
			}

			if (DevSettings.VisualizeCollision) {
				batch.Draw (Collider, Collider.Bounds, CollisionBounds, Color4.White);
				foreach (var bounds in Colliders)
					batch.Draw (Collider, Collider.Bounds, bounds, Color4.White);
			}

			batch.Draw (Crosshair, new Vector2 (game.Mouse.X - 10, game.Mouse.Y - 10), Color4.White);
		}
	}
}

