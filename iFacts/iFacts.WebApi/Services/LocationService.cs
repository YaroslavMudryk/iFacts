﻿using iFacts.Data.Entities;
using iFacts.WebApi.Dtos;
using System.Diagnostics;
using System.Text.Json;

namespace iFacts.WebApi.Services;

public interface ILocationService
{
    Task<Location> GetIpInfoAsync(string ip);
}

public class LocationService : ILocationService
{
    public async Task<Location> GetIpInfoAsync(string ip)
    {
        if (ip == "127.0.1" || ip == "127.0.0.1")
            ip = "localhost";

        var location = new Location
        {
            IP = ip
        };

        try
        {
            using var httpClient = new HttpClient();
            var urlRequest = "http://ip-api.com/json";
            if (string.IsNullOrEmpty(ip))
            {
                urlRequest = urlRequest + "?fields=63700991";
            }
            else
            {
                if (ip.Contains("::1") || ip.Contains("localhost"))
                    urlRequest = urlRequest + "?fields=63700991";
                else
                    urlRequest = urlRequest + $"/{ip}?fields=63700991";
            }
            var resultFromApi = await httpClient.GetAsync(urlRequest);
            if (!resultFromApi.IsSuccessStatusCode)
                return null;
            var content = await resultFromApi.Content.ReadAsStringAsync();
            var res = JsonSerializer.Deserialize<IPGeo>(content);
            location.Provider = res.Isp;
            location.Country = res.Country;
            location.City = res.City;
            location.Region = res.RegionName;
            location.Lat = res.Latitude;
            location.Lon = res.Longitude;
            location.IP = res.Query;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
        return location;
    }
}
