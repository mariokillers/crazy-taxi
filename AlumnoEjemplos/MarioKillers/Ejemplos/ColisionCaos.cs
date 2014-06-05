using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
using TgcViewer.Example;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.MarioKillers.Ejemplos
{
    public class ColisionCaos : TgcExample
    {
        private List<Color> colors = new List<Color>();
        private World world;
        private Random rng = new Random();

        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        public override string getName()
        {
            return "Colisión caótica";
        }

        public override string getDescription()
        {
            return "Genera muchos cuerpos rígidos en posiciones aleatorias y los hace colisionar.";
        }

        public override void init()
        {
            Color[] colorArray = {Color.Red, Color.Yellow, Color.Blue, Color.White, Color.Black, Color.Green};
            colors.AddRange(colorArray);

            world = new World();
            world.DebugEnabled = true;
            for (int i = 0; i < 30; i++)
            {
                RigidBody createdBody = new RigidBody(5.0f, TgcBox.fromSize(new Vector3(10, 10, 10), colors[rng.Next(colors.Count)]).toMesh(""));
                createdBody.Position = new Vector3(rng.Next(-100, 100), rng.Next(-100, 100), rng.Next(-100, 100));
                world.Bodies.Add(createdBody);
            }
        }

        public override void close()
        {
            world.Dispose();
        }

        public override void render(float elapsedTime)
        {
            foreach (RigidBody body in world.Bodies)
            {
                body.ApplyImpulse(Vector3.Normalize(-body.Position));
            }
            world.Step(elapsedTime);
            world.Render();
        }
    }
}
