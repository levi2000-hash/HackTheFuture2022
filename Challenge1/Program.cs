// See https://aka.ms/new-console-template for more information
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;


using (var client = new HttpClient())
{
    client.BaseAddress = new Uri("https://exs-htf2022-api.azurewebsites.net/api/challenges/");
    client.DefaultRequestHeaders.Add("Authorization", "26cc4ab9-47d8-47e8-afe4-c16ed5c32fbf");

    //Get data
    HttpResponseMessage response = await client.GetAsync("room-of-requirement");
    string responseBody = await response.Content.ReadAsStringAsync();

    //Create answer
    Attack attack = JsonSerializer.Deserialize<Attack>(responseBody);
    double answer = attack.CalculateAngle();
    Answer answerObj = new Answer();
    answerObj.answer = Math.Round(answer, 2);

    //Send response
    response = await client.PostAsync("chamber-of-secrets", new StringContent(JsonSerializer.Serialize(answerObj), Encoding.UTF8, "application/json"));
    response.EnsureSuccessStatusCode();

};

public class Attack
{
    public double monsterHeight { get; set; }
    public double monsterNeckDistance { get; set; }
    public double heroDistanceFromMonster { get; set; }
    public double heroWeaponHeight { get; set; }

    public double CalculateAngle()
    {
        double B = monsterHeight - monsterNeckDistance - heroWeaponHeight;
        double C = Math.Sqrt(Math.Pow(heroDistanceFromMonster, 2) + Math.Pow(B, 2));
        return (Math.Asin(B / C) * (180 / Math.PI));
    }
}
public class Answer
{
    public double answer { get; set; }
}
