////////////////////////////////////////////////////////////////
//                                                            //
//  Neoforce Controls                                         //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//         File: ClipBox.cs                                   //
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
  
  public class ClipBox: Control
  {    
  
    #region //// Constructors //////

    ////////////////////////////////////////////////////////////////////////////
    public ClipBox(Manager manager): base(manager)
    {            
      Color =  Color.Transparent;
      BackColor = Color.Transparent;
      CanFocus = false;
      Passive = true;      
    } 
    ////////////////////////////////////////////////////////////////////////////

    #endregion

		#region //// Methods ///////////
			
		////////////////////////////////////////////////////////////////////////////
		protected override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
    {            
      base.DrawControl(renderer, rect, gameTime);
    }
		////////////////////////////////////////////////////////////////////////////   		
    
		#endregion  
		
  }
  
}