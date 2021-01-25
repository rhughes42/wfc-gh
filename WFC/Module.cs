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

        /// <summary>
        /// Default (empty) constructor.
        /// </summary>
        public Module()
        {
            this.Geometry = null;
            this.Edges = new List<Edge>() { new Edge() };
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
    }
}
