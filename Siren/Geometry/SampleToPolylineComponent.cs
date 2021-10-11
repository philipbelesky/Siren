﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using NAudio.Wave;
using Rhino.Geometry;

namespace Siren.Geometry
{
	public class SampleToPolylineComponent : GH_Component
	{
		protected double cachedX;
		protected double cachedY;
		protected int cachedResolution;
		protected int cachedWaveHash;
		protected Polyline polyline;

		/// <summary>
		/// Initializes a new instance of the SampleToCurveComponent class.
		/// </summary>
		public SampleToPolylineComponent()
		  : base("SampleToCurveComponent", "Nickname",
			  "Description",
			  "Siren", "Utilities")
		{
		}

		/// <summary>
		/// Registers all the input parameters for this component.
		/// </summary>
		protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
		{
			pManager.AddParameter(new WaveStreamParameter(), "Wave", "W", "Wave input", GH_ParamAccess.item);
			pManager.AddNumberParameter("Time Factor", "T", "T", GH_ParamAccess.item);
			pManager.AddNumberParameter("Amplitude Factor", "A", "A", GH_ParamAccess.item);
			pManager.AddIntegerParameter("Resolution", "R", "Resolution of the display", GH_ParamAccess.item);
			pManager.AddIntervalParameter("Play Progress", "P", "Provided by the sample player; visualises a playhead on the line", GH_ParamAccess.item);

			pManager[1].Optional = true;
			pManager[2].Optional = true;
			pManager[4].Optional = true;
		}

		/// <summary>
		/// Registers all the output parameters for this component.
		/// </summary>
		protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
		{
			pManager.AddCurveParameter("Polyline", "P", "Polyline", GH_ParamAccess.item);
			pManager.AddCurveParameter("Playhead", "H", "Line representing the current played progress, provided by the corresponding Sample Player ", GH_ParamAccess.item);
		}

		/// <summary>
		/// This is the method that actually does the work.
		/// </summary>
		/// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
		protected override void SolveInstance(IGH_DataAccess DA)
		{
			var waveIn = CachedSound.Empty;
			if (!DA.GetData(0, ref waveIn)) return;

			double X = SirenSettings.TimeScale;
			double Y = SirenSettings.AmplitudeScale;
			int resolution = 10;
			Interval? T = null;

			DA.GetData(1, ref X); if (X <= 0) throw new Exception("T must be positive");
			DA.GetData(2, ref Y); if (Y <= 0) throw new Exception("A must be positive");
			DA.GetData("Resolution", ref resolution); if (resolution <= 0) throw new Exception("Resolution must be positive");
			DA.GetData(4, ref T); 

			// Don't regenerate the waveform if only the playhead input parameter has updated
			if (X != cachedX || Y != cachedY || resolution != cachedResolution || waveIn.GetHashCode() != cachedWaveHash)
			{
				polyline = GeometryFunctions.ISampleToPolyline(waveIn.ToSampleProvider(), X, Y, resolution);
				cachedX = X; cachedY = Y; cachedResolution = resolution;  cachedWaveHash = waveIn.GetHashCode();
			}

			DA.SetData(0, polyline);

			if (T.HasValue)
            {
				var playheadX = (T.Value.T0 / T.Value.T1) * (polyline[polyline.Count - 1].X - polyline[0].X);
				var playhead = new Line(new Point3d(playheadX, Y * -1.0, 0), new Point3d(playheadX, Y, 0));
				DA.SetData(1, playhead);
			}
		}

		/// <summary>
		/// Provides an Icon for the component.
		/// </summary>
		protected override System.Drawing.Bitmap Icon => Properties.Resources.curve;

		/// <summary>
		/// Gets the unique ID for this component. Do not change this ID after release.
		/// </summary>
		public override Guid ComponentGuid
		{
			get { return new Guid("63c5f034-e7a0-4b01-b1be-d51bfcd2c786"); }
		}
	}
}
