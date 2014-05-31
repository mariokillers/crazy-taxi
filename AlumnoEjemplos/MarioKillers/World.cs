using Microsoft.DirectX;

using System.Collections.Generic;

namespace AlumnoEjemplos.MarioKillers
{
    public class World
    {
        private List<RigidBody> bodies = new List<RigidBody>();
        /// <summary>
        /// The acceleration of gravity, in m/s^2.
        /// </summary>
        public Vector3 GravityAcceleration = new Vector3(0, -9.81f, 0);
        public bool GravityEnabled = false;

        /// <summary>
        /// Integrates the whole world forward in time.
        /// Uses Euler's method for integration.
        /// </summary>
        /// <param name="timeStep">Timestep in seconds</param>
        public void Step(float timeStep)
        {
            foreach (RigidBody body in this.bodies)
            {
                if (this.GravityEnabled)
                {
                    body.ApplyImpulse(body.Mass * this.GravityAcceleration);
                }
                foreach (Impulse impulse in body.Impulses)
                {
                    body.LinearVelocity += impulse.Force * (1.0f / body.Mass) * timeStep;
                    body.Position += body.LinearVelocity * timeStep;
                    body.AngularMomentum += timeStep * impulse.Torque();

                    Matrix Aux = body.Orientation * body.InvInertiaTensor * Matrix.TransposeMatrix(body.Orientation);
                    body.AngularVelocity = Vector3.TransformCoordinate(body.AngularMomentum, Aux);
                    body.Orientation *= Matrix.RotationAxis(body.AngularVelocity, body.AngularVelocity.Length());
                    body.Transform = body.Orientation * Matrix.Translation(body.Position);
                }
                // Impulses have to be removed, otherwise they will be integrated next frame
                body.Impulses.Clear();
            }
        }

        public void AddBody(RigidBody body)
        {
            this.bodies.Add(body);
        }

        public void Render()
        {
            foreach (RigidBody body in this.bodies)
            {
                body.Render();
            }
        }

        public void Dispose()
        {
            foreach (RigidBody body in this.bodies)
            {
                body.Dispose();
            }
        }
    }
}
