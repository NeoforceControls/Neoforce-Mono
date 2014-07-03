////////////////////////////////////////////////////////////////
//                                                            //
//  Neoforce Controls                                         //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//         File: Types.cs                                     //
//                                                            //
//      Version: 0.7                                          //
//                                                            //
//         Date: 11/09/2010                                   //
//                                                            //
//       Author: Tom Shane                                    //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//  Copyright (c) by Tom Shane                                //
//                                                            //
////////////////////////////////////////////////////////////////

#region //// Using /////////////

////////////////////////////////////////////////////////////////////////////
using System;
////////////////////////////////////////////////////////////////////////////

#endregion

namespace TomShane.Neoforce.Controls
{

  #region //// Enums /////////////

  ////////////////////////////////////////////////////////////////////////////  
  public enum Message
  {
    Click,    
    MouseDown,
    MouseUp,
    MousePress,
    MouseMove,
    MouseOver,
    MouseOut,
    MouseScroll,
    KeyDown,
    KeyUp,
    KeyPress,
    GamePadDown,
    GamePadUp,
    GamePadPress    
  }
  ////////////////////////////////////////////////////////////////////////////        

  ////////////////////////////////////////////////////////////////////////////
  public enum ControlState
  {    
    Enabled,
    Hovered,
    Pressed,
    Focused,
    Disabled
  }
  ////////////////////////////////////////////////////////////////////////////  
  
  ////////////////////////////////////////////////////////////////////////////
  public enum Alignment
  {
    None,
    TopLeft,
    TopCenter,
    TopRight,
    MiddleLeft,
    MiddleCenter,
    MiddleRight,
    BottomLeft,
    BottomCenter,
    BottomRight
  }
  ////////////////////////////////////////////////////////////////////////////    
  
  ////////////////////////////////////////////////////////////////////////////  
  public enum ModalResult
  {
    None,
    Ok,
    Cancel,
    Yes,
    No,
    Abort,
    Retry,
    Ignore
  }
  ////////////////////////////////////////////////////////////////////////////   

  ////////////////////////////////////////////////////////////////////////////   
  public enum Orientation
  {
    Horizontal,
    Vertical
  }
  ////////////////////////////////////////////////////////////////////////////  

  ////////////////////////////////////////////////////////////////////////////
  public enum ScrollBars
  {
    None,
    Vertical,
    Horizontal,
    Both
  }
  //////////////////////////////////////////////////////////////////////////// 
  
  ////////////////////////////////////////////////////////////////////////////  
  [Flags]
  public enum Anchors
  {    
    None = 0x00,
    Left = 0x01,
    Top = 0x02,
    Right = 0x04,
    Bottom = 0x08,
    Horizontal = Left | Right,
    Vertical = Top | Bottom,
    All = Left | Top | Right | Bottom
  }
  ////////////////////////////////////////////////////////////////////////////  
  
  #endregion

  #region //// Structs ///////////
  
  ////////////////////////////////////////////////////////////////////////////
  public struct Margins
  {
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;   
    
    public int Vertical { get { return (Top + Bottom); } }
    public int Horizontal { get { return (Left + Right); } }
    
    public Margins(int left, int top, int right, int bottom)
    {
      Left = left;
      Top = top;
      Right = right;
      Bottom = bottom;
    }         
  }
  ////////////////////////////////////////////////////////////////////////////

  ////////////////////////////////////////////////////////////////////////////
  public struct Size
  {
    public int Width;
    public int Height;
    
    public Size(int width, int height)
    {
      Width = width;
      Height = height;
    }
    
    public static Size Zero
    {
      get 
      {
        return new Size(0, 0);
      }
    }
  }
  //////////////////////////////////////////////////////////////////////////// 
  
  #endregion    
  
}
  