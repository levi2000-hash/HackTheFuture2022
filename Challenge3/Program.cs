using System;
using System.Text;
using System.Text.Json;

namespace MyApp
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            using (var client = new HttpClient())
            {
                //Create client
                client.BaseAddress = new Uri("https://exs-htf2022-api.azurewebsites.net/api/challenges/");
                client.DefaultRequestHeaders.Add("Authorization", "26cc4ab9-47d8-47e8-afe4-c16ed5c32fbf");

                //Get data
                HttpResponseMessage response = await client.GetAsync("room-of-requirement");
                string responseBody = await response.Content.ReadAsStringAsync();

                List<List<string>> forest = new List<List<string>>();
                string wizard = null;
                List<string> path = Converter.findpath(forest, wizard);
                List<string> monsters = new List<string> { "A", "B", "C", "D", "G", "I", "P", "S", "T", "W" };

                Answer answer = new Answer();
                answer.answer = path;
                response = await client.PostAsync("room-of-requirement",
                    new StringContent(
                        JsonSerializer.Serialize(answer),
                        Encoding.UTF8,
                        "application/json"
                        ));
                response.EnsureSuccessStatusCode();
            };
        }
    }

    public static class Converter
    {
        public static List<string> findpath(List<List<string>> forest, string wizard)
        {
            List<double> fire = new List<double> { 2, 2, 0.5, 0.5, 1, 1, 2, 0.5, 1, 1 };
            List<double> ice = new List<double> { 1, 1, 2, 2, 0.5, 0.5, 1, 1, 2, 0.5 };
            List<double> water = new List<double> { 2, 0.5, 1, 1, 2, 2, 0.5, 0.5, 1, 1 };
            List<double> wind = new List<double> { 1, 1, 2, 0.5, 1, 1, 2, 2, 0.5, 0.5 };
            List<string> monsters = new List<string> { "A", "B", "C", "D", "G", "I", "P", "S", "T", "W" };

            List<List<double>> convertedforest = new List<List<double>> { };
            List<double> wizardSkills = new List<double>();
            List<string> way = new List<string>();
            double highestVal = 0;

            switch(wizard){
                case "fire":
                    wizardSkills = fire;
                    break;
                case "ice":
                    wizardSkills = ice;
                    break;
                case "water":
                    wizardSkills = water;
                    break;
                case "wind":
                    wizardSkills = wind;
                    break;
            }

            for(int i = 0; i < forest.Count; i++)
            {
                for(int j = 0; j < forest[i].Count; j++)
                {
                    for(int g = 0; g< monsters.Count; g++)
                    {
                        if (monsters[g] == forest[i][j])
                        {
                            if (wizardSkills[g] > highestVal)
                            {
                                highestVal = wizardSkills[g];
                                way.Add(forest[i][j]);
                            }
                        }
                    }
                };
            }

            return way;

        }
    }
}

class Answer
{
    public List<string> answer { get; set; }
}



