using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace WFC.Components
{
    public class Compute : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public Compute()
          : base("Compute", "Compute",
              "Compute the collapse of the wave function for a grid.",
              "WFC", "Compute")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Grid", "Grid", "A grid to perform WFC on.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Steps", "Steps", "Maximum number of steps to run.", GH_ParamAccess.item, 1000);
            pManager.AddIntegerParameter("Seed", "Seed", "Random seed to start wave collapse.", GH_ParamAccess.item, 400);
            pManager.AddBooleanParameter("Run", "Run", "Run the algorithm", GH_ParamAccess.item, false);
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
            Grid grid = new Grid();
            int steps = 1000;
            int seed = 400;
            bool run = false;

            if (!DA.GetData(0, ref grid)) { return; }
            if (!DA.GetData(1, ref steps)) { return; }
            if (!DA.GetData(2, ref seed)) { return; }
            if (!DA.GetData(3, ref run)) { return; }

            List<Mesh> geo = new List<Mesh>();
            List<string> text = new List<string>();
            List<string> debug = new List<string>();



            DA.SetDataList(0, debug);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("bd00347b-98f3-4c29-a8e5-f4a09688d23a"); }
        }
    }
}