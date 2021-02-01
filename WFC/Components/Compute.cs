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
            pManager.AddBooleanParameter("Run", "Run", "Run the algorithm.", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Log", "Log", "Algorithm log.", GH_ParamAccess.list);
        }

        Grid grid = null;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int steps = 1000;
            int seed = 400;
            bool run = false;

            if (!DA.GetData(0, ref grid)) { return; }
            if (!DA.GetData(1, ref steps)) { return; }
            if (!DA.GetData(2, ref seed)) { return; }
            if (!DA.GetData(3, ref run)) { return; }

            List<Mesh> geo = new List<Mesh>();
            List<string> text = new List<string>();
            List<string> log = new List<string>();

            if (run)
            {
                // Choose a random cell from the available cells.
                var rand = new Random();
                List<Cell> row = grid.Matrix[(int)Util.Remap(rand.NextDouble(), 0, 1, 0, grid.Matrix[0].Count - 1)];
                Cell start = row[(int)Util.Remap(rand.NextDouble(), 0, 1, 0, row.Count - 1)];

                start.Collapse(out Mesh m);
                geo.Add(m);

                // If there are still uncertain cells in the grid.
                if (grid.Uncertain > 0)
                {
                    log.Add(String.Format("Uncertain cells remaining: {0}", grid.Uncertain.ToString()));

                    grid.Steps += 1;
                    if (grid.Steps > grid.MaxSteps)
                    {
                        log.Add(String.Format("Maximum step count ({0}) exceeded. Stopping...", grid.MaxSteps.ToString()));
                    }

                    log.Add(String.Format("Collapsing cell {0},{1}...", start.X.ToString(), start.Y.ToString()));
                    
                    grid.Propogate(start.X, start.Y);

                    this.Message = String.Format("{0}%", Util.Remap(grid.Uncertain, 0, grid.ExtentsX * grid.ExtentsY, 100, 0).ToString());

                    if (grid.Contradiction)
                        grid.Initialize();

                    DA.SetDataList(0, log);
                }
                this.Message = "WFC Complete";
            }                       

            DA.SetDataList(0, log);
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