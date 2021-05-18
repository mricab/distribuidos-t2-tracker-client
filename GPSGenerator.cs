using System;

namespace GPSGenerator
{
    class GPS
    {
        private static double lastLatitude;
        private static double lastLongitude;

        public GPS() 
        {
            // Inicializa el GPS en la Av. Brasil de Santa Cruz de la Sierra.
            lastLatitude = -17.788949;
            lastLongitude = -63.164865;
        }

        public string getLocation() {
            Random random = new Random();
            double[] newLocation = {
                lastLatitude + (random.NextDouble() * 0.001) + 0.0005, 
                lastLongitude+ (random.NextDouble() * 0.001) + 0.0005,
                };
            lastLatitude = newLocation[0];
            lastLongitude = newLocation[1];
            return newLocation[0].ToString()+","+newLocation[1].ToString();
        }

    }
}