using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueliteSurvivor.Scenes.SceneComponents
{
    public interface IFormComponent
    {
        bool Selected();
        void Selected(bool selected);
        bool MouseOver();
        void MouseOver(MouseState mState);
        bool Visible();
        void Visible(bool visible);
        void Draw(SpriteBatch spriteBatch);
    }
}
