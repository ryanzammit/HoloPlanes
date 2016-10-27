using System;
using System.Collections.Generic;
namespace AirTraffic.DataObjects
{
    public class Flight
    {
            public Flight()
            {
                Track = new Stack<Coordinate>();
#if UNITY_UWP
            lastUpdate = DateTimeOffset.MinValue;
#endif
            }

            public string Id { get; set; }

            public string FlightNr { get; set; }

            public string Aircraft { get; set; }

            public Coordinate Location { get; set; }

            public double? Speed { get; set; }

            public double? Heading { get; set; }

            public Stack<Coordinate> Track { get; set; }

            public TrackType TypeTrack { get; set; }

            public bool IsActive { get; set; }
#if UNITY_UWP
            public DateTimeOffset lastUpdate { get; set; }
#endif
            public override string ToString()
            {
                return string.Format("{0} {1} {2} {3} {4} {5}", FlightNr, Aircraft,
                  Location.Alt, Speed, Heading, TypeTrack).Trim();
            }
        }
}