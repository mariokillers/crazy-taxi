using Microsoft.DirectX;
using System;
using System.Drawing;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.MarioKillers
{
    public class RigidBody
    {
        public Vector3 Position {
            get { return this.Shape.Position; }
            set { this.Shape.Position = value; }
        }
        public Vector3 LinearVelocity = new Vector3(0, 0, 0);
        public Vector3 AngularVelocity = new Vector3(0, 0, 0);
        public Vector3 Rotation {
            get { return this.Shape.Rotation; }
            set { this.Shape.Rotation = value; }
        }
        public Vector3 Force = new Vector3(0, 0, 0);

        /// <summary>
        /// Mass of the body in kilograms
        /// </summary>
        public float Mass;
        public Shape Shape;
        public TgcBoundingSphere BoundingSphere { get { return this.Shape.BoundingSphere; } }

        public RigidBody(float mass, Shape shape)
        {
            this.Shape = shape;
            this.Position = Vector3.Empty;
            if (mass <= 0.0) throw new ArgumentException("A rigid body's mass must be positive");
            this.Mass = mass;
        }

        /// <summary>
        /// Applies an impulse force to the body at its center.
        /// New position and velocity values will be calculated when the
        /// world is stepped forward, which is why no timestep value
        /// is needed for this method.
        /// </summary>
        /// <param name="force">The force to apply.</param>
        public void ApplyForce(Vector3 force)
        {
            this.Force += force;
        }

        public void Render()
        {
            this.Shape.render();
        }

        public void Dispose()
        {
            this.Shape.dispose();
        }
    }
}
