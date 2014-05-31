using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TgcViewer;
using TgcViewer.Example;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.MarioKillers
{
    /// <summary>
    /// Ejemplo del alumno
    /// </summary>
    public class DinamicaBasica : TgcExample
    {
        private TgcMesh box;
        TgcTexture texture = TgcTexture.createTexture(GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Textures\\Madera\\cajaMadera3.jpg");

        private World world;
        private RigidBody rigidBody;
        private TgcD3dInput input = GuiController.Instance.D3dInput;
        private TgcModifiers modifiers = GuiController.Instance.Modifiers;

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
            return "Mover la caja aplicándole fuerzas con WASD, Control y Espacio";
        }

        public override void init()
        {
            this.input = GuiController.Instance.D3dInput;
            GuiController.Instance.RotCamera.setCamera(new Vector3(0, 0, 0), 100);
            this.modifiers.addBoolean("GravityEnabled", "Gravedad", false);
            this.box = TgcBox.fromSize(new Vector3(0,0,0),new Vector3(10, 10, 10),texture).toMesh("caja");
            this.rigidBody = new RigidBody(10f, box);
            this.world = new World();
            this.world.AddBody(rigidBody);
        }

        public override void render(float elapsedTime)
        {
            Vector3 appliedForce = new Vector3(0, 0, 0);
            // TODO: Refactor this into an input handler
            if (input.keyDown(Key.A))
            {
                appliedForce = new Vector3(0.001f, 0, 0);
            }
            else if (input.keyDown(Key.D))
            {
                appliedForce = new Vector3(-0.001f, 0, 0);
            }
            else if (input.keyDown(Key.W))
            {
                appliedForce = new Vector3(0, 0, -0.001f);
            }
            else if (input.keyDown(Key.S))
            {
                appliedForce = new Vector3(0, 0, 0.001f);
            }
            else if (input.keyDown(Key.Space))
            {
                appliedForce = new Vector3(0, 100, 0);
            }
            else if (input.keyDown(Key.LeftControl))
            {
                appliedForce = new Vector3(0, -0.01f, 0);
            }
            this.rigidBody.ApplyImpulse(appliedForce, new Vector3(4f,10f,0));
            this.world.GravityEnabled = (bool)this.modifiers.getValue("GravityEnabled");
            this.world.Step(elapsedTime);
            this.world.Render();
        }

        public override void close()
        {
            this.world.Dispose();
        }
    }
}
