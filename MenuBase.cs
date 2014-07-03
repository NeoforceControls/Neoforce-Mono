////////////////////////////////////////////////////////////////
//                                                            //
//  Neoforce Controls                                         //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//         File: MenuBase.cs                                  //
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

  public class MenuItem: Unknown
  {    

    #region //// Fields ////////////

    ////////////////////////////////////////////////////////////////////////////
    public string Text = "MenuItem";
    public List<MenuItem> Items = new List<MenuItem>();
    public bool Separated = false;     
    public Texture2D Image = null;
    public bool Enabled = true;
    public object Tag { get; set; }
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Constructors //////

    ////////////////////////////////////////////////////////////////////////////
    public MenuItem()
    {
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    public MenuItem(string text): this()
    {
      Text = text;
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    public MenuItem(string text, bool separated): this(text)
    {      
      Separated = separated;
    }
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Methods ///////////

    ////////////////////////////////////////////////////////////////////////////
    public event EventHandler Click;
    public event EventHandler Selected;
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////
    internal void ClickInvoke(EventArgs e)
    {
      if (Click != null) Click.Invoke(this, e);
    } 
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    internal void SelectedInvoke(EventArgs e)
    {
      if (Selected != null) Selected.Invoke(this, e);
    }
    ////////////////////////////////////////////////////////////////////////////

    #endregion
    
  } 

  public abstract class MenuBase: Control
  {

    #region //// Fields ////////////

    ////////////////////////////////////////////////////////////////////////////                 
    private int itemIndex = -1;
    private List<MenuItem> items = new List<MenuItem>();
    private MenuBase childMenu = null;   
    private MenuBase rootMenu = null;
    private MenuBase parentMenu = null;    
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Properties ////////

    ////////////////////////////////////////////////////////////////////////////
    protected internal int ItemIndex { get { return itemIndex; } set { itemIndex = value; } }
    protected internal MenuBase ChildMenu { get { return childMenu; } set { childMenu = value; } }
    protected internal MenuBase RootMenu { get { return rootMenu; } set { rootMenu = value; } }
    protected internal MenuBase ParentMenu { get { return parentMenu; } set { parentMenu = value; } }    
    public List<MenuItem> Items { get { return items; } }    
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Construstors //////

    ////////////////////////////////////////////////////////////////////////////       
    public MenuBase(Manager manager): base(manager)
    {            
      rootMenu = this;            
    }
    ////////////////////////////////////////////////////////////////////////////

    #endregion
        
  }

}
