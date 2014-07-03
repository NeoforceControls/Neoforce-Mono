////////////////////////////////////////////////////////////////
//                                                            //
//  Neoforce Controls                                         //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//         File: ImageBox.cs                                  //
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

using Microsoft.Xna.Framework;
////////////////////////////////////////////////////////////////////////////
using Microsoft.Xna.Framework.Graphics;
////////////////////////////////////////////////////////////////////////////

#endregion

namespace TomShane.Neoforce.Controls
{ 

  public class ImageBox: Control
  {

    #region //// Fields ////////////

    ////////////////////////////////////////////////////////////////////////////                 
    private Texture2D image = null;
    private SizeMode sizeMode = SizeMode.Normal;
    private Rectangle sourceRect = Rectangle.Empty;   
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Properties ////////

    ////////////////////////////////////////////////////////////////////////////    
    public Texture2D Image
    {
      get { return image; }
      set 
      {         
        image = value;
        sourceRect = new Rectangle(0, 0, image.Width, image.Height);
        Invalidate();
        if (!Suspended) OnImageChanged(new EventArgs());
      }
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////
    public Rectangle SourceRect
    {
      get { return sourceRect; }
      set 
      {                 
        if (value != null && image != null)
        {
          int l = value.Left;
          int t = value.Top;
          int w = value.Width;        
          int h = value.Height;
          
          if (l < 0) l = 0;
          if (t < 0) t = 0;
          if (w > image.Width) w = image.Width;
          if (h > image.Height) h = image.Height;
          if (l + w > image.Width) w = (image.Width - l); 
          if (t + h > image.Height) h = (image.Height - t);

          sourceRect = new Rectangle(l, t, w, h); 
        }
        else if (image != null)
        {
          sourceRect = new Rectangle(0, 0, image.Width, image.Height);
        }  
        else
        {
          sourceRect = Rectangle.Empty;
        }                
        Invalidate();
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    public SizeMode SizeMode
    {
      get { return sizeMode; }
      set
      {
        if (value == SizeMode.Auto && image != null)
        {
          Width = image.Width;
          Height = image.Height;
        }
        sizeMode = value;
        Invalidate();
        if (!Suspended) OnSizeModeChanged(new EventArgs());
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Events ////////////

    ////////////////////////////////////////////////////////////////////////////                     
    public event EventHandler ImageChanged;
    public event EventHandler SizeModeChanged;
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Construstors //////

    ////////////////////////////////////////////////////////////////////////////       
    public ImageBox(Manager manager): base(manager)
    {                 
    }    
    ////////////////////////////////////////////////////////////////////////////

    #endregion
    
    #region //// Methods ///////////
    
    ////////////////////////////////////////////////////////////////////////////
    public override void Init()
    {      
      base.Init();  
      CanFocus = false;   
      Color = Color.White;        
    }
    ////////////////////////////////////////////////////////////////////////////                          
    
    ////////////////////////////////////////////////////////////////////////////
    protected override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
    {      
      if (image != null)
      {
        if (sizeMode == SizeMode.Normal)
        {
          renderer.Draw(image, rect.X, rect.Y, sourceRect, Color);
        }
        else if (sizeMode == SizeMode.Auto)
        {          
          renderer.Draw(image, rect.X, rect.Y, sourceRect, Color);
        }
        else if (sizeMode == SizeMode.Stretched)
        {    
          renderer.Draw(image, rect, sourceRect, Color);
        }
        else if (sizeMode == SizeMode.Centered)
        {
          int x = (rect.Width / 2) - (image.Width / 2);
          int y = (rect.Height / 2) - (image.Height / 2);
          
          renderer.Draw(image, x, y, sourceRect, Color);
        }
        else if (sizeMode == SizeMode.Tiled)
        {
            renderer.DrawTileTexture(image, rect, Color);
        }
      }           
    }
    ////////////////////////////////////////////////////////////////////////////     

    ////////////////////////////////////////////////////////////////////////////     
    protected virtual void OnImageChanged(EventArgs e)
    {
      if (ImageChanged != null) ImageChanged.Invoke(this, e);
    }
    ////////////////////////////////////////////////////////////////////////////  

    ////////////////////////////////////////////////////////////////////////////     
    protected virtual void OnSizeModeChanged(EventArgs e)
    {
      if (SizeModeChanged != null) SizeModeChanged.Invoke(this, e);
    }
    ////////////////////////////////////////////////////////////////////////////  

    #endregion  

  }

}
