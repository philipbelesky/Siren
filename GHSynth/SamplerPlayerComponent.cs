﻿using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Grasshopper.Kernel.Types;
using NAudio.Wave;
using System;
using System.Drawing;
using System.Threading;

namespace GHSynth
{
    public class SamplerPlayerComponent : GH_Component
	{
        public IWaveProvider Wave { get; private set; }

        /// <summary>
        /// Initializes a new instance of the SamplerPlayerComponent class.
        /// </summary>
        public SamplerPlayerComponent()
		  : base("SamplerPlayerComponent", "Nickname",
			  "Description",
              "GHSynth", "Subcategory")
		{
            Wave = new RawSourceWaveStream(new byte[0], 0, 0, new WaveFormat());
        }

		public override void CreateAttributes()
		{
			m_attributes = new CustomGHButton(this);
		}

		/// <summary>
		/// Registers all the input parameters for this component.
		/// </summary>
		protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
		{
            pManager.AddParameter(new WaveStreamParameter(), "Wave", "W", "Wave input", GH_ParamAccess.item);
        }

		/// <summary>
		/// Registers all the output parameters for this component.
		/// </summary>
		protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
		{
		}

		/// <summary>
		/// This is the method that actually does the work.
		/// </summary>
		/// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
		protected override void SolveInstance(IGH_DataAccess DA)
		{
            var wave = new RawSourceWaveStream(new byte[0], 0, 0, new WaveFormat()) as IWaveProvider;

            if (!DA.GetData(0, ref wave)) return;
            Wave = wave;

            //GH_ObjectWrapper obj = new GH_ObjectWrapper();
            //if (!DA.GetData(0, ref obj)) return;
            //Sample = (RawSourceWaveStream) obj.Value;
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.playback;

		/// <summary>
		/// Gets the unique ID for this component. Do not change this ID after release.
		/// </summary>
		public override Guid ComponentGuid
		{
			get { return new Guid("55f99243-1902-4ae3-a1e4-b2041ac6abf1"); }
		}
    }

    public class CustomGHButton : GH_ComponentAttributes
    {
        private Bitmap icon;
        private int buttonOffset;
        private Rectangle button;
        //private RectangleF textBox;
        private SamplerPlayerComponent owner;
        
        public CustomGHButton(SamplerPlayerComponent owner) : base(owner)
        {
            icon = Properties.Resources.playback_Off;
            this.owner = owner;
            buttonOffset = (Convert.ToInt32(Bounds.Height) - 24) / 2;
            //textBox = new RectangleF(Bounds.X + buttonOffset / 2, Bounds.Y, Bounds.Width - Bounds.Height, Bounds.Height);
            button = new Rectangle(Convert.ToInt32(Bounds.X) + Convert.ToInt32(Bounds.Width) - Convert.ToInt32(Bounds.Height) + buttonOffset,
                                   Convert.ToInt32(Bounds.Y) + buttonOffset,
                                   Convert.ToInt32(Bounds.Height) - (buttonOffset * 2),
                                   Convert.ToInt32(Bounds.Height) - (buttonOffset * 2));
            
            
        }

        protected override void Layout()
        {
            base.Layout();
            RectangleF updatedBounds = Bounds;
            updatedBounds.Width = 96;
            updatedBounds.Height = 36;
            Bounds = updatedBounds;

            var componentInputs = (this.DocObject as IGH_Component).Params.Input;
            var inputAttributes = componentInputs[0].Attributes;
            //inputAttributes.Pivot = new PointF(10f, 10f);
            //inputAttributes.InputGrip
            RectangleF inputBounds = inputAttributes.Bounds;
            inputBounds.Y += 4;
            //updatedBounds.Y = Bounds.Y + Bounds.Height * 0.5f;//inputAttributes.InputGrip;
            inputAttributes.Bounds = inputBounds;
            //inputAttributes.PerformLayout();
            //var grip = inputAttributes.InputGrip;
            //grip.Y = 0.0f;

            //Rhino.RhinoApp.WriteLine($"input grip count {componentInputs.Count}");

            //Rhino.RhinoApp.WriteLine($"input grip {inputAttributes.InputGrip}");
            //Rhino.RhinoApp.WriteLine($"Bounds location {inputAttributes.Bounds.Location}");
            //Rhino.RhinoApp.WriteLine($"Bounds {inputAttributes.Bounds}");

            ////this.Parent.

            //Rhino.RhinoApp.WriteLine($"top elvel {inputAttributes.IsTopLevel}");
            //Rhino.RhinoApp.WriteLine($"input grip {inputAttributes.HasInputGrip}");

            //Rhino.RhinoApp.WriteLine($"Pivot {inputAttributes.Pivot}");
            //Rhino.RhinoApp.WriteLine($"Bounds {inputAttributes.Bounds}");

            //var inputs = this.InputGrip;
            //inputs.Y = 0.0f;
        }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            if (channel == GH_CanvasChannel.Wires || channel == GH_CanvasChannel.Overlay)
            {
                base.Render(canvas, graphics, channel);
            }

            if (channel == GH_CanvasChannel.Objects)
            {
                //Render output grip.
                GH_CapsuleRenderEngine.RenderInputGrip(graphics, canvas.Viewport.Zoom, InputGrip, true);

                // Updating the capsule rectangles
                buttonOffset = 0;//(Convert.ToInt32(Bounds.Height) - 24) / 2;
                var textBox = new RectangleF(Bounds.X + buttonOffset / 2, Bounds.Y, Bounds.Width - Bounds.Height, Bounds.Height);
                button = new Rectangle(Convert.ToInt32(Bounds.X) + Convert.ToInt32(Bounds.Width) - Convert.ToInt32(Bounds.Height) + buttonOffset,
                                       Convert.ToInt32(Bounds.Y) + buttonOffset,
                                       Convert.ToInt32(Bounds.Height) - (buttonOffset * 2),
                                       Convert.ToInt32(Bounds.Height) - (buttonOffset * 2));

                // Creating the capsules
                GH_Capsule outerCapsule = GH_Capsule.CreateTextCapsule(Bounds, textBox, GH_Palette.Black, "W");
                //GH_Capsule outerCapsule = GH_Capsule.CreateCapsule(Bounds, GH_Palette.Black);
                GH_Capsule buttonCapsule = GH_Capsule.CreateCapsule(button, GH_Palette.Transparent, 2, 2);

                // Rendering the capsules
                outerCapsule.Render(graphics, Selected, Owner.Locked, true);
                buttonCapsule.Render(graphics, icon, Color.Transparent);

                // Disposing of the capsules
                outerCapsule.Dispose();
                buttonCapsule.Dispose();
            }
        }

        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            // Checking if it's a left click, and if it's in the button's area
            if (e.Button == System.Windows.Forms.MouseButtons.Left && ((RectangleF)button).Contains(e.CanvasLocation))
            {
                // Changing the image
                icon = Properties.Resources.playback_On;
            }

            return base.RespondToMouseDown(sender, e);
        }

        public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            // Checking if the left mouse button is the one being used
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                // Updating the icon
                icon = Properties.Resources.playback_Off;

                sender.ScheduleRegen(1);

                // Checking if it was clicked, and if it's in the right area
                if (!Owner.Locked && e.Clicks >= 1 && ((RectangleF)button).Contains(e.CanvasLocation))
                {
                    using (var outputDevice = new WaveOutEvent())
                    {
                        if (owner.Wave is WaveStream)
                            (owner.Wave as WaveStream).CurrentTime = TimeSpan.FromMilliseconds(0);

                        outputDevice.Init(owner.Wave);
                        outputDevice.Play();
                        while (outputDevice.PlaybackState == PlaybackState.Playing)
                        {
                            Thread.Sleep(200);
                        }

                        

                    }
                }
            }
            return base.RespondToMouseDown(sender, e);
        }
    }
}