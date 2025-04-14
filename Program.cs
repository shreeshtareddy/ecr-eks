

namespace CountryCapital
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            CountryCapitalMap map = new CountryCapitalMap();
            map.storeCountryCapital("India", "Delhi");
            map.storeCountryCapital("Japan", "Tokyo");

            Console.WriteLine(map.retrieveCapital("India"));
            Console.WriteLine(map.retrieveCountry("Tokyo"));

            var capitalCountryMap = map.createCapitalCountryMap();
            foreach (var entry in capitalCountryMap)
            {
                Console.WriteLine($"{entry.Key} - {entry.Value}");
            }

            var countries = map.getAllCountryNames();
            Console.WriteLine(string.Join(", ", countries));
        }
    }
}
