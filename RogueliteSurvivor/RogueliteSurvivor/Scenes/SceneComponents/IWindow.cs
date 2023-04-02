using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueliteSurvivor.Scenes.SceneComponents
{
    public interface IWindow
    {
        string Update(GameTime gameTime, params object[] values);
        void Draw(SpriteBatch spriteBatch);
    }
}
