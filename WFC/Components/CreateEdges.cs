using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace WFC.Components
{
    public class CreateEdges : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public CreateEdges()
          : base("Create Edges", "Edges",
              "Create a list of edges to use for connectivity.",
              "WFC", "Modules")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "Name", "Name of the edge to encode.", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Type", "Type", "Type of the edge to encode.", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Edges", "Edges", "Output edge list.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<string> edgeName = new List<string>();
            List<int> edgeType = new List<int>();

            if (!DA.GetDataList(0, edgeName)) { return; }
            if (!DA.GetDataList(1, edgeType)) { return; }

            List<Edge> edgeList = new List<Edge>();

            for(int i = 0; i < edgeName.Count; i++)
            {
                edgeList.Add(new Edge(edgeName[i], edgeType[i]));
            }

            this.Message = String.Format("{0} Edges", edgeList.Count.ToString());

            DA.SetDataList(0, edgeList);
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
            get { return new Guid("a15f14fe-7d06-442d-9b6b-47bdbbf50779"); }
        }
    }
}