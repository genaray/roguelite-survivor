using Microsoft.Xna.Framework.Input;

namespace RogueliteSurvivor.Scenes.SceneComponents
{
    public interface ISelectableComponent : IClickableComponent
    {
        bool Selected { get; set; }
    }
}
