using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace WFC.Components
{
    public class CreateGrid : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public CreateGrid()
          : base("Create Grid", "Grid",
              "Create a grid on which to perform WFC.",
              "WFC", "Grid")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Modules", "Modules", "A list of module geometry to be represented.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Width", "Width", "A number representing the width of the grid in units.", GH_ParamAccess.item, 10);
            pManager.AddIntegerParameter("Length", "Length", "A number representing the length of the grid in units.", GH_ParamAccess.item, 10);
            pManager.AddIntegerParameter("Size", "Size", "The size of the grid spacing in metres.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Grid", "Grid", "Grid object formatted for WFC.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Module> modules = new List<Module>();
            int width = 10;
            int length = 10;
            int size = 6;

            if (!DA.GetDataList(0, modules)) { return; }
            if (!DA.GetData(1, ref width)) { return; }
            if (!DA.GetData(2, ref length)) { return; }
            if (!DA.GetData(3, ref size)) { return; }

            Grid grid = new Grid(width, length, size, modules);
            this.Message = "OK";

            DA.SetData(0, grid);
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
            get { return new Guid("3bc6574c-22cc-43de-b741-e1c163873a0b"); }
        }
    }
}