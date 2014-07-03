////////////////////////////////////////////////////////////////
//                                                            //
//  Neoforce Controls                                         //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//         File: ContextMenu.cs                               //
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
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
////////////////////////////////////////////////////////////////////////////

#endregion

namespace TomShane.Neoforce.Controls
{ 
  
  public class ContextMenu: MenuBase
  {

    #region //// Fields ////////////

    ////////////////////////////////////////////////////////////////////////////                 
    private long timer = 0;
    private Control sender = null; 
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Properties ////////

    ////////////////////////////////////////////////////////////////////////////    
    protected internal Control Sender { get { return sender; } set { sender = value; } }
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Construstors //////

    ////////////////////////////////////////////////////////////////////////////       
    public ContextMenu(Manager manager): base(manager)
    {      
      Visible = false;
      Detached = true;      
      StayOnBack = true;
      
      Manager.Input.MouseDown += new MouseEventHandler(Input_MouseDown);
    }
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Destructors ///////

    ////////////////////////////////////////////////////////////////////////////
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
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
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////                          
    protected internal override void InitSkin()
    {
      base.InitSkin();
      Skin = new SkinControl(Manager.Skin.Controls["ContextMenu"]);
    }
    ////////////////////////////////////////////////////////////////////////////                          
    
    ////////////////////////////////////////////////////////////////////////////
    protected override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
    {                              
      base.DrawControl(renderer, rect, gameTime);
      
      SkinLayer l1 = Skin.Layers["Control"];
      SkinLayer l2 = Skin.Layers["Selection"];      
      
      int vsize = LineHeight();
      Color col = Color.White;
     
      for (int i = 0; i < Items.Count; i++)
      {
        int mod = i > 0 ? 2 : 0;
        int left = rect.Left + l1.ContentMargins.Left + vsize;
        int h = vsize - mod - (i < (Items.Count - 1) ? 1 : 0);
        int top = rect.Top + l1.ContentMargins.Top + (i * vsize) + mod;                
        

        if (Items[i].Separated && i > 0)
        {
          Rectangle r = new Rectangle(left, rect.Top + l1.ContentMargins.Top + (i * vsize), LineWidth() - vsize + 4, 1);
          renderer.Draw(Manager.Skin.Controls["Control"].Layers[0].Image.Resource, r, l1.Text.Colors.Enabled);
        }
        if (ItemIndex != i)
        {                    
          if (Items[i].Enabled)
          {
            Rectangle r = new Rectangle(left, top, LineWidth() - vsize, h);        
            renderer.DrawString(this, l1, Items[i].Text, r, false);        
            col = l1.Text.Colors.Enabled;
          }
          else
          {
            Rectangle r = new Rectangle(left + l1.Text.OffsetX,
                                        top + l1.Text.OffsetY, 
                                        LineWidth() - vsize, h);
            renderer.DrawString(l1.Text.Font.Resource, Items[i].Text, r, l1.Text.Colors.Disabled, l1.Text.Alignment);
            col = l1.Text.Colors.Disabled;
          }  
        }
        else
        {    
          if (Items[i].Enabled)
          {                  
            Rectangle rs = new Rectangle(rect.Left + l1.ContentMargins.Left, 
                                         top, 
                                         Width - (l1.ContentMargins.Horizontal - Skin.OriginMargins.Horizontal), 
                                         h);
            renderer.DrawLayer(this, l2, rs);

            Rectangle r = new Rectangle(left,
                                        top, LineWidth() - vsize, h);

            renderer.DrawString(this, l2, Items[i].Text, r, false);
            col = l2.Text.Colors.Enabled;
          }  
          else
          {
            Rectangle rs = new Rectangle(rect.Left + l1.ContentMargins.Left,
                                         top,
                                         Width - (l1.ContentMargins.Horizontal - Skin.OriginMargins.Horizontal),
                                         vsize);
            renderer.DrawLayer(l2, rs, l2.States.Disabled.Color, l2.States.Disabled.Index);
            
            Rectangle r = new Rectangle(left + l1.Text.OffsetX,
                                        top + l1.Text.OffsetY,
                                        LineWidth() - vsize, h);
            renderer.DrawString(l2.Text.Font.Resource, Items[i].Text, r, l2.Text.Colors.Disabled, l2.Text.Alignment);
            col = l2.Text.Colors.Disabled;
          }
                    
        }
        
        if (Items[i].Image != null)
        {                     
          Rectangle r = new Rectangle(rect.Left + l1.ContentMargins.Left + 3, 
                                      rect.Top + top + 3, 
                                      LineHeight() - 6, 
                                      LineHeight() - 6);
          renderer.Draw(Items[i].Image, r, Color.White);
        }     
        
        if (Items[i].Items != null && Items[i].Items.Count > 0)
        {          
          renderer.Draw(Manager.Skin.Images["Shared.ArrowRight"].Resource, rect.Left + LineWidth() - 4, rect.Top + l1.ContentMargins.Top + (i * vsize) + 8, col);
        }     
      }
    }
    ////////////////////////////////////////////////////////////////////////////     

    ////////////////////////////////////////////////////////////////////////////
    private int LineHeight()
    {
      int h = 0;
      if (Items.Count > 0)
      {
        SkinLayer l = Skin.Layers["Control"];
        h = (int)l.Text.Font.Resource.LineSpacing + 9; 
      }
      return h;
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    private int LineWidth()
    {
      int w = 0;
      SkinFont font = Skin.Layers["Control"].Text.Font;
      if (Items.Count > 0)
      {
        for (int i = 0; i < Items.Count; i++)
        {
          int wx = (int)font.Resource.MeasureString(Items[i].Text).X + 16;
          if (wx > w) w = wx;
        }        
      }      
      
      w += 4 + LineHeight();
      
      return w;
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    private void AutoSize()
    {      
      SkinText font = Skin.Layers["Control"].Text;
      if (Items != null && Items.Count > 0)
      {        
        Height = (LineHeight() * Items.Count) + (Skin.Layers["Control"].ContentMargins.Vertical - Skin.OriginMargins.Vertical);        
        Width = LineWidth() + (Skin.Layers["Control"].ContentMargins.Horizontal - Skin.OriginMargins.Horizontal) + font.OffsetX; 
      }
      else
      {
        Height = 16;
        Width = 16;
      }    
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    private void TrackItem(int x, int y)
    {            
      if (Items != null && Items.Count > 0)
      {
        SkinText font = Skin.Layers["Control"].Text;
        int h = LineHeight();
        y -= Skin.Layers["Control"].ContentMargins.Top;
        int i = (int)((float)y / h);
        if (i < Items.Count)
        {
          if (i != ItemIndex && Items[i].Enabled)
          {
            if (ChildMenu != null)
            {
              this.HideMenu(false);
            }

            if (i >= 0 && i != ItemIndex)
            {
              Items[i].SelectedInvoke(new EventArgs());              
            } 
            
            Focused = true;
            ItemIndex = i;  
            timer = (long)TimeSpan.FromTicks(DateTime.Now.Ticks).TotalMilliseconds;            
          }
          else if (!Items[i].Enabled && ChildMenu == null)
          {
            ItemIndex = -1;
          }                              
        }  
        Invalidate();      
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      TrackItem(e.Position.X, e.Position.Y);
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////
    protected internal override void Update(GameTime gameTime)
    {      
      base.Update(gameTime);
      
      AutoSize();
            
      long time = (long)TimeSpan.FromTicks(DateTime.Now.Ticks).TotalMilliseconds;            
     
      if (timer != 0 && time - timer >= Manager.MenuDelay && ItemIndex >= 0 && Items[ItemIndex].Items.Count > 0 && ChildMenu == null)
      {
        OnClick(new MouseEventArgs(new MouseState(), MouseButton.Left, Point.Zero));                  
      }  
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    protected override void OnMouseOut(MouseEventArgs e)
    {
      base.OnMouseOut(e);
            
      if (!CheckArea(e.State.X, e.State.Y) && ChildMenu == null)
      {
        ItemIndex = -1;
      }  
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    protected override void OnClick(EventArgs e)
    {
      if (sender != null && !(sender is MenuBase)) sender.Focused = true;   
      base.OnClick(e);
      timer = 0;

      MouseEventArgs ex = (e is MouseEventArgs) ? (MouseEventArgs)e : new MouseEventArgs();

      if (ex.Button == MouseButton.Left || ex.Button == MouseButton.None)
      {      
        if (ItemIndex >= 0 && Items[ItemIndex].Enabled)
        {
          if (ItemIndex >= 0 && Items[ItemIndex].Items != null && Items[ItemIndex].Items.Count > 0)
          {
            if (ChildMenu == null)
            {
              ChildMenu = new ContextMenu(Manager);         
              (ChildMenu as ContextMenu).RootMenu = this.RootMenu;
              (ChildMenu as ContextMenu).ParentMenu = this;
              (ChildMenu as ContextMenu).sender = sender;
              ChildMenu.Items.AddRange(Items[ItemIndex].Items);
              (ChildMenu as ContextMenu).AutoSize();                    
            }  
            int y = AbsoluteTop + Skin.Layers["Control"].ContentMargins.Top + (ItemIndex * LineHeight());
            (ChildMenu as ContextMenu).Show(sender, AbsoluteLeft + Width - 1, y);
            if (ex.Button == MouseButton.None) (ChildMenu as ContextMenu).ItemIndex = 0;
          }
          else
          {                                
            if (ItemIndex >= 0)
            {
              Items[ItemIndex].ClickInvoke(ex);
            }          
            if (RootMenu is ContextMenu) (RootMenu as ContextMenu).HideMenu(true);
            else if (RootMenu is MainMenu)
            {           
              (RootMenu as MainMenu).HideMenu();            
            }            
          }      
        }  
      }  
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////
    protected override void OnKeyPress(KeyEventArgs e)
    {
      base.OnKeyPress(e);
      
      timer = 0;
      
      if (e.Key == Keys.Down || (e.Key == Keys.Tab && !e.Shift))
      {
        e.Handled = true;
        ItemIndex += 1;
      }  
      
      if (e.Key == Keys.Up || (e.Key == Keys.Tab && e.Shift))
      {
        e.Handled = true;
        ItemIndex -=1;        
      }  
      
      if (ItemIndex > Items.Count - 1) ItemIndex = 0;            
      if (ItemIndex < 0) ItemIndex = Items.Count - 1;
      
      if (e.Key == Keys.Right && Items[ItemIndex].Items.Count > 0)
      {
        e.Handled = true;
        OnClick(new MouseEventArgs(new MouseState(), MouseButton.None, Point.Zero));
      }  
      if (e.Key == Keys.Left)
      {
        e.Handled = true;
        if (ParentMenu != null && ParentMenu is ContextMenu)
        {
          (ParentMenu as ContextMenu).Focused = true;
          (ParentMenu as ContextMenu).HideMenu(false);        
        }        
      }       
      if (e.Key == Keys.Escape)
      {
        e.Handled = true;
        if (ParentMenu != null) ParentMenu.Focused = true;        
        HideMenu(true);
      }  
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    protected override void OnGamePadPress(GamePadEventArgs e)
    {      
      timer = 0;      

      if (e.Button == GamePadButton.None) return;
      
      if (e.Button == GamePadActions.Down || e.Button == GamePadActions.NextControl)
      {
        e.Handled = true;
        ItemIndex += 1;
      }  
      else if (e.Button == GamePadActions.Up || e.Button == GamePadActions.PrevControl)
      {
        e.Handled = true;
        ItemIndex -= 1;
      }  

      if (ItemIndex > Items.Count - 1) ItemIndex = 0;
      if (ItemIndex < 0) ItemIndex = Items.Count - 1;

      if (e.Button == GamePadActions.Right && Items[ItemIndex].Items.Count > 0)
      {
        e.Handled = true;
        OnClick(new MouseEventArgs(new MouseState(), MouseButton.None, Point.Zero));
      }  
      if (e.Button == GamePadActions.Left)
      {
        e.Handled = true;
        if (ParentMenu != null && ParentMenu is ContextMenu)
        {
          (ParentMenu as ContextMenu).Focused = true;
          (ParentMenu as ContextMenu).HideMenu(false);
        }
      }
      
      base.OnGamePadPress(e);
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    public virtual void HideMenu(bool hideCurrent)
    {                
      if (hideCurrent)
      {
        Visible = false;
        ItemIndex = -1;                
      }  
      if (ChildMenu != null)
      {
        (ChildMenu as ContextMenu).HideMenu(true);
        ChildMenu.Dispose();
        ChildMenu = null;
      }                 
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////
    public override void Show()
    {     
      Show(null, Left, Top);
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////
    public virtual void Show(Control sender, int x, int y)
    {
      AutoSize();
      base.Show();  
      if (!Initialized) Init();          
      if (sender != null && sender.Root != null && sender.Root is Container)
      {
        (sender.Root as Container).Add(this, false);
      }  
      else
      {
        Manager.Add(this);
      }
        
      this.sender = sender;

      if (sender != null && sender.Root != null && sender.Root is Container)
      {
        Left = x - Root.AbsoluteLeft;
        Top = y - Root.AbsoluteTop;                      
      }
      else
      {
        Left = x;
        Top = y;      
      }
      
      if (AbsoluteLeft + Width > Manager.TargetWidth)
      {
        Left = Left - Width;
        if (ParentMenu != null && ParentMenu is ContextMenu)
        {
          Left = Left - ParentMenu.Width + 2;
        }
        else if (ParentMenu != null)
        {
          Left = Manager.TargetWidth - (Parent != null ? Parent.AbsoluteLeft : 0) - Width - 2;
        }
      }
      if (AbsoluteTop + Height > Manager.TargetHeight)
      {
        Top = Top - Height;
        if (ParentMenu != null && ParentMenu is ContextMenu)
        {
          Top = Top + LineHeight();
        }
        else if (ParentMenu != null)
        {
          Top = ParentMenu.Top - Height - 1;
        }  
      }           
      
      Focused = true;
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////    
    private void  Input_MouseDown(object sender, MouseEventArgs e)
    {   
      if ((RootMenu is ContextMenu) && !(RootMenu as ContextMenu).CheckArea(e.Position.X, e.Position.Y) && Visible)
      {
        HideMenu(true);
      }
      else if ((RootMenu is MainMenu) && RootMenu.ChildMenu != null && !(RootMenu.ChildMenu as ContextMenu).CheckArea(e.Position.X, e.Position.Y) && Visible)
      {
        (RootMenu as MainMenu).HideMenu();
      }
    }        
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    private bool CheckArea(int x, int y)
    {
      if (Visible)
      {
        if (x <= AbsoluteLeft ||
            x >= AbsoluteLeft + Width ||
            y <= AbsoluteTop ||
            y >= AbsoluteTop + Height)
        {        
          bool ret = false;          
          if (ChildMenu != null)
          {
            ret = (ChildMenu as ContextMenu).CheckArea(x, y);
          }  
          return ret;
        }  
        else
        {
          return true;
        }        
      }
      else
      {
        return false;
      }      
    }
    ////////////////////////////////////////////////////////////////////////////

    #endregion  

  }

}
