using Microsoft.DirectX;
using System;

namespace AlumnoEjemplos.MarioKillers
{
    public class RigidBody
    {
        public Vector3 position = new Vector3(0, 0, 0);
        public Vector3 linearVelocity = new Vector3(0, 0, 0);
        public Vector3 angularVelocity = new Vector3(0, 0, 0);
        public Vector3 orientation = new Vector3(0, 0, 0);
        public Vector3 force = new Vector3(0, 0, 0);
        public float mass;

        public RigidBody(float mass)
        {
            if (mass <= 0.0) throw new ArgumentException("A rigid body's mass must be positive");
            this.mass = mass;
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
            this.force += force;
        }

    }
}
