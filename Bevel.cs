////////////////////////////////////////////////////////////////
//                                                            //
//  Neoforce Controls                                         //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//         File: Bevel.cs                                     //
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

using Microsoft.Xna.Framework;
////////////////////////////////////////////////////////////////////////////
using Microsoft.Xna.Framework.Graphics;
////////////////////////////////////////////////////////////////////////////

#endregion

namespace TomShane.Neoforce.Controls
{ 

  #region //// Enums /////////////

  ////////////////////////////////////////////////////////////////////////////                 
  public enum BevelStyle
  {
    None,
    Flat,
    Etched,
    Bumped,
    Lowered,
    Raised
  }
  ////////////////////////////////////////////////////////////////////////////

  ////////////////////////////////////////////////////////////////////////////  
  public enum BevelBorder
  {
    None,
    Left,
    Top,
    Right,
    Bottom,
    All
  }
  ////////////////////////////////////////////////////////////////////////////  

  #endregion

  public class Bevel: Control
  {

    #region //// Fields ////////////

    ////////////////////////////////////////////////////////////////////////////                 
    private BevelBorder border = BevelBorder.All;
    private BevelStyle style = BevelStyle.Etched;   
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Properties ////////

    ////////////////////////////////////////////////////////////////////////////
    public BevelBorder Border
    {
      get { return border; }
      set 
      { 
        if (border != value)
        {
          border = value;
          if (!Suspended) OnBorderChanged(new EventArgs());
        }  
      }
    }
    ////////////////////////////////////////////////////////////////////////////        
    
    ////////////////////////////////////////////////////////////////////////////
    public BevelStyle Style
    {
      get { return style; }
      set
      { 
        if (style != value)
        {
          style = value;
          if (!Suspended) OnStyleChanged(new EventArgs());
        }          
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Events ////////////

    ////////////////////////////////////////////////////////////////////////////                 
    public event EventHandler BorderChanged;
    public event EventHandler StyleChanged;
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Construstors //////

    ////////////////////////////////////////////////////////////////////////////       
    public Bevel(Manager manager): base(manager)
    {            
      CanFocus = false;
      Passive = true;      
      Width = 64;
      Height = 64;
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
    }
    ////////////////////////////////////////////////////////////////////////////                                                        
    
    ////////////////////////////////////////////////////////////////////////////
    protected override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
    {   
      if (Border != BevelBorder.None && Style != BevelStyle.None)
      {           
        if (Border != BevelBorder.All)
        {
          DrawPart(renderer, rect, Border, Style, false);  
        }
        else
        {
          DrawPart(renderer, rect, BevelBorder.Left, Style, true);
          DrawPart(renderer, rect, BevelBorder.Top, Style, true);
          DrawPart(renderer, rect, BevelBorder.Right, Style, true);
          DrawPart(renderer, rect, BevelBorder.Bottom, Style, true);
        }  
      }  
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////     
    private void DrawPart(Renderer renderer, Rectangle rect, BevelBorder pos, BevelStyle style, bool all)
    {                  
      SkinLayer layer = Skin.Layers["Control"];   
      Color c1 = Utilities.ParseColor(layer.Attributes["LightColor"].Value);
      Color c2 = Utilities.ParseColor(layer.Attributes["DarkColor"].Value);
      Color c3 = Utilities.ParseColor(layer.Attributes["FlatColor"].Value); 
      
      if (Color != UndefinedColor) c3 = Color;
      
      Texture2D img = Skin.Layers["Control"].Image.Resource;

      int x1 = 0; int y1 = 0; int w1 = 0; int h1 = 0;
      int x2 = 0; int y2 = 0; int w2 = 0; int h2 = 0;
            
      if (style == BevelStyle.Bumped || style == BevelStyle.Etched)     
      {        
        if (all && (pos == BevelBorder.Top || pos == BevelBorder.Bottom))
        {
          rect = new Rectangle(rect.Left + 1, rect.Top, rect.Width - 2, rect.Height);
        }
        else if (all && (pos == BevelBorder.Left))
        {
          rect = new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height - 1);
        }
        switch (pos)
        {
          case BevelBorder.Left:
          {
            x1 = rect.Left; y1 = rect.Top; w1 = 1; h1 = rect.Height;
            x2 = x1 + 1; y2 = y1; w2 = w1; h2 = h1;
            break;
          }
          case BevelBorder.Top:
          {
            x1 = rect.Left; y1 = rect.Top; w1 = rect.Width; h1 = 1;
            x2 = x1; y2 = y1 + 1; w2 = w1; h2 = h1;
            break;
          }
          case BevelBorder.Right:
          {
            x1 = rect.Left + rect.Width - 2; y1 = rect.Top; w1 = 1; h1 = rect.Height;
            x2 = x1 + 1; y2 = y1; w2 = w1; h2 = h1;
            break;
          }
          case BevelBorder.Bottom:
          {
            x1 = rect.Left; y1 = rect.Top + rect.Height - 2; w1 = rect.Width; h1 = 1;
            x2 = x1; y2 = y1 + 1; w2 = w1; h2 = h1;
            break;
          }
        }                                             
      }
      else
      {
        switch (pos)
        {
          case BevelBorder.Left:
          {
            x1 = rect.Left; y1 = rect.Top; w1 = 1; h1 = rect.Height;            
            break;
          }
          case BevelBorder.Top:
          {
            x1 = rect.Left; y1 = rect.Top; w1 = rect.Width; h1 = 1;            
            break;
          }
          case BevelBorder.Right:
          {
            x1 = rect.Left + rect.Width - 1; y1 = rect.Top; w1 = 1; h1 = rect.Height;            
            break;
          }
          case BevelBorder.Bottom:
          {
            x1 = rect.Left; y1 = rect.Top + rect.Height - 1; w1 = rect.Width; h1 = 1;            
            break;
          }
        }
      } 
                            
      switch (Style)
      {
        case BevelStyle.Bumped:
        {
          renderer.Draw(img, new Rectangle(x1, y1, w1, h1), c1);
          renderer.Draw(img, new Rectangle(x2, y2, w2, h2), c2);          
          break;
        }
        case BevelStyle.Etched:
        {
          renderer.Draw(img, new Rectangle(x1, y1, w1, h1), c2);
          renderer.Draw(img, new Rectangle(x2, y2, w2, h2), c1);          
          break;
        }
        case BevelStyle.Raised:
        {
          Color c = c1;
          if (pos == BevelBorder.Left || pos == BevelBorder.Top) c = c1;
          else c = c2;          
          
          renderer.Draw(img, new Rectangle(x1, y1, w1, h1), c);     
          break;
        }
        case BevelStyle.Lowered:
        {
          Color c = c1;
          if (pos == BevelBorder.Left || pos == BevelBorder.Top) c = c2;
          else c = c1; 
          
          renderer.Draw(img, new Rectangle(x1, y1, w1, h1), c);     
          break;
        }
        default:
        {
          renderer.Draw(img, new Rectangle(x1, y1, w1, h1), c3);     
          break;
        }
      }  
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////          
    protected virtual void OnBorderChanged(EventArgs e)
    {
      if (BorderChanged != null) BorderChanged.Invoke(this, e);
    }
    ////////////////////////////////////////////////////////////////////////////          

    ////////////////////////////////////////////////////////////////////////////          
    protected virtual void OnStyleChanged(EventArgs e)
    {
      if (StyleChanged != null) StyleChanged.Invoke(this, e);
    }
    ////////////////////////////////////////////////////////////////////////////          


    #endregion  

  }

}
