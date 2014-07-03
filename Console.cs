////////////////////////////////////////////////////////////////
//                                                            //
//  Neoforce Controls                                         //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//         File: Console.cs                                   //
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
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework.GamerServices;
////////////////////////////////////////////////////////////////////////////

#endregion

namespace TomShane.Neoforce.Controls
{

    public struct ConsoleMessage
    {
        public string Text;
        public byte Channel;
        public DateTime Time;
        public string Sender;

        public ConsoleMessage(string sender, string text, byte channel)
        {
            this.Text = text;
            this.Channel = channel;
            this.Time = DateTime.Now;
            this.Sender = sender;
        }
    }

    public class ChannelList : EventedList<ConsoleChannel>
    {

        #region //// Indexers //////////

        ////////////////////////////////////////////////////////////////////////////
        public ConsoleChannel this[string name]
        {
            get
            {
                for (int i = 0; i < this.Count; i++)
                {
                    ConsoleChannel s = (ConsoleChannel)this[i];
                    if (s.Name.ToLower() == name.ToLower())
                    {
                        return s;
                    }
                }
                return default(ConsoleChannel);
            }

            set
            {
                for (int i = 0; i < this.Count; i++)
                {
                    ConsoleChannel s = (ConsoleChannel)this[i];
                    if (s.Name.ToLower() == name.ToLower())
                    {
                        this[i] = value;
                    }
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        public ConsoleChannel this[byte index]
        {
            get
            {
                for (int i = 0; i < this.Count; i++)
                {
                    ConsoleChannel s = (ConsoleChannel)this[i];
                    if (s.Index == index)
                    {
                        return s;
                    }
                }
                return default(ConsoleChannel);
            }

            set
            {
                for (int i = 0; i < this.Count; i++)
                {
                    ConsoleChannel s = (ConsoleChannel)this[i];
                    if (s.Index == index)
                    {
                        this[i] = value;
                    }
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        #endregion

    }

    public class ConsoleChannel
    {
        private string name;
        private byte index;
        private Color color;

        public ConsoleChannel(byte index, string name, Color color)
        {
            this.name = name;
            this.index = index;
            this.color = color;
        }

        public virtual byte Index
        {
            get { return index; }
            set { index = value; }
        }

        public virtual Color Color
        {
            get { return color; }
            set { color = value; }
        }

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }
    }

    [Flags]
    public enum ConsoleMessageFormats
    {
        None = 0x00,
        ChannelName = 0x01,
        TimeStamp = 0x02,
        Sender = 0x03,
        All = Sender | ChannelName | TimeStamp
    }

    public class Console : Container
    {

        #region //// Fields ////////////

        ////////////////////////////////////////////////////////////////////////////                 
        private TextBox txtMain = null;
        private ComboBox cmbMain;
        private EventedList<ConsoleMessage> buffer = new EventedList<ConsoleMessage>();
        private ChannelList channels = new ChannelList();
        private List<byte> filter = new List<byte>();
        private ConsoleMessageFormats messageFormat = ConsoleMessageFormats.None;
        private bool channelsVisible = true;
        private bool textBoxVisible = true;
        private string sender;
        ////////////////////////////////////////////////////////////////////////////

        #endregion

        #region //// Properties ////////

        public string Sender
        {
            get { return sender; }
            set { sender = value; }
        }

        ////////////////////////////////////////////////////////////////////////////
        public virtual EventedList<ConsoleMessage> MessageBuffer
        {
            get { return buffer; }
            set
            {
                buffer.ItemAdded -= new EventHandler(buffer_ItemAdded);
                buffer = value;
                buffer.ItemAdded += new EventHandler(buffer_ItemAdded);
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        public virtual ChannelList Channels
        {
            get { return channels; }
            set
            {
                channels.ItemAdded -= new EventHandler(channels_ItemAdded);
                channels = value;
                channels.ItemAdded += new EventHandler(channels_ItemAdded);
                channels_ItemAdded(null, null);
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        public virtual List<byte> ChannelFilter
        {
            get { return filter; }
            set { filter = value; }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        public virtual byte SelectedChannel
        {
            set { cmbMain.Text = channels[value].Name; }
            get { return channels[cmbMain.Text].Index; }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        public virtual ConsoleMessageFormats MessageFormat
        {
            get { return messageFormat; }
            set { messageFormat = value; }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////       
        public virtual bool ChannelsVisible
        {
            get { return channelsVisible; }
            set
            {
                cmbMain.Visible = channelsVisible = value;
                if (value && !textBoxVisible) TextBoxVisible = false;
                PositionControls();
            }
        }
        ////////////////////////////////////////////////////////////////////////////       

        ////////////////////////////////////////////////////////////////////////////       
        public virtual bool TextBoxVisible
        {
            get { return textBoxVisible; }
            set
            {
                txtMain.Visible = textBoxVisible = value;
                txtMain.Focused = true;
                if (!value && channelsVisible) ChannelsVisible = false;
                PositionControls();
            }
        }
        ////////////////////////////////////////////////////////////////////////////       

        #endregion

        #region //// Events ////////////

        ////////////////////////////////////////////////////////////////////////////
        public event ConsoleMessageEventHandler MessageSent;
        ////////////////////////////////////////////////////////////////////////////

        #endregion

        #region //// Construstors //////

        ////////////////////////////////////////////////////////////////////////////       
        public Console(Manager manager)
            : base(manager)
        {
            Width = 320;
            Height = 160;
            MinimumHeight = 64;
            MinimumWidth = 64;
            CanFocus = false;
            Resizable = false;
            Movable = false;

            cmbMain = new ComboBox(manager);
            cmbMain.Init();
            cmbMain.Top = Height - cmbMain.Height;
            cmbMain.Left = 0;
            cmbMain.Width = 128;
            cmbMain.Anchor = Anchors.Left | Anchors.Bottom;
            cmbMain.Detached = false;
            cmbMain.DrawSelection = false;
            cmbMain.Visible = channelsVisible;
            Add(cmbMain, false);

            txtMain = new TextBox(manager);
            txtMain.Init();
            txtMain.Top = Height - txtMain.Height;
            txtMain.Left = cmbMain.Width + 1;
            txtMain.Anchor = Anchors.Left | Anchors.Bottom | Anchors.Right;
            txtMain.Detached = false;
            txtMain.Visible = textBoxVisible;
            txtMain.KeyDown += new KeyEventHandler(txtMain_KeyDown);
            txtMain.GamePadDown += new GamePadEventHandler(txtMain_GamePadDown);
            txtMain.FocusGained += new EventHandler(txtMain_FocusGained);
            Add(txtMain, false);

            VerticalScrollBar.Top = 2;
            VerticalScrollBar.Left = Width - 18;
            VerticalScrollBar.Range = 1;
            VerticalScrollBar.PageSize = 1;
            VerticalScrollBar.ValueChanged += new EventHandler(VerticalScrollBar_ValueChanged);
            VerticalScrollBar.Visible = true;

            ClientArea.Draw += new DrawEventHandler(ClientArea_Draw);

            buffer.ItemAdded += new EventHandler(buffer_ItemAdded);
            channels.ItemAdded += new EventHandler(channels_ItemAdded);
            channels.ItemRemoved += new EventHandler(channels_ItemRemoved);

            PositionControls();
        }
        ////////////////////////////////////////////////////////////////////////////       

        #endregion

        #region //// Methods ///////////

        ////////////////////////////////////////////////////////////////////////////
        private void PositionControls()
        {
            if (txtMain != null)
            {
                txtMain.Left = channelsVisible ? cmbMain.Width + 1 : 0;
                txtMain.Width = channelsVisible ? Width - cmbMain.Width - 1 : Width;

                if (textBoxVisible)
                {
                    ClientMargins = new Margins(Skin.ClientMargins.Left, Skin.ClientMargins.Top + 4, VerticalScrollBar.Width + 6, txtMain.Height + 4);
                    VerticalScrollBar.Height = Height - txtMain.Height - 5;
                }
                else
                {
                    ClientMargins = new Margins(Skin.ClientMargins.Left, Skin.ClientMargins.Top + 4, VerticalScrollBar.Width + 6, 2);
                    VerticalScrollBar.Height = Height - 4;
                }
                Invalidate();
            }
        }
        ////////////////////////////////////////////////////////////////////////////

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
            Skin = new SkinControl(Manager.Skin.Controls["Console"]);

            PositionControls();
        }
        ////////////////////////////////////////////////////////////////////////////                          

        ////////////////////////////////////////////////////////////////////////////
        protected internal override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        void ClientArea_Draw(object sender, DrawEventArgs e)
        {
            SpriteFont font = Skin.Layers[0].Text.Font.Resource;
            Rectangle r = new Rectangle(e.Rectangle.Left, e.Rectangle.Top, e.Rectangle.Width, e.Rectangle.Height);
            int pos = 0;

            if (buffer.Count > 0)
            {
                EventedList<ConsoleMessage> b = GetFilteredBuffer(filter);
                int c = b.Count;
                int s = (VerticalScrollBar.Value + VerticalScrollBar.PageSize);
                int f = s - VerticalScrollBar.PageSize;

                if (b.Count > 0)
                {
                    for (int i = s - 1; i >= f; i--)
                    {
                        {
                            int x = 4;
                            int y = r.Bottom - (pos + 1) * ((int)font.LineSpacing + 0);

                            string msg = ((ConsoleMessage)b[i]).Text;
                            string pre = "";
                            ConsoleChannel ch = (channels[((ConsoleMessage)b[i]).Channel] as ConsoleChannel);

                            if ((messageFormat & ConsoleMessageFormats.ChannelName) == ConsoleMessageFormats.ChannelName)
                            {
                                pre += string.Format("[{0}]", channels[((ConsoleMessage)b[i]).Channel].Name);
                            }
                            if ((messageFormat & ConsoleMessageFormats.Sender) == ConsoleMessageFormats.Sender)
                            {
                                pre += string.Format("[{0}]", ((ConsoleMessage)b[i]).Sender);
                            }
                            if ((messageFormat & ConsoleMessageFormats.TimeStamp) == ConsoleMessageFormats.TimeStamp)
                            {
                                pre = string.Format("[{0}]", ((ConsoleMessage)b[i]).Time.ToLongTimeString()) + pre;
                            }

                            if (pre != "") msg = pre + ": " + msg;

                            e.Renderer.DrawString(font,
                                                msg,
                                                x, y,
                                                ch.Color);
                            pos += 1;
                        }
                    }
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        protected override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
        {
            int h = txtMain.Visible ? (txtMain.Height + 1) : 0;
            Rectangle r = new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height - h);
            base.DrawControl(renderer, r, gameTime);
        }
        ////////////////////////////////////////////////////////////////////////////              

        ////////////////////////////////////////////////////////////////////////////     
        void txtMain_FocusGained(object sender, EventArgs e)
        {
            ConsoleChannel ch = channels[cmbMain.Text];
            if (ch != null) txtMain.TextColor = ch.Color;
        }
        ////////////////////////////////////////////////////////////////////////////     

        ////////////////////////////////////////////////////////////////////////////     
        void txtMain_KeyDown(object sender, KeyEventArgs e)
        {
            SendMessage(e);
        }
        ////////////////////////////////////////////////////////////////////////////    

        ////////////////////////////////////////////////////////////////////////////        
        void txtMain_GamePadDown(object sender, GamePadEventArgs e)
        {
            SendMessage(e);
        }
        ////////////////////////////////////////////////////////////////////////////        

        ////////////////////////////////////////////////////////////////////////////        
        private void SendMessage(EventArgs x)
        {
            if (Manager.UseGuide && Guide.IsVisible) return;

            KeyEventArgs k = new KeyEventArgs();
            GamePadEventArgs g = new GamePadEventArgs(PlayerIndex.One);

            if (x is KeyEventArgs) k = x as KeyEventArgs;
            else if (x is GamePadEventArgs) g = x as GamePadEventArgs;

            ConsoleChannel ch = channels[cmbMain.Text];
            if (ch != null)
            {
                txtMain.TextColor = ch.Color;

                string message = txtMain.Text;
                if ((k.Key == Microsoft.Xna.Framework.Input.Keys.Enter || g.Button == GamePadActions.Press) && message != null && message != "")
                {
                    x.Handled = true;

                    ConsoleMessageEventArgs me = new ConsoleMessageEventArgs(new ConsoleMessage(sender, message, ch.Index));
                    OnMessageSent(me);

                    buffer.Add(new ConsoleMessage(sender, me.Message.Text, me.Message.Channel));

                    txtMain.Text = "";
                    ClientArea.Invalidate();

                    CalcScrolling();
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////    

        ////////////////////////////////////////////////////////////////////////////        
        protected virtual void OnMessageSent(ConsoleMessageEventArgs e)
        {
            if (MessageSent != null) MessageSent.Invoke(this, e);
        }
        ////////////////////////////////////////////////////////////////////////////    

        ////////////////////////////////////////////////////////////////////////////
        void channels_ItemAdded(object sender, EventArgs e)
        {
            cmbMain.Items.Clear();
            for (int i = 0; i < channels.Count; i++)
            {
                cmbMain.Items.Add((channels[i] as ConsoleChannel).Name);
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        void channels_ItemRemoved(object sender, EventArgs e)
        {
            cmbMain.Items.Clear();
            for (int i = 0; i < channels.Count; i++)
            {
                cmbMain.Items.Add((channels[i] as ConsoleChannel).Name);
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        void buffer_ItemAdded(object sender, EventArgs e)
        {
            CalcScrolling();
            ClientArea.Invalidate();
        }
        //////////////////////////////////////////////////////////////////////////// 

        ////////////////////////////////////////////////////////////////////////////     
        private void CalcScrolling()
        {
            if (VerticalScrollBar != null)
            {
                int line = Skin.Layers[0].Text.Font.Resource.LineSpacing;
                int c = GetFilteredBuffer(filter).Count;
                int p = (int)Math.Ceiling(ClientArea.ClientHeight / (float)line);

                VerticalScrollBar.Range = c == 0 ? 1 : c;
                VerticalScrollBar.PageSize = c == 0 ? 1 : p;
                VerticalScrollBar.Value = VerticalScrollBar.Range;
            }
        }
        //////////////////////////////////////////////////////////////////////////// 

        //////////////////////////////////////////////////////////////////////////// 
        void VerticalScrollBar_ValueChanged(object sender, EventArgs e)
        {
            ClientArea.Invalidate();
        }
        //////////////////////////////////////////////////////////////////////////// 

        //////////////////////////////////////////////////////////////////////////// 
        protected override void OnResize(ResizeEventArgs e)
        {
            CalcScrolling();
            base.OnResize(e);
        }
        //////////////////////////////////////////////////////////////////////////// 

        //////////////////////////////////////////////////////////////////////////// 
        private EventedList<ConsoleMessage> GetFilteredBuffer(List<byte> filter)
        {
            EventedList<ConsoleMessage> ret = new EventedList<ConsoleMessage>();

            if (filter.Count > 0)
            {
                for (int i = 0; i < buffer.Count; i++)
                {
                    if (filter.Contains(((ConsoleMessage)buffer[i]).Channel))
                    {
                        ret.Add(buffer[i]);
                    }
                }
                return ret;
            }
            else return buffer;
        }
        //////////////////////////////////////////////////////////////////////////// 

        #endregion

    }

}
