////////////////////////////////////////////////////////////////
//                                                            //
//  Neoforce Controls                                         //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//         File: ScrollBar.cs                                 //
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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
////////////////////////////////////////////////////////////////////////////

#endregion

namespace TomShane.Neoforce.Controls
{

  public class ScrollBar: Control
  {

    #region //// Fields ////////////

    ////////////////////////////////////////////////////////////////////////////          
    private int range = 100;
    private int value = 0;
    private int pageSize = 50;
    private int stepSize = 1;   
    private Orientation orientation = Orientation.Vertical;   
    ////////////////////////////////////////////////////////////////////////////          

    ////////////////////////////////////////////////////////////////////////////          
    private Button btnMinus = null;
    private Button btnPlus = null;
    private Button btnSlider = null;

    private string strButton = "ScrollBar.ButtonVert";
    private string strRail = "ScrollBar.RailVert";
    private string strSlider = "ScrollBar.SliderVert";
    private string strGlyph = "ScrollBar.GlyphVert";
    private string strMinus = "ScrollBar.ArrowUp";
    private string strPlus = "ScrollBar.ArrowDown";    
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
          if (this.value > range - pageSize) this.value = range - pageSize;
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
        if (pageSize !=  value)
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
          if (!Suspended) OnStepSizeChanged(new EventArgs());                   
        }  
      }
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
    public ScrollBar(Manager manager, Orientation orientation): base(manager)
    {
      this.orientation = orientation;
      CanFocus = false;     
           

      if (orientation == Orientation.Horizontal)
      {
        strButton = "ScrollBar.ButtonHorz";
        strRail = "ScrollBar.RailHorz";
        strSlider = "ScrollBar.SliderHorz";
        strGlyph = "ScrollBar.GlyphHorz";
        strMinus = "ScrollBar.ArrowLeft";
        strPlus = "ScrollBar.ArrowRight";

        MinimumHeight = 16;
        MinimumWidth = 46;
        Width = 64;
        Height = 16;        
      }
      else
      {
        strButton = "ScrollBar.ButtonVert";
        strRail = "ScrollBar.RailVert";
        strSlider = "ScrollBar.SliderVert";
        strGlyph = "ScrollBar.GlyphVert";
        strMinus = "ScrollBar.ArrowUp";
        strPlus = "ScrollBar.ArrowDown";

        MinimumHeight = 46;
        MinimumWidth = 16;
        Width = 16;
        Height = 64;        
      }

      btnMinus = new Button(Manager);
      btnMinus.Init();
      btnMinus.Text = "";
      btnMinus.MousePress += new MouseEventHandler(ArrowPress);
      btnMinus.CanFocus = false;

      btnSlider = new Button(Manager);
      btnSlider.Init();
      btnSlider.Text = "";
      btnSlider.CanFocus = false;
      btnSlider.MinimumHeight = 16;
      btnSlider.MinimumWidth = 16;      

      btnPlus = new Button(Manager);
      btnPlus.Init();
      btnPlus.Text = "";
      btnPlus.MousePress += new MouseEventHandler(ArrowPress);
      btnPlus.CanFocus = false;

      btnSlider.Move += new MoveEventHandler(btnSlider_Move);

      Add(btnMinus);
      Add(btnSlider);
      Add(btnPlus);
    }
    ////////////////////////////////////////////////////////////////////////////
        
    #endregion

    #region //// Methods ///////////

    ////////////////////////////////////////////////////////////////////////////

    public void ScrollUp()
    {
        Value -= stepSize;
        if (Value < 0) Value = 0;
    }
 
    public void ScrollDown()
    {
        Value += stepSize;
        if (Value > range - pageSize) Value = range - pageSize - 1;
    }
 
    public void ScrollUp(bool alot)
    {
        if (alot)
        {
            Value -= pageSize;
            if (Value < 0) Value = 0;
        }
        else
            ScrollUp();
    }
 
    public void ScrollDown(bool alot)
    {
        if (alot)
        {
            Value += pageSize;
            if (Value > range - pageSize) Value = range - pageSize - 1;
        }
        else
            ScrollDown();
    }

    ////////////////////////////////////////////////////////////////////////////   
    public override void Init()
    {
      base.Init();    
   
      SkinControl sc = new SkinControl(btnPlus.Skin);
      sc.Layers["Control"] = new SkinLayer(Skin.Layers[strButton]);
      sc.Layers[strButton].Name = "Control";
      btnPlus.Skin = btnMinus.Skin = sc;   

      SkinControl ss = new SkinControl(btnSlider.Skin);
      ss.Layers["Control"] = new SkinLayer(Skin.Layers[strSlider]);
      ss.Layers[strSlider].Name = "Control";      
      btnSlider.Skin = ss;      
   
      btnMinus.Glyph = new Glyph(Skin.Layers[strMinus].Image.Resource);
      btnMinus.Glyph.SizeMode = SizeMode.Centered;
      btnMinus.Glyph.Color = Manager.Skin.Controls["Button"].Layers["Control"].Text.Colors.Enabled;
   
      btnPlus.Glyph = new Glyph(Skin.Layers[strPlus].Image.Resource);
      btnPlus.Glyph.SizeMode = SizeMode.Centered;
      btnPlus.Glyph.Color = Manager.Skin.Controls["Button"].Layers["Control"].Text.Colors.Enabled;
   
      btnSlider.Glyph = new Glyph(Skin.Layers[strGlyph].Image.Resource);
      btnSlider.Glyph.SizeMode = SizeMode.Centered;      
   
    }
    ////////////////////////////////////////////////////////////////////////////        
    
    ////////////////////////////////////////////////////////////////////////////   
    protected internal override void InitSkin()
    {
      base.InitSkin();
      Skin = new SkinControl(Manager.Skin.Controls["ScrollBar"]);
    }      
    ////////////////////////////////////////////////////////////////////////////       
           
    ////////////////////////////////////////////////////////////////////////////   
    protected override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
    {      
      RecalcParams();                        
     
      SkinLayer bg = Skin.Layers[strRail];      
      renderer.DrawLayer(bg, rect, Color.White, bg.States.Enabled.Index);
    }
    ////////////////////////////////////////////////////////////////////////////     

    ////////////////////////////////////////////////////////////////////////////
    void ArrowPress(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButton.Left)
      {
        if (sender == btnMinus)
        {
            ScrollUp();
        }
        else if (sender == btnPlus)
        {
            ScrollDown();
        }       
      }
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////     
    protected override void OnResize(ResizeEventArgs e)
    {
      base.OnResize(e);     
      RecalcParams();
      if (Value + PageSize > Range) Value = Range - PageSize;              
    }
    ////////////////////////////////////////////////////////////////////////////     

    ////////////////////////////////////////////////////////////////////////////     
    private void RecalcParams()
    {      
      if (btnMinus != null && btnPlus != null && btnSlider != null)
      {    
        if (orientation == Orientation.Horizontal)
        {
          btnMinus.Width = Height;
          btnMinus.Height = Height;        

          btnPlus.Width = Height;
          btnPlus.Height = Height;
          btnPlus.Left = Width - Height;        
          btnPlus.Top = 0;        
                  
          btnSlider.Movable = true;        
          int size = btnMinus.Width + Skin.Layers[strSlider].OffsetX;

          btnSlider.MinimumWidth = Height;
          int w = (Width - 2 * size);
          btnSlider.Width = (int)Math.Ceiling((pageSize * w) / (float)range);
          btnSlider.Height = Height;


          float px = (float)(Range - PageSize) / (float)(w - btnSlider.Width);
          int pos = (int)(Math.Ceiling(Value / (float)px));
          btnSlider.SetPosition(size + pos, 0);
          if (btnSlider.Left < size) btnSlider.SetPosition(size, 0);
          if (btnSlider.Left + btnSlider.Width + size > Width) btnSlider.SetPosition(Width - size - btnSlider.Width, 0);                    
        }
        else
        {        
          btnMinus.Width = Width;
          btnMinus.Height = Width;                

          btnPlus.Width = Width;
          btnPlus.Height = Width;
          btnPlus.Top = Height - Width;

          btnSlider.Movable = true;
          int size = btnMinus.Height + Skin.Layers[strSlider].OffsetY;

          btnSlider.MinimumHeight = Width;
          int h = (Height - 2 * size);
          btnSlider.Height = (int)Math.Ceiling((pageSize * h) / (float)range);
          btnSlider.Width = Width;               

          float px = (float)(Range - PageSize) / (float)(h - btnSlider.Height);
          int pos = (int)(Math.Ceiling(Value / (float)px));          
          btnSlider.SetPosition(0, size + pos);          
          if (btnSlider.Top < size) btnSlider.SetPosition(0, size);
          if (btnSlider.Top + btnSlider.Height + size > Height) btnSlider.SetPosition(0, Height - size - btnSlider.Height);
        }                
      }      
    }       
    ////////////////////////////////////////////////////////////////////////////     

    ////////////////////////////////////////////////////////////////////////////     
    void btnSlider_Move(object sender, MoveEventArgs e)
    {
      if (orientation == Orientation.Horizontal)
      {
        int size = btnMinus.Width + Skin.Layers[strSlider].OffsetX;
        btnSlider.SetPosition(e.Left, 0);
        if (btnSlider.Left < size) btnSlider.SetPosition(size, 0);
        if (btnSlider.Left + btnSlider.Width + size > Width) btnSlider.SetPosition(Width - size - btnSlider.Width, 0);
      }
      else
      {                                    
        int size = btnMinus.Height + Skin.Layers[strSlider].OffsetY;         
        btnSlider.SetPosition(0, e.Top);
        if (btnSlider.Top < size) btnSlider.SetPosition(0, size);         
        if (btnSlider.Top + btnSlider.Height + size > Height) btnSlider.SetPosition(0, Height - size - btnSlider.Height);                
      }

      if (orientation == Orientation.Horizontal)
      {
        int size = btnMinus.Width + Skin.Layers[strSlider].OffsetX;
        int w = (Width - 2 * size) - btnSlider.Width;
        float px = (float)(Range - PageSize) / (float)w;
        Value = (int)(Math.Ceiling((btnSlider.Left - size) * px));                                        
      }
      else
      {
        int size = btnMinus.Height + Skin.Layers[strSlider].OffsetY;
        int h = (Height - 2 * size) - btnSlider.Height;        
        float px = (float)(Range - PageSize) / (float)h;
        Value = (int)(Math.Ceiling((btnSlider.Top - size) * px));
      } 
    }
    ////////////////////////////////////////////////////////////////////////////        
    
    ////////////////////////////////////////////////////////////////////////////        
    protected override void OnMouseUp(MouseEventArgs e)
    {
      btnSlider.Passive = false;  
      base.OnMouseUp(e);       
    }
    ////////////////////////////////////////////////////////////////////////////        
    
    ////////////////////////////////////////////////////////////////////////////        
    protected override void OnMouseDown(MouseEventArgs e)
    {      
      base.OnMouseDown(e);            
      
      btnSlider.Passive = true;            
      
      if (e.Button == MouseButton.Left)
      {
        if (orientation == Orientation.Horizontal)
        {
          int pos = e.Position.X;
          
          if (pos < btnSlider.Left)
          {
              ScrollUp(true);      
          }
          else if (pos >= btnSlider.Left + btnSlider.Width)
          {
              ScrollDown(true);
          } 
        }
        else
        {
          int pos = e.Position.Y;

          if (pos < btnSlider.Top)
          {
              ScrollUp(true);
          }
          else if (pos >= btnSlider.Top + btnSlider.Height)
          {
              ScrollDown(true);
          }       
        }                               
      }  
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
