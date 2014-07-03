////////////////////////////////////////////////////////////////
//                                                            //
//  Neoforce Controls                                         //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//         File: ComboBox.cs                                  //
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
using System;
using Microsoft.Xna.Framework.Graphics;
////////////////////////////////////////////////////////////////////////////

#endregion

namespace TomShane.Neoforce.Controls
{

  public class ComboBox: TextBox
  {

    #region //// Fields ////////////

    ////////////////////////////////////////////////////////////////////////////                     
    private Button btnDown = null;    
    private List<object> items = new List<object>();
    private ListBox lstCombo = null;    
    private int maxItems = 5;
    private bool drawSelection = true;  
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Properties ////////

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
    public bool DrawSelection
    {
      get { return drawSelection; }
      set { drawSelection = value; }
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////
    public override string Text
    {
        get
        {
            return base.Text;
        }
        set
        {
            base.Text = value;
            //if (!items.Contains(value))  --- bug
            if (!items.ConvertAll(item => item.ToString()).Contains(value))
            {
                ItemIndex = -1;
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
    public int MaxItems
    {
      get { return maxItems; }
      set 
      { 
        if (maxItems != value)
        {
          maxItems = value; 
          if (!Suspended) OnMaxItemsChanged(new EventArgs());
        }  
      }
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////
    public int ItemIndex
    {
      get { return lstCombo.ItemIndex; }
      set 
      { 
        if (lstCombo != null)
        {
          if (value >= 0 && value < items.Count)
          {
            lstCombo.ItemIndex = value; 
            Text = lstCombo.Items[value].ToString();
          }
          else
          {
            lstCombo.ItemIndex = -1;          
          }
        }  
        if (!Suspended) OnItemIndexChanged(new EventArgs());
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Events ////////////

    ////////////////////////////////////////////////////////////////////////////                 
    public event EventHandler MaxItemsChanged;    
    public event EventHandler ItemIndexChanged;    
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Construstors //////

    ////////////////////////////////////////////////////////////////////////////       
    public ComboBox(Manager manager): base(manager)
    {
      Height = 20;
      Width = 64;
      ReadOnly = true;      
            
      btnDown = new Button(Manager);
      btnDown.Init();
      btnDown.Skin = new SkinControl(Manager.Skin.Controls["ComboBox.Button"]);
      btnDown.CanFocus = false;      
      btnDown.Click += new EventHandler(btnDown_Click);      
      Add(btnDown, false);            

      lstCombo = new ListBox(Manager);
      lstCombo.Init();
      lstCombo.HotTrack = true;      
      lstCombo.Detached = true;
      lstCombo.Visible = false;      
      lstCombo.Click += new EventHandler(lstCombo_Click);
      lstCombo.FocusLost += new EventHandler(lstCombo_FocusLost);
      lstCombo.Items = items;      
      manager.Input.MouseDown += new MouseEventHandler(Input_MouseDown);      
    }
    ////////////////////////////////////////////////////////////////////////////

    #endregion            
       
    #region //// Destructors ///////
    
    ////////////////////////////////////////////////////////////////////////////
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        // We added the listbox to another parent than this control, so we dispose it manually
        if (lstCombo != null)
        {
          lstCombo.Dispose();
          lstCombo = null;
        }
        Manager.Input.MouseDown -= Input_MouseDown;
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

      lstCombo.Skin = new SkinControl(Manager.Skin.Controls["ComboBox.ListBox"]);      

      btnDown.Glyph = new Glyph(Manager.Skin.Images["Shared.ArrowDown"].Resource);
      btnDown.Glyph.Color = Manager.Skin.Controls["ComboBox.Button"].Layers["Control"].Text.Colors.Enabled;
      btnDown.Glyph.SizeMode = SizeMode.Centered;            
    }
    ////////////////////////////////////////////////////////////////////////////                          
    
    ////////////////////////////////////////////////////////////////////////////                          
    protected internal override void InitSkin()
    {
      base.InitSkin();
      Skin = new SkinControl(Manager.Skin.Controls["ComboBox"]);
      AdjustMargins();
      ReadOnly = ReadOnly; // To init the right cursor
    }
    ////////////////////////////////////////////////////////////////////////////                          

    ////////////////////////////////////////////////////////////////////////////
    protected override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
    {
      base.DrawControl(renderer, rect, gameTime);      
      
      if (ReadOnly && (Focused || lstCombo.Focused) && drawSelection)
      {
        SkinLayer lr = Skin.Layers[0];
        Rectangle rc = new Rectangle(rect.Left + lr.ContentMargins.Left, 
                                     rect.Top + lr.ContentMargins.Top, 
                                     Width - lr.ContentMargins.Horizontal - btnDown.Width,
                                     Height - lr.ContentMargins.Vertical);
        renderer.Draw(Manager.Skin.Images["ListBox.Selection"].Resource, rc , Color.FromNonPremultiplied(255, 255, 255, 128));
      }  
    }
    ////////////////////////////////////////////////////////////////////////////     

    ////////////////////////////////////////////////////////////////////////////     
    protected override void OnResize(ResizeEventArgs e)
    {
      base.OnResize(e);
      
      if (btnDown != null)
      {
        btnDown.Width = 16;
        btnDown.Height = Height - Skin.Layers[0].ContentMargins.Vertical;
        btnDown.Top = Skin.Layers[0].ContentMargins.Top;
        btnDown.Left = Width - btnDown.Width - 2;                                
      }  
    }
    ////////////////////////////////////////////////////////////////////////////     

    ////////////////////////////////////////////////////////////////////////////     
    void btnDown_Click(object sender, EventArgs e)
    {          
      if (items != null && items.Count > 0)
      {        
        if (this.Root != null && this.Root is Container)
        {          
          (this.Root as Container).Add(lstCombo, false);
          lstCombo.Alpha = Root.Alpha;
          lstCombo.Left = AbsoluteLeft - Root.Left;
          lstCombo.Top = AbsoluteTop - Root.Top + Height + 1;
        }  
        else
        {
          Manager.Add(lstCombo);          
          lstCombo.Alpha = Alpha;
          lstCombo.Left = AbsoluteLeft;
          lstCombo.Top = AbsoluteTop + Height + 1;
        }

        lstCombo.AutoHeight(maxItems);
        if (lstCombo.AbsoluteTop + lstCombo.Height > Manager.TargetHeight)
        {          
          lstCombo.Top = lstCombo.Top - Height - lstCombo.Height - 2;          
        }    
        
        lstCombo.Visible = !lstCombo.Visible;
        lstCombo.Focused = true;
        lstCombo.Width = Width;
        lstCombo.AutoHeight(maxItems);           
      }        
    }
    ////////////////////////////////////////////////////////////////////////////     
    
    ////////////////////////////////////////////////////////////////////////////     
    void Input_MouseDown(object sender, MouseEventArgs e)
    {    
      if (ReadOnly && 
          (e.Position.X >= AbsoluteLeft &&
           e.Position.X <= AbsoluteLeft + Width &&
           e.Position.Y >= AbsoluteTop &&
           e.Position.Y <= AbsoluteTop + Height)) return;
           
      if (lstCombo.Visible &&      
         (e.Position.X < lstCombo.AbsoluteLeft ||
          e.Position.X > lstCombo.AbsoluteLeft + lstCombo.Width ||
          e.Position.Y < lstCombo.AbsoluteTop ||
          e.Position.Y > lstCombo.AbsoluteTop + lstCombo.Height) &&         
         (e.Position.X < btnDown.AbsoluteLeft ||
          e.Position.X > btnDown.AbsoluteLeft + btnDown.Width ||
          e.Position.Y < btnDown.AbsoluteTop ||
          e.Position.Y > btnDown.AbsoluteTop + btnDown.Height))
      {
        //lstCombo.Visible = false;      
        btnDown_Click(sender, e);
      }      
    }   
    ////////////////////////////////////////////////////////////////////////////     
    
    ////////////////////////////////////////////////////////////////////////////     
    void lstCombo_Click(object sender, EventArgs e)
    {
      MouseEventArgs ex = (e is MouseEventArgs) ? (MouseEventArgs)e : new MouseEventArgs();      
      
      if (ex.Button == MouseButton.Left || ex.Button == MouseButton.None)
      {
        lstCombo.Visible = false;        
        if (lstCombo.ItemIndex >= 0)
        {        
          Text = lstCombo.Items[lstCombo.ItemIndex].ToString();
          Focused = true;
          ItemIndex = lstCombo.ItemIndex;
        }  
      }  
    }
    ////////////////////////////////////////////////////////////////////////////     
    
    ////////////////////////////////////////////////////////////////////////////     
    protected override void OnKeyDown(KeyEventArgs e)
    {      
      if (e.Key == Keys.Down)
      {
        e.Handled = true;
        btnDown_Click(this, new MouseEventArgs());        
      }
      base.OnKeyDown(e);
    }
    ////////////////////////////////////////////////////////////////////////////     

    ////////////////////////////////////////////////////////////////////////////
    protected override void OnGamePadDown(GamePadEventArgs e)
    {
      if (!e.Handled)
      {        
        if (e.Button == GamePadActions.Click || e.Button == GamePadActions.Press || e.Button == GamePadActions.Down)
        {
          e.Handled = true;
          btnDown_Click(this, new MouseEventArgs());
        }
      }
      base.OnGamePadDown(e);
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////     
    protected override void OnMouseDown(MouseEventArgs e)
    {
      base.OnMouseDown(e);
      
      if (ReadOnly && e.Button == MouseButton.Left)
      {
        btnDown_Click(this, new MouseEventArgs());
      }
    }
    ////////////////////////////////////////////////////////////////////////////     
    
    ////////////////////////////////////////////////////////////////////////////     
    protected virtual void OnMaxItemsChanged(EventArgs e)
    {
      if (MaxItemsChanged != null) MaxItemsChanged.Invoke(this, e);
    }
    ////////////////////////////////////////////////////////////////////////////     

    ////////////////////////////////////////////////////////////////////////////     
    protected virtual void OnItemIndexChanged(EventArgs e)
    {
      if (ItemIndexChanged != null) ItemIndexChanged.Invoke(this, e);      
    }    
    ////////////////////////////////////////////////////////////////////////////     
    
    ////////////////////////////////////////////////////////////////////////////     
    void lstCombo_FocusLost(object sender, EventArgs e)
    {
      //lstCombo.Visible = false;
      Invalidate();
    }
    ////////////////////////////////////////////////////////////////////////////     
    
    ////////////////////////////////////////////////////////////////////////////     
    protected override void AdjustMargins()
    {
      base.AdjustMargins();
      ClientMargins = new Margins(ClientMargins.Left, ClientMargins.Top, ClientMargins.Right + 16, ClientMargins.Bottom);      
    }
    ////////////////////////////////////////////////////////////////////////////     
    
    #endregion

  }

}
