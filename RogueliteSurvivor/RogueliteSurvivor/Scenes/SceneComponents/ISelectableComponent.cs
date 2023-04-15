using Microsoft.Xna.Framework.Input;

namespace RogueliteSurvivor.Scenes.SceneComponents
{
    public interface ISelectableComponent
    {
        bool Selected { get; set; }
        bool MouseOver();
        void MouseOver(MouseState mState);
    }
}
