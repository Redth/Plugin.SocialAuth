using System;
namespace Plugin.SocialAuth
{
	public class ImageSize
	{
		public ImageSize ()
		{
			Width = 256;
			Height = 256;
		}

		public ImageSize(int width, int height)
		{
			Width = width;
			Height = height;
		}

		public int Width { get;set; } = 256;
		public int Height { get;set; } = 256;

		public bool IsSquare {
			get {
				return Width == Height;
			}
		}
	}
}
