using Microsoft.DirectX;
using System;

namespace AlumnoEjemplos.MarioKillers
{
    public class RigidBody
    {
        public static readonly float CLAMP_DELTA = 1E-5f;

        public Vector3 position = new Vector3(0, 0, 0);
        public Vector3 linearVelocity = new Vector3(0, 0, 0);
        public Vector3 angularVelocity = new Vector3(0, 0, 0);
        public Vector3 acceleration = new Vector3(0, 0, 0);
        public Vector3 orientation = new Vector3(0, 0, 0);
        public float mass;

        public RigidBody(float mass)
        {
            if (mass <= 0.0) throw new ArgumentException("A rigid body's mass must be positive");
            this.mass = mass;
        }

        /// <summary>
        /// Applies an impulse force to the body at its center.
        /// Uses F = m.a, since angular velocity doesn't change.
        /// Integration is done using Euler's method.
        /// </summary>
        /// <param name="force">The force to apply.</param>
        public void ApplyForce(Vector3 force, float elapsedTime)
        {
            Vector3 forceAcceleration = Vector3.Multiply(force, 1.0f / mass);
            this.acceleration += forceAcceleration;
            this.position += elapsedTime * this.linearVelocity;
            this.linearVelocity += elapsedTime * forceAcceleration;
        }

    }
}
