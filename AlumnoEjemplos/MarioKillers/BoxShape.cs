using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcGeometry;
using System.Drawing;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.MarioKillers
{
    /// <summary>
    /// Wrapper class for <cref>TgcBox</cref>
    /// </summary>
    public class BoxShape : Shape
    {
        private TgcBox box;

        public BoxShape(TgcBox box)
        {
            this.box = box;
        }

        public static BoxShape fromSize(Vector3 size, Color color)
        {
            return new BoxShape(TgcBox.fromSize(size, color));
        }

        public override void render()
        {
            this.box.render();
        }

        public override void dispose()
        {
            this.box.dispose();
        }

        public override Vector3 Position
        {
            get { return this.box.Position; }
            set { this.box.Position = value; }
        }

        public override Vector3 Rotation
        {
            get { return this.box.Rotation; }
            set { this.box.Rotation = value; }
        }

        public override Vector3 Scale
        {
            get { return this.box.Scale; }
            set { this.box.Scale = value; }
        }

        public override TgcMesh Mesh
        {
            get { return this.box.toMesh(""); }
        }
    }
}
