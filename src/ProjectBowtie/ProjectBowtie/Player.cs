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
	public class Player : IUpdatable, IDrawable2D
	{
		const float SpeedError = 1f;

		public List<Rectangle> MapColliders;
		public Rectangle Bounds {
			get {
				var playerRect = new Rectangle (
					x: (int) currentX - (PlayerSprites.TileWidth / 2),
					y: (int) currentY - (PlayerSprites.TileHeight / 2),
					width: PlayerSprites.TileWidth,
					height: PlayerSprites.TileHeight
				);
				playerRect.Inflate (-20, -20);
				return playerRect;
			}
		}

		Texture2D Crosshair;
		Texture2D Collider;
		SpriteSheet2D PlayerSprites;
		Animator WalkAnimation;
		Animator DashAnimation;
		PlayerMovement Movement;
		Vector2 Position;
		Vector2 WalkTargetLocation;
		float WalkSpeed;
		float DashSpeed;
		float TargetAngle;
		float Rotation;
		bool LeftButtonDown;
		float currentX;
		float currentY;

		public Player () {
			Movement = PlayerMovement.None;
			Position = Vector2.Zero;
			WalkTargetLocation = Vector2.Zero;
			WalkSpeed = 200f;
			DashSpeed = 500f;
			LeftButtonDown = false;
			Rotation = 0;
			TargetAngle = 0;
			MapColliders = new List<Rectangle> ();
			LoadContent ();
		}

		void LoadContent () {
			var game = UIController.Instance.Game;
			var playertex = game.Content.Load<Texture2D> ("player.png");
			Crosshair = game.Content.Load<Texture2D> ("crosshair.png");
			Collider = game.Content.Load<Texture2D> ("collider.png", TextureConfiguration.Nearest);
			PlayerSprites = new SpriteSheet2D (playertex, 5, 1);
			WalkAnimation = new Animator (PlayerSprites, 2, 1);
			WalkAnimation.DurationInMilliseconds = 250;	
			DashAnimation = new Animator (PlayerSprites, 2, 3);
		}

		public void Update (GameTime time) {
			var game = UIController.Instance.Game;

			// Update movement
			if (game.Mouse.IsInsideWindow ()) {
				if (/*!LeftButtonDown &&*/ game.Mouse.IsButtonDown (MouseButton.Left)) {
					WalkTargetLocation = new Vector2 ((int)game.Mouse.X, (int)game.Mouse.Y);
					LeftButtonDown = true;
				} else if (LeftButtonDown && game.Mouse.IsButtonUp (MouseButton.Left))
					LeftButtonDown = false;
			}

			// Calculate rotation based on mouse position
			// The player should always face the cursor
			float angleRad = (float) Math.Atan2 (game.Mouse.Y - Position.Y, game.Mouse.X - Position.X);
			float angleDeg = ((180f / (float) Math.PI) * angleRad) + 90;
			Rotation = MathHelper.DegreesToRadians (angleDeg);

			// Advanced pathfinding
			var optimizedPosition = new Vector2 ((int)Position.X, (int)Position.Y);
			currentX = Position.X;
			currentY = Position.Y;
			if (Math.Abs (WalkTargetLocation.Length - optimizedPosition.Length) > float.Epsilon) {
				var targetX = WalkTargetLocation.X;
				var targetY = WalkTargetLocation.Y;
				TargetAngle = (float) Math.Atan2 ((targetY - currentY), (targetX - currentX));
				var moveX = true;
				var moveY = true;
				if (targetX + SpeedError < currentX - SpeedError)
					currentX -= Math.Abs (WalkSpeed * (float) Math.Cos (TargetAngle) * (float)time.Elapsed.TotalSeconds);
				else if (targetX - SpeedError > currentX + SpeedError)
					currentX += Math.Abs (WalkSpeed * (float) Math.Cos (TargetAngle) * (float)time.Elapsed.TotalSeconds);
				else
					moveX = false;
				if (targetY + SpeedError < currentY - SpeedError)
					currentY -= Math.Abs ((WalkSpeed * (float) Math.Sin (TargetAngle)) * (float)time.Elapsed.TotalSeconds);
				else if (targetY - SpeedError > currentY + SpeedError)
					currentY += Math.Abs ((WalkSpeed * (float) Math.Sin (TargetAngle)) * (float)time.Elapsed.TotalSeconds);
				else
					moveY = false;
				if (moveX || moveY)
					Movement = PlayerMovement.Walk;
				else
					Movement = PlayerMovement.None;
				if (MapColliders.All (collider => !collider.IntersectsWith (Bounds)))
					Position = new Vector2 (currentX, currentY);
				else
					Movement = PlayerMovement.None;
			}

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

			switch (Movement) {
			case PlayerMovement.None:
				batch.Draw (
					texture: PlayerSprites.Texture,
					sourceRect: PlayerSprites [0],
					position: Position,
					color: Color4.White,
					scale: Vector2.One,
					rotation: Rotation,
					origin: new Vector2 (PlayerSprites [0].Width / 2f, PlayerSprites [0].Height / 2f),
					depth: 0
				);
				break;
			case PlayerMovement.Walk:
				WalkAnimation.Draw (time, batch);
				break;
			}

			//foreach (var bounds in MapColliders)
			//	batch.Draw (Collider, Collider.Bounds, bounds, Color4.White);

			batch.Draw (Crosshair, new Vector2 (game.Mouse.X - 10, game.Mouse.Y - 10), Color4.White);
		}
	}
}

