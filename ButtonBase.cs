////////////////////////////////////////////////////////////////
//                                                            //
//  Neoforce Controls                                         //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//         File: ButtonBase.cs                                //
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
////////////////////////////////////////////////////////////////////////////

#endregion

namespace TomShane.Neoforce.Controls
{

  #region //// Classes ///////////

  ////////////////////////////////////////////////////////////////////////////
  ///  <include file='Documents/ButtonBase.xml' path='ButtonBase/Class[@name="ButtonBase"]/*' />          
  public abstract class ButtonBase: Control
  {

    #region //// Fields ////////////

    ////////////////////////////////////////////////////////////////////////////                  
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Properties ////////

    ////////////////////////////////////////////////////////////////////////////    
    public override ControlState ControlState
    {
      get
      {
        if (DesignMode) return ControlState.Enabled;
        else if (Suspended) return ControlState.Disabled;
        else
        {
          if (!Enabled) return ControlState.Disabled;

          if ((Pressed[(int)MouseButton.Left] && Inside) || (Focused && (Pressed[(int)GamePadActions.Press] || Pressed[(int)MouseButton.None]))) return ControlState.Pressed;
          else if (Hovered && Inside) return ControlState.Hovered;
          else if ((Focused && !Inside) || (Hovered && !Inside) || (Focused && !Hovered && Inside)) return ControlState.Focused;
          else return ControlState.Enabled;
        }
      }
    }    
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Events ////////////

    ////////////////////////////////////////////////////////////////////////////   
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Construstors //////

    ////////////////////////////////////////////////////////////////////////////       
    protected ButtonBase(Manager manager)
      : base(manager)
    {
      SetDefaultSize(72, 24);
      DoubleClicks = false;
    }
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Methods ///////////

    ////////////////////////////////////////////////////////////////////////////
    public override void Init()
    {
      base.Init();
    }
    ////////////////////////////////////////////////////////////////////////////                                 

    ////////////////////////////////////////////////////////////////////////////
    protected override void OnClick(EventArgs e)
    {
      MouseEventArgs ex = (e is MouseEventArgs) ? (MouseEventArgs)e : new MouseEventArgs();      
      if (ex.Button == MouseButton.Left || ex.Button == MouseButton.None)
      {
        base.OnClick(e);
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    #endregion

  }

  ////////////////////////////////////////////////////////////////////////////

  #endregion
  
}
