////////////////////////////////////////////////////////////////
//                                                            //
//  Neoforce Central                                          //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//         File: Dialog.cs                                    //
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

  public class Dialog: Window
  {

    #region //// Fields ////////////

    ////////////////////////////////////////////////////////////////////////////                 
    private Panel pnlTop = null;
    private Label lblCapt = null;
    private Label lblDesc = null;    
    private Panel pnlBottom = null;
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Properties ////////

    ////////////////////////////////////////////////////////////////////////////   
    public Panel TopPanel { get { return pnlTop; } }    
    public Panel BottomPanel { get { return pnlBottom; } }
    public Label Caption { get { return lblCapt; } }
    public Label Description { get { return lblDesc; } }
    ////////////////////////////////////////////////////////////////////////////       

    #endregion

    #region //// Events ////////////

    ////////////////////////////////////////////////////////////////////////////                         
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Construstors //////

    ////////////////////////////////////////////////////////////////////////////       
    public Dialog(Manager manager): base(manager)
    {                                      
      pnlTop = new Panel(manager);
      pnlTop.Anchor = Anchors.Left | Anchors.Top | Anchors.Right;     
      pnlTop.Init();      
      pnlTop.Parent = this;      
      pnlTop.Width = ClientWidth;
      pnlTop.Height = 64;      
      pnlTop.BevelBorder = BevelBorder.Bottom;      
   
      lblCapt = new Label(manager);    
      lblCapt.Init();
      lblCapt.Parent = pnlTop;     
      lblCapt.Width = lblCapt.Parent.ClientWidth - 16;      
      lblCapt.Text = "Caption";
      lblCapt.Left = 8;
      lblCapt.Top = 8;            
      lblCapt.Alignment = Alignment.TopLeft;      
      lblCapt.Anchor = Anchors.Left | Anchors.Top | Anchors.Right;                  

      lblDesc = new Label(manager);
      lblDesc.Init();      
      lblDesc.Parent = pnlTop;
      lblDesc.Width = lblDesc.Parent.ClientWidth - 16;      
      lblDesc.Left = 8;            
      lblDesc.Text = "Description text.";      
      lblDesc.Alignment = Alignment.TopLeft;      
      lblDesc.Anchor = Anchors.Left | Anchors.Top | Anchors.Right;                

      pnlBottom = new Panel(manager);
      pnlBottom.Init();
      pnlBottom.Parent = this;     
      pnlBottom.Width = ClientWidth;
      pnlBottom.Height = 24 + 16;
      pnlBottom.Top = ClientHeight - pnlBottom.Height;
      pnlBottom.BevelBorder = BevelBorder.Top;
      pnlBottom.Anchor = Anchors.Left | Anchors.Bottom | Anchors.Right;

    }
    ////////////////////////////////////////////////////////////////////////////

    #endregion

    #region //// Methods ///////////

    ////////////////////////////////////////////////////////////////////////////
    public override void Init()
    {
      base.Init();
      
      SkinLayer lc = new SkinLayer(lblCapt.Skin.Layers[0]);
      lc.Text.Font.Resource = Manager.Skin.Fonts[Manager.Skin.Controls["Dialog"].Layers["TopPanel"].Attributes["CaptFont"].Value].Resource;
      lc.Text.Colors.Enabled = Utilities.ParseColor(Manager.Skin.Controls["Dialog"].Layers["TopPanel"].Attributes["CaptFontColor"].Value);

      SkinLayer ld = new SkinLayer(lblDesc.Skin.Layers[0]);
      ld.Text.Font.Resource = Manager.Skin.Fonts[Manager.Skin.Controls["Dialog"].Layers["TopPanel"].Attributes["DescFont"].Value].Resource;
      ld.Text.Colors.Enabled = Utilities.ParseColor(Manager.Skin.Controls["Dialog"].Layers["TopPanel"].Attributes["DescFontColor"].Value);
      
      pnlTop.Color = Utilities.ParseColor(Manager.Skin.Controls["Dialog"].Layers["TopPanel"].Attributes["Color"].Value);
      pnlTop.BevelMargin = int.Parse(Manager.Skin.Controls["Dialog"].Layers["TopPanel"].Attributes["BevelMargin"].Value);
      pnlTop.BevelStyle = Utilities.ParseBevelStyle(Manager.Skin.Controls["Dialog"].Layers["TopPanel"].Attributes["BevelStyle"].Value);

      lblCapt.Skin = new SkinControl(lblCapt.Skin);            
      lblCapt.Skin.Layers[0] = lc;      
      lblCapt.Height = Manager.Skin.Fonts[Manager.Skin.Controls["Dialog"].Layers["TopPanel"].Attributes["CaptFont"].Value].Height;            
      
      lblDesc.Skin = new SkinControl(lblDesc.Skin);
      lblDesc.Skin.Layers[0] = ld;
      lblDesc.Height = Manager.Skin.Fonts[Manager.Skin.Controls["Dialog"].Layers["TopPanel"].Attributes["DescFont"].Value].Height;
      lblDesc.Top = lblCapt.Top + lblCapt.Height + 4;  
      lblDesc.Height = lblDesc.Parent.ClientHeight - lblDesc.Top - 8;      
    
      pnlBottom.Color = Utilities.ParseColor(Manager.Skin.Controls["Dialog"].Layers["BottomPanel"].Attributes["Color"].Value);
      pnlBottom.BevelMargin = int.Parse(Manager.Skin.Controls["Dialog"].Layers["BottomPanel"].Attributes["BevelMargin"].Value);
      pnlBottom.BevelStyle = Utilities.ParseBevelStyle(Manager.Skin.Controls["Dialog"].Layers["BottomPanel"].Attributes["BevelStyle"].Value);      
    }
    ////////////////////////////////////////////////////////////////////////////

    #endregion

  }

}
