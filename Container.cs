////////////////////////////////////////////////////////////////
//                                                            //
//  Neoforce Controls                                         //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//         File: Container.cs                                 //
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
////////////////////////////////////////////////////////////////////////////

#endregion

namespace TomShane.Neoforce.Controls
{

  public struct ScrollBarValue
  {
    public int Vertical;
    public int Horizontal;
  }
     
  public class Container: ClipControl
  {   
   
    #region //// Fields ////////////
    
    ////////////////////////////////////////////////////////////////////////////        
    private ScrollBar sbVert;
    private ScrollBar sbHorz;
    private MainMenu mainMenu;
    private ToolBarPanel toolBarPanel;   
    private StatusBar statusBar;
    private bool autoScroll = false;
    private Control defaultControl = null;   

    /// <summary>
    /// Scroll by PageSize (true) or StepSize (false)
    /// </summary>
    private bool scrollAlot = true;
    ////////////////////////////////////////////////////////////////////////////

    #endregion   

    #region //// Properties ////////
   
    ////////////////////////////////////////////////////////////////////////////
    public virtual ScrollBarValue ScrollBarValue
    {
      get
      {
        ScrollBarValue scb = new ScrollBarValue();
        scb.Vertical = (sbVert != null ? sbVert.Value : 0);
        scb.Horizontal = (sbHorz != null ? sbHorz.Value : 0);
        return scb;
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    public override bool Visible
    {
      get
      {
        return base.Visible;        
      }
      set
      {   
        if (value)
        {
          if (DefaultControl != null)
          {
            DefaultControl.Focused = true;  
          }        
        }
        base.Visible = value;
      }
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////
    public virtual Control DefaultControl
    {
      get { return defaultControl; }
      set { defaultControl = value; }
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////
    public virtual bool AutoScroll
    {
      get { return autoScroll; }
      set { autoScroll = value; }
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////    
    public virtual MainMenu MainMenu
    {
      get { return mainMenu; }
      set
      {
        if (mainMenu != null)
        {
          mainMenu.Resize -= Bars_Resize;
          Remove(mainMenu);
        }
        mainMenu = value;
        
        if (mainMenu != null)
        {
          Add(mainMenu, false);
          mainMenu.Resize += Bars_Resize;          
        }
        AdjustMargins();
      }
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////
    public virtual ToolBarPanel ToolBarPanel
    {
      get 
      { 
        return toolBarPanel; 
      }
      set
      {
        if (toolBarPanel != null)
        {
          toolBarPanel.Resize -= Bars_Resize;
          Remove(toolBarPanel);
        }
        toolBarPanel = value;
        
        if (toolBarPanel != null)
        {
          Add(toolBarPanel, false);
          toolBarPanel.Resize += Bars_Resize;          
        }
        AdjustMargins();
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    public virtual StatusBar StatusBar
    {
      get
      {
        return statusBar;
      }
      set
      {
        if (statusBar != null)
        {
          statusBar.Resize -= Bars_Resize;
          Remove(statusBar);        
        }  
        statusBar = value;
        
        if (statusBar != null)
        {
          Add(statusBar, false);
          statusBar.Resize += Bars_Resize;
        }
        AdjustMargins();
      }
    }    
    ////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    /// Scroll by PageSize (true) or StepSize (false)
    /// </summary>
    public virtual bool ScrollAlot
    {
        get { return this.scrollAlot; }
        set { this.scrollAlot = value; }
    }
 
    /// <summary>
    /// Gets the container's vertical scroll bar.
    /// </summary>
    protected virtual ScrollBar VerticalScrollBar
    {
        get { return this.sbVert; }
    }
 
    /// <summary>
    /// Gets the container's horizontal scroll bar.
    /// </summary>
    protected virtual ScrollBar HorizontalScrollBar
    {
        get { return this.sbHorz; }
    }

    #endregion
    
 	  #region //// Constructors //////
	 		
	  ////////////////////////////////////////////////////////////////////////////
		public Container(Manager manager): base(manager)
    {                                           
      sbVert = new ScrollBar(manager, Orientation.Vertical);
      sbVert.Init();      
      sbVert.Detached = false;
      sbVert.Anchor = Anchors.Top | Anchors.Right | Anchors.Bottom;
      sbVert.ValueChanged += new EventHandler(ScrollBarValueChanged);                 
      sbVert.Range = 0;
      sbVert.PageSize = 0;
      sbVert.Value = 0;
      sbVert.Visible = false;

      sbHorz = new ScrollBar(manager, Orientation.Horizontal);
      sbHorz.Init();
      sbHorz.Detached = false;
      sbHorz.Anchor = Anchors.Right | Anchors.Left | Anchors.Bottom;
      sbHorz.ValueChanged += new EventHandler(ScrollBarValueChanged);
      sbHorz.Range = 0;
      sbHorz.PageSize = 0;
      sbHorz.Value = 0;
      sbHorz.Visible = false;
            
      Add(sbVert, false);
      Add(sbHorz, false);
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
    private void Bars_Resize(object sender, ResizeEventArgs e)
    {
      AdjustMargins();      
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////  
    protected override void AdjustMargins()
    {
      Margins m = Skin.ClientMargins;
      
      if (this.GetType() != typeof(Container))
      {
        m = ClientMargins;
      } 
            
      if (mainMenu != null && mainMenu.Visible)
      {
        if (!mainMenu.Initialized) mainMenu.Init();
        mainMenu.Left = m.Left;
        mainMenu.Top = m.Top;
        mainMenu.Width = Width - m.Horizontal;
        mainMenu.Anchor = Anchors.Left | Anchors.Top | Anchors.Right;                
     
        m.Top += mainMenu.Height;        
      }
      if (toolBarPanel != null && toolBarPanel.Visible)
      {
        if (!toolBarPanel.Initialized) toolBarPanel.Init();
        toolBarPanel.Left = m.Left;
        toolBarPanel.Top = m.Top;
        toolBarPanel.Width = Width - m.Horizontal;
        toolBarPanel.Anchor = Anchors.Left | Anchors.Top | Anchors.Right;        
        
        m.Top += toolBarPanel.Height;
      }
      if (statusBar != null && statusBar.Visible)
      {                
        if (!statusBar.Initialized) statusBar.Init();        
        statusBar.Left = m.Left;
        statusBar.Top = Height - m.Bottom - statusBar.Height;
        statusBar.Width = Width - m.Horizontal;
        statusBar.Anchor = Anchors.Left | Anchors.Bottom | Anchors.Right;        

        m.Bottom += statusBar.Height;
      }
      if (sbVert != null && sbVert.Visible)
      {
        m.Right += (sbVert.Width + 2);
      }
      if (sbHorz != null && sbHorz.Visible)
      {
        m.Bottom += (sbHorz.Height + 2);
      }            
      
      ClientMargins = m;            
      
      PositionScrollBars();
      
      base.AdjustMargins();
    }
    ////////////////////////////////////////////////////////////////////////////  
    
    ////////////////////////////////////////////////////////////////////////////
    public override void Add(Control control, bool client)
    {
      base.Add(control, client);
      CalcScrolling();
    }
    ////////////////////////////////////////////////////////////////////////////           

    ////////////////////////////////////////////////////////////////////////////
    protected internal override void Update(GameTime gameTime)
    {      
      base.Update(gameTime);                     
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////
    protected override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
    {      
      base.DrawControl(renderer, rect, gameTime);
    }
    ////////////////////////////////////////////////////////////////////////////  

    ////////////////////////////////////////////////////////////////////////////  
    protected internal override void OnSkinChanged(EventArgs e)
    {
      base.OnSkinChanged(e);
      if (sbVert != null && sbHorz != null)
      {
        sbVert.Visible = false;
        sbHorz.Visible = false;
        CalcScrolling();
      }  
    }
    ////////////////////////////////////////////////////////////////////////////  

    ////////////////////////////////////////////////////////////////////////////
    private void PositionScrollBars()
    {
      if (sbVert != null)
      {
        sbVert.Left = ClientLeft + ClientWidth + 1;
        sbVert.Top = ClientTop + 1;
        int m = (sbHorz != null && sbHorz.Visible) ? 0 : 2;
        sbVert.Height = ClientArea.Height - m;  
        sbVert.Range = ClientArea.VirtualHeight;
        sbVert.PageSize = ClientArea.ClientHeight;
      }
      
      if (sbHorz != null)
      {
        sbHorz.Left = ClientLeft + 1;
        sbHorz.Top = ClientTop + ClientHeight + 1;
        int m = (sbVert != null && sbVert.Visible) ? 0 : 2;              
        sbHorz.Width = ClientArea.Width - m;
        sbHorz.Range = ClientArea.VirtualWidth;
        sbHorz.PageSize = ClientArea.ClientWidth;
      }  
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////
    private void CalcScrolling()
    {                      
      if (sbVert != null && autoScroll)
      {                
        bool vis = sbVert.Visible;        
        sbVert.Visible = ClientArea.VirtualHeight > ClientArea.ClientHeight;                
        if (ClientArea.VirtualHeight <= ClientArea.ClientHeight) sbVert.Value = 0;
        
        if (vis != sbVert.Visible)
        {
          if (!sbVert.Visible)
          {
            foreach (Control c in ClientArea.Controls)
            {
              c.TopModifier = 0;
              c.Invalidate();
            }
          }
          AdjustMargins(); 
        }
        
        PositionScrollBars();                                           
        foreach (Control c in ClientArea.Controls)
        {
          c.TopModifier = -sbVert.Value;
          c.Invalidate();
        }    
      }

      if (sbHorz != null && autoScroll)
      {
        bool vis = sbHorz.Visible;
        sbHorz.Visible = ClientArea.VirtualWidth > ClientArea.ClientWidth;     
        if (ClientArea.VirtualWidth <= ClientArea.ClientWidth) sbHorz.Value = 0;
        
        if (vis != sbHorz.Visible)
        {
          if (!sbHorz.Visible)
          {   
            foreach (Control c in ClientArea.Controls)
            {
              c.LeftModifier = 0;
              sbVert.Refresh();
              c.Invalidate();
            }
          }
          AdjustMargins();
        }
        
        PositionScrollBars();
        foreach (Control c in ClientArea.Controls)
        {
          c.LeftModifier = -sbHorz.Value;
          sbHorz.Refresh();
          c.Invalidate();
        }
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////    
    public virtual void ScrollTo(int x, int y)
    {
      sbVert.Value = y;
      sbHorz.Value = x;
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    public virtual void ScrollTo(Control control)
    {
      if (control != null && ClientArea != null && ClientArea.Contains(control, true))
      {
        if (control.AbsoluteTop + control.Height > ClientArea.AbsoluteTop + ClientArea.Height)
        {
          sbVert.Value = sbVert.Value + control.AbsoluteTop - ClientArea.AbsoluteTop - sbVert.PageSize + control.Height;
        }
        else if (control.AbsoluteTop < ClientArea.AbsoluteTop)
        {
          sbVert.Value = sbVert.Value + control.AbsoluteTop - ClientArea.AbsoluteTop;
        }
        if (control.AbsoluteLeft + control.Width > ClientArea.AbsoluteLeft + ClientArea.Width)
        {
          sbHorz.Value = sbHorz.Value + control.AbsoluteLeft - ClientArea.AbsoluteLeft - sbHorz.PageSize + control.Width;
        }
        else if (control.AbsoluteLeft < ClientArea.AbsoluteLeft)
        {
          sbHorz.Value = sbHorz.Value + control.AbsoluteLeft - ClientArea.AbsoluteLeft;
        }
      }  
    }
    ////////////////////////////////////////////////////////////////////////////   
    
    ////////////////////////////////////////////////////////////////////////////                  
    void ScrollBarValueChanged(object sender, EventArgs e)
    {
      CalcScrolling();     
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////           
    protected override void OnResize(ResizeEventArgs e)
    {             
      base.OnResize(e);
      CalcScrolling();
      
      // Crappy fix to certain scrolling issue
      //if (sbVert != null) sbVert.Value -= 1; 
      //if (sbHorz != null) sbHorz.Value -= 1;      
    }
    ////////////////////////////////////////////////////////////////////////////   
    
    ////////////////////////////////////////////////////////////////////////////   
    public override void Invalidate()
    {        
      base.Invalidate();
    }
    ////////////////////////////////////////////////////////////////////////////   

    ////////////////////////////////////////////////////////////////////////////   
    protected override void OnClick(EventArgs e)
    {
      MouseEventArgs ex = e as MouseEventArgs;      
      ex.Position = new Point(ex.Position.X + sbHorz.Value, ex.Position.Y + sbVert.Value);
      
      base.OnClick(e);
    }
    ////////////////////////////////////////////////////////////////////////////       
   
    protected override void OnMouseScroll(MouseEventArgs e)
    {
        if (!ClientArea.Enabled)
            return;
 
        // If current control doesn't scroll, scroll the parent control
        if (sbVert.Range - sbVert.PageSize < 1)
        {
            Control c = this;
 
            while (c != null)
            {
                var p = c.Parent as Container;
 
                if (p != null && p.Enabled)
                {
                    p.OnMouseScroll(e);
 
                    break;
                }
 
                c = c.Parent;
            }
 
            return;
        }
 
        if (e.ScrollDirection == MouseScrollDirection.Down)
            sbVert.ScrollDown(ScrollAlot);
        else
            sbVert.ScrollUp(ScrollAlot);
 
        base.OnMouseScroll(e);
    }
    #endregion
 }

}
