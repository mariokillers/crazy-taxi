using Microsoft.DirectX;
using System;

namespace AlumnoEjemplos.MarioKillers
{
    public class RigidBody
    {
        public Vector3 Position = new Vector3(0, 0, 0);
        public Vector3 LinearVelocity = new Vector3(0, 0, 0);
        public Vector3 AngularVelocity = new Vector3(0, 0, 0);
        public Vector3 Orientation = new Vector3(0, 0, 0);
        public Vector3 Force = new Vector3(0, 0, 0);
        public float Mass;

        public RigidBody(float mass)
        {
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

    }
}
