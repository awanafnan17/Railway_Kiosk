using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace RailwayKiosk
{
    public static class TrainService
    {
        public static List<Train> LoadTrains(string baseDir)
        {
            var path = Path.Combine(baseDir, "TrainData", "trains.json");
            if (!File.Exists(path)) return new List<Train>();
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<Train>>(json) ?? new List<Train>();
        }

        public static void SaveTrains(string baseDir, List<Train> trains)
        {
            var path = Path.Combine(baseDir, "TrainData", "trains.json");
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(trains, options);
            File.WriteAllText(path, json);
        }

        public static List<Train> Filter(List<Train> source, string? number, string? destination, string? type, DateTime? time)
        {
            var q = source.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(number))
                q = q.Where(t => t.TrainNumber.Contains(number, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(destination) && destination != "All")
                q = q.Where(t => string.Equals(t.Destination, destination, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(type) && type != "All")
                q = q.Where(t => string.Equals(t.TrainType, type, StringComparison.OrdinalIgnoreCase));
            if (time.HasValue)
                q = q.Where(t => Math.Abs((t.DepartureTime - time.Value).TotalMinutes) < 60);
            return q.ToList();
        }
    }
}
