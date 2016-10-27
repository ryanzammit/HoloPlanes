using System;
using System.Collections.Generic;
using AirTraffic.DataObjects;
using HoloToolkit.Unity;
using System.IO;
using UnityEngine;
#if UNITY_UWP
using Newtonsoft.Json;
#endif

public class DataService : Singleton<DataService>
{
#if UNITY_UWP
    public List<Flight> GetFlights()
    {
        {
            try {

                TextAsset asset = Resources.Load<TextAsset>("json");
                if (asset.text != null)
                {
                    string json = asset.text;
                    return JsonConvert.DeserializeObject<List<Flight>>(json);
                }
                return null;

        }
            catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
            return null;
        }

        }
    }
#endif
}
