using Godot;
using System;

public enum ResizeHandleType
{
    Top,
    Bottom,
    Left,
    Right
}

public class ResizeHandle : Control
{
    [Export]
    private ResizeHandleType handleType;
    private bool isResizing = false;
    private Vector2 resizePos = Vector2.Zero;
    private Vector2 startSize = Vector2.Zero;

    public override void _Ready()
    {
        switch(handleType)
        {
            case ResizeHandleType.Top:
            case ResizeHandleType.Bottom:
                MouseDefaultCursorShape = CursorShape.Vsize;
                break;
            case ResizeHandleType.Left:
            case ResizeHandleType.Right:
                MouseDefaultCursorShape = CursorShape.Hsize;
                break;
        }
    }

    public override void _GuiInput(InputEvent ev)
    {
        if(ev is InputEventMouseButton button)
        {
            if(button.ButtonIndex == (int)ButtonList.Left)
            {
                if(button.Pressed && !isResizing) {
                    isResizing = true;
                    startSize = OS.WindowSize;
                    resizePos = GetGlobalMousePosition();
                }
                else
                    isResizing = false;
            }
        }
    }

    public override void _Process(float delta)
    {
        if(isResizing)
        {
            var mousePos = GetGlobalMousePosition();
            switch(handleType)
            {
                case ResizeHandleType.Top:
                {
                    var newPos = OS.WindowPosition + mousePos - resizePos;
                    newPos.x = OS.WindowPosition.x;
                    var deltaPos = OS.WindowPosition - newPos;
                    startSize += new Vector2(0.0f, deltaPos.y);
                    OS.WindowPosition = newPos;
                    OS.WindowSize = startSize;
                    break;
                }
                case ResizeHandleType.Bottom:
                    OS.WindowSize = startSize + new Vector2(0.0f, mousePos.y - resizePos.y);
                    break;
                case ResizeHandleType.Left:
                {
                    var newPos = OS.WindowPosition + mousePos - resizePos;
                    newPos.y = OS.WindowPosition.y;
                    var deltaPos = OS.WindowPosition - newPos;
                    startSize += new Vector2(deltaPos.x, 0.0f);
                    OS.WindowPosition = newPos;
                    OS.WindowSize = startSize;
                    break;
                }
                case ResizeHandleType.Right:
                    OS.WindowSize = startSize + new Vector2(mousePos.x - resizePos.x, 0.0f);
                    break;
            }
        }
    }
}