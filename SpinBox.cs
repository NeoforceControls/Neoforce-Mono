////////////////////////////////////////////////////////////////
//                                                            //
//  Neoforce Controls                                         //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//         File: SpinBox.cs                                   //
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

using System.Collections.Generic;
////////////////////////////////////////////////////////////////////////////
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
////////////////////////////////////////////////////////////////////////////

#endregion

namespace TomShane.Neoforce.Controls
{

  #region //// Enums /////////////

  ////////////////////////////////////////////////////////////////////////////
  public enum SpinBoxMode
  {
    Range,
    List
  }
  ////////////////////////////////////////////////////////////////////////////

  #endregion
  
  public class SpinBox: TextBox
  {

    #region //// Fields ////////////

    ////////////////////////////////////////////////////////////////////////////                 
    private Button btnUp = null;
    private Button btnDown = null;
    private SpinBoxMode mode = SpinBoxMode.List;
    private List<object> items = new List<object>();
    private float value = 0;
    private float minimum = 0;
    private float maximum = 100;
    private float step = 0.25f;
    private int rounding = 2;
    private int itemIndex = -1;    
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Properties ////////

    ////////////////////////////////////////////////////////////////////////////
    public new virtual SpinBoxMode Mode
    {
      get { return mode; }
      set { mode = value; }
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////
    public override bool ReadOnly
    {
      get { return base.ReadOnly; }
      set
      {
        base.ReadOnly = value;
        CaretVisible = !value;
        if (value)
        {
          #if (!XBOX && !XBOX_FAKE)
            Cursor = Manager.Skin.Cursors["Default"].Resource;
          #endif
        }
        else
        {
          #if (!XBOX && !XBOX_FAKE)
            Cursor = Manager.Skin.Cursors["Text"].Resource;
          #endif
        }
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    public virtual List<object> Items
    {
      get { return items; }      
    } 
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////    
    public float Value
    {
      get { return this.value; }
      set 
      { 
        if (this.value != value)
        {
          this.value = value; 
          Invalidate();
        }          
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    public float Minimum
    {
      get { return minimum; }
      set 
      { 
        if (minimum != value)
        {
          minimum = value; 
        }  
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    public float Maximum
    {
      get { return maximum; }
      set 
      { 
        if (maximum != value)
        {
          maximum = value; 
        }  
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    public float Step
    {
      get { return step; }
      set 
      { 
        if (step != value)
        {
          step = value; 
        }          
      }
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////
    public int ItemIndex
    {
      get { return itemIndex; }
      set 
      { 
        if (mode == SpinBoxMode.List)
        {
          itemIndex = value;
          Text = items[itemIndex].ToString();         
        }  
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    public int Rounding
    {
      get { return rounding; }
      set 
      { 
        if (rounding != value)
        {        
          rounding = value; 
          Invalidate();
        }  
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Construstors //////

    ////////////////////////////////////////////////////////////////////////////       
    public SpinBox(Manager manager, SpinBoxMode mode): base(manager)
    {         
      this.mode = mode;
      ReadOnly = true;      
      
      Height = 20;
      Width = 64;           
                     
      btnUp = new Button(Manager);
      btnUp.Init();    
      btnUp.CanFocus = false;
      btnUp.MousePress += new MouseEventHandler(btn_MousePress);
      Add(btnUp, false);

      btnDown = new Button(Manager);
      btnDown.Init();            
      btnDown.CanFocus = false;
      btnDown.MousePress += new MouseEventHandler(btn_MousePress);
      Add(btnDown, false);                  
      
    }
    ////////////////////////////////////////////////////////////////////////////   

    #endregion

    #region //// Methods ///////////

    ////////////////////////////////////////////////////////////////////////////
    public override void Init()
    {
      base.Init();      

      SkinControl sc = new SkinControl(btnUp.Skin);
      sc.Layers["Control"] = new SkinLayer(Skin.Layers["Button"]);
      sc.Layers["Button"].Name = "Control";
      btnUp.Skin = btnDown.Skin = sc;

      btnUp.Glyph = new Glyph(Manager.Skin.Images["Shared.ArrowUp"].Resource);
      btnUp.Glyph.SizeMode = SizeMode.Centered;
      btnUp.Glyph.Color = Manager.Skin.Controls["Button"].Layers["Control"].Text.Colors.Enabled;

      btnDown.Glyph = new Glyph(Manager.Skin.Images["Shared.ArrowDown"].Resource);
      btnDown.Glyph.SizeMode = SizeMode.Centered;
      btnDown.Glyph.Color = Manager.Skin.Controls["Button"].Layers["Control"].Text.Colors.Enabled;
    }
    ////////////////////////////////////////////////////////////////////////////                          
    
    ////////////////////////////////////////////////////////////////////////////                          
    protected internal override void InitSkin()
    {
      base.InitSkin();
      Skin = new SkinControl(Manager.Skin.Controls["SpinBox"]);
    }
    ////////////////////////////////////////////////////////////////////////////                          

    ////////////////////////////////////////////////////////////////////////////
    protected override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
    {
      base.DrawControl(renderer, rect, gameTime);

      if (ReadOnly && Focused)
      {
        SkinLayer lr = Skin.Layers[0];
        Rectangle rc = new Rectangle(rect.Left + lr.ContentMargins.Left,
                                     rect.Top + lr.ContentMargins.Top,
                                     Width - lr.ContentMargins.Horizontal - btnDown.Width - btnUp.Width,
                                     Height - lr.ContentMargins.Vertical);
        renderer.Draw(Manager.Skin.Images["ListBox.Selection"].Resource, rc, Color.FromNonPremultiplied(255, 255, 255, 128));
      }    
    }
    ////////////////////////////////////////////////////////////////////////////    
    
    ////////////////////////////////////////////////////////////////////////////     
    protected override void OnResize(ResizeEventArgs e)
    {
      base.OnResize(e);
      
      if (btnUp != null)
      {
        btnUp.Width = 16;
        btnUp.Height = Height - Skin.Layers["Control"].ContentMargins.Vertical;
        btnUp.Top = Skin.Layers["Control"].ContentMargins.Top;
        btnUp.Left = Width - 16 - 2 - 16 - 1;
      }  
      if (btnDown != null)
      {
        btnDown.Width = 16;
        btnDown.Height = Height - Skin.Layers["Control"].ContentMargins.Vertical;
        btnDown.Top = Skin.Layers["Control"].ContentMargins.Top; ;
        btnDown.Left = Width - 16 - 2;            
      }  
    }
    ////////////////////////////////////////////////////////////////////////////    

    ////////////////////////////////////////////////////////////////////////////
    private void ShiftIndex(bool direction)
    {
      if (mode == SpinBoxMode.List)
      {
        if (items.Count > 0)
        {
          if (direction)
          {
            itemIndex += 1;
          }
          else
          {
            itemIndex -= 1;
          }

          if (itemIndex < 0) itemIndex = 0;
          if (itemIndex > items.Count - 1) itemIndex = itemIndex = items.Count - 1;

          Text = items[itemIndex].ToString();
        }  
      }
      else
      {
        if (direction)
        {
          value += step;
        }
        else
        {
          value -= step;
        }

        if (value < minimum) value = minimum;
        if (value > maximum) value = maximum;

        Text = value.ToString("n" + rounding.ToString());
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    private void btn_MousePress(object sender, MouseEventArgs e)
    {
      Focused = true;
      if (sender == btnUp) ShiftIndex(true);
      else if (sender == btnDown) ShiftIndex(false);
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    protected override void OnKeyPress(KeyEventArgs e)
    {            
      if (e.Key == Keys.Up)
      {
        e.Handled = true;
        ShiftIndex(true);
      }        
      else if (e.Key == Keys.Down)
      {
        e.Handled = true;
        ShiftIndex(false);
      }
      
      base.OnKeyPress(e);
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    protected override void OnGamePadPress(GamePadEventArgs e)
    {
      if (e.Button == GamePadActions.Up)
      {
        e.Handled = true;
        ShiftIndex(true);
      }
      else if (e.Button == GamePadActions.Down)
      {
        e.Handled = true;
        ShiftIndex(false);
      }
      
      base.OnGamePadPress(e);
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    protected override void OnGamePadDown(GamePadEventArgs e)
    {      
      base.OnGamePadDown(e);
    }
    ////////////////////////////////////////////////////////////////////////////
    
    #endregion  

  }

}
