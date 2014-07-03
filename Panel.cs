////////////////////////////////////////////////////////////////
//                                                            //
//  Neoforce Controls                                         //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//         File: Panel.cs                                     //
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
using Microsoft.Xna.Framework.Graphics;
////////////////////////////////////////////////////////////////////////////

#endregion

namespace TomShane.Neoforce.Controls
{ 

  public class Panel: Container
  {

    #region //// Fields ////////////

    ////////////////////////////////////////////////////////////////////////////                 
    private Bevel bevel = null;
    private BevelStyle bevelStyle = BevelStyle.None;
    private BevelBorder bevelBorder = BevelBorder.None;
    private int bevelMargin = 0;
    private Color bevelColor = Color.Transparent;   
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Properties ////////

    ////////////////////////////////////////////////////////////////////////////
    public BevelStyle BevelStyle
    {
      get { return bevelStyle; }
      set 
      { 
        if (bevelStyle != value)
        {
          bevelStyle = bevel.Style = value; 
          AdjustMargins();
          if (!Suspended) OnBevelStyleChanged(new EventArgs());
        }  
      }
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////
    public BevelBorder BevelBorder
    {
      get { return bevelBorder; }
      set 
      { 
        if (bevelBorder != value)
        {
          bevelBorder = bevel.Border = value; 
          bevel.Visible = bevelBorder != BevelBorder.None; 
          AdjustMargins();
          if (!Suspended) OnBevelBorderChanged(new EventArgs());
        }  
      }
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////
    public int BevelMargin
    {
      get { return bevelMargin; }
      set 
      { 
        if (bevelMargin != value)
        {
          bevelMargin = value; 
          AdjustMargins();
          if (!Suspended) OnBevelMarginChanged(new EventArgs());
        }  
      }
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////
    public virtual Color BevelColor
    {
      get { return bevelColor; }
      set 
      {
        bevel.Color = bevelColor = value; 
      }
    }
    ////////////////////////////////////////////////////////////////////////////
       
    #endregion
    
    #region //// Events ////////////

    ////////////////////////////////////////////////////////////////////////////                 
    public event EventHandler BevelBorderChanged;
    public event EventHandler BevelStyleChanged;
    public event EventHandler BevelMarginChanged;
    ////////////////////////////////////////////////////////////////////////////

    #endregion    

    #region //// Construstors //////

    ////////////////////////////////////////////////////////////////////////////       
    public Panel(Manager manager): base(manager)
    {
      Passive = false;
      CanFocus = false;
      Width = 64;
      Height = 64;
      
      bevel = new Bevel(Manager);                     
    }
    ////////////////////////////////////////////////////////////////////////////    

    #endregion
    
    #region //// Methods ///////////
    
    ////////////////////////////////////////////////////////////////////////////
    public override void Init()
    {      
      base.Init();            
      
      bevel.Init();
      bevel.Style = bevelStyle;
      bevel.Border = bevelBorder;
      bevel.Left = 0;
      bevel.Top = 0;
      bevel.Width = Width;
      bevel.Height = Height;
      bevel.Color = bevelColor;
      bevel.Visible = (bevelBorder != BevelBorder.None);
      bevel.Anchor = Anchors.Left | Anchors.Top | Anchors.Right | Anchors.Bottom;
      Add(bevel, false);      
      AdjustMargins();  
    }
    ////////////////////////////////////////////////////////////////////////////                          
    
    ////////////////////////////////////////////////////////////////////////////
    protected internal override void InitSkin()
    {
      base.InitSkin();
      Skin = new SkinControl(Manager.Skin.Controls["Panel"]);  
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    protected override void AdjustMargins()
    {
      int l = 0;
      int t = 0;
      int r = 0;
      int b = 0;
      int s = bevelMargin;

      if (bevelBorder != BevelBorder.None)
      {
        if (bevelStyle != BevelStyle.Flat)
        {
          s += 2;
        }
        else
        {
          s += 1;
        }

        if (bevelBorder == BevelBorder.Left || bevelBorder == BevelBorder.All)
        {
          l = s;
        }
        if (bevelBorder == BevelBorder.Top || bevelBorder == BevelBorder.All)
        {
          t = s;
        }
        if (bevelBorder == BevelBorder.Right || bevelBorder == BevelBorder.All)
        {
          r = s;
        }
        if (bevelBorder == BevelBorder.Bottom || bevelBorder == BevelBorder.All)
        {
          b = s;
        }
      }
      ClientMargins = new Margins(Skin.ClientMargins.Left + l, Skin.ClientMargins.Top + t, Skin.ClientMargins.Right + r, Skin.ClientMargins.Bottom + b);      
      
      base.AdjustMargins();                        
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////
    protected override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
    {              
      int x = rect.Left;
      int y = rect.Top;
      int w = rect.Width;
      int h = rect.Height;
      int s = bevelMargin;

      if (bevelBorder != BevelBorder.None)
      {      
        if (bevelStyle != BevelStyle.Flat)
        {
          s += 2;
        }
        else 
        {
          s += 1;
        }      
      
        if (bevelBorder == BevelBorder.Left || bevelBorder == BevelBorder.All)
        {
          x += s; 
          w -= s; 
        }
        if (bevelBorder == BevelBorder.Top || bevelBorder == BevelBorder.All)
        {
          y += s;
          h -= s;
        } 
        if (bevelBorder == BevelBorder.Right || bevelBorder == BevelBorder.All)
        {
          w -= s;
        } 
        if (bevelBorder == BevelBorder.Bottom || bevelBorder == BevelBorder.All)
        {
          h -= s;
        }              
      }                  
      
      base.DrawControl(renderer, new Rectangle(x, y, w, h), gameTime);      
    }
    ////////////////////////////////////////////////////////////////////////////          
    
    ////////////////////////////////////////////////////////////////////////////     
    protected virtual void OnBevelBorderChanged(EventArgs e)
    {
      if (BevelBorderChanged != null) BevelBorderChanged.Invoke(this, e);
    }
    ////////////////////////////////////////////////////////////////////////////     

    ////////////////////////////////////////////////////////////////////////////     
    protected virtual void OnBevelStyleChanged(EventArgs e)
    {
      if (BevelStyleChanged != null) BevelStyleChanged.Invoke(this, e);
    }
    ////////////////////////////////////////////////////////////////////////////     

    ////////////////////////////////////////////////////////////////////////////     
    protected virtual void OnBevelMarginChanged(EventArgs e)
    {
      if (BevelMarginChanged != null) BevelMarginChanged.Invoke(this, e);
    }
    ////////////////////////////////////////////////////////////////////////////     

    #endregion  

  }

}
