////////////////////////////////////////////////////////////////
//                                                            //
//  Neoforce Controls                                         //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//         File: TextBox.cs                                   //
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
using Microsoft.Xna.Framework.Input;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.GamerServices;
////////////////////////////////////////////////////////////////////////////

#endregion

namespace TomShane.Neoforce.Controls
{

  #region //// Enums /////////////

  ////////////////////////////////////////////////////////////////////////////
  public enum TextBoxMode
  {
    Normal,
    Password,
    Multiline
  }
  //////////////////////////////////////////////////////////////////////////// 

  #endregion

  #region //// Classes ///////////

  ////////////////////////////////////////////////////////////////////////////
  public class TextBox : ClipControl
  {

    #region //// Structs ///////////

    ////////////////////////////////////////////////////////////////////////////    
    private struct Selection
    {
      private int start;
      private int end;

      public int Start
      {
        get
        {
          if (start > end && start != -1 && end != -1) return end;
          else return start;
        }
        set
        {
          start = value;
        }
      }

      public int End
      {
        get
        {
          if (end < start && start != -1 && end != -1) return start;
          else return end;
        }
        set
        {
          end = value;
        }
      }

      public bool IsEmpty
      {
        get { return Start == -1 && End == -1; }
      }

      public int Length
      {
        get { return IsEmpty ? 0 : (End - Start); }
      }

      public Selection(int start, int end)
      {
        this.start = start;
        this.end = end;
      }

      public void Clear()
      {
        Start = -1;
        End = -1;
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Consts ////////////

    ////////////////////////////////////////////////////////////////////////////    
    private const string skTextBox = "TextBox";
    private const string lrTextBox = "Control";
    private const string lrCursor = "Cursor";

    private const string crDefault = "Default";
    private const string crText = "Text";
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Fields ////////////

    ////////////////////////////////////////////////////////////////////////////              
    private bool showCursor = false;
    private double flashTime = 0;
    private int posx = 0;
    private int posy = 0;
    private char passwordChar = '•';
    private TextBoxMode mode = TextBoxMode.Normal;
    private string shownText = "";
    private bool readOnly = false;
    private bool drawBorders = true;    
    private Selection selection = new Selection(-1, -1);
    private bool caretVisible = true;
    private ScrollBar horz = null;
    private ScrollBar vert = null;
    private List<string> lines = new List<string>();
    private int linesDrawn = 0;
    private int charsDrawn = 0;
    private SpriteFont font = null;
    private bool wordWrap = false;
    private ScrollBars scrollBars = ScrollBars.Both;
    private string Separator = "\n";
    private string text = "";
    private string buffer = "";
    private bool autoSelection = true;
    private string placeholder = "";
    private Color placeholderColor = Color.LightGray;
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Properties ////////

    public string Placeholder
    {
        get { return placeholder; }
        set { placeholder = value; }
    }

    public Color PlaceholderColor
    {
        get { return placeholderColor; }
        set { placeholderColor = value; }
    }

    ////////////////////////////////////////////////////////////////////////////    
    private int PosX
    {
      get
      {
        return posx;
      }
      set
      {
        posx = value;

        if (posx < 0) posx = 0;
        if (posx > Lines[PosY].Length) posx = Lines[PosY].Length;
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////    
    private int PosY
    {
      get
      {
        return posy;
      }
      set
      {
        posy = value;

        if (posy < 0) posy = 0;
        if (posy > Lines.Count - 1) posy = Lines.Count - 1;

        PosX = PosX;
      }
    }
    ////////////////////////////////////////////////////////////////////////////


    ////////////////////////////////////////////////////////////////////////////
    private int Pos
    {
      get
      {
        return GetPos(PosX, PosY);
      }
      set
      {
        PosY = GetPosY(value);
        PosX = GetPosX(value);
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    //>>>>
    
    public virtual bool WordWrap
    {
      get { return wordWrap; }
      set 
      { 
        wordWrap = value;        
        if (ClientArea != null) ClientArea.Invalidate(); 
        SetupBars();
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    public virtual ScrollBars ScrollBars
    {
      get { return scrollBars; }
      set
      {
        scrollBars = value;
        SetupBars();
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    public virtual char PasswordChar
    {
      get { return passwordChar; }
      set { passwordChar = value; if (ClientArea != null) ClientArea.Invalidate(); }
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    public virtual bool CaretVisible
    {
      get { return caretVisible; }
      set { caretVisible = value; }
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    public virtual TextBoxMode Mode
    {
      get { return mode; }
      set
      {
        if (value != TextBoxMode.Multiline)
        {
          Text = Text.Replace(Separator, "");
        }
        mode = value;
        selection.Clear();

        if (ClientArea != null) ClientArea.Invalidate();
        SetupBars();
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    public virtual bool ReadOnly
    {
      get { return readOnly; }
      set { readOnly = value; }
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    public virtual bool DrawBorders
    {
      get { return drawBorders; }
      set { drawBorders = value; if (ClientArea != null) ClientArea.Invalidate(); }
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////       
    public virtual int CursorPosition
    {
      get { return Pos; }
      set
      {
        Pos = value;
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    public virtual string SelectedText
    {
      get
      {
        if (selection.IsEmpty)
        {
          return "";
        }
        else
        {
          return Text.Substring(selection.Start, selection.Length);
        }
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    public virtual int SelectionStart
    {
      get
      {
        if (selection.IsEmpty)
        {
          return Pos;
        }
        else
        {
          return selection.Start;
        }
      }
      set
      {
        Pos = value;
        if (Pos < 0) Pos = 0;
        if (Pos > Text.Length) Pos = Text.Length;
        selection.Start = Pos;
        if (selection.End == -1) selection.End = Pos;
        ClientArea.Invalidate();
      }
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////
    public virtual bool AutoSelection
    {
      get { return autoSelection; }
      set { autoSelection = value; }
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    public virtual int SelectionLength
    {
      get
      {
        return selection.Length;
      }
      set
      {
        if (value == 0)
        {
          selection.End = selection.Start;
        }
        else if (selection.IsEmpty)
        {
          selection.Start = 0;
          selection.End = value;
        }
        else if (!selection.IsEmpty)
        {
          selection.End = selection.Start + value;
        }

        if (!selection.IsEmpty)
        {
          if (selection.Start < 0) selection.Start = 0;
          if (selection.Start > Text.Length) selection.Start = Text.Length;
          if (selection.End < 0) selection.End = 0;
          if (selection.End > Text.Length) selection.End = Text.Length;
        }
        ClientArea.Invalidate();
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    private List<string> Lines
    {
      get
      {
        return lines;
      }
      set
      {
        lines = value;
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////       
    public override string Text
    {
      get
      {
        return text;
      }
      set
      {
          if (wordWrap)
              value = WrapWords(value, ClientWidth);

        if (mode != TextBoxMode.Multiline && value != null)
        {
          value = value.Replace(Separator, "");
        }

        text = value;

        if (!Suspended) OnTextChanged(new EventArgs());

        lines = SplitLines(text);
        if (ClientArea != null) ClientArea.Invalidate();

        SetupBars();
        ProcessScrolling();
      }
    }
    ////////////////////////////////////////////////////////////////////////////     

    #endregion

    #region //// Construstors //////

    ////////////////////////////////////////////////////////////////////////////       
    public TextBox(Manager manager)
      : base(manager)
    {
      CheckLayer(Skin, lrCursor);

      SetDefaultSize(128, 20);
      Lines.Add("");

      ClientArea.Draw += new DrawEventHandler(ClientArea_Draw);

      vert = new ScrollBar(manager, Orientation.Vertical);
      horz = new ScrollBar(manager, Orientation.Horizontal);
    }
    ////////////////////////////////////////////////////////////////////////////        

    #endregion

    #region //// Methods ///////////

    ////////////////////////////////////////////////////////////////////////////
    public override void Init()
    {
      base.Init();

      vert.Init();
      vert.Range = 1;
      vert.PageSize = 1;
      vert.Value = 0;
      vert.Anchor = Anchors.Top | Anchors.Right | Anchors.Bottom;
      vert.ValueChanged += new EventHandler(sb_ValueChanged);

      horz.Init();
      horz.Range = ClientArea.Width;
      horz.PageSize = ClientArea.Width;
      horz.Value = 0;
      horz.Anchor = Anchors.Right | Anchors.Left | Anchors.Bottom;
      horz.ValueChanged += new EventHandler(sb_ValueChanged);

      horz.Visible = false;
      vert.Visible = false;

      Add(vert, false);
      Add(horz, false);
    }
    ////////////////////////////////////////////////////////////////////////////                                                    

    ////////////////////////////////////////////////////////////////////////////
    protected internal override void InitSkin()
    {
      base.InitSkin();
      Skin = new SkinControl(Manager.Skin.Controls[skTextBox]);

      #if (!XBOX && !XBOX_FAKE)
        Cursor = Manager.Skin.Cursors[crText].Resource;
      #endif

      font = (Skin.Layers[lrTextBox].Text != null) ? Skin.Layers[lrTextBox].Text.Font.Resource : null;
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    protected override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
    {
      if (drawBorders)
      {
        base.DrawControl(renderer, rect, gameTime);
      }
    }
    ////////////////////////////////////////////////////////////////////////////     

    ////////////////////////////////////////////////////////////////////////////     
    private int GetFitChars(string text, int width)
    {
      int ret = text.Length;
      int size = 0;

      for (int i = 0; i < text.Length; i++)
      {
        size = (int)font.MeasureString(text.Substring(0, i)).X;
        if (size > width)
        {
          ret = i;
          break;
        }
      }

      return ret;
    }
    ////////////////////////////////////////////////////////////////////////////     

    ////////////////////////////////////////////////////////////////////////////     
    private void DeterminePages()
    {
      if (ClientArea != null)
      {
        int sizey = (int)font.LineSpacing;
        linesDrawn = (int)(ClientArea.Height / sizey);
        if (linesDrawn > Lines.Count) linesDrawn = Lines.Count;

        charsDrawn = ClientArea.Width - 1;
      }
    }
    ////////////////////////////////////////////////////////////////////////////     

    ////////////////////////////////////////////////////////////////////////////
    private string GetMaxLine()
    {
      int max = 0;
      int x = 0;

      for (int i = 0; i < Lines.Count; i++)
      {
        if (Lines[i].Length > max)
        {
          max = Lines[i].Length;
          x = i;
        }
      }
      return Lines.Count > 0 ? Lines[x] : "";
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////     
    void ClientArea_Draw(object sender, DrawEventArgs e)
    {
      SkinLayer layer = Skin.Layers[lrTextBox];
      Color col = Skin.Layers[lrTextBox].Text.Colors.Enabled;
      SkinLayer cursor = Skin.Layers[lrCursor];
      Alignment al = mode == TextBoxMode.Multiline ? Alignment.TopLeft : Alignment.MiddleLeft;
      Renderer renderer = e.Renderer;
      Rectangle r = e.Rectangle;
      bool drawsel = !selection.IsEmpty;
      string tmpText = "";

      font = (Skin.Layers[lrTextBox].Text != null) ? Skin.Layers[lrTextBox].Text.Font.Resource : null;
      
      if (Text != null && font != null)
      {
        DeterminePages();

        if (mode == TextBoxMode.Multiline)
        {
          shownText = Text;
          tmpText = Lines[PosY];
        }        
        else if (mode == TextBoxMode.Password)
        {
          shownText = "";
          for (int i = 0; i < Text.Length; i++)
          {
            shownText = shownText + passwordChar;
          }
          tmpText = shownText;
        }
        else
        {
          shownText = Text;
          tmpText = Lines[PosY];        
        }

        if (TextColor != UndefinedColor && ControlState != ControlState.Disabled)
        {
          col = TextColor;
        }

        if (mode != TextBoxMode.Multiline)
        {
          linesDrawn = 0;
          vert.Value = 0;
        }

        if(string.IsNullOrEmpty(text))
        {
            Rectangle rx = new Rectangle(r.Left - horz.Value, r.Top, r.Width, r.Height);
            renderer.DrawString(font, placeholder, rx, placeholderColor, al, false);
        }

        if (drawsel)
        {
          DrawSelection(e.Renderer, r);
/*
          renderer.End();          
          renderer.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
          renderer.SpriteBatch.GraphicsDevice.RenderState.SeparateAlphaBlendEnabled = true;
          renderer.SpriteBatch.GraphicsDevice.RenderState.SourceBlend = Blend.DestinationColor;
          renderer.SpriteBatch.GraphicsDevice.RenderState.DestinationBlend = Blend.SourceColor;
          renderer.SpriteBatch.GraphicsDevice.RenderState.BlendFunction = BlendFunction.Subtract;          
          //renderer.SpriteBatch.GraphicsDevice.RenderState.AlphaFunction = CompareFunction.Equal;
          //renderer.SpriteBatch.GraphicsDevice.RenderState.AlphaSourceBlend = Blend.One;
          //renderer.SpriteBatch.GraphicsDevice.RenderState.AlphaDestinationBlend = Blend.DestinationAlpha;
 */
        }

        int sizey = (int)font.LineSpacing;

        if (showCursor && caretVisible)
        {
          Vector2 size = Vector2.Zero;
          if (PosX > 0 && PosX <= tmpText.Length)
          {
            size = font.MeasureString(tmpText.Substring(0, PosX));
          }
          if (size.Y == 0)
          {
            size = font.MeasureString(" ");
            size.X = 0;
          }

          int m = r.Height - font.LineSpacing;

          Rectangle rc = new Rectangle(r.Left - horz.Value + (int)size.X, r.Top + m / 2, cursor.Width, font.LineSpacing);

          if (mode == TextBoxMode.Multiline)
          {
            rc = new Rectangle(r.Left + (int)size.X - horz.Value, r.Top + (int)((PosY - vert.Value) * font.LineSpacing), cursor.Width, font.LineSpacing);
          }
          cursor.Alignment = al;
          renderer.DrawLayer(cursor, rc, col, 0);
        }

        for (int i = 0; i < linesDrawn + 1; i++)
        {
          int ii = i + vert.Value;
          if (ii >= Lines.Count || ii < 0) break;

          if (Lines[ii] != "")
          {
            if (mode == TextBoxMode.Multiline)
            {
              renderer.DrawString(font, Lines[ii], r.Left - horz.Value, r.Top + (i * sizey), col);
            }
            else
            {
              Rectangle rx = new Rectangle(r.Left - horz.Value, r.Top, r.Width, r.Height);
              renderer.DrawString(font, shownText, rx, col, al, false);
            }
          }
        }
      /*  if (drawsel)
        {
          renderer.End();
          renderer.Begin(BlendingMode.Premultiplied);
        }*/
      }
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    private int GetStringWidth(string text, int count)
    {
      if (count > text.Length) count = text.Length;
      return (int)font.MeasureString(text.Substring(0, count)).X;
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////    
    private void ProcessScrolling()
    {
      if (vert != null && horz != null)
      {
        vert.PageSize = linesDrawn;
        horz.PageSize = charsDrawn;

        if (horz.PageSize > horz.Range) horz.PageSize = horz.Range;

        if (PosY >= vert.Value + vert.PageSize)
        {
          vert.Value = (PosY + 1) - vert.PageSize;
        }
        else if (PosY < vert.Value)
        {
          vert.Value = PosY;
        }

        if (GetStringWidth(Lines[PosY], PosX) >= horz.Value + horz.PageSize)
        {
          horz.Value = (GetStringWidth(Lines[PosY], PosX) + 1) - horz.PageSize;
        }
        else if (GetStringWidth(Lines[PosY], PosX) < horz.Value)
        {
          horz.Value = GetStringWidth(Lines[PosY], PosX) - horz.PageSize;
        }
      }
    }
    ////////////////////////////////////////////////////////////////////////////   

    ////////////////////////////////////////////////////////////////////////////     
    private void DrawSelection(Renderer renderer, Rectangle rect)
    {
      if (!selection.IsEmpty)
      {
        int s = selection.Start;
        int e = selection.End;

        int sl = GetPosY(s);
        int el = GetPosY(e);
        int sc = GetPosX(s);
        int ec = GetPosX(e);

        int hgt = font.LineSpacing;

        int start = sl;
        int end = el;
        
        if (start < vert.Value) start = vert.Value;
        if (end > vert.Value + linesDrawn) end = vert.Value + linesDrawn;
        
        for (int i = start; i <= end; i++)
        {
          Rectangle r = Rectangle.Empty;

          if (mode == TextBoxMode.Normal)
          {
            int m = ClientArea.Height - font.LineSpacing;
            r = new Rectangle(rect.Left - horz.Value + (int)font.MeasureString(Lines[i].Substring(0, sc)).X, rect.Top + m / 2,
                             (int)font.MeasureString(Lines[i].Substring(0, ec + 0)).X - (int)font.MeasureString(Lines[i].Substring(0, sc)).X, hgt);
          }
          else if (sl == el)
          {
            r = new Rectangle(rect.Left - horz.Value + (int)font.MeasureString(Lines[i].Substring(0, sc)).X, rect.Top + (i - vert.Value) * hgt,
                              (int)font.MeasureString(Lines[i].Substring(0, ec + 0)).X - (int)font.MeasureString(Lines[i].Substring(0, sc)).X, hgt);
          }
          else
          {
            if (i == sl) r = new Rectangle(rect.Left - horz.Value + (int)font.MeasureString(Lines[i].Substring(0, sc)).X, rect.Top + (i - vert.Value) * hgt, (int)font.MeasureString(Lines[i]).X - (int)font.MeasureString(Lines[i].Substring(0, sc)).X, hgt);
            else if (i == el) r = new Rectangle(rect.Left - horz.Value, rect.Top + (i - vert.Value) * hgt, (int)font.MeasureString(Lines[i].Substring(0, ec + 0)).X, hgt);
            else r = new Rectangle(rect.Left - horz.Value, rect.Top + (i - vert.Value) * hgt, (int)font.MeasureString(Lines[i]).X, hgt);
          }

          renderer.Draw(Manager.Skin.Images["Control"].Resource, r, Color.FromNonPremultiplied(160, 160, 160, 128));
        }
      }
    }
    ////////////////////////////////////////////////////////////////////////////           

    ////////////////////////////////////////////////////////////////////////////     
    protected internal override void Update(GameTime gameTime)
    {
      base.Update(gameTime);

      bool sc = showCursor;

      showCursor = Focused;

      if (Focused)
      {
        flashTime += gameTime.ElapsedGameTime.TotalSeconds;
        showCursor = flashTime < 0.5;
        if (flashTime > 1) flashTime = 0;
      }
      if (sc != showCursor) ClientArea.Invalidate();
    }
    ////////////////////////////////////////////////////////////////////////////        

    ////////////////////////////////////////////////////////////////////////////
    private int FindPrevWord(string text)
    {
      bool letter = false;

      int p = Pos - 1;
      if (p < 0) p = 0;
      if (p >= text.Length) p = text.Length - 1;


      for (int i = p; i >= 0; i--)
      {
        if (char.IsLetterOrDigit(text[i]))
        {
          letter = true;
          continue;
        }
        if (letter && !char.IsLetterOrDigit(text[i]))
        {
          return i + 1;
        }
      }

      return 0;
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    private int FindNextWord(string text)
    {
      bool space = false;

      for (int i = Pos; i < text.Length - 1; i++)
      {
        if (!char.IsLetterOrDigit(text[i]))
        {
          space = true;
          continue;
        }
        if (space && char.IsLetterOrDigit(text[i]))
        {
          return i;
        }
      }

      return text.Length;
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    private int GetPosY(int pos)
    {
      if (pos >= Text.Length) return Lines.Count - 1;

      int p = pos;
      for (int i = 0; i < Lines.Count; i++)
      {
        p -= Lines[i].Length + Separator.Length;
        if (p < 0)
        {
          p = p + Lines[i].Length + Separator.Length;
          return i;
        }
      }
      return 0;
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    private int GetPosX(int pos)
    {
      if (pos >= Text.Length) return Lines[Lines.Count - 1].Length;

      int p = pos;
      for (int i = 0; i < Lines.Count; i++)
      {
        p -= Lines[i].Length + Separator.Length;
        if (p < 0)
        {
          p = p + Lines[i].Length + Separator.Length;
          return p;
        }
      }
      return 0;
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////     
    private int GetPos(int x, int y)
    {
      int p = 0;

      for (int i = 0; i < y; i++)
      {
        p += Lines[i].Length + Separator.Length;
      }
      p += x;

      return p;
    }
    ////////////////////////////////////////////////////////////////////////////     

    ////////////////////////////////////////////////////////////////////////////     
    private int CharAtPos(Point pos)
    {
      int x = pos.X;
      int y = pos.Y;
      int px = 0;
      int py = 0;

      if (mode == TextBoxMode.Multiline)
      {
        py = vert.Value + (int)((y - ClientTop) / font.LineSpacing);
        if (py < 0) py = 0;
        if (py >= Lines.Count) py = Lines.Count - 1;
      }
      else
      {
        py = 0;
      }

      string str = mode == TextBoxMode.Multiline ? Lines[py] : shownText;

      if (str != null && str != "")
      {
        for (int i = 1; i <= Lines[py].Length; i++)
        {
          Vector2 v = font.MeasureString(str.Substring(0, i)) - (font.MeasureString(str[i - 1].ToString()) / 3);
          if (x <= (ClientLeft + (int)v.X) - horz.Value)
          {
            px = i - 1;
            break;
          }
        }
        if (x > ClientLeft + ((int)font.MeasureString(str).X) - horz.Value - (font.MeasureString(str[str.Length - 1].ToString()).X / 3)) px = str.Length;
      }

      return GetPos(px, py);
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////    
    protected override void OnMouseDown(MouseEventArgs e)
    {
      base.OnMouseDown(e);

      flashTime = 0;

      Pos = CharAtPos(e.Position);        
      selection.Clear();
 
      if (e.Button == MouseButton.Left && caretVisible && mode != TextBoxMode.Password)
      {
        selection.Start = Pos;
        selection.End = Pos;
      }   
      ClientArea.Invalidate();       
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////
    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);

      if (e.Button == MouseButton.Left && !selection.IsEmpty && mode != TextBoxMode.Password && selection.Length < Text.Length)
      {
        int pos = CharAtPos(e.Position);
        selection.End = CharAtPos(e.Position);
        Pos = pos;

        ClientArea.Invalidate();

        ProcessScrolling();
      }
    }
    ////////////////////////////////////////////////////////////////////////////       

    ////////////////////////////////////////////////////////////////////////////       
    protected override void OnMouseUp(MouseEventArgs e)
    {
      base.OnMouseUp(e);

      if (e.Button == MouseButton.Left && !selection.IsEmpty && mode != TextBoxMode.Password)
      {
        if (selection.Length == 0) selection.Clear();
      }      
    }
    ////////////////////////////////////////////////////////////////////////////       

    protected override void OnMouseScroll(MouseEventArgs e)
    {
        if (Mode != TextBoxMode.Multiline)
        {
            base.OnMouseScroll(e);
            return;
        }
 
        if (e.ScrollDirection == MouseScrollDirection.Down)
            vert.ScrollDown();
        else
            vert.ScrollUp();
 
        base.OnMouseScroll(e);
    }

    ////////////////////////////////////////////////////////////////////////////     
    protected override void OnKeyPress(KeyEventArgs e)
    {
      flashTime = 0;
      
      if (Manager.UseGuide && Guide.IsVisible) return;
      
      if (!e.Handled)
      {
        if (e.Key == Keys.A && e.Control && mode != TextBoxMode.Password)
        {
          SelectAll();
        }
        if (e.Key == Keys.Up)
        {
          e.Handled = true;

          if (e.Shift && selection.IsEmpty && mode != TextBoxMode.Password)
          {
            selection.Start = Pos;
          }
          if (!e.Control)
          {
            PosY -= 1;
          }
        }
        else if (e.Key == Keys.Down)
        {
          e.Handled = true;
          if (e.Shift && selection.IsEmpty && mode != TextBoxMode.Password)
          {
            selection.Start = Pos;
          }
          if (!e.Control)
          {
            PosY += 1;
          }
        }
        else if (e.Key == Keys.Back && !readOnly)
        {
          e.Handled = true;
          if (!selection.IsEmpty)
          {
            Text = Text.Remove(selection.Start, selection.Length);
            Pos = selection.Start;
          }
          else if (Text.Length > 0 && Pos > 0)
          {
            Pos -= 1;
            Text = Text.Remove(Pos, 1);
          }
          selection.Clear();
        }
        else if (e.Key == Keys.Delete && !readOnly)
        {
          e.Handled = true;
          if (!selection.IsEmpty)
          {
            Text = Text.Remove(selection.Start, selection.Length);
            Pos = selection.Start;
          }
          else if (Pos < Text.Length)
          {
            Text = Text.Remove(Pos, 1);
          }
          selection.Clear();
        }
        else if (e.Key == Keys.Left)
        {
          e.Handled = true;
          if (e.Shift && selection.IsEmpty && mode != TextBoxMode.Password)
          {
            selection.Start = Pos;
          }
          if (!e.Control)
          {
            Pos -= 1;
          }
          if (e.Control)
          {
            Pos = FindPrevWord(shownText);
          }
        }
        else if (e.Key == Keys.Right)
        {
          e.Handled = true;
          if (e.Shift && selection.IsEmpty && mode != TextBoxMode.Password)
          {
            selection.Start = Pos;
          }
          if (!e.Control)
          {
            Pos += 1;
          }
          if (e.Control)
          {
            Pos = FindNextWord(shownText);
          }
        }
        else if (e.Key == Keys.Home)
        {
          e.Handled = true;
          if (e.Shift && selection.IsEmpty && mode != TextBoxMode.Password)
          {
            selection.Start = Pos;
          }
          if (!e.Control)
          {
            PosX = 0;
          }
          if (e.Control)
          {
            Pos = 0;
          }
        }
        else if (e.Key == Keys.End)
        {
          e.Handled = true;
          if (e.Shift && selection.IsEmpty && mode != TextBoxMode.Password)
          {
            selection.Start = Pos;
          }
          if (!e.Control)
          {
            PosX = Lines[PosY].Length;
          }
          if (e.Control)
          {
            Pos = Text.Length;
          }
        }
        else if (e.Key == Keys.PageUp)
        {
          e.Handled = true;
          if (e.Shift && selection.IsEmpty && mode != TextBoxMode.Password)
          {
            selection.Start = Pos;
          }
          if (!e.Control)
          {
            PosY -= linesDrawn;
          }
        }
        else if (e.Key == Keys.PageDown)
        {
          e.Handled = true;
          if (e.Shift && selection.IsEmpty && mode != TextBoxMode.Password)
          {
            selection.Start = Pos;
          }
          if (!e.Control)
          {
            PosY += linesDrawn;
          }
        }
        else if (e.Key == Keys.Enter && mode == TextBoxMode.Multiline && !readOnly)
        {
          e.Handled = true;
          Text = Text.Insert(Pos, Separator);
          PosX = 0;
          PosY += 1;
        }
        else if (e.Key == Keys.Tab)
        {          
        }
        else if (!readOnly && !e.Control)
        {
          string c = Manager.KeyboardLayout.GetKey(e);
          if (selection.IsEmpty)
          {
            Text = Text.Insert(Pos, c);
            if (c != "") PosX += 1;
          }
          else
          {
            if (Text.Length > 0)
            {
              Text = Text.Remove(selection.Start, selection.Length);            
              Text = Text.Insert(selection.Start, c);
              Pos = selection.Start + 1;
            }            
            selection.Clear();
          }
        }

        if (e.Shift && !selection.IsEmpty)
        {
          selection.End = Pos;
        }

          /*
           * TODO: Fix
           * MONOTODO: Fix
        if (e.Control && e.Key == Keys.C && mode != TextBoxMode.Password)
        {
#if (!XBOX && !XBOX_FAKE)
          System.Windows.Forms.Clipboard.Clear();
          if (mode != TextBoxMode.Password && !selection.IsEmpty)
          {
            System.Windows.Forms.Clipboard.SetText((Text.Substring(selection.Start, selection.Length)).Replace("\n", Environment.NewLine));
          }
#endif
        }
        else if (e.Control && e.Key == Keys.V && !readOnly && mode != TextBoxMode.Password)
        {
#if (!XBOX && !XBOX_FAKE)
          string t = System.Windows.Forms.Clipboard.GetText().Replace(Environment.NewLine, "\n");
          if (selection.IsEmpty)
          {
            Text = Text.Insert(Pos, t);
            Pos = Pos + t.Length;
          }
          else
          {
            Text = Text.Remove(selection.Start, selection.Length);
            Text = Text.Insert(selection.Start, t);
            PosX = selection.Start + t.Length;
            selection.Clear();
          }
#endif
        }
          */
        if ((!e.Shift && !e.Control) || Text.Length <= 0)
        {
          selection.Clear();
        }

        if (e.Control && e.Key == Keys.Down)
        {
          e.Handled = true;
          HandleGuide(PlayerIndex.One);
        }
        flashTime = 0;
        if (ClientArea != null) ClientArea.Invalidate();

        DeterminePages();
        ProcessScrolling();
      }
      base.OnKeyPress(e);
    }
    ////////////////////////////////////////////////////////////////////////////        

    ////////////////////////////////////////////////////////////////////////////        
    protected override void OnGamePadDown(GamePadEventArgs e)
    {
      if (Manager.UseGuide && Guide.IsVisible) return;

      if (!e.Handled)
      {
        if (e.Button == GamePadActions.Click)
        {
          e.Handled = true;
          HandleGuide(e.PlayerIndex);
        }
      }
      base.OnGamePadDown(e);
    }
    ////////////////////////////////////////////////////////////////////////////        

    ////////////////////////////////////////////////////////////////////////////
    private void HandleGuide(PlayerIndex pi)
    {
      if (Manager.UseGuide && !Guide.IsVisible)
      {        
        Guide.BeginShowKeyboardInput(pi, "Enter Text", "", Text, GetText, pi.ToString());
      }   
    }
    ////////////////////////////////////////////////////////////////////////////
    
    ////////////////////////////////////////////////////////////////////////////
    private void GetText(IAsyncResult result)
    {      
      string res = Guide.EndShowKeyboardInput(result);
      Text = res != null ? res : "";
      Pos = text.Length;
    }
    ////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////        
    private void SetupBars()
    {
      DeterminePages();

      if (vert != null) vert.Range = Lines.Count;
      if (horz != null)
      {
        horz.Range = (int)font.MeasureString(GetMaxLine()).X;
        if (horz.Range == 0) horz.Range = ClientArea.Width;
      }

      if (vert != null)
      {
        vert.Left = Width - 16 - 2;
        vert.Top = 2;
        vert.Height = Height - 4 - 16;

        if (Height < 50 || (scrollBars != ScrollBars.Both && scrollBars != ScrollBars.Vertical)) vert.Visible = false;
        else if ((scrollBars == ScrollBars.Vertical || scrollBars == ScrollBars.Both) && mode == TextBoxMode.Multiline) vert.Visible = true;
      }
      if (horz != null)
      {
        horz.Left = 2;
        horz.Top = Height - 16 - 2;
        horz.Width = Width - 4 - 16;

        if (Width < 50 || wordWrap || (scrollBars != ScrollBars.Both && scrollBars != ScrollBars.Horizontal)) horz.Visible = false;
        else if ((scrollBars == ScrollBars.Horizontal || scrollBars == ScrollBars.Both) && mode == TextBoxMode.Multiline && !wordWrap) horz.Visible = true;
      }   
      
      AdjustMargins();  

      if (vert != null) vert.PageSize = linesDrawn;
      if (horz != null) horz.PageSize = charsDrawn;
    }
    ////////////////////////////////////////////////////////////////////////////        

    ////////////////////////////////////////////////////////////////////////////        
    protected override void AdjustMargins()
    {
      if (horz != null && !horz.Visible)
      {
        vert.Height = Height - 4;
        ClientMargins = new Margins(ClientMargins.Left, ClientMargins.Top, ClientMargins.Right, Skin.ClientMargins.Bottom);
      }
      else
      {
        ClientMargins = new Margins(ClientMargins.Left, ClientMargins.Top, ClientMargins.Right, 18 + Skin.ClientMargins.Bottom);
      }

      if (vert != null && !vert.Visible)
      {
        horz.Width = Width - 4;
        ClientMargins = new Margins(ClientMargins.Left, ClientMargins.Top, Skin.ClientMargins.Right, ClientMargins.Bottom);
      }
      else
      {
        ClientMargins = new Margins(ClientMargins.Left, ClientMargins.Top, 18 + Skin.ClientMargins.Right, ClientMargins.Bottom);
      }
      base.AdjustMargins();
    }
    ////////////////////////////////////////////////////////////////////////////        

    ////////////////////////////////////////////////////////////////////////////        
    protected override void OnResize(ResizeEventArgs e)
    {
      base.OnResize(e);
      selection.Clear();
      SetupBars();
    }
    ////////////////////////////////////////////////////////////////////////////                  

    //////////////////////////////////////////////////////////////////////////// 
    private string WrapWords(string text, int size)
    {
      string ret = "";
      string line = "";

      string[] words = text.Replace("\v", "").Split(" ".ToCharArray());

      for (int i = 0; i < words.Length; i++)
      {
        if (font.MeasureString(line + words[i]).X > size)
        {
          ret += line + "\n";
          line = words[i] + " ";
        }
        else
        {
          line += words[i] + " ";
        }
      }

      ret += line;

      return ret.Remove(ret.Length - 1, 1);
    }
    //////////////////////////////////////////////////////////////////////////// 

    //////////////////////////////////////////////////////////////////////////// 
    public virtual void SelectAll()
    {
        if (text.Length > 0)
        {
            selection.Start = 0;
            selection.End = Text.Length;
        }
    }
    //////////////////////////////////////////////////////////////////////////// 

    ////////////////////////////////////////////////////////////////////////////     
    private List<string> SplitLines(string text)
    {
      if (buffer != text)
      {
        buffer = text;
        List<string> list = new List<string>();
        string[] s = text.Split(new char[] { Separator[0] });
        list.Clear();

        //Before adding the lines back in, we will want to first, measure the lines, and split words if needed...

        list.AddRange(s);

        if (posy < 0) posy = 0;
        if (posy > list.Count - 1) posy = list.Count - 1;

        if (posx < 0) posx = 0;
        if (posx > list[PosY].Length) posx = list[PosY].Length;

        return list;
      }
      else return lines;
    }
    //////////////////////////////////////////////////////////////////////////// 

    //////////////////////////////////////////////////////////////////////////// 
    void sb_ValueChanged(object sender, EventArgs e)
    {
      ClientArea.Invalidate();
    }
    //////////////////////////////////////////////////////////////////////////// 
    
    //////////////////////////////////////////////////////////////////////////// 
    protected override void OnFocusLost(EventArgs e)
    {
      selection.Clear();      
      ClientArea.Invalidate();
      base.OnFocusLost(e);      
    }
    //////////////////////////////////////////////////////////////////////////// 
    
    //////////////////////////////////////////////////////////////////////////// 
    protected override void OnFocusGained(EventArgs e)
    {
      if (!readOnly && autoSelection)
      {        
        SelectAll();        
        ClientArea.Invalidate();
      }
 
      base.OnFocusGained(e);
    }
    //////////////////////////////////////////////////////////////////////////// 

    #endregion

  }
  ////////////////////////////////////////////////////////////////////////////

  #endregion

}