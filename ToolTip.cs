////////////////////////////////////////////////////////////////
//                                                            //
//  Neoforce Controls                                         //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//         File: ToolTip.cs                                   //
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
using Microsoft.Xna.Framework.Input;
////////////////////////////////////////////////////////////////////////////

#endregion

namespace TomShane.Neoforce.Controls
{  
  public class ToolTip: Control
  {

    #region //// Fields ////////////

    ////////////////////////////////////////////////////////////////////////////                     
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Properties ////////

    ////////////////////////////////////////////////////////////////////////////
    public override bool Visible
    {
      set
      {
        if (value && Text != null && Text != "" && Skin != null && Skin.Layers[0] != null)
        {
          Vector2 size = Skin.Layers[0].Text.Font.Resource.MeasureString(Text);
          Width = (int)size.X + Skin.Layers[0].ContentMargins.Horizontal;
          Height = (int)size.Y + Skin.Layers[0].ContentMargins.Vertical;
          Left = Mouse.GetState().X;
          Top = Mouse.GetState().Y + 24;
          base.Visible = value;
        }
        else
        {
          base.Visible = false;
        }
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Construstors //////

    ////////////////////////////////////////////////////////////////////////////       
    public ToolTip(Manager manager): base(manager)
    {  
      Text = "";          
    }    
    ////////////////////////////////////////////////////////////////////////////

    #endregion
    
    #region //// Methods ///////////
    
    ////////////////////////////////////////////////////////////////////////////
    public override void Init()
    {      
      base.Init();
      CanFocus = false;
      Passive = true;          
    }
    ////////////////////////////////////////////////////////////////////////////                          

    ////////////////////////////////////////////////////////////////////////////                          
    protected internal override void InitSkin()
    {
      base.InitSkin();
      Skin = Manager.Skin.Controls["ToolTip"];
    }
    ////////////////////////////////////////////////////////////////////////////                          
    
    ////////////////////////////////////////////////////////////////////////////
    protected override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
    {                      
      renderer.DrawLayer(this, Skin.Layers[0], rect);
      renderer.DrawString(this, Skin.Layers[0], Text, rect, true);
    }
    ////////////////////////////////////////////////////////////////////////////     

    #endregion  

  }

}
