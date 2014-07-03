////////////////////////////////////////////////////////////////
//                                                            //
//  Neoforce Controls                                         //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//         File: ContentReaders.cs                            //
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

//////////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework.Content;

#if (!XBOX && !XBOX_FAKE)
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
#endif
//////////////////////////////////////////////////////////////////////////////

#endregion

namespace TomShane.Neoforce.Controls
{

    ////////////////////////////////////////////////////////////////////////////
    public class LayoutXmlDocument : XmlDocument { }
    public class SkinXmlDocument : XmlDocument { }
    ////////////////////////////////////////////////////////////////////////////


    public class SkinReader : ContentTypeReader<SkinXmlDocument>
    {

        #region //// Methods ///////////

        ////////////////////////////////////////////////////////////////////////////
        protected override SkinXmlDocument Read(ContentReader input, SkinXmlDocument existingInstance)
        {
            if (existingInstance == null)
            {
                SkinXmlDocument doc = new SkinXmlDocument();
                doc.LoadXml(input.ReadString());
                return doc;
            }
            else
            {
                existingInstance.LoadXml(input.ReadString());
            }

            return existingInstance;
        }
        ////////////////////////////////////////////////////////////////////////////

        #endregion

    }

    public class LayoutReader : ContentTypeReader<LayoutXmlDocument>
    {

        #region //// Methods ///////////

        ////////////////////////////////////////////////////////////////////////////
        protected override LayoutXmlDocument Read(ContentReader input, LayoutXmlDocument existingInstance)
        {
            if (existingInstance == null)
            {
                LayoutXmlDocument doc = new LayoutXmlDocument();
                doc.LoadXml(input.ReadString());
                return doc;
            }
            else
            {
                existingInstance.LoadXml(input.ReadString());
            }

            return existingInstance;
        }
        ////////////////////////////////////////////////////////////////////////////

        #endregion

    }

#if (!XBOX && !XBOX_FAKE)

    public class CursorReader : ContentTypeReader<Cursor>
    {

        #region //// Methods ///////////

        ////////////////////////////////////////////////////////////////////////////
        protected override Cursor Read(ContentReader input, Cursor existingInstance)
        {
            if (existingInstance == null)
            {
                int count = input.ReadInt32();
                byte[] data = input.ReadBytes(count);

                string path = Path.GetTempFileName();
                File.WriteAllBytes(path, data);
                string tPath = Path.GetTempFileName();
                using(System.Drawing.Icon i = System.Drawing.Icon.ExtractAssociatedIcon(path))
                {
                    using (System.Drawing.Bitmap b = i.ToBitmap())
                    {

                        b.Save(tPath, System.Drawing.Imaging.ImageFormat.Png);
                        b.Dispose();
                    }
                    
                    i.Dispose();
                }
                //TODO: Replace with xml based solution for getting hotspot and size instead
                IntPtr handle = NativeMethods.LoadCursor(path);
                System.Windows.Forms.Cursor c = new System.Windows.Forms.Cursor(handle);
                Vector2 hs = new Vector2(c.HotSpot.X, c.HotSpot.Y);
                int w = c.Size.Width;
                int h = c.Size.Height;
                c.Dispose();
                File.Delete(path);

                return new Cursor(tPath, hs, w, h);
            }
            else
            {
            }

            return existingInstance;
        }
        ////////////////////////////////////////////////////////////////////////////

        #endregion

    }

#endif

}

