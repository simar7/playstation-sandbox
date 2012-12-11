using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

using Sce.PlayStation.Core.Imaging;

namespace SDKTUT2
{
	public class AppMain
	{
			
		public static void Main (string[] args)
		{
			Director.Initialize();
			
			//Initialize ();
			
			Scene scene = new Scene();
			scene.Camera.SetViewFromViewport();
			
			var width = Director.Instance.GL.Context.GetViewport().Width;
			var height = Director.Instance.GL.Context.GetViewport().Height;
			
			/* Color Matrix Values 
			 * Unfortunately C# doesn't allow the regular #define usage,
			 * See: The #define directive cannot be used to declare constant values 
			 * as is typically done in C and C++. Constants in C# are best defined as static members of a 
			 * class or struct. If you have several such constants, consider creating a separate "Constants"
			 * class to hold them.
			 * 
			 */
			const int RED = 255;
			const int BLUE = 0;
			const int GREEN = 0;
			const int ALPHA = 0;
			
			Image img = new Image(ImageMode.Rgba, new ImageSize(width, height), new ImageColor(RED,GREEN,BLUE,ALPHA));
			
			img.DrawText("Hello World", new ImageColor(RED,GREEN,BLUE,ALPHA+255), 
			new Font(FontAlias.System,170,FontStyle.Regular), new ImagePosition(0,150));
			
			Texture2D texture = new Texture2D(width,height,false,PixelFormat.Rgba);
			texture.SetPixels(0,img.ToBuffer());
			img.Dispose();
			
			TextureInfo ti = new TextureInfo();
			ti.Texture = texture;
			
			SpriteUV sprite = new SpriteUV();
			sprite.TextureInfo = ti;
			
			sprite.Quad.S = ti.TextureSizef;
			sprite.CenterSprite();
			sprite.Position = scene.Camera.CalcBounds().Center;
			
			scene.AddChild(sprite);
			
			/* Important: The second argument 'true' defines that the Scene processing would be done manually,
			 * So in this case, we would like to do the following ourselves:
			 * Update();	-- Tells us to move to the next frame.
			 * Render();	-- Draws the next frame.
			 * SwapBuffers(); -- Swaps what the Render() drew in the previous frame. {Hence happens after the render}
			 * PostSwap(); -- Tells us that we've finished with SwapBuffer();
			 * */
			
			Director.Instance.RunWithScene(scene, true);
			
			bool gameOver = false;
			
			while(!gameOver)
			{
				Sce.PlayStation.HighLevel.GameEngine2D.Director.Instance.Update();
				// Rotate Left: Preferred Method to Rotate, although not so smooth.
				/*
				if(Input2.GamePad.GetData(0).Left.Release)
				{
					sprite.Rotate(Sce.PlayStation.HighLevel.GameEngine2D.Base.Math.Deg2Rad(1));
				}
				*/
				
				/*
				 * AWWx1 to do rotation per interrupt.
				 if(Input2.GamePad0.Left.Release)
				 {
				 	sprite.Rotate(Sce.PlayStation.HighLevel.GameEngine2D.Base.Math.Deg2Rad(90));
				 }
				 */ 
				
				// AWWx2 to do rotation per interrupt, with bitwise comparison.
				// Interesting Fact, doing a bitwise comparison does a continous rendering. So doing it 1 degree at a time smoothens it out.
				// Smoothness = 1/InputDegree.
				if((Sce.PlayStation.Core.Input.GamePad.GetData(0).Buttons & GamePadButtons.Left) == GamePadButtons.Left)
				{
					sprite.Rotate(Sce.PlayStation.HighLevel.GameEngine2D.Base.Math.Deg2Rad(1.0f));
				}
				
				
				// Rotate Right
				// Notice that this is NOT continous as compared to the Left Rotate.
				if(Input2.GamePad.GetData(0).Right.Release)
				{
					sprite.Rotate(Sce.PlayStation.HighLevel.GameEngine2D.Base.Math.Deg2Rad(-90));
				}
				
				// Scale Up by a factor of 1/10th.
				if((Sce.PlayStation.Core.Input.GamePad.GetData(0).Buttons & GamePadButtons.Up) == GamePadButtons.Up)
				{
					sprite.Quad.S = new Vector2(sprite.Quad.S.X += 10.0f, sprite.Quad.S.Y += 10.0f);
					sprite.CenterSprite();
				}
				
				// Scale Down by a factor of 1/10th.
				if((Sce.PlayStation.Core.Input.GamePad.GetData(0).Buttons & GamePadButtons.Down) == GamePadButtons.Down)
				{
					sprite.Quad.S = new Vector2(sprite.Quad.S.X -= 10.0f, sprite.Quad.S.Y -= 10.0f);
					sprite.CenterSprite();
				}
				
				// Exit if Circle is Pressed.
			 	if(Input2.GamePad0.Circle.Press == true)
				{
					gameOver = true;
				}
				
				Sce.PlayStation.HighLevel.GameEngine2D.Director.Instance.Render();
				Sce.PlayStation.HighLevel.GameEngine2D.Director.Instance.GL.Context.SwapBuffers();
				Sce.PlayStation.HighLevel.GameEngine2D.Director.Instance.PostSwap();
			}
			
			Director.Terminate();
		}

	}
}
