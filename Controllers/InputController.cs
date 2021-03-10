using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

enum InputType {
    KBM,
    Controller
}


public static class InputController
{
    private static OrthographicCamera _camera;
    private static Vector2 _lastMousePos = new Vector2(0,0);
    private static Vector2 _lastStickAng = new Vector2(0,0);
    private static InputType _lastInput = InputType.KBM;
    private static MouseState LastMouseState;

    private static bool LastEscState = false;

    public static void Setup(OrthographicCamera camera)
    {
        _camera = camera;
        LastMouseState = Mouse.GetState();        
    }

    public static bool LeftClickPressed {
        get {
            var state = Mouse.GetState();
            if (state.LeftButton == ButtonState.Pressed && LastMouseState.LeftButton != ButtonState.Pressed)
            {
                LastMouseState = state;
                return true;
            }
            
            return false;
        }
    }
    public static bool LeftClickReleased {
        get {
            var state = Mouse.GetState();

            if (state.LeftButton == ButtonState.Released && LastMouseState.LeftButton != ButtonState.Released)
            {
                LastMouseState = state;
                return true;
            }

            return false;
        }
    }

    public static bool PausePressed {
        get {
            var kb = Keyboard.GetState();

            if (kb.IsKeyDown(Keys.Escape) && LastEscState == false)
            {
                LastEscState = true;
                return true;
            }

            return false;
        }
    }

    public static bool PauseReleased {
        get {
            var kb = Keyboard.GetState();

            if (kb.IsKeyUp(Keys.Escape) && LastEscState == true)
            {
                LastEscState = false;
                return true;
            }

            return false;
        }
    }


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

    public static Vector2 GetAimNormal(Vector2 relativeTo)
    {
        var pad = GamePad.GetState(PlayerIndex.One);
        var mouse = Mouse.GetState();

        var mousePos = new Vector2(mouse.X, mouse.Y);
        var mouseMoved = mousePos != _lastMousePos;
        _lastMousePos = mousePos;

        var stickVec = pad != null ? new Vector2(pad.ThumbSticks.Right.X, pad.ThumbSticks.Right.Y) : new Vector2(0,0);
        var stickMoved = stickVec != _lastStickAng;
        _lastStickAng = stickVec;

        if (mouseMoved) _lastInput = InputType.KBM;
        if (stickMoved) _lastInput = InputType.Controller;

        if (_lastInput == InputType.KBM) {
            return (_camera.ScreenToWorld(mousePos) - relativeTo).NormalizedCopy();
        } else {
            _lastStickAng = stickVec;
            return stickVec.NormalizedCopy();
        }
    }
}