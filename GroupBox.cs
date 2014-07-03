////////////////////////////////////////////////////////////////
//                                                            //
//  Neoforce Controls                                         //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//         File: GroupBox.cs                                  //
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

    public enum GroupBoxType
    {
        Normal,
        Flat
    }

    public class GroupBox : Container
    {

        #region //// Fields ////////////

        ////////////////////////////////////////////////////////////////////////////       
        private GroupBoxType type = GroupBoxType.Normal;
        ////////////////////////////////////////////////////////////////////////////

        #endregion

        #region //// Properties ////////

        ////////////////////////////////////////////////////////////////////////////
        public virtual GroupBoxType Type
        {
            get { return type; }
            set { type = value; Invalidate(); }
        }
        ////////////////////////////////////////////////////////////////////////////    

        #endregion

        #region //// Construstors //////

        ////////////////////////////////////////////////////////////////////////////       
        public GroupBox(Manager manager)
            : base(manager)
        {
            CheckLayer(Skin, "Control");
            CheckLayer(Skin, "Flat");

            CanFocus = false;
            Passive = true;
            Width = 64;
            Height = 64;
            BackColor = Color.Transparent;
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


        private void AdjustClientMargins()
        {
            SkinLayer layer = this.type == GroupBoxType.Normal ? this.Skin.Layers["Control"] : this.Skin.Layers["Flat"];
            SpriteFont font = (layer.Text != null && layer.Text.Font != null) ? layer.Text.Font.Resource : null;
            Vector2 size = font.MeasureString(this.Text);
            var cm = this.ClientMargins;
            cm.Top = string.IsNullOrWhiteSpace(this.Text) ? this.ClientTop : (int)size.Y;
            this.ClientMargins = new Margins(cm.Left, cm.Top, cm.Right, cm.Bottom);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            AdjustClientMargins();
        }

        protected internal override void OnSkinChanged(EventArgs e)
        {
            base.OnSkinChanged(e);
            AdjustClientMargins();
        }

        ////////////////////////////////////////////////////////////////////////////   
        protected override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
        {
            SkinLayer layer = type == GroupBoxType.Normal ? Skin.Layers["Control"] : Skin.Layers["Flat"];
            SpriteFont font = (layer.Text != null && layer.Text.Font != null) ? layer.Text.Font.Resource : null;
            Color col = (layer.Text != null) ? layer.Text.Colors.Enabled : Color.White;
            Point offset = new Point(layer.Text.OffsetX, layer.Text.OffsetY);
            Vector2 size = font.MeasureString(Text);
            size.Y = font.LineSpacing;
            Rectangle r = new Rectangle(rect.Left, rect.Top + (int)(size.Y / 2), rect.Width, rect.Height - (int)(size.Y / 2));

            renderer.DrawLayer(this, layer, r);

            if (font != null && Text != null && Text != "")
            {
                Rectangle bg = new Rectangle(r.Left + offset.X, (r.Top - (int)(size.Y / 2)) + offset.Y, (int)size.X + layer.ContentMargins.Horizontal, (int)size.Y);
                renderer.DrawLayer(Manager.Skin.Controls["Control"].Layers[0], bg, new Color(64, 64, 64), 0);
                renderer.DrawString(this, layer, Text, new Rectangle(r.Left, r.Top - (int)(size.Y / 2), (int)(size.X), (int)size.Y), true, 0, 0, false);
            }
        }
        ////////////////////////////////////////////////////////////////////////////     

        #endregion

    }

}
