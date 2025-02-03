using System;

namespace Model
{
    public class Location : BaseEntity
    {
        private double latitude;
        private double longitude;
        
        public double Latitude { get => latitude; set => latitude = value; } // TODO: Logic for setting a coordinate to make sure it is on the map!.
        public double Longitude { get => longitude; set => longitude = value; }

        public override string ToString()
        {
            return $"(ID: {Id}) -> Latitude: {latitude}, Longitude: {longitude}";
        }

        public Location()
        {
            
        }

        public Location(double latitude, double longitude)
        {
            if (latitude > 0.0 && longitude > 0.0)
            {
                this.latitude = latitude;
                this.longitude = longitude;
            }
            else
            {
                this.latitude = 0.0;
                this.longitude = 0.0;
                Console.WriteLine("Latitude or Longitude are invalid, setting them to 0.0");
            }
        }
    }
}