using System;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.MarioKillers
{
    /// <summary>
    /// Helper class to encapusulate an object than can be both rendered
    /// and transformed. An abstraction like this should have been used
    /// instead of implementing these two interfaces everywhere.
    /// Only the bare minimum methods are marked as abstract here to
    /// force their implementation in the subclasses.
    /// </summary>
    public abstract class Shape : IRenderObject, ITransformObject
    {
        public abstract void render();
        public abstract void dispose();
        public abstract Vector3 Position { get; set; }
        public abstract Vector3 Rotation { get; set; }
        public abstract Vector3 Scale { get; set; }

        public bool AlphaBlendEnable
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Matrix Transform
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool AutoTransformEnable
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void move(Vector3 v)
        {
            throw new NotImplementedException();
        }

        public void move(float x, float y, float z)
        {
            throw new NotImplementedException();
        }

        public void moveOrientedY(float movement)
        {
            throw new NotImplementedException();
        }

        public void getPosition(Vector3 pos)
        {
            throw new NotImplementedException();
        }

        public void rotateX(float angle)
        {
            throw new NotImplementedException();
        }

        public void rotateY(float angle)
        {
            throw new NotImplementedException();
        }

        public void rotateZ(float angle)
        {
            throw new NotImplementedException();
        }
    }
}
