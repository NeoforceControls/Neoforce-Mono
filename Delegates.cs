////////////////////////////////////////////////////////////////
//                                                            //
//  Neoforce Controls                                         //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//         File: Delegates.cs                                 //
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
////////////////////////////////////////////////////////////////////////////

#endregion

namespace TomShane.Neoforce.Controls
{

  #region //// Delegates /////////
  
  ////////////////////////////////////////////////////////////////////////////  
  public delegate void DeviceEventHandler(DeviceEventArgs e);
  public delegate void SkinEventHandler(EventArgs e);  
  ////////////////////////////////////////////////////////////////////////////  
  
  ////////////////////////////////////////////////////////////////////////////  
  public delegate void EventHandler(object sender, EventArgs e);
  public delegate void MouseEventHandler(object sender, MouseEventArgs e);
  public delegate void KeyEventHandler(object sender, KeyEventArgs e);
  public delegate void GamePadEventHandler(object sender, GamePadEventArgs e);
  public delegate void DrawEventHandler(object sender, DrawEventArgs e);
  public delegate void MoveEventHandler(object sender, MoveEventArgs e);
  public delegate void ResizeEventHandler(object sender, ResizeEventArgs e);
  public delegate void WindowClosingEventHandler(object sender, WindowClosingEventArgs e);
  public delegate void WindowClosedEventHandler(object sender, WindowClosedEventArgs e);
  public delegate void ConsoleMessageEventHandler(object sender, ConsoleMessageEventArgs e);
  ////////////////////////////////////////////////////////////////////////////

  #endregion
  
}
