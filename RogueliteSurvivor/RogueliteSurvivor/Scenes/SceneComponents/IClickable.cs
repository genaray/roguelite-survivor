﻿using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueliteSurvivor.Scenes.SceneComponents
{
    public interface IClickableComponent : IComponent
    {
        bool MouseOver();
        void MouseOver(MouseState mState);
    }
}