using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.MarioKillers
{
    public class World
    {
        private List<RigidBody> bodies = new List<RigidBody>();
        private Vector3 gravityAcceleration = new Vector3(0, -9.81f, 0);

        /// <summary>
        /// Integrates the whole world forward in time.
        /// Uses Euler's method for integration.
        /// </summary>
        /// <param name="timeStep">Timestep in seconds</param>
        public void Step(float timeStep)
        {
            foreach (RigidBody body in this.bodies) {
                body.linearVelocity += body.force * (1.0f / body.mass) * timeStep;
                body.position += body.linearVelocity * timeStep;
                // Force has to be set to zero, otherwise it will be integrated next step
                body.force = new Vector3(0, 0, 0);
            }
        }

        public void AddBody(RigidBody body)
        {
            this.bodies.Add(body);
        }
    }
}
