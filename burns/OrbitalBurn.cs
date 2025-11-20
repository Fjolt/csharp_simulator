using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using One_Sgp4;
using Utils;

namespace Burns
{
    public static class ImpulseBurn
    {
        private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;
        public static SatState burnWithDeltaV(SatState stateOld, double thrusterX, double thrusterY, double thrusterZ)
        {
            return new SatState
            {
                PositionX = stateOld.PositionX,
                PositionY = stateOld.PositionY,
                PositionZ = stateOld.PositionZ,
                VelocityX = stateOld.VelocityX + thrusterX,
                VelocityY = stateOld.VelocityY + thrusterY,
                VelocityZ = stateOld.VelocityZ + thrusterZ,
                EpochUtc = stateOld.EpochUtc,
            };
        }
        
        
    }
    
}