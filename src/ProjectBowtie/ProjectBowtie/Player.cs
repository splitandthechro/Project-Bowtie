using System;
using nginz;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;

namespace ProjectBowtie
{
	public class Player : IUpdatable, IDrawable2D
	{
		Texture2D Crosshair;
		SpriteSheet2D PlayerSprites;
		Animator WalkAnimation;
		Animator DashAnimation;
		PlayerMovement Movement;
		Vector2 Position;
		Vector2 WalkTargetLocation;
		float WalkSpeed;
		float DashSpeed;
		float SpeedError = 1f;
		float Rotation;

		bool LeftButtonDown;

		public Player () {
			Movement = PlayerMovement.None;
			Position = Vector2.Zero;
			WalkTargetLocation = Vector2.Zero;
			WalkSpeed = 200f;
			DashSpeed = 500f;
			LeftButtonDown = false;
			Rotation = 0;
			LoadContent ();
		}

		void LoadContent () {
			var game = UIController.Instance.Game;
			var playertex = game.Content.Load<Texture2D> ("player.png");
			Crosshair = game.Content.Load<Texture2D> ("crosshair.png");
			PlayerSprites = new SpriteSheet2D (playertex, 5, 1);
			WalkAnimation = new Animator (PlayerSprites, 2, 3);
			WalkAnimation.DurationInMilliseconds = 250;
			DashAnimation = new Animator (PlayerSprites, 2, 3);
		}

		public void Update (GameTime time) {
			var game = UIController.Instance.Game;

			// Update movement
			if (game.Mouse.IsInsideWindow ()) {
				if (/*!LeftButtonDown &&*/ game.Mouse.IsButtonDown (MouseButton.Left)) {
					WalkTargetLocation = new Vector2 (game.Mouse.X, game.Mouse.Y);
					LeftButtonDown = true;
				} else if (LeftButtonDown && game.Mouse.IsButtonUp (MouseButton.Left))
					LeftButtonDown = false;
			}

			// Calculate rotation based on mouse position
			// The player should always face the cursor
			float angleRad = (float) Math.Atan2 (game.Mouse.Y - Position.Y, game.Mouse.X - Position.X);
			float angleDeg = (180f / (float) Math.PI) * angleRad + 75;
			Rotation = MathHelper.DegreesToRadians (angleDeg);

			// Basic pathfinding
			if (Math.Abs (WalkTargetLocation.Length - Position.Length) > float.Epsilon) {
				var currentX = Position.X;
				var currentY = Position.Y;
				var targetX = WalkTargetLocation.X;
				var targetY = WalkTargetLocation.Y;
				var moveX = true;
				var moveY = true;
				if (targetX + SpeedError < currentX - SpeedError)
					currentX -= WalkSpeed * (float)time.Elapsed.TotalSeconds;
				else if (targetX - SpeedError > currentX + SpeedError)
					currentX += WalkSpeed * (float)time.Elapsed.TotalSeconds;
				else
					moveX = false;
				if (targetY + SpeedError < currentY - SpeedError)
					currentY -= WalkSpeed * (float)time.Elapsed.TotalSeconds;
				else if (targetY - SpeedError > currentY + SpeedError)
					currentY += WalkSpeed * (float)time.Elapsed.TotalSeconds;
				else
					moveY = false;
				if (moveX || moveY)
					Movement = PlayerMovement.Walk;
				else
					Movement = PlayerMovement.None;
				Position = new Vector2 (currentX, currentY);
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

			batch.Draw (Crosshair, new Vector2 (game.Mouse.X - 10, game.Mouse.Y - 10), Color4.White);
		}
	}
}

