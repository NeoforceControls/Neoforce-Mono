////////////////////////////////////////////////////////////////
//                                                            //
//  Neoforce Controls                                         //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//         File: TrackBar.cs                                  //
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
using System;
////////////////////////////////////////////////////////////////////////////

#endregion

namespace TomShane.Neoforce.Controls
{

  public class TrackBar: Control
  {

    #region //// Fields ////////////

    ////////////////////////////////////////////////////////////////////////////           
    private int range = 100;
    private int value = 0; 
    private int stepSize = 1;
    private int pageSize = 5;
    private bool scale = true;    
    private Button btnSlider;    
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Properties ////////

    ////////////////////////////////////////////////////////////////////////////    
    public virtual int Value
    {
      get { return this.value; }
      set
      {
        if (this.value != value)
        {
          this.value = value;
          if (this.value < 0) this.value = 0;
          if (this.value > range) this.value = range;
          Invalidate();
          if (!Suspended) OnValueChanged(new EventArgs());
        }
      }
    }
    ////////////////////////////////////////////////////////////////////////////    

    ////////////////////////////////////////////////////////////////////////////    
    public virtual int Range
    {
      get { return range; }
      set
      {
        if (range != value)
        {                    
          range = value;
          range = value;
          if (pageSize > range) pageSize = range;                
          RecalcParams();          
          if (!Suspended) OnRangeChanged(new EventArgs());
        }        
      }
    }
    ////////////////////////////////////////////////////////////////////////////   

    ////////////////////////////////////////////////////////////////////////////    
    public virtual int PageSize
    {
      get { return pageSize; }
      set
      {
        if (pageSize != value)
        {
          pageSize = value;
          if (pageSize > range) pageSize = range;
          RecalcParams();
          if (!Suspended) OnPageSizeChanged(new EventArgs());
        }
      }
    }
    ////////////////////////////////////////////////////////////////////////////    

    ////////////////////////////////////////////////////////////////////////////    
    public virtual int StepSize
    {
      get { return stepSize; }
      set
      {
        if (stepSize != value)
        {
          stepSize = value;
          if (stepSize > range) stepSize = range;
          if (!Suspended) OnStepSizeChanged(new EventArgs());
        }
      }
    }
    //////////////////////////////////////////////////////////////////////////// 
    
    //////////////////////////////////////////////////////////////////////////// 
    public virtual bool Scale
    {
      get { return scale; }
      set { scale = value; }
    }
    //////////////////////////////////////////////////////////////////////////// 

    #endregion

    #region //// Events ////////////

    ////////////////////////////////////////////////////////////////////////////            
    public event EventHandler ValueChanged;
    public event EventHandler RangeChanged;
    public event EventHandler StepSizeChanged;
    public event EventHandler PageSizeChanged;
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Construstors //////

    ////////////////////////////////////////////////////////////////////////////       
    public TrackBar(Manager manager): base(manager)
    {     
      Width = 64;
      Height = 20;      
      CanFocus = false;

      btnSlider = new Button(Manager);
      btnSlider.Init();
      btnSlider.Text = "";
      btnSlider.CanFocus = true;
      btnSlider.Parent = this;
      btnSlider.Anchor = Anchors.Left | Anchors.Top | Anchors.Bottom;
      btnSlider.Detached = true;
      btnSlider.Movable = true;
      btnSlider.Move += new MoveEventHandler(btnSlider_Move);
      btnSlider.KeyPress += new KeyEventHandler(btnSlider_KeyPress);
      btnSlider.GamePadPress += new GamePadEventHandler(btnSlider_GamePadPress);
    }    
    ////////////////////////////////////////////////////////////////////////////
    
    #endregion

    #region //// Methods ///////////

    ////////////////////////////////////////////////////////////////////////////   
    public override void Init()
    {
      base.Init();
      btnSlider.Skin = new SkinControl(Manager.Skin.Controls["TrackBar.Button"]);      
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////
    protected internal override void InitSkin()
    {
      base.InitSkin();
      Skin = new SkinControl(Manager.Skin.Controls["TrackBar"]);      
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////   
    protected override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
    {
      RecalcParams();
      
      SkinLayer p = Skin.Layers["Control"];
      SkinLayer l = Skin.Layers["Scale"];            
      
      float ratio = 0.66f;
      int h = (int)(ratio * rect.Height);
      int t = rect.Top + (Height - h) / 2;      
            
      float px = ((float)value / (float)range);
      int w = (int)Math.Ceiling(px * (rect.Width - p.ContentMargins.Horizontal - btnSlider.Width)) + 2;
            
      if (w < l.SizingMargins.Vertical) w = l.SizingMargins.Vertical;
      if (w > rect.Width - p.ContentMargins.Horizontal) w = rect.Width - p.ContentMargins.Horizontal;
      
      Rectangle r1 = new Rectangle(rect.Left + p.ContentMargins.Left, t + p.ContentMargins.Top, w, h - p.ContentMargins.Vertical);
            
      base.DrawControl(renderer, new Rectangle(rect.Left, t, rect.Width, h), gameTime);      
      if (scale) renderer.DrawLayer(this, l, r1);      
    }
    ////////////////////////////////////////////////////////////////////////////     
    
    ////////////////////////////////////////////////////////////////////////////     
    void btnSlider_Move(object sender, MoveEventArgs e)
    {      
      SkinLayer p = Skin.Layers["Control"];
      int size = btnSlider.Width;
      int w = Width - p.ContentMargins.Horizontal - size;
      int pos = e.Left;

      if (pos < p.ContentMargins.Left) pos = p.ContentMargins.Left;
      if (pos > w + p.ContentMargins.Left) pos = w + p.ContentMargins.Left;
      
      btnSlider.SetPosition(pos, 0);      
            
      float px = (float)range / (float)w;
      Value = (int)(Math.Ceiling((pos - p.ContentMargins.Left) * px));
    }
    ////////////////////////////////////////////////////////////////////////////   

    ////////////////////////////////////////////////////////////////////////////     
    private void RecalcParams()
    {
      if (btnSlider != null)
      {
        if (btnSlider.Width > 12)
        {        
          btnSlider.Glyph = new Glyph(Manager.Skin.Images["Shared.Glyph"].Resource);
          btnSlider.Glyph.SizeMode = SizeMode.Centered;      
        }
        else
        {
          btnSlider.Glyph = null;
        }
      
        SkinLayer p = Skin.Layers["Control"];
        btnSlider.Width = (int)(Height * 0.8);
        btnSlider.Height = Height;
        int size = btnSlider.Width;
        int w = Width - p.ContentMargins.Horizontal - size;

        float px = (float)range / (float)w;
        int pos = p.ContentMargins.Left + (int)(Math.Ceiling(Value / (float)px));                                
        
        if (pos < p.ContentMargins.Left) pos = p.ContentMargins.Left;
        if (pos > w + p.ContentMargins.Left) pos = w + p.ContentMargins.Left;
        
        btnSlider.SetPosition(pos, 0);                
      }
    }
    ////////////////////////////////////////////////////////////////////////////   

    ////////////////////////////////////////////////////////////////////////////        
    protected override void OnMousePress(MouseEventArgs e)
    {
      base.OnMouseDown(e);  
      
      if (e.Button == MouseButton.Left)
      {        
        int pos = e.Position.X;
          
        if (pos < btnSlider.Left)
        {
          Value -= pageSize;         
        }
        else if (pos >= btnSlider.Left + btnSlider.Width)
        {
          Value += pageSize;          
        } 
      }         
    }
    ////////////////////////////////////////////////////////////////////////////           

    ////////////////////////////////////////////////////////////////////////////
    void btnSlider_GamePadPress(object sender, GamePadEventArgs e)
    {
      if (e.Button == GamePadActions.Left || e.Button == GamePadActions.Down) Value -= stepSize;
      if (e.Button == GamePadActions.Right || e.Button == GamePadActions.Up) Value += stepSize;
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    void btnSlider_KeyPress(object sender, KeyEventArgs e)
    {
      if (e.Key == Microsoft.Xna.Framework.Input.Keys.Left || e.Key == Microsoft.Xna.Framework.Input.Keys.Down) Value -= stepSize;
      else if (e.Key == Microsoft.Xna.Framework.Input.Keys.Right || e.Key == Microsoft.Xna.Framework.Input.Keys.Up) Value += stepSize;
      else if (e.Key == Microsoft.Xna.Framework.Input.Keys.PageDown) Value -= pageSize;
      else if (e.Key == Microsoft.Xna.Framework.Input.Keys.PageUp) Value += pageSize;
      else if (e.Key == Microsoft.Xna.Framework.Input.Keys.Home) Value = 0;
      else if (e.Key == Microsoft.Xna.Framework.Input.Keys.End) Value = Range;
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////   
    protected override void OnResize(ResizeEventArgs e)
    {
      base.OnResize(e);
      RecalcParams();      
    }
    ////////////////////////////////////////////////////////////////////////////          

    ////////////////////////////////////////////////////////////////////////////        
    protected virtual void OnValueChanged(EventArgs e)
    {
      if (ValueChanged != null) ValueChanged.Invoke(this, e);
    }
    ////////////////////////////////////////////////////////////////////////////        

    ////////////////////////////////////////////////////////////////////////////        
    protected virtual void OnRangeChanged(EventArgs e)
    {
      if (RangeChanged != null) RangeChanged.Invoke(this, e);
    }
    ////////////////////////////////////////////////////////////////////////////         

    ////////////////////////////////////////////////////////////////////////////        
    protected virtual void OnPageSizeChanged(EventArgs e)
    {
      if (PageSizeChanged != null) PageSizeChanged.Invoke(this, e);
    }
    ////////////////////////////////////////////////////////////////////////////        

    ////////////////////////////////////////////////////////////////////////////        
    protected virtual void OnStepSizeChanged(EventArgs e)
    {
      if (StepSizeChanged != null) StepSizeChanged.Invoke(this, e);
    }
    ////////////////////////////////////////////////////////////////////////////        

    #endregion

  }

}
