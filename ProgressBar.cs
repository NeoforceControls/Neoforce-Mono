////////////////////////////////////////////////////////////////
//                                                            //
//  Neoforce Controls                                         //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//         File: ProgressBar.cs                               //
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
using System;
////////////////////////////////////////////////////////////////////////////

#endregion

namespace TomShane.Neoforce.Controls
{

  #region //// Enums /////////////

  ////////////////////////////////////////////////////////////////////////////
  public enum ProgressBarMode
  {
    Default,
    Infinite
  }
  ////////////////////////////////////////////////////////////////////////////

  #endregion

  public class ProgressBar : Control
  {


    #region //// Fields ////////////

    ////////////////////////////////////////////////////////////////////////////                 
    private int range = 100;
    private int value = 0;
    private double time = 0;
    private int sign = 1;
    private ProgressBarMode mode = ProgressBarMode.Default;
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Properties ////////

    ////////////////////////////////////////////////////////////////////////////
    public int Value
    {
      get { return this.value; }
      set
      {
        if (mode == ProgressBarMode.Default)
        {
          if (this.value != value)
          {
            this.value = value;
            if (this.value > range) this.value = range;
            if (this.value < 0) this.value = 0;
            Invalidate();

            if (!Suspended) OnValueChanged(new EventArgs());
          }
        }
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    public ProgressBarMode Mode
    {
      get { return mode; }
      set
      {
        if (mode != value)
        {
          mode = value;
          if (mode == ProgressBarMode.Infinite)
          {
            range = 100;
            this.value = 0;
            time = 0;
            sign = 1;
          }
          else
          {
            this.value = 0;
            range = 100;
          }
          Invalidate();

          if (!Suspended) OnModeChanged(new EventArgs());
        }
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    public int Range
    {
      get { return range; }
      set
      {
        if (range != value)
        {
          if (mode == ProgressBarMode.Default)
          {
            range = value;
            if (range < 0) range = 0;
            if (range < this.value) this.value = range;
            Invalidate();

            if (!Suspended) OnRangeChanged(new EventArgs());
          }
        }
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Events ////////////

    ////////////////////////////////////////////////////////////////////////////                 
    public event EventHandler ValueChanged;
    public event EventHandler RangeChanged;
    public event EventHandler ModeChanged;
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Construstors //////

    ////////////////////////////////////////////////////////////////////////////       
    public ProgressBar(Manager manager)
      : base(manager)
    {
      Width = 128;
      Height = 16;
      MinimumHeight = 8;
      MinimumWidth = 32;
      Passive = true;
      CanFocus = false;
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
    protected override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
    {
      CheckLayer(Skin, "Control");
      CheckLayer(Skin, "Scale");

      base.DrawControl(renderer, rect, gameTime);

      if (Value > 0 || mode == ProgressBarMode.Infinite)
      {
        SkinLayer p = Skin.Layers["Control"];
        SkinLayer l = Skin.Layers["Scale"];
        Rectangle r = new Rectangle(rect.Left + p.ContentMargins.Left,
                                    rect.Top + p.ContentMargins.Top,
                                    rect.Width - p.ContentMargins.Vertical,
                                    rect.Height - p.ContentMargins.Horizontal);

        float perc = ((float)value / range) * 100;
        int w = (int)((perc / 100) * r.Width);
        Rectangle rx;
        if (mode == ProgressBarMode.Default)
        {
          if (w < l.SizingMargins.Vertical) w = l.SizingMargins.Vertical;
          rx = new Rectangle(r.Left, r.Top, w, r.Height);
        }
        else
        {
          int s = r.Left + w;
          if (s > r.Left + p.ContentMargins.Left + r.Width - (r.Width / 4)) s = r.Left + p.ContentMargins.Left + r.Width - (r.Width / 4);
          rx = new Rectangle(s, r.Top, (r.Width / 4), r.Height);
        }
        
        renderer.DrawLayer(this, l, rx);
      }
    }
    ////////////////////////////////////////////////////////////////////////////     

    ////////////////////////////////////////////////////////////////////////////     
    protected internal override void Update(GameTime gameTime)
    {
      base.Update(gameTime);

      if (mode == ProgressBarMode.Infinite && Enabled && Visible)
      {
        time += gameTime.ElapsedGameTime.TotalMilliseconds;
        if (time >= 33f)
        {
          value += sign * (int)Math.Ceiling(time / 20f);
          if (value >= Range - (Range / 4))
          {
            value = Range - (Range / 4);
            sign = -1;
          }
          else if (value <= 0)
          {
            value = 0;
            sign = 1;
          }  
          time = 0;          
          Invalidate();
        }                        
      }
    }
    ////////////////////////////////////////////////////////////////////////////     

    ////////////////////////////////////////////////////////////////////////////     
    protected virtual void OnValueChanged(EventArgs e)
    {
      if (ValueChanged != null) ValueChanged.Invoke(this, e);
    }
    ////////////////////////////////////////////////////////////////////////////     

    ////////////////////////////////////////////////////////////////////////////     
    protected virtual void OnRangeChanged(EventArgs e)
    {
      if (RangeChanged != null) RangeChanged.Invoke(this, e);
    }
    ////////////////////////////////////////////////////////////////////////////     

    ////////////////////////////////////////////////////////////////////////////     
    protected virtual void OnModeChanged(EventArgs e)
    {
      if (ModeChanged != null) ModeChanged.Invoke(this, e);
    }
    ////////////////////////////////////////////////////////////////////////////     

    #endregion

  }

}
