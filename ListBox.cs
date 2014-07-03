////////////////////////////////////////////////////////////////
//                                                            //
//  Neoforce Controls                                         //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//         File: ListBox.cs                                   //
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
using Microsoft.Xna.Framework.Input;
////////////////////////////////////////////////////////////////////////////

#endregion

namespace TomShane.Neoforce.Controls
{

    #region //// Classes ///////////

    ////////////////////////////////////////////////////////////////////////////
    ///  <include file='Documents/ListBox.xml' path='ListBox/Class[@name="ListBox"]/*' />          
    public class ListBox : Control
    {

        #region //// Fields ////////////

        ////////////////////////////////////////////////////////////////////////////                     
        private List<object> items = new List<object>();
        private ScrollBar sbVert = null;
        private ClipBox pane = null;
        private int itemIndex = -1;
        private bool hotTrack = false;
        private int itemsCount = 0;
        private bool hideSelection = true;
        ////////////////////////////////////////////////////////////////////////////

        #endregion

        #region //// Properties ////////

        ////////////////////////////////////////////////////////////////////////////
        public virtual List<object> Items
        {
            get { return items; }
            internal set { items = value; }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        public virtual bool HotTrack
        {
            get { return hotTrack; }
            set
            {
                if (hotTrack != value)
                {
                    hotTrack = value;
                    if (!Suspended) OnHotTrackChanged(new EventArgs());
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        public virtual int ItemIndex
        {
            get { return itemIndex; }
            set
            {
                //if (itemIndex != value)
                {
                    if (value >= 0 && value < items.Count)
                    {
                        itemIndex = value;
                    }
                    else
                    {
                        itemIndex = -1;
                    }
                    ScrollTo(itemIndex);

                    if (!Suspended) OnItemIndexChanged(new EventArgs());
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        public virtual bool HideSelection
        {
            get { return hideSelection; }
            set
            {
                if (hideSelection != value)
                {
                    hideSelection = value;
                    Invalidate();
                    if (!Suspended) OnHideSelectionChanged(new EventArgs());
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        #endregion

        #region //// Events ////////////

        ////////////////////////////////////////////////////////////////////////////                 
        public event EventHandler HotTrackChanged;
        public event EventHandler ItemIndexChanged;
        public event EventHandler HideSelectionChanged;
        ////////////////////////////////////////////////////////////////////////////

        #endregion

        #region //// Construstors //////

        ////////////////////////////////////////////////////////////////////////////       
        public ListBox(Manager manager)
            : base(manager)
        {
            Width = 64;
            Height = 64;
            MinimumHeight = 16;

            sbVert = new ScrollBar(Manager, Orientation.Vertical);
            sbVert.Init();
            sbVert.Parent = this;
            sbVert.Left = Left + Width - sbVert.Width - Skin.Layers["Control"].ContentMargins.Right;
            sbVert.Top = Top + Skin.Layers["Control"].ContentMargins.Top;
            sbVert.Height = Height - Skin.Layers["Control"].ContentMargins.Vertical;
            sbVert.Anchor = Anchors.Top | Anchors.Right | Anchors.Bottom;
            sbVert.PageSize = 25;
            sbVert.Range = 1;
            sbVert.PageSize = 1;
            sbVert.StepSize = 10;

            pane = new ClipBox(manager);
            pane.Init();
            pane.Parent = this;
            pane.Top = Skin.Layers["Control"].ContentMargins.Top;
            pane.Left = Skin.Layers["Control"].ContentMargins.Left;
            pane.Width = Width - sbVert.Width - Skin.Layers["Control"].ContentMargins.Horizontal - 1;
            pane.Height = Height - Skin.Layers["Control"].ContentMargins.Vertical;
            pane.Anchor = Anchors.All;
            pane.Passive = true;
            pane.CanFocus = false;
            pane.Draw += new DrawEventHandler(DrawPane);

            CanFocus = true;
            Passive = false;
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
        public virtual void AutoHeight(int maxItems)
        {
            if (items != null && items.Count < maxItems) maxItems = items.Count;
            if (maxItems < 3)
            {
                //maxItems = 3;
                sbVert.Visible = false;
                pane.Width = Width - Skin.Layers["Control"].ContentMargins.Horizontal - 1;
            }
            else
            {
                pane.Width = Width - sbVert.Width - Skin.Layers["Control"].ContentMargins.Horizontal - 1;
                sbVert.Visible = true;
            }

            SkinText font = Skin.Layers["Control"].Text;
            if (items != null && items.Count > 0)
            {
                int h = (int)font.Font.Resource.MeasureString(items[0].ToString()).Y;
                Height = (h * maxItems) + (Skin.Layers["Control"].ContentMargins.Vertical);// - Skin.OriginMargins.Vertical);
            }
            else
            {
                Height = 32;
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        public override int MinimumHeight
        {
            get { return base.MinimumHeight; }
            set
            {
                base.MinimumHeight = value;
                if (this.sbVert != null) this.sbVert.MinimumHeight = value;
            }
        }

        ////////////////////////////////////////////////////////////////////////////
        protected override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
        {
            sbVert.Invalidate();
            pane.Invalidate();
            //DrawPane(this, new DrawEventArgs(renderer, rect, gameTime));

            base.DrawControl(renderer, rect, gameTime);
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        private void DrawPane(object sender, DrawEventArgs e)
        {
            if (items != null && items.Count > 0)
            {
                SkinText font = Skin.Layers["Control"].Text;
                SkinLayer sel = Skin.Layers["ListBox.Selection"];
                int h = (int)font.Font.Resource.MeasureString(items[0].ToString()).Y;
                int v = (sbVert.Value / 10);
                int p = (sbVert.PageSize / 10);
                int d = (int)(((sbVert.Value % 10) / 10f) * h);
                int c = items.Count;
                int s = itemIndex;

                for (int i = v; i <= v + p + 1; i++)
                {
                    if (i < c)
                    {
                        e.Renderer.DrawString(this, Skin.Layers["Control"], items[i].ToString(), new Rectangle(e.Rectangle.Left, e.Rectangle.Top - d + ((i - v) * h), e.Rectangle.Width, h), false);
                    }
                }
                if (s >= 0 && s < c && (Focused || !hideSelection))
                {
                    int pos = -d + ((s - v) * h);
                    if (pos > -h && pos < (p + 1) * h)
                    {
                        e.Renderer.DrawLayer(this, sel, new Rectangle(e.Rectangle.Left, e.Rectangle.Top + pos, e.Rectangle.Width, h));
                        e.Renderer.DrawString(this, sel, items[s].ToString(), new Rectangle(e.Rectangle.Left, e.Rectangle.Top + pos, e.Rectangle.Width, h), false);
                    }
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButton.Left || e.Button == MouseButton.Right)
            {
                TrackItem(e.Position.X, e.Position.Y);
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        private void TrackItem(int x, int y)
        {
            if (items != null && items.Count > 0 && (pane.ControlRect.Contains(new Point(x, y))))
            {
                SkinText font = Skin.Layers["Control"].Text;
                int h = (int)font.Font.Resource.MeasureString(items[0].ToString()).Y;
                int d = (int)(((sbVert.Value % 10) / 10f) * h);
                int i = (int)Math.Floor((sbVert.Value / 10f) + ((float)y / h));
                if (i >= 0 && i < Items.Count && i >= (int)Math.Floor((float)sbVert.Value / 10f) && i < (int)Math.Ceiling((float)(sbVert.Value + sbVert.PageSize) / 10f)) ItemIndex = i;
                Focused = true;
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (hotTrack)
            {
                TrackItem(e.Position.X, e.Position.Y);
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        protected override void OnKeyPress(KeyEventArgs e)
        {
            if (e.Key == Keys.Down)
            {
                e.Handled = true;
                itemIndex += sbVert.StepSize / 10;
            }
            else if (e.Key == Keys.Up)
            {
                e.Handled = true;
                itemIndex -= sbVert.StepSize / 10;
            }
            else if (e.Key == Keys.PageDown)
            {
                e.Handled = true;
                itemIndex += sbVert.PageSize / 10;
            }
            else if (e.Key == Keys.PageUp)
            {
                e.Handled = true;
                itemIndex -= sbVert.PageSize / 10;
            }
            else if (e.Key == Keys.Home)
            {
                e.Handled = true;
                itemIndex = 0;
            }
            else if (e.Key == Keys.End)
            {
                e.Handled = true;
                itemIndex = items.Count - 1;
            }

            if (itemIndex < 0) itemIndex = 0;
            else if (itemIndex >= Items.Count) itemIndex = Items.Count - 1;

            ItemIndex = itemIndex;

            base.OnKeyPress(e);
        }
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Handles mouse scroll events for the list box.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseScroll(MouseEventArgs e)
        {
            Focused = true;
 
            if (e.ScrollDirection == MouseScrollDirection.Down)
            {
                e.Handled = true;
                itemIndex += sbVert.StepSize / 10;
            }
            else if (e.ScrollDirection == MouseScrollDirection.Up)
            {
                e.Handled = true;
                itemIndex -= sbVert.StepSize / 10;
            }
 
            // Wrap index in collection range.
            if (itemIndex < 0) itemIndex = 0;
            else if (itemIndex >= Items.Count) itemIndex = Items.Count - 1;
 
            ItemIndex = itemIndex;
 
            base.OnMouseScroll(e);
        }

        ////////////////////////////////////////////////////////////////////////////
        protected override void OnGamePadPress(GamePadEventArgs e)
        {
            if (e.Button == GamePadActions.Down)
            {
                e.Handled = true;
                itemIndex += sbVert.StepSize / 10;
            }
            else if (e.Button == GamePadActions.Up)
            {
                e.Handled = true;
                itemIndex -= sbVert.StepSize / 10;
            }

            if (itemIndex < 0) itemIndex = 0;
            else if (itemIndex >= Items.Count) itemIndex = Items.Count - 1;

            ItemIndex = itemIndex;
            base.OnGamePadPress(e);
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        private void ItemsChanged()
        {
            if (items != null && items.Count > 0)
            {
                SkinText font = Skin.Layers["Control"].Text;
                int h = (int)font.Font.Resource.MeasureString(items[0].ToString()).Y;

                int sizev = Height - Skin.Layers["Control"].ContentMargins.Vertical;
                sbVert.Range = items.Count * 10;
                sbVert.PageSize = (int)Math.Floor((float)sizev * 10 / h);
                Invalidate();
            }
            else if (items == null || items.Count <= 0)
            {
                sbVert.Range = 1;
                sbVert.PageSize = 1;
                Invalidate();
            }
        }
        ////////////////////////////////////////////////////////////////////////////    

        ////////////////////////////////////////////////////////////////////////////
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            ItemsChanged();
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        public virtual void ScrollTo(int index)
        {
            ItemsChanged();
            if ((index * 10) < sbVert.Value)
            {
                sbVert.Value = index * 10;
            }
            else if (index >= (int)Math.Floor(((float)sbVert.Value + sbVert.PageSize) / 10f))
            {
                sbVert.Value = ((index + 1) * 10) - sbVert.PageSize;
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        protected internal override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Visible && items != null && items.Count != itemsCount)
            {
                itemsCount = items.Count;
                ItemsChanged();
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////     
        protected virtual void OnItemIndexChanged(EventArgs e)
        {
            if (ItemIndexChanged != null) ItemIndexChanged.Invoke(this, e);
        }
        ////////////////////////////////////////////////////////////////////////////     

        ////////////////////////////////////////////////////////////////////////////     
        protected virtual void OnHotTrackChanged(EventArgs e)
        {
            if (HotTrackChanged != null) HotTrackChanged.Invoke(this, e);
        }
        ////////////////////////////////////////////////////////////////////////////  

        ////////////////////////////////////////////////////////////////////////////     
        protected virtual void OnHideSelectionChanged(EventArgs e)
        {
            if (HideSelectionChanged != null) HideSelectionChanged.Invoke(this, e);
        }
        ////////////////////////////////////////////////////////////////////////////       

        #endregion

    }
    ////////////////////////////////////////////////////////////////////////////

    #endregion

}
