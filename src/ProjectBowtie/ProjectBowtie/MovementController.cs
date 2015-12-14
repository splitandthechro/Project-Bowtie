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
				// Bounds at 0 deg rotation
				var rect0 = new Rectangle (
					x: (int) (CurrentX - (Width / 2f)),
					y: (int) (CurrentY - (Height / 2f)),
					width: Width,
					height: Height
				);
				// Bounds at 90 deg rotation
				var rect90 = new Rectangle (
					x: (int) (CurrentX - (Height / 2f)),
					y: (int) (CurrentY - (Width / 2f)),
					width: Height,
					height: Width
				);
				// 50% blended bounds
				var blend = new Rectangle (
					x: (rect0.X / 2) + (rect90.X / 2),
					y: (rect0.Y / 2) + (rect90.Y / 2),
					width: (rect0.Width / 2) + (rect90.Width / 2),
					height: (rect0.Height / 2) + (rect90.Height / 2)
				);
				// Shrink factor
				var shrinkFactor = (int) (((Width / 2f) + (Height / 2f)) / 6f);
				// Shrink rectangle by shrink
				blend.Inflate (-shrinkFactor, -shrinkFactor);
				return blend;
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

