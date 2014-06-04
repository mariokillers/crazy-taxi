using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.MarioKillers
{
    public class RigidBody
    {
        private Vector3 position;
        public Vector3 Position
        {
            get { return this.position; }
            set { this.position = value; this.BoundingSphere.setCenter(value); }
        }
        public Vector3 LinearVelocity = Vector3.Empty;
        public Vector3 AngularVelocity = Vector3.Empty;
        public Vector3 AngularMomentum = Vector3.Empty;
        public Vector3 Rotation
        {
            get { return this.Mesh.Rotation; }
            set { this.Mesh.Rotation = value; }
        }
        public Matrix Transform
        {
            get { return this.Mesh.Transform; }
            set { this.Mesh.Transform = value; }
        }
        public Matrix Orientation = Matrix.Identity;
        public Matrix InvInertiaTensor = Matrix.Invert(Matrix.Identity);

        /// <summary>
        /// The impulses that have been applied to the body in this frame.
        /// </summary>
        public List<Impulse> Impulses = new List<Impulse>();

        /// <summary>
        /// Mass of the body in kilograms
        /// </summary>
        public float Mass;
        public TgcMesh Mesh;

        public TgcBoundingSphere BoundingSphere;

        public bool IsIntersectingWith(RigidBody other)
        {
            Vector3 centerDiff = this.BoundingSphere.Center - other.BoundingSphere.Center;
            float radiusSum = this.BoundingSphere.Radius + other.BoundingSphere.Radius;
            return Vector3.Dot(centerDiff, centerDiff) <= radiusSum * radiusSum;
        }

        public RigidBody(float mass, TgcMesh Mesh)
        {
            this.Mesh = Mesh;
            this.Mesh.AutoTransformEnable = false;
            if (mass <= 0.0) throw new ArgumentException("A rigid body's mass must be positive");
            this.Mass = mass;
            this.InvInertiaTensor = Matrix.Invert(boxInertiaTensor(10, 10, 10, mass));
            this.BoundingSphere = TgcBoundingSphere.computeFromMesh(Mesh);
        }

        /// <summary>
        /// Applies an impulse force to the body at its center.
        /// New position and velocity values will be calculated when the
        /// world is stepped forward, which is why no timestep value
        /// is needed for this method.
        /// </summary>
        /// <param name="force">The force to apply.</param>
        public void ApplyImpulse(Vector3 force)
        {
            this.Impulses.Add(new Impulse(force, Vector3.Empty));
        }

        /// <summary>
        /// Applies an impulse force to the body at a specified position,
        /// relative to the body's coordinates. This will cause the body
        /// to move and rotate.
        ///
        /// New position and velocity values will be calculated when the
        /// world is stepped forward, which is why no timestep value
        /// is needed for this method.
        /// <param name="Force">The force to apply.</param>
        /// <param name="RelativePosition">
        ///     The point of application in body coordinates.
        /// </param>
        /// </summary>
        public void ApplyImpulse(Vector3 Force, Vector3 RelativePosition)
        {
            this.Impulses.Add(new Impulse(Force, RelativePosition));
        }

        public void Render()
        {
            this.Mesh.render();
        }

        public void Dispose()
        {
            this.Mesh.dispose();
        }

        private Matrix boxInertiaTensor(float x, float y, float z, float mass)
        {
            Matrix result = Matrix.Zero;
            result.M11 = (mass / 12) * (y * y + z * z);
            result.M22 = (mass / 12) * (x * x + z * z);
            result.M33 = (mass / 12) * (x * x + y * y);
            result.M44 = 1;
            return result;
        }
    }

}
