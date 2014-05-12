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
        private TgcDebugBox box;
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
            this.box = new TgcDebugBox();
            this.box.setPositionSize(new Vector3(0, 0, 0), new Vector3(10, 10, 10));
            this.box.Color = Color.Orange;
            this.rigidBody = new RigidBody(1.0f);
        }

        public override void render(float elapsedTime)
        {
            this.box.render();
            Vector3 appliedForce;
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
            else
            {
                appliedForce = new Vector3(0, 0, 0);
            }
            this.rigidBody.ApplyForce(appliedForce, elapsedTime);
            this.box.setPositionSize(rigidBody.position, new Vector3(30, 30, 30));
            this.box.updateValues();
        }

        public override void close()
        {
            this.box.dispose();
        }
    }
}
