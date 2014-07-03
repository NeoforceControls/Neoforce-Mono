////////////////////////////////////////////////////////////////
//                                                            //
//  Neoforce Controls                                         //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//         File: Window.cs                                    //
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
using Microsoft.Xna.Framework.Input;
////////////////////////////////////////////////////////////////////////////

#endregion

namespace TomShane.Neoforce.Controls
{

  #region //// Classes ///////////
  
  ////////////////////////////////////////////////////////////////////////////
  public class WindowGamePadActions: GamePadActions
  {
    public GamePadButton Accept = GamePadButton.Start;
    public GamePadButton Cancel = GamePadButton.Back;
  }
  ////////////////////////////////////////////////////////////////////////////
  
  ////////////////////////////////////////////////////////////////////////////
  ///  <include file='Documents/Window.xml' path='Window/Class[@name="Window"]/*' />          
  public class Window: ModalContainer
  {

    #region //// Consts ////////////

    ////////////////////////////////////////////////////////////////////////////
    private const string skWindow       = "Window";
    private const string lrWindow       = "Control";
    private const string lrCaption      = "Caption";        
    private const string lrFrameTop     = "FrameTop";    
    private const string lrFrameLeft    = "FrameLeft";
    private const string lrFrameRight   = "FrameRight";
    private const string lrFrameBottom  = "FrameBottom";
    private const string lrIcon         = "Icon";  
      
    private const string skButton       = "Window.CloseButton";
    private const string lrButton       = "Control";
    
    private const string skShadow       = "Window.Shadow";
    private const string lrShadow       = "Control";        
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Fields ////////////

    ////////////////////////////////////////////////////////////////////////////    
    private Button btnClose;
    private bool closeButtonVisible = true;
    private bool iconVisible = true;
    private Texture2D icon = null;
    private bool shadow = true;
    private bool captionVisible = true;    
    private bool borderVisible = true;
    private byte oldAlpha = 255;
    private byte dragAlpha = 200;  
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Events ////////////

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Properties ////////

    ////////////////////////////////////////////////////////////////////////////
    public virtual Texture2D Icon
    {
      get { return icon; }
      set { icon = value; }
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////
    public virtual bool Shadow 
    { 
      get { return shadow; } 
      set { shadow = value; } 
    }
    ////////////////////////////////////////////////////////////////////////////      

    ////////////////////////////////////////////////////////////////////////////
    public virtual bool CloseButtonVisible
    {
      get
      {
        return closeButtonVisible;
      }
      set
      {
        closeButtonVisible = value;
        if (btnClose != null) btnClose.Visible = value;
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    public virtual bool IconVisible
    {
      get
      {
        return iconVisible;
      }
      set
      {
        iconVisible = value;
      }
    }
    ////////////////////////////////////////////////////////////////////////////   

    ////////////////////////////////////////////////////////////////////////////   
    public virtual bool CaptionVisible
    {
      get { return captionVisible; }
      set 
      { 
        captionVisible = value; 
        AdjustMargins();        
      }
    }
    ////////////////////////////////////////////////////////////////////////////   

    ////////////////////////////////////////////////////////////////////////////   
    public virtual bool BorderVisible
    {
      get { return borderVisible; }
      set
      {
        borderVisible = value;
        AdjustMargins();
      }
    }
    //////////////////////////////////////////////////////////////////////////// 
    
    //////////////////////////////////////////////////////////////////////////// 
    public virtual byte DragAlpha
    {
      get { return dragAlpha; }
      set { dragAlpha = value; }
    }
    //////////////////////////////////////////////////////////////////////////// 

    #endregion

    #region //// Constructors //////

    ////////////////////////////////////////////////////////////////////////////
    public Window(Manager manager): base(manager)
    {
      CheckLayer(Skin, lrWindow);
      CheckLayer(Skin, lrCaption);      
      CheckLayer(Skin, lrFrameTop);      
      CheckLayer(Skin, lrFrameLeft);
      CheckLayer(Skin, lrFrameRight);
      CheckLayer(Skin, lrFrameBottom);
      CheckLayer(Manager.Skin.Controls[skButton], lrButton);
      CheckLayer(Manager.Skin.Controls[skShadow], lrShadow);
      
      SetDefaultSize(640, 480);
      SetMinimumSize(100, 75);

      btnClose = new Button(manager);
      btnClose.Skin = new SkinControl(Manager.Skin.Controls[skButton]);
      btnClose.Init();
      btnClose.Detached = true;
      btnClose.CanFocus = false;      
      btnClose.Text = null;
      btnClose.Click += new EventHandler(btnClose_Click);
      btnClose.SkinChanged += new EventHandler(btnClose_SkinChanged);

      AdjustMargins();      
      
      AutoScroll = true;
      Movable = true;
      Resizable = true;
      Center();
     
      Add(btnClose, false);   
      
      oldAlpha = Alpha;               
    }   
    ////////////////////////////////////////////////////////////////////////////             

    #endregion
    
    ////////////////////////////////////////////////////////////////////////////             
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {       
      }
      base.Dispose(disposing);
    }
    ////////////////////////////////////////////////////////////////////////////             

    #region //// Methods ///////////

    ////////////////////////////////////////////////////////////////////////////
    public override void Init()
    {
      base.Init();

      SkinLayer l = btnClose.Skin.Layers[lrButton];            
      btnClose.Width = l.Width - btnClose.Skin.OriginMargins.Horizontal;
      btnClose.Height = l.Height - btnClose.Skin.OriginMargins.Vertical;
      btnClose.Left = OriginWidth - Skin.OriginMargins.Right - btnClose.Width + l.OffsetX;                  
      btnClose.Top = Skin.OriginMargins.Top + l.OffsetY;
      btnClose.Anchor = Anchors.Top | Anchors.Right;

      //SkinControl sc = new SkinControl(ClientArea.Skin);
      //sc.Layers[0] = Skin.Layers[lrWindow];
      //ClientArea.Color = Color.Transparent;
      //ClientArea.BackColor = Color.Transparent;
      //ClientArea.Skin = sc;                     
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    protected internal override void InitSkin()
    {
      base.InitSkin();
      Skin = new SkinControl(Manager.Skin.Controls[skWindow]);
      AdjustMargins();
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////
    void btnClose_SkinChanged(object sender, EventArgs e)
    {
      btnClose.Skin = new SkinControl(Manager.Skin.Controls[skButton]);
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////
    internal override void Render(Renderer renderer, GameTime gameTime)
    {      
      if (Visible && Shadow)
      {        
        SkinControl c = Manager.Skin.Controls[skShadow];
        SkinLayer l = c.Layers[lrShadow];

        Color cl = Color.FromNonPremultiplied(l.States.Enabled.Color.R, l.States.Enabled.Color.G, l.States.Enabled.Color.B, Alpha);

        renderer.Begin(BlendingMode.Default);
        renderer.DrawLayer(l, new Rectangle(Left - c.OriginMargins.Left, Top - c.OriginMargins.Top, Width + c.OriginMargins.Horizontal, Height + c.OriginMargins.Vertical), cl, 0);
        renderer.End();
      }
      base.Render(renderer, gameTime);
    }
    ////////////////////////////////////////////////////////////////////////////
   
    ////////////////////////////////////////////////////////////////////////////
    private Rectangle GetIconRect()
    {     
      SkinLayer l1 = Skin.Layers[lrCaption];
      SkinLayer l5 = Skin.Layers[lrIcon];             
      
      int s = l1.Height - l1.ContentMargins.Vertical;
      return new Rectangle(DrawingRect.Left + l1.ContentMargins.Left + l5.OffsetX,
                           DrawingRect.Top + l1.ContentMargins.Top + l5.OffsetY,
                           s, s);
                                       
    }
    ////////////////////////////////////////////////////////////////////////////    

    ////////////////////////////////////////////////////////////////////////////
    protected override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
    {
      SkinLayer l1 = captionVisible ? Skin.Layers[lrCaption] : Skin.Layers[lrFrameTop];      
      SkinLayer l2 = Skin.Layers[lrFrameLeft];
      SkinLayer l3 = Skin.Layers[lrFrameRight];
      SkinLayer l4 = Skin.Layers[lrFrameBottom];      
      SkinLayer l5 = Skin.Layers[lrIcon];      
      LayerStates s1, s2, s3, s4;
      SpriteFont f1 = l1.Text.Font.Resource;
      Color c1 = l1.Text.Colors.Enabled;                                                              
                              
      if ((Focused || (Manager.FocusedControl != null && Manager.FocusedControl.Root == this.Root)) && ControlState != ControlState.Disabled)
      {
        s1 = l1.States.Focused;
        s2 = l2.States.Focused;
        s3 = l3.States.Focused;
        s4 = l4.States.Focused;        
        c1 = l1.Text.Colors.Focused;        
      }
      else if (ControlState == ControlState.Disabled)
      {
        s1 = l1.States.Disabled;
        s2 = l2.States.Disabled;
        s3 = l3.States.Disabled;
        s4 = l4.States.Disabled;
        c1 = l1.Text.Colors.Disabled;
      }
      else
      {
        s1 = l1.States.Enabled;
        s2 = l2.States.Enabled;
        s3 = l3.States.Enabled;
        s4 = l4.States.Enabled;
        c1 = l1.Text.Colors.Enabled;
      }

      renderer.DrawLayer(Skin.Layers[lrWindow], rect, Skin.Layers[lrWindow].States.Enabled.Color, Skin.Layers[lrWindow].States.Enabled.Index);
      
      if (borderVisible)
      {
        renderer.DrawLayer(l1, new Rectangle(rect.Left, rect.Top, rect.Width, l1.Height), s1.Color, s1.Index);
        renderer.DrawLayer(l2, new Rectangle(rect.Left, rect.Top + l1.Height, l2.Width, rect.Height - l1.Height - l4.Height), s2.Color, s2.Index);      
        renderer.DrawLayer(l3, new Rectangle(rect.Right - l3.Width, rect.Top + l1.Height, l3.Width, rect.Height - l1.Height - l4.Height), s3.Color, s3.Index);
        renderer.DrawLayer(l4, new Rectangle(rect.Left, rect.Bottom - l4.Height, rect.Width, l4.Height),  s4.Color, s4.Index);
     
        if (iconVisible && (icon != null || l5 != null) && captionVisible)
        {       
          Texture2D i = (icon != null) ? icon : l5.Image.Resource;        
          renderer.Draw(i, GetIconRect(), Color.White);        
        }

        int icosize = 0;
        if (l5 != null && iconVisible && captionVisible)
        {
          icosize = l1.Height - l1.ContentMargins.Vertical + 4 + l5.OffsetX;
        }
        int closesize = 0;
        if (btnClose.Visible)
        {
          closesize = btnClose.Width - (btnClose.Skin.Layers[lrButton].OffsetX);
        }

        Rectangle r = new Rectangle(rect.Left + l1.ContentMargins.Left + icosize,
                                    rect.Top + l1.ContentMargins.Top,
                                    rect.Width - l1.ContentMargins.Horizontal - closesize - icosize,
                                    l1.Height - l1.ContentMargins.Top - l1.ContentMargins.Bottom);
        int ox = l1.Text.OffsetX;
        int oy = l1.Text.OffsetY;
        renderer.DrawString(f1, Text, r, c1, l1.Text.Alignment, ox, oy, true);
      }  
    }
    ////////////////////////////////////////////////////////////////////////////                               

    ////////////////////////////////////////////////////////////////////////////
    void btnClose_Click(object sender, EventArgs e)
    {      
      Close(ModalResult = ModalResult.Cancel);
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////           
    public virtual void Center()
    {
      Left = (Manager.ScreenWidth / 2) - (Width / 2);
      Top = (Manager.ScreenHeight - Height) / 2;
    }
    ////////////////////////////////////////////////////////////////////////////           


    ////////////////////////////////////////////////////////////////////////////           
    protected override void OnResize(ResizeEventArgs e)
    {
      SetMovableArea();
      base.OnResize(e);
    }
    //////////////////////////////////////////////////////////////////////////// 

    ////////////////////////////////////////////////////////////////////////////
    protected override void OnMoveBegin(EventArgs e)
    {
      base.OnMoveBegin(e);
     
      try
      {     
        oldAlpha = Alpha;   
        Alpha = dragAlpha;
      }
      catch
      {
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    protected override void OnMoveEnd(EventArgs e)
    {
      base.OnMoveEnd(e);
      try
      {
        Alpha = oldAlpha;
      }
      catch
      {
      }
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////  
    protected override void OnDoubleClick(EventArgs e)
    {
      base.OnDoubleClick(e);
                  
      MouseEventArgs ex = (e is MouseEventArgs) ? (MouseEventArgs) e : new MouseEventArgs();
      
      if (IconVisible && ex.Button == MouseButton.Left)
      {
        Rectangle r = GetIconRect();
        r.Offset(AbsoluteLeft, AbsoluteTop);
        if (r.Contains(ex.Position))
        {
          Close();
        }
      }  
    }
    ////////////////////////////////////////////////////////////////////////////     
    
    ////////////////////////////////////////////////////////////////////////////    
    protected override void AdjustMargins()
    {                  
       
      if (captionVisible && borderVisible)
      {
        ClientMargins = new Margins(Skin.ClientMargins.Left, Skin.Layers[lrCaption].Height, Skin.ClientMargins.Right, Skin.ClientMargins.Bottom);                
      }
      else if (!captionVisible && borderVisible)     
      {
        ClientMargins = new Margins(Skin.ClientMargins.Left, Skin.ClientMargins.Top, Skin.ClientMargins.Right, Skin.ClientMargins.Bottom);        
      }
      else if (!borderVisible)
      {
        ClientMargins = new Margins(0, 0, 0, 0);        
      }
      
      if (btnClose != null)
      {
        btnClose.Visible = closeButtonVisible && captionVisible && borderVisible;
      }              
                
      SetMovableArea();
      
      base.AdjustMargins();
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    private void SetMovableArea()
    {
      if (captionVisible && borderVisible)
      {
        MovableArea = new Rectangle(Skin.OriginMargins.Left, Skin.OriginMargins.Top, Width, Skin.Layers[lrCaption].Height - Skin.OriginMargins.Top);
      }
      else if (!captionVisible)
      {
        MovableArea = new Rectangle(0, 0, Width, Height);
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    #endregion
  }

  ////////////////////////////////////////////////////////////////////////////

  #endregion
  
}
