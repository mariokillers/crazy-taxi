using Microsoft.DirectX;
using System.Drawing;
using TgcViewer.Example;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.MarioKillers.Ejemplos
{
    public class ColisionBoundingSpheres : TgcExample
    {
        private World world;
        private RigidBody cube;
        private RigidBody cube2;
        private RigidBody box;

        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        public override string getName()
        {
            return "Colisión entre bounding spheres";
        }

        public override string getDescription()
        {
            return "Muestra la colisión entre cuerpos rígidos mediante sus bounding spheres. A los cuerpos se les aplica una fuerza constante hacia el origen de módulo 1.";
        }

        public override void init()
        {
            world = new World();
            world.DebugEnabled = true;
            box = new RigidBody(30.0f, TgcBox.fromSize(new Vector3(13, 5, 5), Color.Turquoise).toMesh(""),world);
            cube = new RigidBody(15.0f, TgcBox.fromSize(new Vector3(9, 9, 9), Color.HotPink).toMesh(""),world);
            cube2 = new RigidBody(10.0f, TgcBox.fromSize(new Vector3(5, 5, 5), Color.Blue).toMesh(""),world);
            box.Position = new Vector3(-30, 0, 0);
            cube.Position = new Vector3(0, 0, 30);
            cube2.Position = new Vector3(0, 30, 0);
            world.AddBody(box);
            world.AddBody(cube);
            world.AddBody(cube2);
        }

        public override void close()
        {
            world.Dispose();
        }

        public override void render(float elapsedTime)
        {
            box.ApplyImpulse(Vector3.Normalize(-box.Position));
            cube.ApplyImpulse(Vector3.Normalize(-cube.Position));
            cube2.ApplyImpulse(Vector3.Normalize(-cube2.Position));
            world.Step(elapsedTime);
            world.Render();
        }
    }
}
