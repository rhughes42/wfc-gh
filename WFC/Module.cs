using System;
using System.Collections.Generic;

using Rhino.Geometry;

namespace WFC
{
    /// <summary>
    /// Represents a tileset module (tile) consisting of mesh geometry and four edge connectors.
    /// </summary>
    public class Module
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Module"/> class with no geometry and a default edge set.
        /// </summary>
        public Module()
        {
            Geometry = null;
            Edges = new List<Edge> { new Edge() };
            Origin = Point3d.Origin;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Module"/> class from a mesh and edge list.
        /// </summary>
        /// <param name="geometry">The module mesh.</param>
        /// <param name="edges">
        /// The module edges in <c>N, E, S, W</c> order. The solver expects four edges.
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="geometry"/> or <paramref name="edges"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="edges"/> does not contain exactly four items.</exception>
        public Module(Mesh geometry, List<Edge> edges)
        {
            if (geometry == null) throw new ArgumentNullException(nameof(geometry));
            if (edges == null) throw new ArgumentNullException(nameof(edges));
            if (edges.Count != 4) throw new ArgumentException("Module requires exactly four edges (N, E, S, W).", nameof(edges));

            Geometry = geometry;
            Edges = edges;
            Origin = geometry.GetBoundingBox(true).Min;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Module"/> class from a list of meshes and edge list.
        /// </summary>
        /// <param name="geometry">The module meshes, which will be appended into a single mesh.</param>
        /// <param name="edges">
        /// The module edges in <c>N, E, S, W</c> order. The solver expects four edges.
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="geometry"/> or <paramref name="edges"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="edges"/> does not contain exactly four items.</exception>
        public Module(List<Mesh> geometry, List<Edge> edges)
        {
            if (geometry == null) throw new ArgumentNullException(nameof(geometry));
            if (edges == null) throw new ArgumentNullException(nameof(edges));
            if (edges.Count != 4) throw new ArgumentException("Module requires exactly four edges (N, E, S, W).", nameof(edges));

            var mesh = new Mesh();
            mesh.Append(geometry);

            Geometry = mesh;
            Edges = edges;
            Origin = mesh.GetBoundingBox(true).Min;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Module"/> class from meshes and raw edge name/type data.
        /// </summary>
        /// <param name="geometry">The module meshes, which will be appended into a single mesh.</param>
        /// <param name="edgeName">A list of edge connector names.</param>
        /// <param name="edgeType">
        /// A list of edge connector types. Convention: <c>1</c> and <c>2</c> are opposites, <c>0</c> matches <c>0</c>.
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
        /// <exception cref="ArgumentException">Thrown when edge lists are different lengths or not exactly four items.</exception>
        public Module(List<Mesh> geometry, List<string> edgeName, List<int> edgeType)
        {
            if (geometry == null) throw new ArgumentNullException(nameof(geometry));
            if (edgeName == null) throw new ArgumentNullException(nameof(edgeName));
            if (edgeType == null) throw new ArgumentNullException(nameof(edgeType));
            if (edgeName.Count != edgeType.Count) throw new ArgumentException("Edge name/type lists must be the same length.");
            if (edgeName.Count != 4) throw new ArgumentException("Module requires exactly four edges (N, E, S, W).");

            var mesh = new Mesh();
            mesh.Append(geometry);
            Geometry = mesh;

            var edgeList = new List<Edge>();
            for (var i = 0; i < edgeName.Count; i++)
            {
                edgeList.Add(new Edge(edgeName[i], edgeType[i]));
            }

            Edges = edgeList;
            Origin = mesh.GetBoundingBox(true).Min;
        }

        /// <summary>
        /// Gets or sets the module mesh geometry.
        /// </summary>
        public Mesh Geometry { get; set; }

        /// <summary>
        /// Gets or sets the module edge connectors in <c>N, E, S, W</c> order.
        /// </summary>
        public List<Edge> Edges { get; set; }

        /// <summary>
        /// Gets or sets the origin used when positioning modules in the output grid.
        /// </summary>
        public Point3d Origin { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format("Module: {0} Edges", Edges.Count);
        }
    }

    /// <summary>
    /// Represents a single edge connector used to define tile adjacency compatibility.
    /// </summary>
    public class Edge : IEquatable<Edge>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Edge"/> class with a default connector.
        /// </summary>
        public Edge()
        {
            Name = "A";
            Type = 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Edge"/> class.
        /// </summary>
        /// <param name="name">The connector name.</param>
        /// <param name="type">
        /// The connector type. Convention: <c>1</c> and <c>2</c> are opposites, <c>0</c> matches <c>0</c>.
        /// </param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null, empty, or whitespace.</exception>
        public Edge(string name, int type)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Edge name must be non-empty.", nameof(name));

            Name = name;
            Type = type;
        }

        /// <summary>
        /// Gets or sets the connector name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the connector type.
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Creates a connector that matches this connector on the opposing side of an adjacency.
        /// </summary>
        /// <returns>The opposing connector.</returns>
        public Edge Opposite()
        {
            // Matches the original convention: 1 <-> 2, 0 -> 0.
            if (Type == 1) return new Edge(Name, 2);
            if (Type == 2) return new Edge(Name, 1);
            return new Edge(Name, Type);
        }

        /// <summary>
        /// Determines whether this connector can connect to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The connector on the opposing tile edge.</param>
        /// <returns><see langword="true"/> when compatible; otherwise <see langword="false"/>.</returns>
        public bool Matches(Edge other)
        {
            if (other == null) return false;
            return Name == other.Name && Opposite().Type == other.Type;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format("Edge: {0} - {1}", Name, Type);
        }

        /// <inheritdoc />
        public bool Equals(Edge other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name) && Type == other.Type;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as Edge);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = (hash * 31) + (Name != null ? Name.GetHashCode() : 0);
                hash = (hash * 31) + Type.GetHashCode();
                return hash;
            }
        }
    }
}
