using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Audio;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;
using Sce.PlayStation.Core.Input;
 
namespace Pong
{
	public class TitleScene : Scene
	{
		private TextureInfo _ti;
		private Texture2D _texture;
		
		private Bgm _titleSong;
		private BgmPlayer _songPlayer;
		
		public TitleScene ()
		{
			this.Camera.SetViewFromViewport();
			_texture = new Texture2D("Application/images/title.png",false);
			_ti = new TextureInfo(_texture);
			SpriteUV titleScreen = new SpriteUV(_ti);
			titleScreen.Scale = _ti.TextureSizef;
			titleScreen.Pivot = new Vector2(0.5f,0.5f);
			titleScreen.Position = new Vector2(Director.Instance.GL.Context.GetViewport().Width/2,
			                                  Director.Instance.GL.Context.GetViewport().Height/2);
			this.AddChild(titleScreen);
			
			Vector4 origColor = titleScreen.Color;
			titleScreen.Color = new Vector4(0,0,0,0);
			var tintAction = new TintTo(origColor,10.0f);
			ActionManager.Instance.AddAction(tintAction,titleScreen);
			tintAction.Run();
			
			_titleSong = new Bgm("/Application/audio/titlesong.mp3");
			
			if(_songPlayer != null)
			_songPlayer.Dispose();
			_songPlayer = _titleSong.CreatePlayer();
			
			Scheduler.Instance.ScheduleUpdateForTarget(this,0,false);

			// Clear any queued clicks so we dont immediately exit if coming in from the menu
			Touch.GetData(0).Clear();
		}
		
		public override void OnEnter ()
		{
			_songPlayer.Loop = true;
			_songPlayer.Play();
		}
		public override void OnExit ()
		{
			base.OnExit ();
			_songPlayer.Stop();
			_songPlayer.Dispose();
			_songPlayer = null;
		}
		
		public override void Update (float dt)
		{
			base.Update (dt);
			var touches = Touch.GetData(0).ToArray();
			if((touches.Length >0 && touches[0].Status == TouchStatus.Down) || Input2.GamePad0.Cross.Press)
			{
				Director.Instance.ReplaceScene(new MenuScene());
			}
		}
	
		~TitleScene()
		{
			_texture.Dispose();
			_ti.Dispose ();
		}
	}
}
 