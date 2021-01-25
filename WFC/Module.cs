using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rhino.Geometry;

namespace WFC
{
    /// <summary>
    /// Our tileset module class.
    /// </summary>
    public class Module
    {
        public Mesh Geometry { get; set; }
        public List<Edge> Edges { get; set; }
        public Point3d Origin { get; set; }

        /// <summary>
        /// Default (empty) constructor.
        /// </summary>
        public Module()
        {
            this.Geometry = null;
            this.Edges = new List<Edge>() { new Edge() };
            this.Origin = Point3d.Origin;
        }

        /// <summary>
        /// Constructor with single mesh and edge list.
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="edges"></param>
        public Module(Mesh geometry, List<Edge> edges)
        {
            this.Geometry = geometry;
            this.Edges = edges;

            this.Origin = geometry.GetBoundingBox(true).Min;
        }

        /// <summary>
        /// Constructor with predefined edge list.
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="edges"></param>
        public Module(List<Mesh> geometry, List<Edge> edges)
        {
            Mesh m = new Mesh();
            m.Append(geometry);

            this.Geometry = m;
            this.Edges = edges;

            this.Origin = m.GetBoundingBox(true).Min;
        }

        /// <summary>
        /// Standard constructor.
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="edgeName"></param>
        /// <param name="edgeType"></param>
        public Module(List<Mesh> geometry, List<string> edgeName, List<int> edgeType)
        {
            Mesh m = new Mesh();
            m.Append(geometry);

            this.Geometry = m;

            List<Edge> edgeList = new List<Edge>();
            for (int i = 0; i < edgeName.Count; i++)
            {
                edgeList.Add(new Edge(edgeName[i], edgeType[i]));
            }
            this.Edges = edgeList;

            this.Origin = m.GetBoundingBox(true).Min;
        }

        public override string ToString()
        {
            return String.Format("Module: {0} Edges", this.Edges.Count.ToString());
        }
    }

    public class Edge
    {
        public string Name { get; set; }
        public int Type { get; set; }

        /// <summary>
        /// Default (empty) constructor.
        /// </summary>
        public Edge()
        {
            this.Name = "A";
            this.Type = 1;
        }

        /// <summary>
        /// Standard constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public Edge(string name, int type)
        {
            this.Name = name;
            this.Type = type;
        }

        public override string ToString()
        {
            return String.Format("Edge: {0} - {1}", this.Name.ToString(), this.Type.ToString());
        }
    }
}
