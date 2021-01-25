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

            if (reflect)
            {
                List<Module> reflected = new List<Module>();
                foreach (Module m in modules)
                {
                    reflected.Add(m); // Add the standard tile
                    Mesh xGeo = m.Geometry.DuplicateMesh();
                    Mesh yGeo = m.Geometry.DuplicateMesh();

                    // Mirror X
                    List<Edge> xEdges = new List<Edge> { m.Edges[2], m.Edges[1], m.Edges[0], m.Edges[3] };
                    Plane xPlane = Plane.WorldYZ;
                    xPlane.Origin = xGeo.GetBoundingBox(true).Center;
                    Transform xForm = Transform.Mirror(xPlane);
                    xGeo.Transform(xForm);

                    reflected.Add(new Module(xGeo, xEdges));

                    // Mirror Y
                    List<Edge> yEdges = new List<Edge> { m.Edges[0], m.Edges[3], m.Edges[2], m.Edges[1] };
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