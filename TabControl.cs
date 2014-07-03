////////////////////////////////////////////////////////////////
//                                                            //
//  Neoforce Controls                                         //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//         File: TabControl.cs                                //
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
////////////////////////////////////////////////////////////////////////////

#endregion

namespace TomShane.Neoforce.Controls
{

  ////////////////////////////////////////////////////////////////////////////
  public class TabControlGamePadActions: GamePadActions
  {
    public GamePadButton NextTab = GamePadButton.RightTrigger;
    public GamePadButton PrevTab = GamePadButton.LeftTrigger;
  }
  ////////////////////////////////////////////////////////////////////////////
  
  ////////////////////////////////////////////////////////////////////////////
  public class TabPage: Control
  {
  
    #region //// Fields ////////////

    ////////////////////////////////////////////////////////////////////////////                 
    private Rectangle headerRect = Rectangle.Empty;
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Properties ////////

    ////////////////////////////////////////////////////////////////////////////                     
    protected internal Rectangle HeaderRect
    {
      get { return headerRect; }
    }    
    ////////////////////////////////////////////////////////////////////////////

    #endregion
   
    #region //// Constructors //////

    ////////////////////////////////////////////////////////////////////////////
    public TabPage(Manager manager): base(manager)
    {
      Color = Color.Transparent;
      Passive = true;
      CanFocus = false;      
    }
    ////////////////////////////////////////////////////////////////////////////

    #endregion      

		#region //// Methods ///////////
			
		////////////////////////////////////////////////////////////////////////////
		protected internal void CalcRect(Rectangle prev, SpriteFont font, Margins margins, Point offset, bool first)
    {
      int size = (int)Math.Ceiling(font.MeasureString(Text).X) + margins.Horizontal;
      
      if (first) offset.X = 0;
      
      headerRect = new Rectangle(prev.Right + offset.X, prev.Top, size, prev.Height);
    }
   	////////////////////////////////////////////////////////////////////////////
			
		#endregion  
		
  }
  ////////////////////////////////////////////////////////////////////////////
  
  ////////////////////////////////////////////////////////////////////////////
  public class TabControl: Container
  {

    #region //// Fields ////////////

    ////////////////////////////////////////////////////////////////////////////                 
    private List<TabPage> tabPages = new List<TabPage>();           
    private int selectedIndex = 0;
    private int hoveredIndex = -1;    
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Properties ////////

    ////////////////////////////////////////////////////////////////////////////
    public TabPage[] TabPages
    {
      get { return tabPages.ToArray(); }      
    }    
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    public virtual int SelectedIndex
    {
      get { return selectedIndex; }
      set
      {
        if (selectedIndex >= 0 && selectedIndex < tabPages.Count && value >= 0 && value < tabPages.Count)
        {
          TabPages[selectedIndex].Visible = false;
        }
        if (value >= 0 && value < tabPages.Count)
        {
          TabPages[value].Visible = true;          
          ControlsList c = TabPages[value].Controls as ControlsList;          
          if (c.Count > 0) c[0].Focused = true;
          selectedIndex = value;
          if (!Suspended) OnPageChanged(new EventArgs());
        }
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    public virtual TabPage SelectedPage
    {
      get { return tabPages[SelectedIndex]; }
      set
      {
        for (int i = 0; i < tabPages.Count; i++)
        {
          if (tabPages[i] == value)
          {
            SelectedIndex = i;
            break;
          }
        }
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Events ////////////

    ////////////////////////////////////////////////////////////////////////////
    public event EventHandler PageChanged;
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Construstors //////

    ////////////////////////////////////////////////////////////////////////////       
    public TabControl(Manager manager): base(manager)
    {
      GamePadActions = new TabControlGamePadActions();
      Manager.Input.GamePadDown += new GamePadEventHandler(Input_GamePadDown);
      this.CanFocus = false;
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
      SkinLayer l1 = Skin.Layers["Control"];
      SkinLayer l2 = Skin.Layers["Header"];
      Color col = this.Color != UndefinedColor ? this.Color : Color.White;
      
      Rectangle r1 = new Rectangle(rect.Left, rect.Top + l1.OffsetY, rect.Width, rect.Height - l1.OffsetY);
      if (tabPages.Count <= 0)
      {
        r1 = rect;
      }
            
      base.DrawControl(renderer, r1, gameTime);               
    
      if (tabPages.Count > 0)
      {

        Rectangle prev = new Rectangle(rect.Left, rect.Top + l2.OffsetY, 0, l2.Height);
        for (int i = 0; i < tabPages.Count; i++)
        {                   
          SpriteFont font = l2.Text.Font.Resource;
          Margins margins = l2.ContentMargins;
          Point offset = new Point(l2.OffsetX, l2.OffsetY);          
          if (i > 0) prev = tabPages[i - 1].HeaderRect;
                    
          tabPages[i].CalcRect(prev, font, margins, offset, i==0);
        }
      
        for (int i = tabPages.Count - 1; i >= 0; i--)
        {
          int li = tabPages[i].Enabled ? l2.States.Enabled.Index : l2.States.Disabled.Index;
          Color lc = tabPages[i].Enabled ? l2.Text.Colors.Enabled : l2.Text.Colors.Disabled;
          if (i == hoveredIndex)
          {
            li = l2.States.Hovered.Index;
            lc = l2.Text.Colors.Hovered;
          }
          
          
          Margins m = l2.ContentMargins;
          Rectangle rx = tabPages[i].HeaderRect;          
          Rectangle sx = new Rectangle(rx.Left + m.Left, rx.Top + m.Top, rx.Width - m.Horizontal, rx.Height - m.Vertical);
          if (i != selectedIndex)
          {
            renderer.DrawLayer(l2, rx, col, li);   
            renderer.DrawString(l2.Text.Font.Resource, tabPages[i].Text, sx, lc, l2.Text.Alignment);
          }  
        }
        
        Margins mi = l2.ContentMargins;
        Rectangle ri = tabPages[selectedIndex].HeaderRect;
        Rectangle si = new Rectangle(ri.Left + mi.Left, ri.Top + mi.Top, ri.Width - mi.Horizontal, ri.Height - mi.Vertical);
        renderer.DrawLayer(l2, ri, col, l2.States.Focused.Index);        
        renderer.DrawString(l2.Text.Font.Resource, tabPages[selectedIndex].Text, si, l2.Text.Colors.Focused, l2.Text.Alignment, l2.Text.OffsetX, l2.Text.OffsetY, false);
      }      
    }
    ////////////////////////////////////////////////////////////////////////////     

    ////////////////////////////////////////////////////////////////////////////     
    public virtual TabPage AddPage(string text)
    {
      TabPage p = AddPage();
      p.Text = text;
      
      return p;
    }
    ////////////////////////////////////////////////////////////////////////////     
    
    ////////////////////////////////////////////////////////////////////////////     
    public virtual TabPage AddPage()
    {
      TabPage page = new TabPage(Manager);
      page.Init();      
      page.Left = 0;
      page.Top = 0;
      page.Width = ClientWidth;
      page.Height = ClientHeight;
      page.Anchor = Anchors.All;
      page.Text = "Tab " + (tabPages.Count + 1).ToString();
      page.Visible = false;      
      Add(page, true);
      tabPages.Add(page);
      tabPages[0].Visible = true;
      
      return page;
    }
    ////////////////////////////////////////////////////////////////////////////     

    ////////////////////////////////////////////////////////////////////////////     
    public virtual void RemovePage(TabPage page, bool dispose)
    {      
      tabPages.Remove(page);
      if (dispose)
      {
        page.Dispose();
        page = null;
      }
      SelectedIndex = 0;
    }
    ////////////////////////////////////////////////////////////////////////////         

    ////////////////////////////////////////////////////////////////////////////         
    public virtual void RemovePage(TabPage page)
    {
      RemovePage(page, true);
    }
    ////////////////////////////////////////////////////////////////////////////         

    ////////////////////////////////////////////////////////////////////////////     
    protected override void OnMouseDown(MouseEventArgs e)
    {
      base.OnMouseDown(e);
      
      if (tabPages.Count > 1)
      {
        Point p = new Point(e.State.X - Root.AbsoluteLeft, e.State.Y - Root.AbsoluteTop);
        for (int i = 0; i < tabPages.Count; i++)
        {
          Rectangle r = tabPages[i].HeaderRect;
          if (p.X >= r.Left && p.X <= r.Right && p.Y >= r.Top && p.Y <= r.Bottom)
          {
            SelectedIndex = i;          
            break;
          }
        }        
      }      
    }
    ////////////////////////////////////////////////////////////////////////////     

    ////////////////////////////////////////////////////////////////////////////     
    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e); 
      if (tabPages.Count > 1)
      {
        int index = hoveredIndex;
        Point p = new Point(e.State.X - Root.AbsoluteLeft, e.State.Y - Root.AbsoluteTop);
        for (int i = 0; i < tabPages.Count; i++)
        {
          Rectangle r = tabPages[i].HeaderRect;
          if (p.X >= r.Left && p.X <= r.Right && p.Y >= r.Top && p.Y <= r.Bottom && tabPages[i].Enabled)
          {
            index = i;
            break;
          }
          else
          {
            index = -1;
          }         
        }
        if (index != hoveredIndex)
        {
          hoveredIndex = index;
          Invalidate();
        }
      }      
    }
    ////////////////////////////////////////////////////////////////////////////     
    
    ////////////////////////////////////////////////////////////////////////////     
    void Input_GamePadDown(object sender, GamePadEventArgs e)
    {
      if (this.Contains(Manager.FocusedControl, true))
      {
        if (e.Button == (GamePadActions as TabControlGamePadActions).NextTab)
        {
          e.Handled = true;
          SelectedIndex += 1;
        }
        else if (e.Button == (GamePadActions as TabControlGamePadActions).PrevTab)
        {
          e.Handled = true;
          SelectedIndex -= 1;
        }
      }  
    }
    ////////////////////////////////////////////////////////////////////////////     

    ////////////////////////////////////////////////////////////////////////////
    protected virtual void OnPageChanged(EventArgs e)
    {
      if (PageChanged != null) PageChanged.Invoke(this, e);
    }
    ////////////////////////////////////////////////////////////////////////////


    #endregion  

  }

}
