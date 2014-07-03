////////////////////////////////////////////////////////////////
//                                                            //
//  Neoforce Controls                                         //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//         File: ModalContainer.cs                            //
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
////////////////////////////////////////////////////////////////////////////

#endregion

namespace TomShane.Neoforce.Controls
{
   
  public class ModalContainer: Container
  {

    #region //// Fields ////////////

    ////////////////////////////////////////////////////////////////////////////
    private ModalResult modalResult = ModalResult.None;
    private ModalContainer lastModal = null; 
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Properties ////////

    ////////////////////////////////////////////////////////////////////////////
    public override bool Visible
    {
      get
      {
        return base.Visible;
      }
      set
      {
        if (value) Focused = true;
        base.Visible = value;
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    public virtual bool IsModal
    {
      get { return Manager.ModalWindow == this; }
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    public virtual ModalResult ModalResult
    {
      get
      {
        return modalResult;
      }
      set
      {
        modalResult = value;
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Events ////////////

    ////////////////////////////////////////////////////////////////////////////
    public event WindowClosingEventHandler Closing;
    public event WindowClosedEventHandler Closed;
    ////////////////////////////////////////////////////////////////////////////

    #endregion
  
    #region //// Consructors ///////

    ////////////////////////////////////////////////////////////////////////////
    public ModalContainer(Manager manager): base(manager)
    {      
      Manager.Input.GamePadDown += new GamePadEventHandler(Input_GamePadDown);
      GamePadActions = new WindowGamePadActions();
    }
    ////////////////////////////////////////////////////////////////////////////

    #endregion 

    #region //// Methods ///////////

    ////////////////////////////////////////////////////////////////////////////
    public virtual void ShowModal()
    {
      lastModal = Manager.ModalWindow;
      Manager.ModalWindow = this;
      Manager.Input.KeyDown += new KeyEventHandler(Input_KeyDown);
      Manager.Input.GamePadDown += new GamePadEventHandler(Input_GamePadDown);
    }
    ////////////////////////////////////////////////////////////////////////////  

    ////////////////////////////////////////////////////////////////////////////
    public virtual void Close()
    {
      WindowClosingEventArgs ex = new WindowClosingEventArgs();
      OnClosing(ex);
      if (!ex.Cancel)
      {
        Manager.Input.KeyDown -= Input_KeyDown;
        Manager.Input.GamePadDown -= Input_GamePadDown;
        Manager.ModalWindow = lastModal;
        if (lastModal != null) lastModal.Focused = true;
        Hide();
        WindowClosedEventArgs ev = new WindowClosedEventArgs();
        OnClosed(ev);        

        if (ev.Dispose)
        {
          this.Dispose();          
        }
      }
    }
    ////////////////////////////////////////////////////////////////////////////    

    ////////////////////////////////////////////////////////////////////////////    
    public virtual void Close(ModalResult modalResult)
    {
      ModalResult = modalResult;
      Close();
    }
    ////////////////////////////////////////////////////////////////////////////    

    ////////////////////////////////////////////////////////////////////////////    
    protected virtual void OnClosing(WindowClosingEventArgs e)
    {
      if (Closing != null) Closing.Invoke(this, e);
    }
    //////////////////////////////////////////////////////////////////////////// 

    ////////////////////////////////////////////////////////////////////////////    
    protected virtual void OnClosed(WindowClosedEventArgs e)
    {
      if (Closed != null) Closed.Invoke(this, e);
    }
    ////////////////////////////////////////////////////////////////////////////      

    ////////////////////////////////////////////////////////////////////////////
    void Input_KeyDown(object sender, KeyEventArgs e)
    {
      if (Visible && (Manager.FocusedControl != null && Manager.FocusedControl.Root == this) && 
          e.Key == Microsoft.Xna.Framework.Input.Keys.Escape)
      {        
        //Close(ModalResult.Cancel);
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    void Input_GamePadDown(object sender, GamePadEventArgs e)
    {
      if (Visible && (Manager.FocusedControl != null && Manager.FocusedControl.Root == this))
      {
        if (e.Button == (GamePadActions as WindowGamePadActions).Accept)
        {          
          Close(ModalResult.Ok);
        }
        else if (e.Button == (GamePadActions as WindowGamePadActions).Cancel)
        {          
          Close(ModalResult.Cancel);
        }
      }  
    }
    ////////////////////////////////////////////////////////////////////////////

    #endregion  
  
  }        

}
