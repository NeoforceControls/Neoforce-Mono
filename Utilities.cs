////////////////////////////////////////////////////////////////
//                                                            //
//  Neoforce Controls                                         //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//         File: Utilities.cs                                 //
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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
////////////////////////////////////////////////////////////////////////////

#endregion

namespace TomShane.Neoforce.Controls
{
  static class Utilities
  {
  
    #region //// Methods ///////////
    
    ////////////////////////////////////////////////////////////////////////////
    public static string DeriveControlName(Control control)
    {
      if (control != null)
      {
        try
        {
          string str = control.ToString();
          int i = str.LastIndexOf(".");
          return str.Remove(0, i + 1);
        }
        catch
        {
          return control.ToString();
        }
      }
      return control.ToString();
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    public static Color ParseColor(string str)
    {

      string[] val = str.Split(';');
      byte r = 255, g = 255, b = 255, a = 255;

      if (val.Length >= 1) r = byte.Parse(val[0]);
      if (val.Length >= 2) g = byte.Parse(val[1]);
      if (val.Length >= 3) b = byte.Parse(val[2]);
      if (val.Length >= 4) a = byte.Parse(val[3]);

      return Color.FromNonPremultiplied(r, g, b, a);
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    public static BevelStyle ParseBevelStyle(string str)
    {
      return (BevelStyle)Enum.Parse(typeof(BevelStyle), str, true);
    }
    ////////////////////////////////////////////////////////////////////////////    

    #endregion    
    
  }
}
