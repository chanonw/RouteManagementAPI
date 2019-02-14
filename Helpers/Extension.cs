using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace RouteAPI.Helpers
{
    public static class Extension
    {
        public static void AddApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }
        public static IEnumerable<T> Randomize<T>(this IEnumerable<T> collection)
        {
            var list = new List<T>(collection);
            var randomizer = new Random();
            for (int i = list.Count; i > 0; i--)
            {
                int r = randomizer.Next(0, i);
                yield return list[r];
                list.RemoveAt(r);
            }
        }
    }
}