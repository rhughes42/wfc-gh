using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace WFC.Components
{
    public class Augment : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public Augment()
          : base("Augment Modules", "Augment",
              "Augment the set of modules through rotation and reflection.",
              "WFC", "Augment")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Modules", "Modules", "Original list of modules.", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Rotate", "Rotate", "Allow rotation of the modules", GH_ParamAccess.item, true);
            pManager.AddBooleanParameter("Reflect", "Reflect", "Allow reflection of the modules", GH_ParamAccess.item, true);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Modules", "Modules", "Augmented list of modules.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Module> modules = new List<Module>();
            bool rotate = true;
            bool reflect = true;

            if (!DA.GetDataList(0, modules)) { return; }
            if (!DA.GetData(1, ref rotate)) { return; }
            if (!DA.GetData(2, ref reflect)) { return; }

            if (rotate)
            {
                foreach(Module m in Module)
                {
                    List<Edge> rotEdges = new List<Edge>();
                    rotEdges = (m.edges[0][0], m.edges[0][1] * 2 % 3),
                    (m.edges[3][0], m.edges[3][1] * 2 % 3),
                    (m.edges[2][0], m.edges[2][1] * 2 % 3),
                    (m.edges[1][0], m.edges[1][1] * 2 % 3)]
                }
            }
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
            get { return new Guid("18dd89ea-eede-4405-833e-49a94aae0ecb"); }
        }
    }
}