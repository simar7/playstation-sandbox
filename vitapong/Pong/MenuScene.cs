using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

using Sce.PlayStation.HighLevel.UI;

namespace Pong
{
	public class MenuScene : Sce.PlayStation.HighLevel.GameEngine2D.Scene
	{
		private Sce.PlayStation.HighLevel.UI.Scene _uiScene;
		
		public MenuScene ()
		{
			this.Camera.SetViewFromViewport();
			Sce.PlayStation.HighLevel.UI.Panel dialog = new Panel();
			dialog.Width = Director.Instance.GL.Context.GetViewport().Width;
			dialog.Height = Director.Instance.GL.Context.GetViewport().Height;
			
			ImageBox ib = new ImageBox();
			ib.Width = dialog.Width;
			ib.Image = new ImageAsset("/Application/images/title.png",false);
			ib.Height = dialog.Height;
			ib.SetPosition(0.0f,0.0f);
			
			Button buttonUI1 = new Button();
			buttonUI1.Name = "buttonPlay";
			buttonUI1.Text = "Play Game";
			buttonUI1.Width = 300;
			buttonUI1.Height = 50;
			buttonUI1.Alpha = 0.8f;
			buttonUI1.SetPosition(dialog.Width/2 - 150,200.0f);
			buttonUI1.TouchEventReceived += (sender, e) => {
			    Director.Instance.ReplaceScene(new GameScene());
			};
			
			Button buttonUI2 = new Button();
			buttonUI2.Name = "buttonMenu";
			buttonUI2.Text = "Main Menu";
			buttonUI2.Width = 300;
			buttonUI2.Height = 50;
			buttonUI2.Alpha = 0.8f;
			buttonUI2.SetPosition(dialog.Width/2 - 150,250.0f);
			buttonUI2.TouchEventReceived += (sender, e) => {
			Director.Instance.ReplaceScene(new TitleScene());
			};		
				
			dialog.AddChildLast(ib);
			dialog.AddChildLast(buttonUI1);
			dialog.AddChildLast(buttonUI2);
			_uiScene = new Sce.PlayStation.HighLevel.UI.Scene();
			_uiScene.RootWidget.AddChildLast(dialog);
			UISystem.SetScene(_uiScene);
			Scheduler.Instance.ScheduleUpdateForTarget(this,0,false);
		}
		public override void Update (float dt)
		{
			base.Update (dt);
			UISystem.Update(Touch.GetData(0));
			
		}
		
		public override void Draw ()
		{
			base.Draw();
			UISystem.Render ();
		}
		
		~MenuScene()
		{
			
		}
	}
}

