using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.MarioKillers
{
    public class RigidBody
    {
        const float EPSILON = 0.00000001f;
        private Vector3 position;
        public Matrix scale = Matrix.Scaling(new Vector3(1,1,1));
        public World world;
        public Impulse FRoz = new Impulse(Vector3.Empty, Vector3.Empty);
        public bool affectedByGravity = true;
        public bool floorCollisionsEnabled = true;
        public bool strong = false;
        public Vector3 antVelocity = Vector3.Empty;
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

        public float Elasticity = 0.8f;

        public bool IsIntersectingWith(RigidBody other)
        {
            Vector3 centerDiff = this.BoundingSphere.Center - other.BoundingSphere.Center;
            float radiusSum = this.BoundingSphere.Radius + other.BoundingSphere.Radius;
            return Vector3.Dot(centerDiff, centerDiff) <= radiusSum * radiusSum;
        }

        public RigidBody(float mass, TgcMesh Mesh, World World)
        {
            this.world = World;
            this.Mesh = Mesh;
            this.Mesh.AutoTransformEnable = false;
            if (mass <= 0.0) throw new ArgumentException("A rigid body's mass must be positive");
            this.Mass = mass;
            this.BoundingSphere = boundingSphereFromPoints(Mesh.getVertexPositions());
        }
        public void updateBoundingSphere(){
            this.BoundingSphere = boundingSphereFromPoints(Mesh.getVertexPositions());
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

        public Matrix WorldMomentsInverse
        {
            get
            {
                return this.Orientation * this.InvInertiaTensor * Matrix.TransposeMatrix(this.Orientation);
            }
        }

        public void HandleCollisionWith(RigidBody other)
        {
            
            Vector3 cNormal = this.collisionNormal(other);
            Vector3 collisionPoint = 0.5f * (this.Position + this.BoundingSphere.Radius * cNormal)
                + 0.5f * (other.Position - other.BoundingSphere.Radius * cNormal);
            Vector3 r1 = collisionPoint - this.Position;
            Vector3 r2 = collisionPoint - other.Position;
            Vector3 vel1 = this.LinearVelocity + Vector3.Cross(this.AngularVelocity, r1);
            Vector3 vel2 = other.LinearVelocity + Vector3.Cross(other.AngularVelocity, r2);
            Vector3 relativeVelocity = vel1 - vel2;

            float vDotN = Vector3.Dot(relativeVelocity, cNormal);
            // If the bodies are moving away from each other, nothing needs to be done
            if (vDotN < 0) return;

            // Impulse factor
            float denominator = (1.0f / this.Mass + 1.0f / other.Mass) * Vector3.Dot(cNormal, cNormal);
            // Angular factors
            Vector3 cross1 = Vector3.Cross(r1, cNormal);
            Vector3 cross2 = Vector3.Cross(r2, cNormal);
            cross1 = Vector3.TransformCoordinate(cross1, this.WorldMomentsInverse);
            cross2 = Vector3.TransformCoordinate(cross2, other.WorldMomentsInverse);
            Vector3 sum = Vector3.Cross(cross1, r1) + Vector3.Cross(cross2, r2);
            denominator += Vector3.Dot(sum, cNormal);
            float modifiedVel = vDotN / denominator;
            float j1 = -(1.0f + this.Elasticity) * modifiedVel;
            float j2 = -(1.0f + other.Elasticity) * modifiedVel;
            // Update velocities
            this.LinearVelocity += j1 / this.Mass * cNormal;
            other.LinearVelocity -= j2 / other.Mass * cNormal;
            // Update angular velocities
            this.AngularMomentum += Vector3.Cross(r1, j1 * cNormal) ;
            this.AngularVelocity = Vector3.TransformCoordinate(this.AngularMomentum, this.WorldMomentsInverse);
            other.AngularMomentum += Vector3.Cross(r2, j2 * cNormal);
            other.AngularVelocity = Vector3.TransformCoordinate(other.AngularMomentum, other.WorldMomentsInverse);

        }

        private Vector3 collisionNormal(RigidBody other)
        {
            return Vector3.Normalize(other.BoundingSphere.Center - this.BoundingSphere.Center);
        }

        /// <summary>
        /// Push penetrating objects away from each other to ensure there is only one
        /// point of collision between them.
        /// </summary>
        /// <param name="other">The other penetrating body.</param>
        private void removePenetrationWith(RigidBody other)
        {
            float distance = penetrationDistance(other);
            Vector3 centerDiff = this.Position - other.Position;
            this.Position -= 0.5f * distance * centerDiff;
            other.Position += 0.5f * distance * centerDiff;
        }

        private float penetrationDistance(RigidBody other)
        {
            float radiusSum = this.BoundingSphere.Radius + other.BoundingSphere.Radius;
            Vector3 normalDirection = other.BoundingSphere.Center - this.BoundingSphere.Center;
            return radiusSum - Vector3.Length(normalDirection);
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

        /// <summary>
        /// Utility method to calculate bounding spheres with greater precision
        /// than TgcBoundingSphere.computeFromMesh()
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        private TgcBoundingSphere boundingSphereFromPoints(Vector3[] vertices)
        {
            Vector3 center = this.Mesh.BoundingBox.calculateBoxCenter();
            float radius = vertices.Max(v => (v - this.Position).Length());
            return new TgcBoundingSphere(center, radius);
        }


        /// <summary>
        /// Detecci�n de colisiones recursiva
        /// </summary>
        public void doCollideWithWorld(Vector3 movementVector, int recursionDepth)
        {
            //Console.WriteLine(this.BoundingSphere.Radius);
            //Vector3 movementVector = this.LinearVelocity;
            TgcBoundingSphere characterSphere = this.BoundingSphere;
            List<TgcMesh> obstaculos = this.world.staticBodies;
            //Limitar recursividad
            if (recursionDepth > 5)
            {
                return;
            }

            //Ver si la distancia a recorrer es para tener en cuenta
            float distanceToTravelSq = movementVector.LengthSq();
            //if (distanceToTravelSq < EPSILON)
            //{
              //  return;
            //}

            //Posicion deseada
            Vector3 originalSphereCenter = characterSphere.Center;
            Vector3 nextSphereCenter = originalSphereCenter + movementVector;

            //Buscar el punto de colision mas cercano de todos los objetos candidatos
            float minCollisionDistSq = float.MaxValue;
            Vector3 realMovementVector = movementVector;
            TgcBoundingBox.Face collisionFace = null;
            TgcBoundingBox collisionObstacle = null;
            Vector3 nearestPolygonIntersectionPoint = Vector3.Empty;
            foreach (TgcMesh obstaculo in obstaculos)
            {
                TgcBoundingBox obstaculoBB = obstaculo.BoundingBox;
                obstaculoBB.render();
                //Obtener los pol�gonos que conforman las 6 caras del BoundingBox
                TgcBoundingBox.Face[] bbFaces = obstaculoBB.computeFaces();
                

                foreach (TgcBoundingBox.Face bbFace in bbFaces)
                {
                    Vector3 pNormal = TgcCollisionUtils.getPlaneNormal(bbFace.Plane);

                    TgcRay movementRay = new TgcRay(originalSphereCenter, movementVector);
                    float brutePlaneDist;
                    Vector3 brutePlaneIntersectionPoint;
                    if (!TgcCollisionUtils.intersectRayPlane(movementRay, bbFace.Plane, out brutePlaneDist, out brutePlaneIntersectionPoint))
                    {
                        continue;
                    }

                    float movementRadiusLengthSq = Vector3.Multiply(movementVector, characterSphere.Radius).LengthSq();
                    /*if (brutePlaneDist * brutePlaneDist > movementRadiusLengthSq)
                    {
                       continue;
                   }*/


                    //Obtener punto de colisi�n en el plano, seg�n la normal del plano
                    float pDist;
                    Vector3 planeIntersectionPoint;
                    Vector3 sphereIntersectionPoint = Vector3.Empty;
                    TgcRay planeNormalRay = new TgcRay(originalSphereCenter, -pNormal);
                    bool embebbed = false;
                    bool collisionFound = false;
                    if (TgcCollisionUtils.intersectRayPlane(planeNormalRay, bbFace.Plane, out pDist, out planeIntersectionPoint))
                    {
                        //Console.WriteLine("PDIST:"+ pDist);
                        //Console.WriteLine("RADIUS" + characterSphere.Radius);
                        //Ver si el plano est� embebido en la esfera
                        if (pDist <= characterSphere.Radius)
                        {
                            embebbed = true;
                            //collisionFound = true;

                            //TODO: REVISAR ESTO, caso embebido a analizar con m�s detalle
                            sphereIntersectionPoint = originalSphereCenter - pNormal * characterSphere.Radius;
                            if (pointInBounbingBoxFace(planeIntersectionPoint, bbFace))
                            {
                                if (embebbed)
                                {
                                    //TODO: REVISAR ESTO, nunca deber�a pasar
                                    //throw new Exception("El pol�gono est� dentro de la esfera");
                                }

                                collisionFound = true;
                            }
                        }
                       

                        if (collisionFound)
                        {
                            this.HandleStaticCollisionWith(sphereIntersectionPoint,planeIntersectionPoint, bbFace, obstaculoBB);
                            //Nuevo vector de movimiento acotado
                            /*newMovementVector = polygonIntersectionPoint - sphereIntersectionPoint;
                            newMoveDistSq = newMovementVector.LengthSq();

                            if (newMoveDistSq <= distanceToTravelSq && newMoveDistSq < minCollisionDistSq)
                            {
                                minCollisionDistSq = newMoveDistSq;
                                realMovementVector = newMovementVector;
                                nearestPolygonIntersectionPoint = polygonIntersectionPoint;
                                collisionFace = bbFace;
                                collisionObstacle = obstaculoBB;

                            }*/
                        }
                    }
                }

            }


        }

        public void HandleStaticCollisionWith(Vector3 collisionPoint, Vector3 planeIntersectionPoint, TgcBoundingBox.Face staticBodieFace, TgcBoundingBox staticBody)
        {
            //this.LinearVelocity = Vector3.Empty;
            //this.BoundingSphere.render();
            Vector3 collisionNormal = collisionPoint - planeIntersectionPoint;
            Vector3.Normalize(collisionNormal);
            Vector3 planeNormal = TgcCollisionUtils.getPlaneNormal(staticBodieFace.Plane);
            float planeNormalVelocity = Vector3.Dot(LinearVelocity, planeNormal);
            float modifiedVel = planeNormalVelocity / (1.0f / this.Mass);
            planeNormalVelocity = -(1.0f + this.Elasticity) * planeNormalVelocity;
            this.LinearVelocity += Vector3.Multiply(planeNormal, planeNormalVelocity);
            /*
            Vector3 Acceleration = LinearVelocity - antVelocity;
            Acceleration = Acceleration - Vector3.Multiply(planeNormal, Vector3.Dot(Acceleration, planeNormal));
            Vector3 RozForce = Mass * -Acceleration * 0.02f;
           
            this.FRoz =new Impulse( RozForce, collisionPoint);
            */
            /*
            float j1 = -(1.0f + this.Elasticity) * modifiedVel;
            if (Vector3.Cross(collisionPoint, j1 * Vector3.Normalize(planeNormal)) != Vector3.Empty)
            {
                Console.WriteLine(collisionPoint);
                Console.WriteLine(j1);

            }
            AngularMomentum += Vector3.Cross(collisionPoint, j1 * Vector3.Normalize(planeNormal));
            //WorldMomentsInverse
            Matrix WorldMomentsInverse = this.Orientation * this.InvInertiaTensor * Matrix.TransposeMatrix(this.Orientation);
            AngularVelocity = Vector3.TransformCoordinate(AngularMomentum,WorldMomentsInverse);

            /*
            Vector3 RelativeCollisionPoint = collisionPoint - this.Position;
            Vector3 velocity1 = LinearVelocity + Vector3.Cross(AngularVelocity, RelativeCollisionPoint);
            Vector3 CollisionNormal = Vector3.Normalize(collisionPoint - this.Position);
            //Compute angular factor
            Vector3 angularFactor = Vector3.Cross(RelativeCollisionPoint,CollisionNormal);
            //Just current body elasticity affects velocity since the target object is static
            Vector3   = -(1.0f + this.Elasticity) * modifiedVel;




            AngularMomentum+= Vector3.Cross(collisionPoint,j1*/
            //this.world.GravityEnabled = false;
        }

        /// <summary>
        /// Ver si un punto pertenece a una cara de un BoundingBox
        /// </summary>
        /// <returns>True si pertenece</returns>
        private bool pointInBounbingBoxFace(Vector3 p, TgcBoundingBox.Face bbFace)
        {
            Vector3 min = bbFace.Extremes[0];
            Vector3 max = bbFace.Extremes[3];

            return p.X >= min.X && p.Y >= min.Y && p.Z >= min.Z &&
               p.X <= max.X && p.Y <= max.Y && p.Z <= max.Z;
        }


    }
}