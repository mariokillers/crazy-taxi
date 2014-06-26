using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TgcViewer.Example;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.MarioKillers.Ejemplos
{
    public class PruebaTunneling : TgcExample
    {
        private float speed;
        private World world;
        private RigidBody body1;
        private RigidBody body2;

        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        public override string getName()
        {
            return "Prueba de tunneling";
        }

        public override string getDescription()
        {
            return "Hace que dos cuerpos choquen con cada vez más velocidad hasta que se traspasan (por tunneling).";
        }

        public override void init()
        {
            world = new World();
            world.DebugEnabled = true;
            body1 = new RigidBody(1, TgcBox.fromSize(new Vector3(1, 1, 1), Color.Red).toMesh(""),world);
            body2 = new RigidBody(1, TgcBox.fromSize(new Vector3(1, 1, 1), Color.Green).toMesh(""),world);
            world.AddBody(body1);
            world.AddBody(body2);
            reset();
        }

        public override void close()
        {
            world.Dispose();
        }

        public override void render(float elapsedTime)
        {
            world.Step(elapsedTime);
            if (body1.LinearVelocity.X < 0)
            {
                reset();
                speed += 5;
                body1.LinearVelocity = new Vector3(speed, 0, 0);
                body2.LinearVelocity = new Vector3(-speed, 0, 0);
            }
            if (body1.Position.X > 6)
            {
                MessageBox.Show("Tunneling detectado. Velocidad: " + speed + " FPS: " + 1.0f / elapsedTime);
                reset();
                speed = 3f;
            }
            world.Render();
        }

        private void reset()
        {
            body1.LinearVelocity = new Vector3(3, 0, 0);
            body1.Position = new Vector3(-4, 0, 0);
            body2.LinearVelocity = new Vector3(-3, 0, 0);
            body2.Position = new Vector3(4, 0, 0);
        }
    }
}
