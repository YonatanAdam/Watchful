using System;

namespace Model
{
    public class NavHistory : BaseEntity
    {
        
        // User
        
        private DateTime startDate;
        private DateTime endDate;
        
        private POI startPOI;
        private POI endPOI;

        private double distance;
        private DateTime duration;
        
        private NavigationType navigationType;

        public DateTime StartDate
        {
            get => startDate;
            set => startDate = value;
        }

        public DateTime EndDate
        {
            get => endDate;
            set => endDate = value;
        }

        public POI StartPOI
        {
            get => startPOI;
            set => startPOI = value;
        }

        public POI EndPOI
        {
            get => endPOI;
            set => endPOI = value;
        }

        public double Distance
        {
            get => distance;
            set => distance = value;
        }

        public DateTime Duration
        {
            get => duration;
            set => duration = value;
        }

        public NavigationType NavigationType
        {
            get => navigationType;
            set => navigationType = value;
        }
    }
}