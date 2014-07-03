////////////////////////////////////////////////////////////////
//                                                            //
//  Neoforce Controls                                         //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//         File: RadioButton.cs                               //
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
using Microsoft.Xna.Framework;
using System.Collections.Generic;
////////////////////////////////////////////////////////////////////////////

#endregion

namespace TomShane.Neoforce.Controls
{

  #region //// Enums /////////////

  ////////////////////////////////////////////////////////////////////////////               
  public enum RadioButtonMode
  {
    Auto,
    Manual
  }
  ////////////////////////////////////////////////////////////////////////////

 #endregion

  public class RadioButton: CheckBox
  {

    #region //// Consts ////////////

    ////////////////////////////////////////////////////////////////////////////
    private const string skRadioButton = "RadioButton";
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Fields ////////////

    ////////////////////////////////////////////////////////////////////////////               
    private RadioButtonMode mode = RadioButtonMode.Auto;  
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Properties ////////
    
    ////////////////////////////////////////////////////////////////////////////
    public RadioButtonMode Mode
    {
      get { return mode; }
      set { mode = value; }
    }
    ////////////////////////////////////////////////////////////////////////////       

    #endregion

    #region //// Events ////////////

    ////////////////////////////////////////////////////////////////////////////                         
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Construstors //////

    ////////////////////////////////////////////////////////////////////////////       
    public RadioButton(Manager manager): base(manager)
    {
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
    protected internal override void InitSkin()
    {
      base.InitSkin();
      Skin = new SkinControl(Manager.Skin.Controls[skRadioButton]);
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    protected override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
    {
      base.DrawControl(renderer, rect, gameTime);
    }
    ////////////////////////////////////////////////////////////////////////////     

    ////////////////////////////////////////////////////////////////////////////     
    protected override void OnClick(EventArgs e)
    {
      MouseEventArgs ex = (e is MouseEventArgs) ? (MouseEventArgs)e : new MouseEventArgs();      
      
      if (ex.Button == MouseButton.Left || ex.Button == MouseButton.None)      
      {      
        if (mode == RadioButtonMode.Auto)
        {
          if (Parent != null)
          {
            ControlsList lst = Parent.Controls as ControlsList;
            for (int i = 0; i < lst.Count; i++)
            {
              if (lst[i] is RadioButton)
              {
                (lst[i] as RadioButton).Checked = false;
              }
            }  
          }
          else if (Parent == null && Manager != null)
          {
            ControlsList lst = Manager.Controls as ControlsList;
            
            for (int i = 0; i < lst.Count; i++)
            {              
              if (lst[i] is RadioButton)
              {
                (lst[i] as RadioButton).Checked = false;
              }
            }  
          }  
        }  
      }              
      base.OnClick(e);
    }
    ////////////////////////////////////////////////////////////////////////////          

    #endregion

  }

}
