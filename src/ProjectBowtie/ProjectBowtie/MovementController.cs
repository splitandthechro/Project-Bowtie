using System;
using System.Collections.Generic;
using System.Drawing;
using nginz;
using OpenTK;
using System.Linq;

namespace ProjectBowtie
{
	public class MovementController
	{
		public int Width;
		public int Height;
		public Rectangle CollisionBounds {
			get {
				var rect = new Rectangle (
					x: (int) CurrentX - (Width / 2),
					y: (int) CurrentY - (Height / 2),
					width: Width,
					height: Height
				);
				rect.Inflate (-20, -20);
				return rect;
			}
		}

		const float SpeedError = 5f;
		public List<Rectangle> Colliders;
		public bool AvoidCollision = false;
		public bool DisableCollision = false;
		public float CurrentX;
		public float CurrentY;
		public float Rotation;
		public float Speed;
		public Vector2 Position;
		public Vector2 TargetLocation;

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
				if (DisableCollision || !CollidesWithAny ())
					Position = new Vector2 (CurrentX, CurrentY);
				return moveX || moveY;
			}
			return false;
		}

		public bool CollidesWithAny () {
			return Colliders.Any (collider => collider.IntersectsWith (CollisionBounds));
		}
	}
}

