////////////////////////////////////////////////////////////////
//                                                            //
//  Neoforce Controls                                         //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//         File: MainMenu.cs                                  //
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

  public class MainMenu: MenuBase
  {

    #region //// Fields ////////////

    ////////////////////////////////////////////////////////////////////////////                 
    private Rectangle[] rs;
    private int lastIndex = -1;    
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Properties ////////

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Constructors //////

    ////////////////////////////////////////////////////////////////////////////       
    public MainMenu(Manager manager): base(manager)
    {      
      Left = 0;
      Top = 0;
      Height = 24;
      Detached = false;
      DoubleClicks = false;    
      StayOnBack = true;  
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
      Skin = new SkinControl(Manager.Skin.Controls["MainMenu"]);
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////
    protected override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
    {                                          
      SkinLayer l1 = Skin.Layers["Control"];
      SkinLayer l2 = Skin.Layers["Selection"];
      rs = new Rectangle[Items.Count];       

      renderer.DrawLayer(this, l1, rect, ControlState.Enabled);
      
      int prev = l1.ContentMargins.Left;
      for (int i = 0; i < Items.Count; i++)
      {     
        MenuItem mi = Items[i];

        int tw = (int)l1.Text.Font.Resource.MeasureString(mi.Text).X + l1.ContentMargins.Horizontal;
        rs[i] = new Rectangle(rect.Left + prev, rect.Top + l1.ContentMargins.Top, tw, Height - l1.ContentMargins.Vertical);
        prev += tw;      
        
        if (ItemIndex != i)
        {          
          if (mi.Enabled && Enabled)
          {             
            renderer.DrawString(this, l1, mi.Text, rs[i], ControlState.Enabled, false);
          }
          else
          {
            renderer.DrawString(this, l1, mi.Text, rs[i], ControlState.Disabled, false);
          }  
        }
        else
        {
          if (Items[i].Enabled && Enabled)
          {      
            renderer.DrawLayer(this, l2, rs[i], ControlState.Enabled);
            renderer.DrawString(this, l2, mi.Text, rs[i], ControlState.Enabled, false);           
          }  
          else
          {
            renderer.DrawLayer(this, l2, rs[i], ControlState.Disabled);
            renderer.DrawString(this, l2, mi.Text, rs[i], ControlState.Disabled, false);  
          }
        }  
      }      
    }
    ////////////////////////////////////////////////////////////////////////////  

    ////////////////////////////////////////////////////////////////////////////
    private void TrackItem(int x, int y)
    {          
      if (Items != null && Items.Count > 0 && rs != null)
      {
        Invalidate();
        for (int i = 0; i < rs.Length; i++)
        {
          if (rs[i].Contains(x, y))
          {
            if (i >= 0 && i != ItemIndex)
            {
              Items[i].SelectedInvoke(new EventArgs());              
            } 
            ItemIndex = i;        
            return;
          }
        }
        if (ChildMenu == null) ItemIndex = -1;        
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    private bool CheckArea(int x, int y)
    {
      return true;       
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      int i = lastIndex;      
                  
      TrackItem(e.State.X - Root.AbsoluteLeft, e.State.Y - Root.AbsoluteTop);

      if (ItemIndex >= 0 && (i == -1 || i != ItemIndex) && Items[ItemIndex].Items != null && Items[ItemIndex].Items.Count > 0 && ChildMenu != null)
      {                               
        HideSubMenu();
        lastIndex = ItemIndex; 
        OnClick(e);          
      }
      else if (ChildMenu != null && i != ItemIndex)
      {                 
         HideSubMenu();
         Focused = true;             
      }
    }
    ////////////////////////////////////////////////////////////////////////////   

    ////////////////////////////////////////////////////////////////////////////   
    protected override void OnMouseDown(MouseEventArgs e)
    {
      base.OnMouseDown(e);
    }
    ////////////////////////////////////////////////////////////////////////////   

    ////////////////////////////////////////////////////////////////////////////   
    protected override void OnMouseOut(MouseEventArgs e)
    {
      base.OnMouseOut(e);
      
      OnMouseMove(e);
    }
    ////////////////////////////////////////////////////////////////////////////   
  
    ////////////////////////////////////////////////////////////////////////////   
    private void HideSubMenu()
    {
      if (ChildMenu != null)
      {
        (ChildMenu as ContextMenu).HideMenu(true);
        ChildMenu.Dispose();
        ChildMenu = null;
      }      
    }
    ////////////////////////////////////////////////////////////////////////////   

    ////////////////////////////////////////////////////////////////////////////   
    public virtual void HideMenu()
    {
      if (ChildMenu != null)
      {
        (ChildMenu as ContextMenu).HideMenu(true);
        ChildMenu.Dispose();
        ChildMenu = null;
      }
      if (Manager.FocusedControl is MenuBase) Focused = true;
      Invalidate();
      ItemIndex = -1;
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    protected override void OnClick(EventArgs e)
    {      
      base.OnClick(e);

      MouseEventArgs ex = (e is MouseEventArgs) ? (MouseEventArgs)e : new MouseEventArgs();
      
      if (ex.Button == MouseButton.Left || ex.Button == MouseButton.None)
      {
        if (ItemIndex >= 0 && Items[ItemIndex].Enabled)
        {
          if (ItemIndex >= 0 && Items[ItemIndex].Items != null && Items[ItemIndex].Items.Count > 0)
          {
            if (ChildMenu != null)
            {
              ChildMenu.Dispose();
              ChildMenu = null;
            }
            ChildMenu = new ContextMenu(Manager);
            (ChildMenu as ContextMenu).RootMenu = this;
            (ChildMenu as ContextMenu).ParentMenu = this;
            (ChildMenu as ContextMenu).Sender = this.Root;
            ChildMenu.Items.AddRange(Items[ItemIndex].Items);                

            int y = Root.AbsoluteTop + rs[ItemIndex].Bottom + 1;
            (ChildMenu as ContextMenu).Show(this.Root, Root.AbsoluteLeft + rs[ItemIndex].Left, y);
            if (ex.Button == MouseButton.None) (ChildMenu as ContextMenu).ItemIndex = 0;            
          }        
          else
          {
            if (ItemIndex >= 0)
            {
              Items[ItemIndex].ClickInvoke(ex);
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

      if (e.Key == Keys.Right)
      {
        ItemIndex += 1;
        e.Handled = true;
      }
      if (e.Key == Keys.Left)
      {
        ItemIndex -= 1;
        e.Handled = true;
      } 

      if (ItemIndex > Items.Count - 1) ItemIndex = 0;
      if (ItemIndex < 0) ItemIndex = Items.Count - 1;

      if (e.Key == Keys.Down && Items.Count > 0 && Items[ItemIndex].Items.Count > 0)
      {
        e.Handled = true;
        OnClick(new MouseEventArgs(new MouseState(), MouseButton.None, Point.Zero));        
      }  
      if (e.Key == Keys.Escape)
      {
        e.Handled = true;
        ItemIndex = -1;
      }      
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    protected override void OnGamePadPress(GamePadEventArgs e)
    {
      base.OnGamePadPress(e);

      if (e.Button == GamePadActions.Right)
      {
        ItemIndex += 1;
        e.Handled = true;
      }
      if (e.Button == GamePadActions.Left)
      {
        ItemIndex -= 1;
        e.Handled = true;
      }

      if (ItemIndex > Items.Count - 1) ItemIndex = 0;
      if (ItemIndex < 0) ItemIndex = Items.Count - 1;

      if (e.Button == GamePadActions.Down && Items[ItemIndex].Items.Count > 0)
      {
        e.Handled = true;
        OnClick(new MouseEventArgs(new MouseState(), MouseButton.None, Point.Zero));
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    protected override void OnFocusGained(EventArgs e)
    {
      base.OnFocusGained(e);  
      if (ItemIndex < 0 && Items.Count > 0) ItemIndex = 0;    
    }    
    ////////////////////////////////////////////////////////////////////////////    
    
    ////////////////////////////////////////////////////////////////////////////    
    protected override void OnFocusLost(EventArgs e)
    {
      base.OnFocusLost(e);
      if (ChildMenu == null || !ChildMenu.Visible) ItemIndex = -1;
    }
    ////////////////////////////////////////////////////////////////////////////    
      
    #endregion  

  }

}
