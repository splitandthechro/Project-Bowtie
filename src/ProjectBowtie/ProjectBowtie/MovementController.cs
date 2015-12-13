using System;
using System.Collections.Generic;
using System.Drawing;
using nginz;
using OpenTK;
using System.Linq;

namespace ProjectBowtie
{
	public class CharacterController
	{
		public int Width;
		public int Height;
		public Rectangle Bounds {
			get {
				var playerRect = new Rectangle (
					x: (int) CurrentX - (Width / 2),
					y: (int) CurrentY - (Height / 2),
					width: Width,
					height: Height
				);
				playerRect.Inflate (-20, -20);
				return playerRect;
			}
		}

		const float SpeedError = 1f;
		public List<Rectangle> Colliders;
		public float CurrentX;
		public float CurrentY;
		public float Rotation;
		public float Speed;
		public Vector2 Position;
		public Vector2 TargetLocation;

		public CharacterController () { }

		public void MoveTo (Vector2 pos) {
			TargetLocation = pos;
		}

		public bool UpdatePathing (GameTime time) {
			var optimizedPosition = new Vector2 ((int)Position.X, (int)Position.Y);
			CurrentX = Position.X;
			CurrentY = Position.Y;
			if (Math.Abs (TargetLocation.Length - optimizedPosition.Length) > float.Epsilon) {
				var targetX = TargetLocation.X;
				var targetY = TargetLocation.Y;
				var TargetAngle = (float)Math.Atan2 ((targetY - CurrentY), (targetX - CurrentX));
				var moveX = true;
				var moveY = true;
				if (targetX + SpeedError < CurrentX - SpeedError)
					CurrentX -= Math.Abs (Speed * (float)Math.Cos (TargetAngle) * (float)time.Elapsed.TotalSeconds);
				else if (targetX - SpeedError > CurrentX + SpeedError)
					CurrentX += Math.Abs (Speed * (float)Math.Cos (TargetAngle) * (float)time.Elapsed.TotalSeconds);
				else
					moveX = false;
				if (targetY + SpeedError < CurrentY - SpeedError)
					CurrentY -= Math.Abs ((Speed * (float)Math.Sin (TargetAngle)) * (float)time.Elapsed.TotalSeconds);
				else if (targetY - SpeedError > CurrentY + SpeedError)
					CurrentY += Math.Abs ((Speed * (float)Math.Sin (TargetAngle)) * (float)time.Elapsed.TotalSeconds);
				else
					moveY = false;
				return moveX || moveY;
			}
			return false;
		}

		public bool CollidesWithAny () {
			return Colliders.All (collider => !collider.IntersectsWith (Bounds));
		}
	}
}

