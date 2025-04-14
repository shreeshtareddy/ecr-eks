using System;
using System.Collections.Generic;

public class CountryCapitalMap
{
    private Dictionary<string, string> _M1;

    public CountryCapitalMap()
    {
        _M1 = new Dictionary<string, string>();
    }

    public Dictionary<string, string> storeCountryCapital(string CountryName, string capital)
    {
        _M1[CountryName] = capital;
        return _M1;
    }

    public string retrieveCapital(string CountryName)
    {
        return _M1.TryGetValue(CountryName, out var capital) ? capital : null;
    }

    public string retrieveCountry(string capitalName)
    {
        foreach (var entry in _M1)
        {
            if (entry.Value.Equals(capitalName, StringComparison.OrdinalIgnoreCase))
            {
                return entry.Key;
            }
        }
        return null;
    }

    public Dictionary<string, string> createCapitalCountryMap()
    {
        Dictionary<string, string> _M2 = new Dictionary<string, string>();
        foreach (var entry in _M1)
        {
            _M2[entry.Value] = entry.Key;
        }
        return _M2;
    }

    public List<string> getAllCountryNames()
    {
        List<string> countryNames = new List<string>(_M1.Keys);
        return countryNames;
    }
}





