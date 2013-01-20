using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;
using Sce.PlayStation.HighLevel.Physics2D;

namespace Pong
{
	public class Ball : SpriteUV
	{
		private PhysicsBody _physicsBody;
		// Change this value to make the game faster or slower
		public const float BALL_VELOCITY = 5.0f;
		
		public Ball (PhysicsBody physicsBody)
		{
			_physicsBody = physicsBody;
			
			this.TextureInfo = new TextureInfo(new Texture2D("Application/images/ball.png",false));
			this.Scale = this.TextureInfo.TextureSizef;
			this.Pivot = new Sce.PlayStation.Core.Vector2(0.5f,0.5f);
			this.Position = new Sce.PlayStation.Core.Vector2(
				Director.Instance.GL.Context.GetViewport().Width/2 -Scale.X/2,
				Director.Instance.GL.Context.GetViewport().Height/2 -Scale.Y/2);
			
			
			//Right angles are exceedingly boring, so make sure we dont start on one
			//So if our Random angle is between 90 +- 25 degrees or 270 +- 25 degrees
			//we add 25 degree to value, ie, making 90 into 115 instead
			System.Random rand = new System.Random();
			float angle = (float)rand.Next(0,360);
		
			if((angle%90) <=25) angle +=25.0f;
			this._physicsBody.Velocity = new Vector2(0.0f,BALL_VELOCITY).Rotate(PhysicsUtility.GetRadian(angle));;
			
			Scheduler.Instance.ScheduleUpdateForTarget(this,0,false);
		}
		
		public override void Update (float dt)
		{

			this.Position = _physicsBody.Position * PongPhysics.PtoM;

			
			// We want to prevent situations where the balls is bouncing side to side
			// so if there isnt a certain amount of movement on the Y axis, set it to + or - 0.2 randomly
			// Note, this can result in the ball bouncing "back", as in it comes from the top of the screen
			// But riccochets back up at the user.  Frankly, this keeps things interesting imho
			var normalizedVel = _physicsBody.Velocity.Normalize();
			if(System.Math.Abs (normalizedVel.Y) < 0.2f) 
			{
				System.Random rand = new System.Random();
				if(rand.Next (0,1) == 0)
					normalizedVel.Y+= 0.2f;
				
				else
					normalizedVel.Y-= 0.2f;
			}
			
			// Pong is a mess with physics, so just fix the ball velocity
			// Otherwise the ball could get faster and faster ( or slower ) on each collision
			_physicsBody.Velocity = normalizedVel * BALL_VELOCITY;

		}
		
		~Ball()
		{
			this.TextureInfo.Texture.Dispose();
			this.TextureInfo.Dispose();
		}
	}
}

