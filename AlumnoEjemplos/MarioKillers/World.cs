using Microsoft.DirectX;

using System.Collections.Generic;

namespace AlumnoEjemplos.MarioKillers
{
    public class World
    {
        public List<RigidBody> Bodies = new List<RigidBody>();
        /// <summary>
        /// The acceleration of gravity, in m/s^2.
        /// </summary>
        public Vector3 GravityAcceleration = new Vector3(0, -9.81f, 0);

        public bool GravityEnabled = false;

        /// <summary>
        /// If enabled, draws bounding spheres around every rigid body.
        /// </summary>
        public bool DebugEnabled = false;

        /// <summary>
        /// Integrates the whole world forward in time.
        /// Uses Euler's method for integration.
        /// </summary>
        /// <param name="timeStep">Timestep in seconds</param>
        public void Step(float timeStep)
        {
            foreach (RigidBody body in this.Bodies)
            {
                if (this.GravityEnabled)
                {
                    body.LinearVelocity += this.GravityAcceleration * timeStep;
                }
                foreach (Impulse impulse in body.Impulses)
                {
                    body.AngularMomentum += timeStep * impulse.Torque();
                    Matrix Aux = body.Orientation * body.InvInertiaTensor * Matrix.TransposeMatrix(body.Orientation);
                    body.AngularVelocity = Vector3.TransformCoordinate(body.AngularMomentum, Aux);
                    body.Orientation *= Matrix.RotationAxis(body.AngularVelocity, body.AngularVelocity.Length());

                    if (impulse.RelativePosition == Vector3.Empty)
                    {
                        body.LinearVelocity += impulse.Force * (1.0f / body.Mass);
                    }
                    else if (impulse.Force != Vector3.Empty)
                    {
                        body.LinearVelocity += Vector3.Cross(body.AngularVelocity, impulse.RelativePosition);
                    }
                }
                body.Transform = body.Orientation * Matrix.Translation(body.Position);
                body.Position += body.LinearVelocity * timeStep;
                // Impulses have to be removed, otherwise they will be integrated next frame
                body.Impulses.Clear();

                this.HandleCollisions();
            }
        }

        /// <summary>
        /// Process collisions between all the bodies in the World through brute force.
        /// Collisions are tested pairwise between every body, which is inefficient.
        /// </summary>
        public void HandleCollisions()
        {
            foreach (RigidBody body in this.Bodies)
            {
                foreach (RigidBody otherBody in this.Bodies)
                {
                    if (body != otherBody && body.IsIntersectingWith(otherBody))
                    {
                        body.HandleCollisionWith(otherBody);
                    }
                }
            }
        }

        public void AddBody(RigidBody body)
        {
            this.Bodies.Add(body);
        }

        public void Render()
        {
            foreach (RigidBody body in this.Bodies)
            {
                if (this.DebugEnabled)
                {
                    body.BoundingSphere.render();
                }
                body.Render();
            }
        }

        public void Dispose()
        {
            foreach (RigidBody body in this.Bodies)
            {
                body.Dispose();
            }
        }
    }
}
