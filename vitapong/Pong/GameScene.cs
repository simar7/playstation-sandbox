using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;
using Sce.PlayStation.HighLevel.Physics2D;
using Sce.PlayStation.Core.Audio;

namespace Pong
{
	public class GameScene : Scene
	{
	private Paddle _player,_ai;
	public static Ball ball;
	private PongPhysics _physics;
	private Scoreboard _scoreboard;
	private SoundPlayer _pongBlipSoundPlayer;
	private Sound _pongSound;
		
		// Change the following value to true if you want bounding boxes to be rendered
		private static Boolean DEBUG_BOUNDINGBOXS = false;
		
		public GameScene ()
		{
			this.Camera.SetViewFromViewport();
			_physics = new PongPhysics();

			
			ball = new Ball(_physics.SceneBodies[(int)PongPhysics.BODIES.Ball]);
			_player = new Paddle(Paddle.PaddleType.PLAYER, 
			                     _physics.SceneBodies[(int)PongPhysics.BODIES.Player]);
			_ai = new Paddle(Paddle.PaddleType.AI, 
			                 _physics.SceneBodies[(int)PongPhysics.BODIES.Ai]);
			_scoreboard = new Scoreboard();
			
			this.AddChild(_scoreboard);
			this.AddChild(ball);
			this.AddChild(_player);
			this.AddChild(_ai);
			
			
			// This is debug routine that will draw the physics bounding box around the players paddle
			if(DEBUG_BOUNDINGBOXS)
			{
				this.AdHocDraw += () => {
					var bottomLeftPlayer = _physics.SceneBodies[(int)PongPhysics.BODIES.Player].AabbMin;
					var topRightPlayer = _physics.SceneBodies[(int)PongPhysics.BODIES.Player].AabbMax;
					Director.Instance.DrawHelpers.DrawBounds2Fill(
						new Bounds2(bottomLeftPlayer*PongPhysics.PtoM,topRightPlayer*PongPhysics.PtoM));

					var bottomLeftAi = _physics.SceneBodies[(int)PongPhysics.BODIES.Ai].AabbMin;
					var topRightAi = _physics.SceneBodies[(int)PongPhysics.BODIES.Ai].AabbMax;
					Director.Instance.DrawHelpers.DrawBounds2Fill(
						new Bounds2(bottomLeftAi*PongPhysics.PtoM,topRightAi*PongPhysics.PtoM));

					var bottomLeftBall = _physics.SceneBodies[(int)PongPhysics.BODIES.Ball].AabbMin;
					var topRightBall = _physics.SceneBodies[(int)PongPhysics.BODIES.Ball].AabbMax;
					Director.Instance.DrawHelpers.DrawBounds2Fill(
						new Bounds2(bottomLeftBall*PongPhysics.PtoM,topRightBall*PongPhysics.PtoM));
				};
			}
			
			//Now load the sound fx and create a player
			_pongSound = new Sound("/Application/audio/pongblip.wav");
			_pongBlipSoundPlayer = _pongSound.CreatePlayer();
			
			Scheduler.Instance.ScheduleUpdateForTarget(this,0,false);
		}
		
		private void ResetBall()
		{
			//Move ball to screen center and release in a random directory
			_physics.SceneBodies[(int)PongPhysics.BODIES.Ball].Position = 
				new Vector2(Director.Instance.GL.Context.GetViewport().Width/2,
				            Director.Instance.GL.Context.GetViewport().Height/2) / PongPhysics.PtoM;
			
			System.Random rand = new System.Random();
			float angle = (float)rand.Next(0,360);
		
			if((angle%90) <=15) angle +=15.0f;
		
			_physics.SceneBodies[(int)PongPhysics.BODIES.Ball].Velocity = 
				new Vector2(0.0f,5.0f).Rotate(PhysicsUtility.GetRadian(angle));
		}
		
		public override void Update (float dt)
		{
			base.Update (dt);
			
			if(Input2.GamePad0.Select.Press)
				Director.Instance.ReplaceScene(new MenuScene());
			
			//We don't need these, but sadly, the Simulate call does.
			Vector2 dummy1 = new Vector2();
			Vector2 dummy2 = new Vector2();
			
			//Update the physics simulation
			_physics.Simulate(-1,ref dummy1,ref dummy2);
			
			//Now check if the ball it either paddle, and if so, play the sound
			if(_physics.QueryContact((uint)PongPhysics.BODIES.Ball,(uint)PongPhysics.BODIES.Player) ||
				_physics.QueryContact((uint)PongPhysics.BODIES.Ball,(uint)PongPhysics.BODIES.Ai))
			{
				if(_pongBlipSoundPlayer.Status == SoundStatus.Stopped)
					_pongBlipSoundPlayer.Play();
			}
			
			//Check if the ball went off the top or bottom of the screen and update score accordingly
			Results result = Results.StillPlaying;
			bool scored = false;
			
			if(ball.Position.Y > Director.Instance.GL.Context.GetViewport().Height + ball.Scale.Y/2)
			{
				result = _scoreboard.AddScore(true);
				scored = true;
			}
			if(ball.Position.Y < 0 - ball.Scale.Y/2)
			{
				result =_scoreboard.AddScore(false);
				scored = true;
			}
			
			// Did someone win?  If so, show the GameOver scene
			if(result == Results.AiWin) 
				Director.Instance.ReplaceScene(new GameOverScene(false));
			if(result == Results.PlayerWin) 
				Director.Instance.ReplaceScene(new GameOverScene(true));
			
			//If someone did score, but game isn't over, reset the ball position to the middle of the screen
			if(scored == true)
			{
				ResetBall ();
			}
			
			//Finally a sanity check to make sure the ball didn't leave the field.
			var ballPB = _physics.SceneBodies[(int)PongPhysics.BODIES.Ball];
			
			if(ballPB.Position.X < -(ball.Scale.X/2f)/PongPhysics.PtoM ||
			   ballPB.Position.X > (Director.Instance.GL.Context.GetViewport().Width)/PongPhysics.PtoM)
			{
				ResetBall();
			}
		}
		
		~GameScene(){
			_pongBlipSoundPlayer.Dispose();
		}
	}
}

