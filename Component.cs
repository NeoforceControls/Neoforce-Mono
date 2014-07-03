////////////////////////////////////////////////////////////////
//                                                            //
//  Neoforce Controls                                         //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//         File: Component.cs                                 //
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
////////////////////////////////////////////////////////////////////////////

#endregion

namespace TomShane.Neoforce.Controls
{

  public class Component: Disposable
  {

    #region //// Fields ////////////

    ////////////////////////////////////////////////////////////////////////////
    private Manager manager = null;
    private bool initialized = false;      
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Properties ////////

    ////////////////////////////////////////////////////////////////////////////
    public virtual Manager Manager { get { return manager; } set { manager = value; } }
    public virtual bool Initialized { get { return initialized; } }    
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Constructors //////

    ////////////////////////////////////////////////////////////////////////////
    public Component(Manager manager)
    {      
      if (manager != null)
      {
       this.manager = manager;                 
      }
      else
      {
        throw new Exception("Component cannot be created. Manager instance is needed.");
      }                 
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
    public virtual void Init()
    {
      initialized = true;
    }
    ////////////////////////////////////////////////////////////////////////////          

    ////////////////////////////////////////////////////////////////////////////   
    protected internal virtual void Update(GameTime gameTime)
    {       
    }
    ////////////////////////////////////////////////////////////////////////////   

    #endregion

  }

}
