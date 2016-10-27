﻿using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using AirTraffic.DataObjects;
#if UNITY_UWP
using System.Diagnostics;
#endif
public class AircraftLoader : MonoBehaviour {

    public GameObject Aircraft;

    public string TopLevelName = "HologramCollection";

    private Dictionary<string, GameObject> _aircrafts;

    private Queue<FlightSet> _receivedData;
#if UNITY_UWP
    private TimeSpan _waitTime = TimeSpan.FromSeconds(5);

    private DateTimeOffset _lastUpdate = DateTimeOffset.MinValue;
#endif
    private GameObject _topLevelObject;

    private bool _isUpdated;

    private FlightSet set = new FlightSet();

    void Start()
    {
        _aircrafts = new Dictionary<string, GameObject>();
        _topLevelObject = GameObject.Find(TopLevelName);
        _receivedData = new Queue<FlightSet>();
#if UNITY_UWP
        _receivedData.Enqueue(new FlightSet(DataService.Instance.GetFlights()));
#endif

    }

    public void OnReset()
    {
        foreach (var aircraft in _aircrafts)
        {
            DestroyImmediate(aircraft.Value);
        }
        _aircrafts = new Dictionary<string, GameObject>();
        _receivedData = new Queue<FlightSet>();
#if UNITY_UWP
        _receivedData.Enqueue(new FlightSet(DataService.Instance.GetFlights()));
#endif
        _isUpdated = false;
    }

    public void OnSlowDown()
    {
#if UNITY_UWP
        _waitTime = _waitTime.Add(TimeSpan.FromSeconds(5));
#endif
    }

    public void OnSpeedUp()
    {
#if UNITY_UWP
        _waitTime = _waitTime.Subtract(TimeSpan.FromSeconds(5));
#endif
    }

    public void OnResetTime()
    {
#if UNITY_UWP
        _waitTime = TimeSpan.FromSeconds(5);
#endif
    }

#if UNITY_UWP

    private void ProcessData(List<Flight> flightData)
    {
        var set = new FlightSet(flightData);
        _receivedData.Enqueue(set);
    }
#endif
    void Update()
    {   
        if (!_isUpdated)
        {
            _isUpdated = true;
            set = _receivedData.Dequeue();
            var flightIds = set.Flights.Select(p => p.Id).ToList();

            var aircraftToDelete =
              _aircrafts.Keys.Where(p => !flightIds.Contains(p)).ToList();
            DeleteAircraft(aircraftToDelete);

            var aircraftToAdd =
              set.Flights.Where(p => !_aircrafts.Keys.Contains(p.Id)).ToList();
            CreateAircraft(aircraftToAdd);
        }

#if UNITY_UWP
        foreach (var flight in set.Flights)
        {
            if ((flight.lastUpdate - DateTimeOffset.Now).Duration() > _waitTime)
            {
                flight.lastUpdate = DateTimeOffset.Now;
                OnElapsed(flight);
            }
        }
#endif
    }

    private void CreateAircraft(IEnumerable<Flight> flights)
    {
        foreach (var flight in flights)
        {
            var aircraft = Instantiate(Aircraft);
            aircraft.transform.parent = _topLevelObject.transform;
            aircraft.transform.localScale = new Vector3(0f, 0f, 0f);
            SetNewFlightData(aircraft, flight);
            _aircrafts.Add(flight.Id, aircraft);
#if UNITY_UWP
            flight.lastUpdate = DateTimeOffset.Now;
#endif
        }
    }


    private void DeleteAircraft(IEnumerable<string> keys)
    {
        foreach (var key in keys)
        {
            var aircraft = _aircrafts[key];
            Destroy(aircraft);
            _aircrafts.Remove(key);
        }
    }

    private void SetNewFlightData(GameObject aircraft, Flight flight)
    {
        var controller = aircraft.GetComponent<AircraftController>();
        if (controller != null)
        {
            controller.SetNewFlightData(flight);
        }
    }
#if UNITY_UWP
    private void OnElapsed(Flight sender)
    {
        if (sender.Track.Count != 0)
        {
            sender.Location = sender.Track.Pop();
            if (_aircrafts.ContainsKey(sender.Id))
            {
                var aircraft = _aircrafts[sender.Id];
                SetNewFlightData(aircraft, sender);
            }
        }
        else
        {
            List<string> list = new List<string>();
            list.Add(((Flight)sender).Id);
            DeleteAircraft(list);
        }
    }
#endif
}