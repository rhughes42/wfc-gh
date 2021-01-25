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

            // Reflect the tile through the YZ and ZX planes.
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
                    Plane yPlane = Plane.WorldZX;
                    yPlane.Origin = yGeo.GetBoundingBox(true).Center;
                    Transform yForm = Transform.Mirror(yPlane);
                    yGeo.Transform(yForm);

                    reflected.Add(new Module(yGeo, yEdges));
                }
                // Important - make sure to rotate the newly augmented tiles too!
                modules = reflected;
            }

            if (rotate)
            {
                List<Module> rotated = new List<Module>();
                foreach (Module m in modules)
                {
                    Vector3d vec = Vector3d.ZAxis;
                    Point3d center = m.Geometry.GetBoundingBox(true).Center;

                    // No rotations
                    List<Edge> oneEdges = new List<Edge> { m.Edges[0], m.Edges[1], m.Edges[2], m.Edges[3] };
                    Mesh oneGeo = m.Geometry.DuplicateMesh();
                    Transform oneRot = Transform.Rotation(Util.ToDegrees(0), vec, center);
                    oneGeo.Transform(oneRot);
                    rotated.Add(new Module(oneGeo, oneEdges));

                    // One rotation
                    List<Edge> twoEdges = new List<Edge> { m.Edges[3], m.Edges[0], m.Edges[1], m.Edges[2] };
                    Mesh twoGeo = m.Geometry.DuplicateMesh();
                    Transform twoRot = Transform.Rotation(Util.ToDegrees(90), vec, center);
                    twoGeo.Transform(twoRot);
                    rotated.Add(new Module(twoGeo, twoEdges));

                    // Two rotations
                    List<Edge> threeEdges = new List<Edge> { m.Edges[2], m.Edges[3], m.Edges[0], m.Edges[1] };
                    Mesh threeGeo = m.Geometry.DuplicateMesh();
                    Transform threeRot = Transform.Rotation(Util.ToDegrees(180), vec, center);
                    threeGeo.Transform(threeRot);
                    rotated.Add(new Module(threeGeo, threeEdges));

                    // Three rotations
                    List<Edge> fourEdges = new List<Edge> { m.Edges[1], m.Edges[2], m.Edges[3], m.Edges[0] };
                    Mesh fourGeo = m.Geometry.DuplicateMesh();
                    Transform fourRot = Transform.Rotation(Util.ToDegrees(270), vec, center);
                    fourGeo.Transform(fourRot);
                    rotated.Add(new Module(fourGeo, fourEdges));
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