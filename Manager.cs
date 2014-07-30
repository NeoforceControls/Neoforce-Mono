////////////////////////////////////////////////////////////////
//                                                            //
//  Neoforce Controls                                         //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//         File: Manager.cs                                   //
//                                                            //
//      Version: 0.8                                          //
//                                                            //
//         Date: 05/07/2014                                   //
//                                                            //
//       Author: Nathan 'Grimston' Pipes                      //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//  Copyright (c) by Tom Shane 2010                           //
//  Copyright (c) by Nathan Pipes 2014                        //
//                                                            //
////////////////////////////////////////////////////////////////


#region //// Using /////////////

////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;
using Microsoft.Xna.Framework.Input;

#if (!XBOX && !XBOX_FAKE)
using System.IO;
using System.Text;
using System.Media;
#endif
////////////////////////////////////////////////////////////////////////////

#endregion

[assembly: CLSCompliant(false)]

namespace TomShane.Neoforce.Controls
{

    #region //// Classes ///////////

    ////////////////////////////////////////////////////////////////////////////  
    /// <summary>
    /// Manages rendering of all controls.
    /// </summary>  
    public class Manager : DrawableGameComponent
    {

        private struct ControlStates
        {
            public Control[] Buttons;
            public int Click;
            public Control Over;
        }

        #region //// Consts ////////////

        ////////////////////////////////////////////////////////////////////////////        
        internal Version _SkinVersion = new Version(0, 7);
        internal Version _LayoutVersion = new Version(0, 7);
        internal const string _SkinDirectory = ".\\Content\\Skins\\";
        internal const string _LayoutDirectory = ".\\Content\\Layout\\";
        internal const string _DefaultSkin = "Default";
        internal const string _SkinExtension = ".skin";
        internal const int _MenuDelay = 500;
        internal const int _ToolTipDelay = 500;
        internal const int _DoubleClickTime = 500;
        internal const int _TextureResizeIncrement = 32;
        internal const RenderTargetUsage _RenderTargetUsage = RenderTargetUsage.DiscardContents;
        ////////////////////////////////////////////////////////////////////////////    

        #endregion

        #region //// Fields ////////////

        ////////////////////////////////////////////////////////////////////////////            

        ////////////////////////////////////////////////////////////////////////////                    
        private bool deviceReset = false;
        private bool renderTargetValid = false;
        private RenderTarget2D renderTarget = null;
        private int targetFrames = 60;
        private long drawTime = 0;
        private long updateTime = 0;
        private GraphicsDeviceManager graphics = null;
        private ArchiveManager content = null;
        private Renderer renderer = null;
        private InputSystem input = null;
        private bool inputEnabled = true;
        private List<Component> components = null;
        private ControlsList controls = null;
        private ControlsList orderList = null;
        private Skin skin = null;
        private string skinName = _DefaultSkin;
        private string layoutDirectory = _LayoutDirectory;
        private string skinDirectory = _SkinDirectory;
        private string skinExtension = _SkinExtension;
        private Control focusedControl = null;
        private ModalContainer modalWindow = null;
        private float globalDepth = 0.0f;
        private int toolTipDelay = _ToolTipDelay;
        private bool toolTipsEnabled = true;
        private int menuDelay = _MenuDelay;
        private int doubleClickTime = _DoubleClickTime;
        private int textureResizeIncrement = _TextureResizeIncrement;
        private bool logUnhandledExceptions = true;
        private ControlStates states = new ControlStates();
        private KeyboardLayout keyboardLayout = null;
        private List<KeyboardLayout> keyboardLayouts = new List<KeyboardLayout>();
        private bool disposing = false;
        private bool useGuide = false;
        private bool autoUnfocus = true;
        private bool autoCreateRenderTarget = true;
        private Cursor cursor = null;
        private bool softwareCursor = false;

        ////////////////////////////////////////////////////////////////////////////          

        #endregion

        #region //// Properties ////////

        ////////////////////////////////////////////////////////////////////////////  
        /// <summary>
        /// Gets a value indicating whether Manager is in the process of disposing.
        /// </summary>
        public virtual bool Disposing
        {
            get { return disposing; }
        }
        ////////////////////////////////////////////////////////////////////////////  

        /// <summary>
        /// Gets or sets an application cursor.
        /// </summary>
        public Cursor Cursor
        {
            get { return cursor; }
            set { cursor = value; }
        }

        /// <summary>
        /// Should a software cursor be drawn? Very handy on a PC build.
        /// </summary>
        public bool ShowSoftwareCursor
        {
            get { return softwareCursor; }
            set { softwareCursor = value; }
        }
        ////////////////////////////////////////////////////////////////////////////            

        ////////////////////////////////////////////////////////////////////////////            
        /// <summary>
        /// Returns associated <see cref="Game"/> component.
        /// </summary>
        public virtual new Game Game { get { return base.Game; } }

        /// <summary>
        /// Returns associated <see cref="GraphicsDevice"/>.
        /// </summary>
        public virtual new GraphicsDevice GraphicsDevice { get { return base.GraphicsDevice; } }

        /// <summary>
        /// Returns associated <see cref="GraphicsDeviceManager"/>.
        /// </summary>
        public virtual GraphicsDeviceManager Graphics { get { return graphics; } }

        /// <summary>
        /// Returns <see cref="Renderer"/> used for rendering controls.
        /// </summary>
        public virtual Renderer Renderer { get { return renderer; } }

        /// <summary>
        /// Returns <see cref="ArchiveManager"/> used for loading assets.
        /// </summary>
        public virtual ArchiveManager Content { get { return content; } }

        /// <summary>
        /// Returns <see cref="InputSystem"/> instance responsible for managing user input.
        /// </summary>
        public virtual InputSystem Input { get { return input; } }

        /// <summary>
        /// Returns list of components added to the manager.
        /// </summary>
        public virtual IEnumerable<Component> Components { get { return components; } }

        /// <summary>
        /// Returns list of controls added to the manager.
        /// </summary>
        public virtual IEnumerable<Control> Controls { get { return controls; } }

        /// <summary>
        /// Gets or sets the depth value used for rendering sprites.
        /// </summary>
        public virtual float GlobalDepth { get { return globalDepth; } set { globalDepth = value; } }

        /// <summary>
        /// Gets or sets the time that passes before the <see cref="ToolTip"/> appears.
        /// </summary>
        public virtual int ToolTipDelay { get { return toolTipDelay; } set { toolTipDelay = value; } }

        /// <summary>
        /// Gets or sets the time that passes before a submenu appears when hovered over menu item.
        /// </summary>
        public virtual int MenuDelay { get { return menuDelay; } set { menuDelay = value; } }

        /// <summary>
        /// Gets or sets the maximum number of milliseconds that can elapse between a first click and a second click to consider the mouse action a double-click.
        /// </summary>
        public virtual int DoubleClickTime { get { return doubleClickTime; } set { doubleClickTime = value; } }

        /// <summary>
        /// Gets or sets texture size increment in pixel while performing controls resizing.
        /// </summary>
        public virtual int TextureResizeIncrement { get { return textureResizeIncrement; } set { textureResizeIncrement = value; } }

        /// <summary>
        /// Enables or disables showing of tooltips globally.
        /// </summary>
        public virtual bool ToolTipsEnabled { get { return toolTipsEnabled; } set { toolTipsEnabled = value; } }

        /// <summary>
        /// Enables or disables logging of unhandled exceptions. 
        /// </summary>
        public virtual bool LogUnhandledExceptions { get { return logUnhandledExceptions; } set { logUnhandledExceptions = value; } }

        /// <summary>
        /// Enables or disables input processing.                   
        /// </summary>
        public virtual bool InputEnabled { get { return inputEnabled; } set { inputEnabled = value; } }

        /// <summary>
        /// Gets or sets render target for drawing.                 
        /// </summary>    
        public virtual RenderTarget2D RenderTarget { get { return renderTarget; } set { renderTarget = value; } }

        /// <summary>
        /// Gets or sets update interval for drawing, logic and input.                           
        /// </summary>    
        public virtual int TargetFrames { get { return targetFrames; } set { targetFrames = value; } }

        //////////////////////////////////////////////////////////////////////////// 
        /// <summary>
        /// Gets or sets collection of active keyboard layouts.     
        /// </summary>
        public virtual List<KeyboardLayout> KeyboardLayouts
        {
            get { return keyboardLayouts; }
            set { keyboardLayouts = value; }
        }
        //////////////////////////////////////////////////////////////////////////// 

        //////////////////////////////////////////////////////////////////////////// 
        /// <summary>
        /// Gets or sets a value indicating if Guide component can be used
        /// </summary>
        public bool UseGuide
        {
            get { return useGuide; }
            set { useGuide = value; }
        }
        //////////////////////////////////////////////////////////////////////////// 

        //////////////////////////////////////////////////////////////////////////// 
        /// <summary>
        /// Gets or sets a value indicating if a control should unfocus if you click outside on the screen.
        /// </summary>
        //////////////////////////////////////////////////////////////////////////// 
        public virtual bool AutoUnfocus
        {
            get { return autoUnfocus; }
            set { autoUnfocus = value; }
        }
        //////////////////////////////////////////////////////////////////////////// 

        //////////////////////////////////////////////////////////////////////////// 
        /// <summary>
        /// Gets or sets a value indicating wheter Manager should create render target automatically.
        /// </summary>    
        //////////////////////////////////////////////////////////////////////////// 
        public virtual bool AutoCreateRenderTarget
        {
            get { return autoCreateRenderTarget; }
            set { autoCreateRenderTarget = value; }
        }
        ////////////////////////////////////////////////////////////////////////////     

        //////////////////////////////////////////////////////////////////////////// 
        /// <summary>
        /// Gets or sets current keyboard layout for text input.    
        /// </summary>
        public virtual KeyboardLayout KeyboardLayout
        {
            get
            {
                if (keyboardLayout == null)
                {
                    keyboardLayout = new KeyboardLayout();
                }
                return keyboardLayout;
            }
            set
            {
                keyboardLayout = value;
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets the initial directory for looking for the skins in.
        /// </summary>
        public virtual string SkinDirectory
        {
            get
            {
                if (!skinDirectory.EndsWith("\\"))
                {
                    skinDirectory += "\\";
                }
                return skinDirectory;
            }
            set
            {
                skinDirectory = value;
                if (!skinDirectory.EndsWith("\\"))
                {
                    skinDirectory += "\\";
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets the initial directory for looking for the layout files in.
        /// </summary>
        public virtual string LayoutDirectory
        {
            get
            {
                if (!layoutDirectory.EndsWith("\\"))
                {
                    layoutDirectory += "\\";
                }
                return layoutDirectory;
            }
            set
            {
                layoutDirectory = value;
                if (!layoutDirectory.EndsWith("\\"))
                {
                    layoutDirectory += "\\";
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets file extension for archived skin files.
        /// </summary>
        public string SkinExtension
        {
            get
            {
                if (!skinExtension.StartsWith("."))
                {
                    skinExtension = "." + skinExtension;
                }
                return skinExtension;
            }
            set
            {
                skinExtension = value;
                if (!skinExtension.StartsWith("."))
                {
                    skinExtension = "." + skinExtension;
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets width of the selected render target in pixels.
        /// </summary>
        public virtual int TargetWidth
        {
            get
            {
                if (renderTarget != null)
                {
                    return renderTarget.Width;
                }
                else return ScreenWidth;
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets height of the selected render target in pixels.
        /// </summary>
        public virtual int TargetHeight
        {
            get
            {
                if (renderTarget != null)
                {
                    return renderTarget.Height;
                }
                else return ScreenHeight;
            }
        }
        ////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets current width of the screen in pixels.
        /// </summary>
        public virtual int ScreenWidth
        {
            get
            {
                if (GraphicsDevice != null)
                {
                    return GraphicsDevice.PresentationParameters.BackBufferWidth;
                }
                else return 0;
            }

        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets current height of the screen in pixels.
        /// </summary>
        public virtual int ScreenHeight
        {
            get
            {
                if (GraphicsDevice != null)
                {
                    return GraphicsDevice.PresentationParameters.BackBufferHeight;
                }
                else return 0;
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets or sets new skin used by all controls.
        /// </summary>
        public virtual Skin Skin
        {
            get
            {
                return skin;
            }
            set
            {
                SetSkin(value);
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns currently active modal window.
        /// </summary>
        public virtual ModalContainer ModalWindow
        {
            get
            {
                return modalWindow;
            }
            internal set
            {
                modalWindow = value;

                if (value != null)
                {
                    value.ModalResult = ModalResult.None;

                    value.Visible = true;
                    value.Focused = true;
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Returns currently focused control.
        /// </summary>
        public virtual Control FocusedControl
        {
            get
            {
                return focusedControl;
            }
            internal set
            {
                if (value != null && value.Visible && value.Enabled)
                {
                    if (value != null && value.CanFocus)
                    {
                        if (focusedControl == null || (focusedControl != null && value.Root != focusedControl.Root) || !value.IsRoot)
                        {
                            if (focusedControl != null && focusedControl != value)
                            {
                                focusedControl.Focused = false;
                            }
                            focusedControl = value;
                        }
                    }
                    else if (value != null && !value.CanFocus)
                    {
                        if (focusedControl != null && value.Root != focusedControl.Root)
                        {
                            if (focusedControl != value.Root)
                            {
                                focusedControl.Focused = false;
                            }
                            focusedControl = value.Root;
                        }
                        else if (focusedControl == null)
                        {
                            focusedControl = value.Root;
                        }
                    }
                    BringToFront(value.Root);
                }
                else if (value == null)
                {
                    focusedControl = value;
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////  

        ////////////////////////////////////////////////////////////////////////////
        internal virtual ControlsList OrderList { get { return orderList; } }
        ////////////////////////////////////////////////////////////////////////////

        #endregion

        #region //// Events ////////////

        ////////////////////////////////////////////////////////////////////////////                 
        /// <summary>
        /// Occurs when the GraphicsDevice settings are changed.
        /// </summary>
        public event DeviceEventHandler DeviceSettingsChanged;

        /// <summary>
        /// Occurs when the skin is about to change.
        /// </summary>
        public event SkinEventHandler SkinChanging;

        /// <summary>
        /// Occurs when the skin changes.
        /// </summary>
        public event SkinEventHandler SkinChanged;

        /// <summary>
        /// Occurs when game window is about to close.
        /// </summary>
        public event WindowClosingEventHandler WindowClosing;
        ////////////////////////////////////////////////////////////////////////////

        #endregion

        #region //// Constructors //////

        ////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Initializes a new instance of the Manager class.
        /// </summary>
        /// <param name="game">
        /// The Game class.
        /// </param>
        /// <param name="graphics">
        /// The GraphicsDeviceManager class provided by the Game class.
        /// </param>
        /// <param name="skin">
        /// The name of the skin being loaded at the start.
        /// </param>
        public Manager(Game game, GraphicsDeviceManager graphics, string skin)
            : base(game)
        {
            disposing = false;

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(HandleUnhadledExceptions);

            content = new ArchiveManager(Game.Services);
            input = new InputSystem(this, new InputOffset(0, 0, 1f, 1f));
            components = new List<Component>();
            controls = new ControlsList();
            orderList = new ControlsList();

            this.graphics = graphics;
            graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(PrepareGraphicsDevice);

            skinName = skin;

#if (XBOX_FAKE)
        game.Window.Title += " (XBOX_FAKE)";
#endif

            states.Buttons = new Control[32];
            states.Click = -1;
            states.Over = null;

            input.MouseDown += new MouseEventHandler(MouseDownProcess);
            input.MouseUp += new MouseEventHandler(MouseUpProcess);
            input.MousePress += new MouseEventHandler(MousePressProcess);
            input.MouseMove += new MouseEventHandler(MouseMoveProcess);
            input.MouseScroll += new MouseEventHandler(MouseScrollProcess);

            input.GamePadDown += new GamePadEventHandler(GamePadDownProcess);
            input.GamePadUp += new GamePadEventHandler(GamePadUpProcess);
            input.GamePadPress += new GamePadEventHandler(GamePadPressProcess);

            input.KeyDown += new KeyEventHandler(KeyDownProcess);
            input.KeyUp += new KeyEventHandler(KeyUpProcess);
            input.KeyPress += new KeyEventHandler(KeyPressProcess);

            keyboardLayouts.Add(new KeyboardLayout());
            keyboardLayouts.Add(new CzechKeyboardLayout());
            keyboardLayouts.Add(new GermanKeyboardLayout());
        }
        ////////////////////////////////////////////////////////////////////////////                   

        ////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Initializes a new instance of the Manager class.
        /// </summary>
        /// <param name="game">
        /// The Game class.
        /// </param>   
        /// <param name="skin">
        /// The name of the skin being loaded at the start.
        /// </param>
        public Manager(Game game, string skin)
            : this(game, game.Services.GetService(typeof(IGraphicsDeviceManager)) as GraphicsDeviceManager, skin)
        {
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Initializes a new instance of the Manager class, loads the default skin and registers manager in the game class automatically.
        /// </summary>
        /// <param name="game">
        /// The Game class.
        /// </param>
        /// <param name="graphics">
        /// The GraphicsDeviceManager class provided by the Game class.
        /// </param>
        public Manager(Game game, GraphicsDeviceManager graphics)
            : this(game, graphics, _DefaultSkin)
        {
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Initializes a new instance of the Manager class, loads the default skin and registers manager in the game class automatically.
        /// </summary>
        /// <param name="game">
        /// The Game class.
        /// </param>
        public Manager(Game game)
            : this(game, game.Services.GetService(typeof(IGraphicsDeviceManager)) as GraphicsDeviceManager, _DefaultSkin)
        {
        }
        ////////////////////////////////////////////////////////////////////////////

        #endregion

        #region //// Destructors ///////

        ////////////////////////////////////////////////////////////////////////////
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.disposing = true;

                // Recursively disposing all controls added to the manager and its child controls.
                if (controls != null)
                {
                    int c = controls.Count;
                    for (int i = 0; i < c; i++)
                    {
                        if (controls.Count > 0) controls[0].Dispose();
                    }
                }

                // Disposing all components added to manager.
                if (components != null)
                {
                    int c = components.Count;
                    for (int i = 0; i < c; i++)
                    {
                        if (components.Count > 0) components[0].Dispose();
                    }
                }

                if (content != null)
                {
                    content.Unload();
                    content.Dispose();
                    content = null;
                }

                if (renderer != null)
                {
                    renderer.Dispose();
                    renderer = null;
                }
                if (input != null)
                {
                    input.Dispose();
                    input = null;
                }
            }
            if (GraphicsDevice != null)
                GraphicsDevice.DeviceReset -= new System.EventHandler<System.EventArgs>(GraphicsDevice_DeviceReset);
            base.Dispose(disposing);
        }
        ////////////////////////////////////////////////////////////////////////////

        #endregion

        #region //// Methods ///////////


        public void SetCursor(Cursor cursor)
        {
            this.cursor = cursor;
            if (this.cursor.CursorTexture == null)
            {
                this.cursor.CursorTexture = Texture2D.FromStream(GraphicsDevice, new FileStream(
                    this.cursor.cursorPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None));
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        private void InitSkins()
        {
            // Initializing skins for every control created, even not visible or 
            // not added to the manager or another parent.
            foreach (Control c in Control.Stack)
            {
                c.InitSkin();
            }
        }
        ////////////////////////////////////////////////////////////////////////////          

        ////////////////////////////////////////////////////////////////////////////
        private void InitControls()
        {
            // Initializing all controls created, even not visible or 
            // not added to the manager or another parent.
            foreach (Control c in Control.Stack)
            {
                c.Init();
            }
        }
        ////////////////////////////////////////////////////////////////////////////       

        ////////////////////////////////////////////////////////////////////////////     
        private void SortLevel(ControlsList cs)
        {
            if (cs != null)
            {
                foreach (Control c in cs)
                {
                    if (c.Visible)
                    {
                        OrderList.Add(c);
                        SortLevel(c.Controls as ControlsList);
                    }
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////  

        ////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Method used as an event handler for the GraphicsDeviceManager.PreparingDeviceSettings event.
        /// </summary>
        protected virtual void PrepareGraphicsDevice(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = _RenderTargetUsage;
            int w = e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth;
            int h = e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight;

            foreach (Control c in Controls)
            {
                SetMaxSize(c, w, h);
            }


            if (DeviceSettingsChanged != null) DeviceSettingsChanged.Invoke(new DeviceEventArgs(e));
        }
        ////////////////////////////////////////////////////////////////////////////      

        ////////////////////////////////////////////////////////////////////////////      
        private void SetMaxSize(Control c, int w, int h)
        {
            if (c.Width > w)
            {
                w -= (c.Skin != null) ? c.Skin.OriginMargins.Horizontal : 0;
                c.Width = w;
            }
            if (c.Height > h)
            {
                h -= (c.Skin != null) ? c.Skin.OriginMargins.Vertical : 0;
                c.Height = h;
            }

            foreach (Control cx in c.Controls)
            {
                SetMaxSize(cx, w, h);
            }
        }
        ////////////////////////////////////////////////////////////////////////////      

        ////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Initializes the controls manager.
        /// </summary>    
        ////////////////////////////////////////////////////////////////////////////
        public override void Initialize()
        {
            base.Initialize();

            if (autoCreateRenderTarget)
            {
                if (renderTarget != null)
                {
                    renderTarget.Dispose();
                }
                renderTarget = CreateRenderTarget();
            }

            GraphicsDevice.DeviceReset += new System.EventHandler<System.EventArgs>(GraphicsDevice_DeviceReset);

            input.Initialize();
            renderer = new Renderer(this);
            SetSkin(skinName);
        }
        ////////////////////////////////////////////////////////////////////////////

        private void InvalidateRenderTarget()
        {
            renderTargetValid = false;
        }

        ////////////////////////////////////////////////////////////////////////////
        public virtual RenderTarget2D CreateRenderTarget()
        {
            return CreateRenderTarget(ScreenWidth, ScreenHeight);
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        public virtual RenderTarget2D CreateRenderTarget(int width, int height)
        {
            Input.InputOffset = new InputOffset(0, 0, ScreenWidth / (float)width, ScreenHeight / (float)height);
            return new RenderTarget2D(GraphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None, GraphicsDevice.PresentationParameters.MultiSampleCount, _RenderTargetUsage);
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sets and loads the new skin.
        /// </summary>
        /// <param name="name">
        /// The name of the skin being loaded.
        /// </param>
        public virtual void SetSkin(string name)
        {
            Skin skin = new Skin(this, name);
            SetSkin(skin);
        }
        ////////////////////////////////////////////////////////////////////////////        

        ////////////////////////////////////////////////////////////////////////////        
        /// <summary>
        /// Sets the new skin.
        /// </summary>
        /// <param name="skin">
        /// The skin being set.
        /// </param>
        public virtual void SetSkin(Skin skin)
        {
            if (SkinChanging != null) SkinChanging.Invoke(new EventArgs());

            if (this.skin != null)
            {
                Remove(this.skin);
                this.skin.Dispose();
                this.skin = null;
                GC.Collect();
            }
            this.skin = skin;
            this.skin.Init();
            Add(this.skin);
            skinName = this.skin.Name;

#if (!XBOX && !XBOX_FAKE)
            if (this.skin.Cursors["Default"] != null)
            {
                SetCursor(this.skin.Cursors["Default"].Resource);
            }
#endif

            InitSkins();
            if (SkinChanged != null) SkinChanged.Invoke(new EventArgs());

            InitControls();
        }
        ////////////////////////////////////////////////////////////////////////////        

        ////////////////////////////////////////////////////////////////////////////    
        /// <summary>
        /// Brings the control to the front of the z-order.
        /// </summary>
        /// <param name="control">
        /// The control being brought to the front.
        /// </param>
        public virtual void BringToFront(Control control)
        {
            if (control != null && !control.StayOnBack)
            {
                ControlsList cs = (control.Parent == null) ? controls as ControlsList : control.Parent.Controls as ControlsList;
                if (cs.Contains(control))
                {
                    cs.Remove(control);
                    if (!control.StayOnTop)
                    {
                        int pos = cs.Count;
                        for (int i = cs.Count - 1; i >= 0; i--)
                        {
                            if (!cs[i].StayOnTop)
                            {
                                break;
                            }
                            pos = i;
                        }
                        cs.Insert(pos, control);
                    }
                    else
                    {
                        cs.Add(control);
                    }
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sends the control to the back of the z-order.
        /// </summary>
        /// <param name="control">
        /// The control being sent back.
        /// </param>
        public virtual void SendToBack(Control control)
        {
            if (control != null && !control.StayOnTop)
            {
                ControlsList cs = (control.Parent == null) ? controls as ControlsList : control.Parent.Controls as ControlsList;
                if (cs.Contains(control))
                {
                    cs.Remove(control);
                    if (!control.StayOnBack)
                    {
                        int pos = 0;
                        for (int i = 0; i < cs.Count; i++)
                        {
                            if (!cs[i].StayOnBack)
                            {
                                break;
                            }
                            pos = i;
                        }
                        cs.Insert(pos, control);
                    }
                    else
                    {
                        cs.Insert(0, control);
                    }
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////       

        ////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Called when the manager needs to be updated.
        /// </summary>
        /// <param name="gameTime">
        /// Time elapsed since the last call to Update.
        /// </param>
        public override void Update(GameTime gameTime)
        {
            updateTime += gameTime.ElapsedGameTime.Ticks;
            double ms = TimeSpan.FromTicks(updateTime).TotalMilliseconds;

            if (targetFrames == 0 || ms == 0 || ms >= (1000f / targetFrames))
            {
                TimeSpan span = TimeSpan.FromTicks(updateTime);
                gameTime = new GameTime(gameTime.TotalGameTime, span);
                updateTime = 0;

                if (inputEnabled)
                {
                    input.Update(gameTime);
                }

                if (components != null)
                {
                    foreach (Component c in components)
                    {
                        c.Update(gameTime);
                    }
                }

                ControlsList list = new ControlsList(controls);

                if (list != null)
                {
                    foreach (Control c in list)
                    {
                        c.Update(gameTime);
                    }
                }

                OrderList.Clear();
                SortLevel(controls);
            }
        }
        ////////////////////////////////////////////////////////////////////////////   

        ////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Adds a component or a control to the manager.
        /// </summary>
        /// <param name="component">
        /// The component or control being added.
        /// </param>
        public virtual void Add(Component component)
        {
            if (component != null)
            {
                if (component is Control && !controls.Contains(component as Control))
                {
                    Control c = (Control)component;

                    if (c.Parent != null) c.Parent.Remove(c);

                    controls.Add(c);
                    c.Manager = this;
                    c.Parent = null;
                    if (focusedControl == null) c.Focused = true;

                    DeviceSettingsChanged += new DeviceEventHandler((component as Control).OnDeviceSettingsChanged);
                    SkinChanging += new SkinEventHandler((component as Control).OnSkinChanging);
                    SkinChanged += new SkinEventHandler((component as Control).OnSkinChanged);
                }
                else if (!(component is Control) && !components.Contains(component))
                {
                    components.Add(component);
                    component.Manager = this;
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////    

        ////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Removes a component or a control from the manager.
        /// </summary>
        /// <param name="component">
        /// The component or control being removed.
        /// </param>
        public virtual void Remove(Component component)
        {
            if (component != null)
            {
                if (component is Control)
                {
                    Control c = component as Control;
                    SkinChanging -= c.OnSkinChanging;
                    SkinChanged -= c.OnSkinChanged;
                    DeviceSettingsChanged -= c.OnDeviceSettingsChanged;

                    if (c.Focused) c.Focused = false;
                    controls.Remove(c);
                }
                else
                {
                    components.Remove(component);
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////   

        ////////////////////////////////////////////////////////////////////////////   
        public virtual void Prepare(GameTime gameTime)
        {

        }
        ////////////////////////////////////////////////////////////////////////////   

        ////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Renders all controls added to the manager.
        /// </summary>
        /// <param name="gameTime">
        /// Time passed since the last call to Draw.
        /// </param>
        public virtual void BeginDraw(GameTime gameTime)
        {
            if (!renderTargetValid && AutoCreateRenderTarget)
            {
                if (renderTarget != null) RenderTarget.Dispose();
                RenderTarget = CreateRenderTarget();
                renderer = new Renderer(this);
            }
            Draw(gameTime);
        }
        //////////////////////////////////////////////////////////////////////////// 

        ////////////////////////////////////////////////////////////////////////////   
        public override void Draw(GameTime gameTime)
        {
            if (renderTarget != null)
            {
                drawTime += gameTime.ElapsedGameTime.Ticks;
                double ms = TimeSpan.FromTicks(drawTime).TotalMilliseconds;

                //if (targetFrames == 0 || (ms == 0 || ms >= (1000f / targetFrames)))
                //{
                TimeSpan span = TimeSpan.FromTicks(drawTime);
                gameTime = new GameTime(gameTime.TotalGameTime, span);
                drawTime = 0;

                if ((controls != null))
                {
                    ControlsList list = new ControlsList();
                    list.AddRange(controls);

                    foreach (Control c in list)
                    {
                        c.PrepareTexture(renderer, gameTime);
                    }

                    GraphicsDevice.SetRenderTarget(renderTarget);
                    GraphicsDevice.Clear(Color.Transparent);

                    if (renderer != null)
                    {
                        foreach (Control c in list)
                        {
                            c.Render(renderer, gameTime);
                        }
                    }
                }

                if (softwareCursor && Cursor != null)
                {
                    if (this.cursor.CursorTexture == null)
                    {
                        this.cursor.CursorTexture = Texture2D.FromStream(GraphicsDevice, new FileStream(
                            this.cursor.cursorPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None));
                    }
                    renderer.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    MouseState mstate = Mouse.GetState();
                    Rectangle rect = new Rectangle(mstate.X, mstate.Y, Cursor.Width, Cursor.Height);
                    renderer.SpriteBatch.Draw(Cursor.CursorTexture, rect, null, Color.White, 0f, Cursor.HotSpot, SpriteEffects.None, 0f);
                    renderer.SpriteBatch.End();
                }

                GraphicsDevice.SetRenderTarget(null);
                //}
            }
            else
            {
                throw new Exception("Manager.RenderTarget has to be specified. Assign a render target or set Manager.AutoCreateRenderTarget property to true.");
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Draws texture resolved from RenderTarget used for rendering.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////
        public virtual void EndDraw()
        {
            EndDraw(new Rectangle(0, 0, ScreenWidth, ScreenHeight));
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Draws texture resolved from RenderTarget to specified rectangle.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////
        public virtual void EndDraw(Rectangle rect)
        {
            if (renderTarget != null && !deviceReset)
            {
                renderer.Begin(BlendingMode.Default);
                renderer.Draw(RenderTarget, rect, Color.White);
                renderer.End();
            }
            else if (deviceReset)
            {
                deviceReset = false;
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        public virtual Control GetControl(string name)
        {
            foreach (Control c in Controls)
            {
                if (c.Name.ToLower() == name.ToLower())
                {
                    return c;
                }
            }
            return null;
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////     
        private void HandleUnhadledExceptions(object sender, UnhandledExceptionEventArgs e)
        {
            if (LogUnhandledExceptions)
            {
                LogException(e.ExceptionObject as Exception);
            }
        }
        ////////////////////////////////////////////////////////////////////////////     

        ////////////////////////////////////////////////////////////////////////////     
        private void GraphicsDevice_DeviceReset(object sender, System.EventArgs e)
        {
            deviceReset = true;
            InvalidateRenderTarget();
            /*if (AutoCreateRenderTarget) 
            {
              if (renderTarget != null) RenderTarget.Dispose();
              RenderTarget = CreateRenderTarget();        
            }
            }*/
        }
        ////////////////////////////////////////////////////////////////////////////     

        ////////////////////////////////////////////////////////////////////////////     
        public virtual void LogException(Exception e)
        {
#if (!XBOX && !XBOX_FAKE)
            string an = Assembly.GetEntryAssembly().Location;
            Assembly asm = Assembly.GetAssembly(typeof(Manager));
            string path = Path.GetDirectoryName(an);
            string fn = path + "\\" + Path.GetFileNameWithoutExtension(asm.Location) + ".log";

            File.AppendAllText(fn, "////////////////////////////////////////////////////////////////\n" +
                                   "    Date: " + DateTime.Now.ToString() + "\n" +
                                   "Assembly: " + Path.GetFileName(asm.Location) + "\n" +
                                   " Version: " + asm.GetName().Version.ToString() + "\n" +
                                   " Message: " + e.Message + "\n" +
                                   "////////////////////////////////////////////////////////////////\n" +
                                   e.StackTrace + "\n" +
                                   "////////////////////////////////////////////////////////////////\n\n", Encoding.Default);
#endif
        }
        ////////////////////////////////////////////////////////////////////////////     

        #endregion

        #region //// Input /////////////

        ////////////////////////////////////////////////////////////////////////////
        private bool CheckParent(Control control, Point pos)
        {
            if (control.Parent != null && !CheckDetached(control))
            {
                Control parent = control.Parent;
                Control root = control.Root;

                Rectangle pr = new Rectangle(parent.AbsoluteLeft,
                                             parent.AbsoluteTop,
                                             parent.Width,
                                             parent.Height);

                Margins margins = root.Skin.ClientMargins;
                Rectangle rr = new Rectangle(root.AbsoluteLeft + margins.Left,
                                             root.AbsoluteTop + margins.Top,
                                             root.OriginWidth - margins.Horizontal,
                                             root.OriginHeight - margins.Vertical);


                return (rr.Contains(pos) && pr.Contains(pos));
            }

            return true;
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        private bool CheckState(Control control)
        {
            bool modal = (ModalWindow == null) ? true : (ModalWindow == control.Root);

            return (control != null && !control.Passive && control.Visible && control.Enabled && modal);
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        private bool CheckOrder(Control control, Point pos)
        {
            if (!CheckPosition(control, pos)) return false;

            for (int i = OrderList.Count - 1; i > OrderList.IndexOf(control); i--)
            {
                Control c = OrderList[i];

                if (!c.Passive && CheckPosition(c, pos) && CheckParent(c, pos))
                {
                    return false;
                }
            }

            return true;
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        private bool CheckDetached(Control control)
        {
            bool ret = control.Detached;
            if (control.Parent != null)
            {
                if (CheckDetached(control.Parent)) ret = true;
            }
            return ret;
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        private bool CheckPosition(Control control, Point pos)
        {
            return (control.AbsoluteLeft <= pos.X &&
                    control.AbsoluteTop <= pos.Y &&
                    control.AbsoluteLeft + control.Width >= pos.X &&
                    control.AbsoluteTop + control.Height >= pos.Y &&
                    CheckParent(control, pos));
        }
        ////////////////////////////////////////////////////////////////////////////   

        ////////////////////////////////////////////////////////////////////////////
        private bool CheckButtons(int index)
        {
            for (int i = 0; i < states.Buttons.Length; i++)
            {
                if (i == index) continue;
                if (states.Buttons[i] != null) return false;
            }

            return true;
        }
        ////////////////////////////////////////////////////////////////////////////    

        ////////////////////////////////////////////////////////////////////////////
        private void TabNextControl(Control control)
        {
            int start = OrderList.IndexOf(control);
            int i = start;

            do
            {
                if (i < OrderList.Count - 1) i += 1;
                else i = 0;
            }
            while ((OrderList[i].Root != control.Root || !OrderList[i].CanFocus || OrderList[i].IsRoot || !OrderList[i].Enabled) && i != start);

            OrderList[i].Focused = true;
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////    
        private void TabPrevControl(Control control)
        {
            int start = OrderList.IndexOf(control);
            int i = start;

            do
            {
                if (i > 0) i -= 1;
                else i = OrderList.Count - 1;
            }
            while ((OrderList[i].Root != control.Root || !OrderList[i].CanFocus || OrderList[i].IsRoot || !OrderList[i].Enabled) && i != start);
            OrderList[i].Focused = true;
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////    
        private void ProcessArrows(Control control, KeyEventArgs kbe, GamePadEventArgs gpe)
        {
            Control c = control;
            if (c.Parent != null && c.Parent.Controls != null)
            {
                int index = -1;

                if ((kbe.Key == Microsoft.Xna.Framework.Input.Keys.Left && !kbe.Handled) ||
                    (gpe.Button == c.GamePadActions.Left && !gpe.Handled))
                {
                    int miny = int.MaxValue;
                    int minx = int.MinValue;
                    for (int i = 0; i < (c.Parent.Controls as ControlsList).Count; i++)
                    {
                        Control cx = (c.Parent.Controls as ControlsList)[i];
                        if (cx == c || !cx.Visible || !cx.Enabled || cx.Passive || !cx.CanFocus) continue;

                        int cay = (int)(c.Top + (c.Height / 2));
                        int cby = (int)(cx.Top + (cx.Height / 2));

                        if (Math.Abs(cay - cby) <= miny && (cx.Left + cx.Width) >= minx && (cx.Left + cx.Width) <= c.Left)
                        {
                            miny = Math.Abs(cay - cby);
                            minx = cx.Left + cx.Width;
                            index = i;
                        }
                    }
                }
                else if ((kbe.Key == Microsoft.Xna.Framework.Input.Keys.Right && !kbe.Handled) ||
                         (gpe.Button == c.GamePadActions.Right && !gpe.Handled))
                {
                    int miny = int.MaxValue;
                    int minx = int.MaxValue;
                    for (int i = 0; i < (c.Parent.Controls as ControlsList).Count; i++)
                    {
                        Control cx = (c.Parent.Controls as ControlsList)[i];
                        if (cx == c || !cx.Visible || !cx.Enabled || cx.Passive || !cx.CanFocus) continue;

                        int cay = (int)(c.Top + (c.Height / 2));
                        int cby = (int)(cx.Top + (cx.Height / 2));

                        if (Math.Abs(cay - cby) <= miny && cx.Left <= minx && cx.Left >= (c.Left + c.Width))
                        {
                            miny = Math.Abs(cay - cby);
                            minx = cx.Left;
                            index = i;
                        }
                    }
                }
                else if ((kbe.Key == Microsoft.Xna.Framework.Input.Keys.Up && !kbe.Handled) ||
                         (gpe.Button == c.GamePadActions.Up && !gpe.Handled))
                {
                    int miny = int.MinValue;
                    int minx = int.MaxValue;
                    for (int i = 0; i < (c.Parent.Controls as ControlsList).Count; i++)
                    {
                        Control cx = (c.Parent.Controls as ControlsList)[i];
                        if (cx == c || !cx.Visible || !cx.Enabled || cx.Passive || !cx.CanFocus) continue;

                        int cax = (int)(c.Left + (c.Width / 2));
                        int cbx = (int)(cx.Left + (cx.Width / 2));

                        if (Math.Abs(cax - cbx) <= minx && (cx.Top + cx.Height) >= miny && (cx.Top + cx.Height) <= c.Top)
                        {
                            minx = Math.Abs(cax - cbx);
                            miny = cx.Top + cx.Height;
                            index = i;
                        }
                    }
                }
                else if ((kbe.Key == Microsoft.Xna.Framework.Input.Keys.Down && !kbe.Handled) ||
                         (gpe.Button == c.GamePadActions.Down && !gpe.Handled))
                {
                    int miny = int.MaxValue;
                    int minx = int.MaxValue;
                    for (int i = 0; i < (c.Parent.Controls as ControlsList).Count; i++)
                    {
                        Control cx = (c.Parent.Controls as ControlsList)[i];
                        if (cx == c || !cx.Visible || !cx.Enabled || cx.Passive || !cx.CanFocus) continue;

                        int cax = (int)(c.Left + (c.Width / 2));
                        int cbx = (int)(cx.Left + (cx.Width / 2));

                        if (Math.Abs(cax - cbx) <= minx && cx.Top <= miny && cx.Top >= (c.Top + c.Height))
                        {
                            minx = Math.Abs(cax - cbx);
                            miny = cx.Top;
                            index = i;
                        }
                    }
                }

                if (index != -1)
                {
                    (c.Parent.Controls as ControlsList)[index].Focused = true;
                    kbe.Handled = true;
                    gpe.Handled = true;
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////               

        ////////////////////////////////////////////////////////////////////////////            
        private void MouseDownProcess(object sender, MouseEventArgs e)
        {
            ControlsList c = new ControlsList();
            c.AddRange(OrderList);

            if (autoUnfocus && focusedControl != null && focusedControl.Root != modalWindow)
            {
                bool hit = false;

                foreach (Control cx in Controls)
                {
                    if (cx.AbsoluteRect.Contains(e.Position))
                    {
                        hit = true;
                        break;
                    }
                }
                if (!hit)
                {
                    for (int i = 0; i < Control.Stack.Count; i++)
                    {
                        if (Control.Stack[i].Visible && Control.Stack[i].Detached && Control.Stack[i].AbsoluteRect.Contains(e.Position))
                        {
                            hit = true;
                            break;
                        }
                    }
                }
                if (!hit) focusedControl.Focused = false;
            }

            for (int i = c.Count - 1; i >= 0; i--)
            {
                if (CheckState(c[i]) && CheckPosition(c[i], e.Position))
                {
                    states.Buttons[(int)e.Button] = c[i];
                    c[i].SendMessage(Message.MouseDown, e);

                    if (states.Click == -1)
                    {
                        states.Click = (int)e.Button;

                        if (FocusedControl != null)
                        {
                            FocusedControl.Invalidate();
                        }
                        c[i].Focused = true;
                    }
                    return;
                }
            }

            if (ModalWindow != null)
            {
#if (!XBOX && !XBOX_FAKE)
                SystemSounds.Beep.Play();
#endif
            }
        }
        ////////////////////////////////////////////////////////////////////////////   

        ////////////////////////////////////////////////////////////////////////////            
        private void MouseUpProcess(object sender, MouseEventArgs e)
        {
            Control c = states.Buttons[(int)e.Button];
            if (c != null)
            {
                if (CheckPosition(c, e.Position) && CheckOrder(c, e.Position) && states.Click == (int)e.Button && CheckButtons((int)e.Button))
                {
                    c.SendMessage(Message.Click, e);
                }
                states.Click = -1;
                c.SendMessage(Message.MouseUp, e);
                states.Buttons[(int)e.Button] = null;
                MouseMoveProcess(sender, e);
            }
        }
        //////////////////////////////////////////////////////////////////////////// 

        ////////////////////////////////////////////////////////////////////////////            
        private void MousePressProcess(object sender, MouseEventArgs e)
        {
            Control c = states.Buttons[(int)e.Button];
            if (c != null)
            {
                if (CheckPosition(c, e.Position))
                {
                    c.SendMessage(Message.MousePress, e);
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////              

        ////////////////////////////////////////////////////////////////////////////            
        private void MouseMoveProcess(object sender, MouseEventArgs e)
        {
            ControlsList c = new ControlsList();
            c.AddRange(OrderList);

            for (int i = c.Count - 1; i >= 0; i--)
            {
                bool chpos = CheckPosition(c[i], e.Position);
                bool chsta = CheckState(c[i]);

                if (chsta && ((chpos && states.Over == c[i]) || (states.Buttons[(int)e.Button] == c[i])))
                {
                    c[i].SendMessage(Message.MouseMove, e);
                    break;
                }
            }

            for (int i = c.Count - 1; i >= 0; i--)
            {
                bool chpos = CheckPosition(c[i], e.Position);
                bool chsta = CheckState(c[i]) || (c[i].ToolTip.Text != "" && c[i].ToolTip.Text != null && c[i].Visible);

                if (chsta && !chpos && states.Over == c[i] && states.Buttons[(int)e.Button] == null)
                {
                    states.Over = null;
                    c[i].SendMessage(Message.MouseOut, e);
                    break;
                }
            }

            for (int i = c.Count - 1; i >= 0; i--)
            {
                bool chpos = CheckPosition(c[i], e.Position);
                bool chsta = CheckState(c[i]) || (c[i].ToolTip.Text != "" && c[i].ToolTip.Text != null && c[i].Visible);

                if (chsta && chpos && states.Over != c[i] && states.Buttons[(int)e.Button] == null)
                {
                    if (states.Over != null)
                    {
                        states.Over.SendMessage(Message.MouseOut, e);
                    }
                    states.Over = c[i];
                    c[i].SendMessage(Message.MouseOver, e);
                    break;
                }
                else if (states.Over == c[i]) break;
            }
        }
        ////////////////////////////////////////////////////////////////////////////      

        /// <summary>
        /// Processes mouse scroll events for the manager.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseScrollProcess(object sender, MouseEventArgs e)
        {
            ControlsList c = new ControlsList();
            c.AddRange(OrderList);

            for (int i = c.Count - 1; i >= 0; i--)
            {
                bool chpos = CheckPosition(c[i], e.Position);
                bool chsta = CheckState(c[i]);

                if (chsta && chpos && states.Over == c[i])
                {
                    c[i].SendMessage(Message.MouseScroll, e);
                    break;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////
        void GamePadDownProcess(object sender, GamePadEventArgs e)
        {
            Control c = FocusedControl;

            if (c != null && CheckState(c))
            {
                if (states.Click == -1)
                {
                    states.Click = (int)e.Button;
                }
                states.Buttons[(int)e.Button] = c;
                c.SendMessage(Message.GamePadDown, e);

                if (e.Button == c.GamePadActions.Click)
                {
                    c.SendMessage(Message.Click, new MouseEventArgs(new MouseState(), MouseButton.None, Point.Zero));
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        void GamePadUpProcess(object sender, GamePadEventArgs e)
        {
            Control c = states.Buttons[(int)e.Button];

            if (c != null)
            {
                if (e.Button == c.GamePadActions.Press)
                {
                    c.SendMessage(Message.Click, new MouseEventArgs(new MouseState(), MouseButton.None, Point.Zero));
                }
                states.Click = -1;
                states.Buttons[(int)e.Button] = null;
                c.SendMessage(Message.GamePadUp, e);
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        void GamePadPressProcess(object sender, GamePadEventArgs e)
        {
            Control c = states.Buttons[(int)e.Button];
            if (c != null)
            {
                c.SendMessage(Message.GamePadPress, e);

                if ((e.Button == c.GamePadActions.Right ||
                     e.Button == c.GamePadActions.Left ||
                     e.Button == c.GamePadActions.Up ||
                     e.Button == c.GamePadActions.Down) && !e.Handled && CheckButtons((int)e.Button))
                {
                    ProcessArrows(c, new KeyEventArgs(), e);
                    GamePadDownProcess(sender, e);
                }
                else if (e.Button == c.GamePadActions.NextControl && !e.Handled && CheckButtons((int)e.Button))
                {
                    TabNextControl(c);
                    GamePadDownProcess(sender, e);
                }
                else if (e.Button == c.GamePadActions.PrevControl && !e.Handled && CheckButtons((int)e.Button))
                {
                    TabPrevControl(c);
                    GamePadDownProcess(sender, e);
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////   

        ////////////////////////////////////////////////////////////////////////////
        void KeyDownProcess(object sender, KeyEventArgs e)
        {
            Control c = FocusedControl;

            if (c != null && CheckState(c))
            {
                if (states.Click == -1)
                {
                    states.Click = (int)MouseButton.None;
                }
                states.Buttons[(int)MouseButton.None] = c;
                c.SendMessage(Message.KeyDown, e);

                if (e.Key == Microsoft.Xna.Framework.Input.Keys.Enter)
                {
                    c.SendMessage(Message.Click, new MouseEventArgs(new MouseState(), MouseButton.None, Point.Zero));
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        void KeyUpProcess(object sender, KeyEventArgs e)
        {
            Control c = states.Buttons[(int)MouseButton.None];

            if (c != null)
            {
                if (e.Key == Microsoft.Xna.Framework.Input.Keys.Space)
                {
                    c.SendMessage(Message.Click, new MouseEventArgs(new MouseState(), MouseButton.None, Point.Zero));
                }
                states.Click = -1;
                states.Buttons[(int)MouseButton.None] = null;
                c.SendMessage(Message.KeyUp, e);
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////
        void KeyPressProcess(object sender, KeyEventArgs e)
        {
            Control c = states.Buttons[(int)MouseButton.None];
            if (c != null)
            {
                c.SendMessage(Message.KeyPress, e);

                if ((e.Key == Microsoft.Xna.Framework.Input.Keys.Right ||
                     e.Key == Microsoft.Xna.Framework.Input.Keys.Left ||
                     e.Key == Microsoft.Xna.Framework.Input.Keys.Up ||
                     e.Key == Microsoft.Xna.Framework.Input.Keys.Down) && !e.Handled && CheckButtons((int)MouseButton.None))
                {
                    ProcessArrows(c, e, new GamePadEventArgs(PlayerIndex.One));
                    KeyDownProcess(sender, e);
                }
                else if (e.Key == Microsoft.Xna.Framework.Input.Keys.Tab && !e.Shift && !e.Handled && CheckButtons((int)MouseButton.None))
                {
                    TabNextControl(c);
                    KeyDownProcess(sender, e);
                }
                else if (e.Key == Microsoft.Xna.Framework.Input.Keys.Tab && e.Shift && !e.Handled && CheckButtons((int)MouseButton.None))
                {
                    TabPrevControl(c);
                    KeyDownProcess(sender, e);
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////   

        #endregion

    }
    ////////////////////////////////////////////////////////////////////////////

    #endregion

}
