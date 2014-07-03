////////////////////////////////////////////////////////////////
//                                                            //
//  Neoforce Controls                                         //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//         File: Button.cs                                    //
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
  ///  <include file='Documents/Button.xml' path='Button/Class[@name="SizeMode"]/*' />
  public enum SizeMode
  {
    Normal, 
    Auto,
    Centered,
    Stretched,
      /// <summary>
      /// Only Supported by ImageBox
      /// </summary>
    Tiled
  }  
  ////////////////////////////////////////////////////////////////////////////

  ////////////////////////////////////////////////////////////////////////////              
  ///  <include file='Documents/Button.xml' path='Button/Class[@name="SizeMode"]/*' />
  public enum ButtonMode
  {
    Normal,
    PushButton
  }
  ////////////////////////////////////////////////////////////////////////////


  #endregion

  #region //// Classes ///////////
  
  ////////////////////////////////////////////////////////////////////////////              
  ///  <include file='Documents/Button.xml' path='Button/Class[@name="Glyph"]/*' />          
  public class Glyph
  {
    ////////////////////////////////////////////////////////////////////////////  
    public Texture2D Image = null;
    public SizeMode SizeMode = SizeMode.Stretched;
    public Color Color = Color.White;
    public Point Offset = Point.Zero;  
    public Rectangle SourceRect = Rectangle.Empty;
    ////////////////////////////////////////////////////////////////////////////  
    
    ////////////////////////////////////////////////////////////////////////////  
    public Glyph(Texture2D image)
    {
      Image = image;
    }
    ////////////////////////////////////////////////////////////////////////////  
    
    ////////////////////////////////////////////////////////////////////////////  
    public Glyph(Texture2D image, Rectangle sourceRect): this(image)
    {      
      SourceRect = sourceRect;
    }
  }
  ////////////////////////////////////////////////////////////////////////////  
    
  ////////////////////////////////////////////////////////////////////////////  
  ///  <include file='Documents/Button.xml' path='Button/Class[@name="Button"]/*' />          
  public class Button: ButtonBase
  {

    #region //// Consts ////////////

    ////////////////////////////////////////////////////////////////////////////
    private const string skButton = "Button";
    private const string lrButton = "Control";
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Fields ////////////

    ////////////////////////////////////////////////////////////////////////////              
    private Glyph glyph = null;
    private ModalResult modalResult = ModalResult.None;
    private ButtonMode mode = ButtonMode.Normal;
    private bool pushed = false;    
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Properties ////////

    ////////////////////////////////////////////////////////////////////////////        
    public Glyph Glyph
    {
      get { return glyph; }
      set 
      { 
        glyph = value;
        if (!Suspended) OnGlyphChanged(new EventArgs());        
      }
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////
    public ModalResult ModalResult
    {
      get { return modalResult; }
      set { modalResult = value; }
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////
    public ButtonMode Mode
    {
      get { return mode; }
      set { mode = value; }
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////
    public bool Pushed
    {
      get { return pushed; }
      set 
      { 
        pushed = value; 
        Invalidate();
      }      
    }
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Events ////////////

    ////////////////////////////////////////////////////////////////////////////                 
    public event EventHandler GlyphChanged;
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Construstors //////

    ////////////////////////////////////////////////////////////////////////////       
    public Button(Manager manager): base(manager)
    {            
      SetDefaultSize(72, 24);      
    }    
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Destructors ///////

    ////////////////////////////////////////////////////////////////////////////
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
      }
      base.Dispose(disposing);
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
      Skin = new SkinControl(Manager.Skin.Controls[skButton]);
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////
    protected override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
    {                    

      if (mode == ButtonMode.PushButton && pushed)
      {
        SkinLayer l = Skin.Layers[lrButton];
        renderer.DrawLayer(l, rect, l.States.Pressed.Color, l.States.Pressed.Index);
        if (l.States.Pressed.Overlay)
        {
          renderer.DrawLayer(l, rect, l.Overlays.Pressed.Color, l.Overlays.Pressed.Index);          
        }                
      }
      else
      {
        base.DrawControl(renderer, rect, gameTime);
      }
    
      SkinLayer layer = Skin.Layers[lrButton];                                  
      SpriteFont font = (layer.Text != null && layer.Text.Font != null) ? layer.Text.Font.Resource : null;
      Color col = Color.White;              
      int ox = 0; int oy = 0;
    
      if (ControlState == ControlState.Pressed)
      {       
        if (layer.Text != null) col = layer.Text.Colors.Pressed;
        ox = 1; oy = 1;
      }                               
      if (glyph != null)
      {
        Margins cont = layer.ContentMargins;
        Rectangle r = new Rectangle(rect.Left + cont.Left, 
                                    rect.Top + cont.Top, 
                                    rect.Width - cont.Horizontal, 
                                    rect.Height - cont.Vertical);
        renderer.DrawGlyph(glyph, r);
      }
      else 
      {
        renderer.DrawString(this, layer, Text, rect, true, ox, oy);
      }       
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////
    private void OnGlyphChanged(EventArgs e)
    {
      if (GlyphChanged != null) GlyphChanged.Invoke(this, e);
    }
    //////////////////////////////////////////////////////////////////////////// 

    //////////////////////////////////////////////////////////////////////////// 
    protected override void OnClick(EventArgs e)
    {
      MouseEventArgs ex = (e is MouseEventArgs) ? (MouseEventArgs)e : new MouseEventArgs();      
      
      if (ex.Button == MouseButton.Left || ex.Button == MouseButton.None)
      {
        pushed = !pushed;
      }  
      
      base.OnClick(e);           
      
      if ((ex.Button == MouseButton.Left || ex.Button == MouseButton.None) && Root != null)
      {
        if (Root is Window)
        {
          Window wnd = (Window)Root;
          if (ModalResult != ModalResult.None)
          {            
            wnd.Close(ModalResult);
          }
        }        
      }
    }
    ////////////////////////////////////////////////////////////////////////////   
  
    #endregion  
     
  }
  
  #endregion
  
}
