using Microsoft.DirectX;
using System.Drawing;
using TgcViewer.Example;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.MarioKillers.Ejemplos
{
    public class InterseccionEsferas : TgcExample
    {
        private World world;
        private RigidBody cube;
        private RigidBody box;

        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        public override string getName()
        {
            return "Intersección entre esferas";
        }

        public override string getDescription()
        {
            return "Muestra la intersección entre dos cuerpos rígidos mediante sus bounding spheres.";
        }

        public override void init()
        {
            world = new World();
            world.DebugEnabled = true;
            box = new RigidBody(5.0f, TgcBox.fromSize(new Vector3(13, 5, 5), Color.Turquoise).toMesh(""));
            cube = new RigidBody(5.0f, TgcBox.fromSize(new Vector3(9, 9, 9), Color.HotPink).toMesh(""));
            box.Position = new Vector3(-20, 0, 0);
            cube.Position = new Vector3(10, 0, 0);
            world.AddBody(box);
            world.AddBody(cube);
        }

        public override void close()
        {
            world.Dispose();
        }

        public override void render(float elapsedTime)
        {
            world.Step(elapsedTime);
            box.ApplyImpulse(Vector3.Normalize(cube.Position - box.Position));
            cube.ApplyImpulse(Vector3.Normalize(box.Position - cube.Position));
            Color boundingSphereColor = cube.IsIntersectingWith(box) ? Color.Red : Color.Yellow;
            box.BoundingSphere.setRenderColor(boundingSphereColor);
            cube.BoundingSphere.setRenderColor(boundingSphereColor);
            world.Render();
        }
    }
}
