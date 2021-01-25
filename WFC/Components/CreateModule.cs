using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace WFC.Components
{
    public class CreateModule : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public CreateModule()
          : base("Create Module", "Module",
              "Create a tile module.",
              "WFC", "Modules")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "Mesh", "A list of the mesh geometry to be represented.", GH_ParamAccess.list);
            pManager.AddGenericParameter("Edges", "Edges", "A list of the edges to connect the module.", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Modules", "Modules", "Output module list.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Mesh> geometry = new List<Mesh>();
            List<Edge> edges = new List<Edge>();

            if (!DA.GetDataList(0, geometry)) { return; }
            if (!DA.GetDataList(1, edges)) { return; }

            Module module = new Module(geometry, edges);

            this.Message = module.ToString();

            DA.SetData(0, module);
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
            get { return new Guid("ee2f679e-401a-4582-81d6-d835944d43d4"); }
        }
    }
}