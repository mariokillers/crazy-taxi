using Microsoft.DirectX;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using TgcViewer.Example;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using System.Text;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.Shaders;
using TgcViewer.Utils.Interpolation;
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;
using System.Text.RegularExpressions;



namespace AlumnoEjemplos.MarioKillers.Ejemplos
{
    public class Pool : TgcExample
    {
        private List<Color> colors = new List<Color>();

        private World world;
        private Random rng = new Random();
        TgcScene Mesa;
        TgcScene scene;
        TgcBox lightMesh;
        TgcMesh bola;
        TgcMesh palo;
        RigidBody blanca;
        RigidBody punta;
        bool Dragging;
        float DraggingAmount = 0;
        int count = 0;
        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        public override string getName()
        {
            return "Mesa de Pool";
        }

        public override string getDescription()
        {
            return "Juego tipico de pool.";
        }

        public override void init()
        {
            //ILUMINACION
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            //Cargar escenario
            TgcSceneLoader loader = new TgcSceneLoader();
            //Configurar MeshFactory customizado
            scene = loader.loadSceneFromFile(GuiController.Instance.ExamplesMediaDir + "DepositoVacio-TgcScene.xml");

            //Camara en 1ra persona
            //GuiController.Instance.FpsCamera.Enable = true;
           // GuiController.Instance.FpsCamera.MovementSpeed = 400f;
           // GuiController.Instance.FpsCamera.JumpSpeed = 300f;


            //Mesh para la luz
            lightMesh = TgcBox.fromSize(new Vector3(10, 10, 10), Color.Red);

            //Modifiers de la luz
            GuiController.Instance.Modifiers.addBoolean("lightEnable", "lightEnable", true);
            GuiController.Instance.Modifiers.addVertex3f("lightPos", new Vector3(-200, -100, -200), new Vector3(200, 200, 300), new Vector3(0, 200, 0));
            GuiController.Instance.Modifiers.addVertex3f("lightDir", new Vector3(-1, -1, -1), new Vector3(1, 1, 1), new Vector3(0, -1, 0));
            GuiController.Instance.Modifiers.addColor("lightColor", Color.White);
            GuiController.Instance.Modifiers.addFloat("lightIntensity", 0, 150, 75);
            GuiController.Instance.Modifiers.addFloat("lightAttenuation", 0.1f, 2, 0.3f);
            GuiController.Instance.Modifiers.addFloat("specularEx", 0, 20, 9f);
            GuiController.Instance.Modifiers.addFloat("spotAngle", 0, 180, 39f);
            GuiController.Instance.Modifiers.addFloat("spotExponent", 0, 20, 7f);

            //Modifiers de material
            GuiController.Instance.Modifiers.addColor("mEmissive", Color.Black);
            GuiController.Instance.Modifiers.addColor("mAmbient", Color.White);
            GuiController.Instance.Modifiers.addColor("mDiffuse", Color.White);
            GuiController.Instance.Modifiers.addColor("mSpecular", Color.White);
            //
            

            string sphere = GuiController.Instance.ExamplesMediaDir + "ModelosTgc\\pelotasdepool2-TgcScene.xml";
            string mesa = GuiController.Instance.ExamplesMediaDir + "ModelosTgc\\famosisimamesadepool-TgcScene.xml";

            world = new World();
            world.DebugEnabled = true;
            //Mesa = loader.loadSceneFromFile(GuiController.Instance.ExamplesMediaDir + "ModelosTgc\\Sphere\\Sphere-TgcScene.xml").Meshes[0];
            setPelotas(loader, sphere, d3dDevice);
            
           //POSICIONO LA MESA
            Mesa = loader.loadSceneFromFile(mesa);

            foreach (TgcMesh mesh in Mesa.Meshes.Where(mesh => new Regex("ChamferBox00[2-8]").IsMatch(mesh.Name)))
            {
                world.AddStaticBody(mesh);
                mesh.move(new Vector3(0,0.5f,0));
            }
            foreach(TgcMesh mesh in Mesa.Meshes.Where(mesh => mesh.Name=="Box001")){
                world.AddStaticBody(mesh);
            }
           
             world.GravityEnabled = true;
            TgcBox caja = TgcBox.fromSize(new Vector3(0, 25, 0),new Vector3 (100,1,100));
            TgcBox caja2 = TgcBox.fromSize(new Vector3(0, 25, 0), new Vector3(100, 15, 2));
            TgcBox caja3 = TgcBox.fromSize(new Vector3(0, 25, 0), new Vector3(100, 15, 2));
            TgcBox caja4 = TgcBox.fromSize(new Vector3(0, 25, 0), new Vector3(100, 15, 2));
            TgcBox caja5 = TgcBox.fromSize(new Vector3(0, 25, 0), new Vector3(100, 15, 2));

            caja2.AutoTransformEnable = false;
            caja3.AutoTransformEnable = false;
            caja4.AutoTransformEnable = false;
            caja5.AutoTransformEnable = false;
            caja.AutoTransformEnable = false;
            caja2.Transform = Matrix.Translation(new Vector3(0, 25, -25));
            caja.Transform = Matrix.Translation(new Vector3(0, 25, 0));
            //world.AddStaticBody(caja2.toMesh("caja2"));
            //world.AddStaticBody(caja.toMesh("caja"));
            caja.render();
            caja2.render();
            GuiController.Instance.RotCamera.setCamera(new Vector3(-44, 200, 175),100);
            GuiController.Instance.RotCamera.targetObject(Mesa.BoundingBox);

          
        }

        public override void close()
        {
            world.Dispose();
            Mesa.disposeAll();
            scene.disposeAll();
        }

        public override void render(float elapsedTime)
        {
            //caja2.render();

            TgcD3dInput input = GuiController.Instance.D3dInput;

           /* if (input.keyDown(Key.Down))
            {
                if (!Dragging)
                {
                    Dragging = true;
                } 
                DraggingAmount += 1;
                punta.Position -= new Vector3(-1,0,0);
            }*/
            if (input.keyDown(Key.Up))
            {
                punta.ApplyImpulse(new Vector3(-150, 0, 0));
            }
         /*   if (input.keyUp(Key.Down))
            {
                if (Dragging || DraggingAmount!=0)
                {
                    punta.ApplyImpulse(new Vector3(blanca.Position.X - punta.Position.X,0,0));
                    Dragging = false;
                    DraggingAmount--;
                    count++;
                    Console.WriteLine(count);
                }
            }*/
            
            Microsoft.DirectX.Direct3D.Device device = GuiController.Instance.D3dDevice;


            //Habilitar luz
            bool lightEnable = (bool)GuiController.Instance.Modifiers["lightEnable"];
            Microsoft.DirectX.Direct3D.Effect currentShader;
            if (lightEnable)
            {
                //Con luz: Cambiar el shader actual por el shader default que trae el framework para iluminacion dinamica con SpotLight
                currentShader = GuiController.Instance.Shaders.TgcMeshSpotLightShader;
            }
            else
            {
                //Sin luz: Restaurar shader default
                currentShader = GuiController.Instance.Shaders.TgcMeshShader;
            }

            //Aplicar a cada mesh el shader actual
            foreach (TgcMesh mesh in scene.Meshes)
            {
                mesh.Effect = currentShader;
                //El Technique depende del tipo RenderType del mesh
                mesh.Technique = GuiController.Instance.Shaders.getTgcMeshTechnique(mesh.RenderType);
            }


            //Actualzar posición de la luz
            Vector3 lightPos = (Vector3)GuiController.Instance.Modifiers["lightPos"];
            lightMesh.Position = lightPos;

            //Normalizar direccion de la luz
            Vector3 lightDir = (Vector3)GuiController.Instance.Modifiers["lightDir"];
            lightDir.Normalize();
            
            //Renderizar meshes
            foreach (TgcMesh mesh in scene.Meshes)
            {
                if (lightEnable)
                {
                    //Cargar variables shader de la luz
                    mesh.Effect.SetValue("lightColor", ColorValue.FromColor((Color)GuiController.Instance.Modifiers["lightColor"]));
                    mesh.Effect.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(lightPos));
                    mesh.Effect.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(GuiController.Instance.FpsCamera.getPosition()));
                    mesh.Effect.SetValue("spotLightDir", TgcParserUtils.vector3ToFloat3Array(lightDir));
                    mesh.Effect.SetValue("lightIntensity", (float)GuiController.Instance.Modifiers["lightIntensity"]);
                    mesh.Effect.SetValue("lightAttenuation", (float)GuiController.Instance.Modifiers["lightAttenuation"]);
                    mesh.Effect.SetValue("spotLightAngleCos", FastMath.ToRad((float)GuiController.Instance.Modifiers["spotAngle"]));
                    mesh.Effect.SetValue("spotLightExponent", (float)GuiController.Instance.Modifiers["spotExponent"]);

                    //Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
                    mesh.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor((Color)GuiController.Instance.Modifiers["mEmissive"]));
                    mesh.Effect.SetValue("materialAmbientColor", ColorValue.FromColor((Color)GuiController.Instance.Modifiers["mAmbient"]));
                    mesh.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor((Color)GuiController.Instance.Modifiers["mDiffuse"]));
                    mesh.Effect.SetValue("materialSpecularColor", ColorValue.FromColor((Color)GuiController.Instance.Modifiers["mSpecular"]));
                    mesh.Effect.SetValue("materialSpecularExp", (float)GuiController.Instance.Modifiers["specularEx"]);
                }

                //Renderizar modelo
                mesh.render();
                mesh.BoundingBox.render();
            }
            foreach (TgcMesh mesh in Mesa.Meshes)
            {
               // mesh.move(new Vector3(0, -50, -200));
                mesh.Effect = currentShader;
                //El Technique depende del tipo RenderType del mesh
                mesh.Technique = GuiController.Instance.Shaders.getTgcMeshTechnique(mesh.RenderType);
                mesh.render();

            }

            foreach (RigidBody body in world.Bodies)
            {
                
                body.Mesh.Effect = currentShader;
                //El Technique depende del tipo RenderType del mesh
                body.Mesh.Technique = GuiController.Instance.Shaders.getTgcMeshTechnique(body.Mesh.RenderType);
            }
            world.Step(elapsedTime);
            world.Render();


            //Renderizar mesh de luz
            lightMesh.render();
            palo.Transform = Matrix.Translation(punta.Position- new Vector3(-25,0,0));
            palo.render();
            
        }

        public void setPelotas(TgcSceneLoader loader, string sphere,Microsoft.DirectX.Direct3D.Device d3dDevice)
        {
            List<Vector3> PosicionesPelotas = new List<Vector3>();
            PosicionesPelotas.Insert(0,new Vector3(0,50,0));
            PosicionesPelotas.Insert(1, new Vector3(-4.5f, 50, 2.75f));
            PosicionesPelotas.Insert(2, new Vector3(-4.5f, 50, -2.75f));
            PosicionesPelotas.Insert(3, new Vector3(-9, 50, 5.5f));
            PosicionesPelotas.Insert(4, new Vector3(-9, 50, 0));
            PosicionesPelotas.Insert(5, new Vector3(-9, 50, -5.5f));
            PosicionesPelotas.Insert(6, new Vector3(-13.5f, 50, 8.25f));
            PosicionesPelotas.Insert(7, new Vector3(-13.5f, 50, 2.75f));
            PosicionesPelotas.Insert(8, new Vector3(-13.5f, 50, -2.75f));
            PosicionesPelotas.Insert(9, new Vector3(-13.5f, 50, -8.25f));
            PosicionesPelotas.Insert(10, new Vector3(-18, 50, 11));
            PosicionesPelotas.Insert(11, new Vector3(-18, 50, 5.5f));
            PosicionesPelotas.Insert(12, new Vector3(-18, 50, 0));
            PosicionesPelotas.Insert(13, new Vector3(-18, 50, -5.5f));
            PosicionesPelotas.Insert(14, new Vector3(-18, 50, -11));
            PosicionesPelotas.Insert(15, new Vector3(45, 50, 0));


            TgcScene pelotas = loader.loadSceneFromFile(sphere);
            
            for (int i = 1; i < 16; i++)
            {
                bola = pelotas.Meshes[i - 1];
                bola.createBoundingBox();
                bola.BoundingBox.setRenderColor(Color.Violet);
                TgcBoundingSphere boundingSphere = new TgcBoundingSphere(bola.BoundingBox.calculateBoxCenter(), bola.BoundingBox.calculateBoxRadius());//boundingSphereFromPoints(Mesh.getVertexPositions());
                RigidBody createdBody = new RigidBody(20.0f, bola, world);
                createdBody.Position = PosicionesPelotas[i-1];
                world.Bodies.Add(createdBody);
            }
            bola = loader.loadSceneFromFile(sphere).Meshes[0];
            bola.createBoundingBox();
            bola.changeDiffuseMaps(new TgcTexture[] { TgcTexture.createTexture(d3dDevice, GuiController.Instance.ExamplesMediaDir + "Billiard\\Textures\\blanca.jpg") });
            TgcBoundingSphere boundingSphere1 = new TgcBoundingSphere(bola.BoundingBox.calculateBoxCenter(), bola.BoundingBox.calculateBoxRadius());//boundingSphereFromPoints(Mesh.getVertexPositions());
            blanca = new RigidBody(5.0f, bola, world);
            blanca.Position = PosicionesPelotas[15];
            
            world.Bodies.Add(blanca);
            punta = new RigidBody(100,TgcBox.fromSize(new Vector3(0, 0, 0),new Vector3(5f,5f,5f),Color.Gray).toMesh("punta"),world);
            punta.Position = new Vector3(60, 28, 0);
            punta.affectedByGravity = false;
            punta.strong = true;
            world.Bodies.Add(punta);
            palo = TgcBox.fromSize(new Vector3(0,0,0), new Vector3(50f, 0.5f, 0.5f), Color.Brown).toMesh("palo");
            punta.floorCollisionsEnabled = false;
            palo.AutoTransformEnable = false;

        }
    }
}
