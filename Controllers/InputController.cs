using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

public static class InputController
{
    public static float X {
        get {
            var pad = GamePad.GetState(PlayerIndex.One);
            var kb = Keyboard.GetState();

            float retVal = 0;
            if (pad != null) retVal += pad.ThumbSticks.Left.X;
            if (kb.IsKeyDown(Keys.A)) retVal -= 1;
            if (kb.IsKeyDown(Keys.D)) retVal += 1;
            return retVal;        
        }
    }
    public static float Y {
        get {
            var pad = GamePad.GetState(PlayerIndex.One);
            var kb = Keyboard.GetState();

            float retVal = 0;
            if (pad != null) retVal += pad.ThumbSticks.Left.Y;
            if (kb.IsKeyDown(Keys.S)) retVal -= 1;
            if (kb.IsKeyDown(Keys.W)) retVal += 1;
            return retVal;    
        }
    }
    public static bool Jump {
        get {
            var pad = GamePad.GetState(PlayerIndex.One);
            var kb = Keyboard.GetState();

            return (pad != null && pad.Buttons.A == ButtonState.Pressed || kb != null && kb.IsKeyDown(Keys.Space));
        }
    }
}