////////////////////////////////////////////////////////////////
//                                                            //
//  Neoforce Controls                                         //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//         File: Control.cs                                   //
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
      
  public class ClipControl: Control
  {
   
    #region //// Fields ////////////

    ////////////////////////////////////////////////////////////////////////////    
    private ClipBox clientArea;  
    ////////////////////////////////////////////////////////////////////////////

    #endregion   

    #region //// Properties ////////
   
    ////////////////////////////////////////////////////////////////////////////    
    public virtual ClipBox ClientArea
    {
      get { return clientArea; }
      set { clientArea = value; }
    }   
    ////////////////////////////////////////////////////////////////////////////          

    ////////////////////////////////////////////////////////////////////////////
    public override Margins ClientMargins
    {
      get
      {
        return base.ClientMargins;
      }
      set
      {
        base.ClientMargins = value;
        if (clientArea != null)
        {
          clientArea.Left = ClientLeft;
          clientArea.Top = ClientTop;
          clientArea.Width = ClientWidth;
          clientArea.Height = ClientHeight;        
        }  
      }
    }
    ////////////////////////////////////////////////////////////////////////////
        
    #endregion
    
 	  #region //// Constructors //////
	 		
	  ////////////////////////////////////////////////////////////////////////////
		public ClipControl(Manager manager): base(manager)
    {                              
      clientArea = new ClipBox(manager);      
      
      clientArea.Init();
      clientArea.MinimumWidth = 0;
      clientArea.MinimumHeight = 0;
      clientArea.Left = ClientLeft;
      clientArea.Top = ClientTop;
      clientArea.Width = ClientWidth;
      clientArea.Height = ClientHeight;
                
      base.Add(clientArea);
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
    public virtual void Add(Control control, bool client)
    {
      if (client)
      {
        clientArea.Add(control);        
      }
      else
      {
        base.Add(control);
      }                  
    }
    ////////////////////////////////////////////////////////////////////////////           

    ////////////////////////////////////////////////////////////////////////////
    public override void Add(Control control)
    {
      Add(control, true);
    }
    ////////////////////////////////////////////////////////////////////////////       

    ////////////////////////////////////////////////////////////////////////////       
    public override void Remove(Control control)
    {
      base.Remove(control);
      clientArea.Remove(control);
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
    protected override void OnResize(ResizeEventArgs e)
    {
      base.OnResize(e);
      
      if (clientArea != null)
      {      
        clientArea.Left = ClientLeft;
        clientArea.Top = ClientTop;
        clientArea.Width = ClientWidth;
        clientArea.Height = ClientHeight;
      }  
    }
    ////////////////////////////////////////////////////////////////////////////             

    ////////////////////////////////////////////////////////////////////////////  
    protected virtual void AdjustMargins()
    {
    }
    ////////////////////////////////////////////////////////////////////////////
                         
    #endregion
  }

}
