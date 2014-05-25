using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TgcViewer;
using TgcViewer.Example;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.MarioKillers
{
    /// <summary>
    /// Ejemplo del alumno
    /// </summary>
    public class DinamicaBasica : TgcExample
    {
        private Shape box;
        private World world;
        private RigidBody rigidBody;
        private TgcD3dInput input = GuiController.Instance.D3dInput;

        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        public override string getName()
        {
            return "Dinámica básica";
        }

        public override string getDescription()
        {
            return "Mover la caja aplicándole fuerzas con WASD";
        }

        public override void init()
        {
            this.input = GuiController.Instance.D3dInput;
            GuiController.Instance.RotCamera.setCamera(new Vector3(0, 0, 0), 100);
            this.box = BoxShape.fromSize(new Vector3(10, 10, 10), Color.Orange);
            this.rigidBody = new RigidBody(1.0f, box);
            this.world = new World();
            this.world.AddBody(rigidBody);
        }

        public override void render(float elapsedTime)
        {
            Vector3 appliedForce = new Vector3(0, 0, 0);
            if (input.keyDown(Key.A))
            {
                appliedForce = new Vector3(100, 0, 0);
            }
            else if (input.keyDown(Key.D))
            {
                appliedForce = new Vector3(-100, 0, 0);
            }
            else if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.W))
            {
                appliedForce = new Vector3(0, 0, -100);
            }
            else if (input.keyDown(Key.S))
            {
                appliedForce = new Vector3(0, 0, 100);
            }
            this.rigidBody.ApplyForce(appliedForce);
            this.world.Step(elapsedTime);
            this.world.Render();
        }

        public override void close()
        {
            this.world.Dispose();
        }
    }
}
