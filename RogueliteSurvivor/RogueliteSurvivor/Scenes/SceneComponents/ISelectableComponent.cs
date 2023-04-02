using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueliteSurvivor.Scenes.SceneComponents
{
    public interface ISelectableComponent
    {
        bool Selected();
        void Selected(bool selected);
        bool MouseOver();
        void MouseOver(MouseState mState);
    }
}
