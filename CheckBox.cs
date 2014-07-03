////////////////////////////////////////////////////////////////
//                                                            //
//  Neoforce Controls                                         //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//         File: CheckBox.cs                                  //
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
////////////////////////////////////////////////////////////////////////////

#endregion

namespace TomShane.Neoforce.Controls
{ 

  public class CheckBox: ButtonBase
  {

    #region //// Consts ////////////

    ////////////////////////////////////////////////////////////////////////////
    private const string skCheckBox = "CheckBox";
    private const string lrCheckBox = "Control";
    private const string lrChecked  = "Checked";   
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Fields ////////////

    ////////////////////////////////////////////////////////////////////////////               
    private bool state = false;
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Properties ////////

    ////////////////////////////////////////////////////////////////////////////
    public virtual bool Checked 
    { 
      get 
      { 
        return state; 
      } 
      set 
      { 
        state = value; 
        Invalidate();
        if (!Suspended) OnCheckedChanged(new EventArgs());
      } 
    }   
    ////////////////////////////////////////////////////////////////////////////

    #endregion
    
    #region //// Events ////////////

    ////////////////////////////////////////////////////////////////////////////                     
    public event EventHandler CheckedChanged;
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Construstors //////

    ////////////////////////////////////////////////////////////////////////////       
    public CheckBox(Manager manager): base(manager)
    {
      CheckLayer(Skin, lrChecked);      

      Width = 64;
      Height = 16;           
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
      Skin = new SkinControl(Manager.Skin.Controls[skCheckBox]);
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////
    protected override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
    {      
      SkinLayer layer = Skin.Layers[lrChecked];
      SkinText font = Skin.Layers[lrChecked].Text;      
      
      if (!state)
      {
        layer = Skin.Layers[lrCheckBox];
        font = Skin.Layers[lrCheckBox].Text;      
      }
            
      rect.Width = layer.Width;
      rect.Height = layer.Height;      
      Rectangle rc = new Rectangle(rect.Left + rect.Width + 4, rect.Y,  Width - (layer.Width + 4), rect.Height);
      
      renderer.DrawLayer(this, layer, rect);            
      renderer.DrawString(this, layer, Text, rc, false, 0, 0);
    }
    ////////////////////////////////////////////////////////////////////////////     

    ////////////////////////////////////////////////////////////////////////////     
    protected override void OnClick(EventArgs e)    
    {
      MouseEventArgs ex = (e is MouseEventArgs) ? (MouseEventArgs)e : new MouseEventArgs();      
      
      if (ex.Button == MouseButton.Left || ex.Button == MouseButton.None)      
      {        
        Checked = !Checked;
      }  
      base.OnClick(e);
    }
    ////////////////////////////////////////////////////////////////////////////     
    
    ////////////////////////////////////////////////////////////////////////////     
    protected virtual void OnCheckedChanged(EventArgs e)
    {
      if (CheckedChanged != null) CheckedChanged.Invoke(this, e);
    }
    ////////////////////////////////////////////////////////////////////////////     
    

    #endregion  

  }

}
