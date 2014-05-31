using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.MarioKillers
{
    public class Impulse
    {
        public Vector3 Force = Vector3.Empty;
        public Vector3 RelativePosition = Vector3.Empty;

        public Impulse(Vector3 Force, Vector3 RelativePosition)
        {
            this.Force = Force;
            this.RelativePosition = RelativePosition;
        }

        public Vector3 Torque()
        {
            return Vector3.Cross(RelativePosition, Force);
        }
    }
}