using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System;

namespace RogueliteSurvivor.Scenes.SceneComponents
{
    public class LinkLabel : IFormComponent, IClickableComponent
    {
        public string Name { get; set; }
        public SpriteFont Font { get; set; }
        public string Text { get; set; }
        public Vector2 Position { get; set; }
        public Color ForeColor { get; set; }
        public bool Visible { get; set; }
        public string Link { get; set; }

        bool mouseOver = false;
        Rectangle labelArea;

        public LinkLabel(string name, SpriteFont font, string text, Vector2 position, Color foreColor, string link, bool visible = true)
        {
            Name = name;
            Font = font;
            Text = text;
            Position = position;
            ForeColor = foreColor;
            Visible = visible;
            Link = link;

            labelArea = new Rectangle((position * Game1.ScaleFactor).ToPoint(), (Font.MeasureString(text) * Game1.ScaleFactor).ToPoint());
        }

        public bool MouseOver()
        {
            return mouseOver;
        }
        public void MouseOver(MouseState mState)
        {
            mouseOver = labelArea.Contains(mState.Position);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                spriteBatch.DrawString(
                    Font,
                    Text,
                    Position,
                    ForeColor
                );
            }
        }

        public void GoToLink()
        {
            try
            {
                Process.Start(Link);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    string url = Link.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", Link);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", Link);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
