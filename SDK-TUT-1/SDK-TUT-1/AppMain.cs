using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

using Sce.PlayStation.Core.Imaging;

namespace SDKTUT1
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
			
			Director.Instance.RunWithScene(scene);

	
		}

	}
}
