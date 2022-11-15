using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

using (var client = new HttpClient())
{
    //Create client
    client.BaseAddress = new Uri("https://exs-htf2022-api.azurewebsites.net/api/challenges/");
    client.DefaultRequestHeaders.Add("Authorization", "26cc4ab9-47d8-47e8-afe4-c16ed5c32fbf");

    //Get data
    HttpResponseMessage response = await client.GetAsync("ministery-of-magic");
    string responseBody = await response.Content.ReadAsStringAsync();

    //Deserialize and add zeros 
    Spell spell = JsonSerializer.Deserialize<Spell>(responseBody);
    byte[] spellToDecrypt = Convert.FromBase64String(spell.spell);

    if (spell.key.Length < 16)
    {
        int difference = 16 - spell.key.Length;
        for (int i = 0; i < difference; i++)
        {
            spell.key += "0";
        }
    }
    if (spell.iv.Length < 16)
    {
        int difference = 16 - spell.iv.Length;
        for (int i = 0; i < difference; i++)
        {
            spell.iv += "0";
        }
    }

    //Create decryptor
    Aes aes = new AesManaged();
    aes.Key = Encoding.UTF8.GetBytes(spell.key);
    aes.IV = Encoding.UTF8.GetBytes(spell.iv);
    aes.Mode = CipherMode.CBC;
    var cipher = aes.CreateDecryptor();

    //Decrypt
    byte[] base_enc = cipher.TransformFinalBlock(spellToDecrypt, 0, spellToDecrypt.Length);


    //Send solution
    Answer answer = new Answer();
    answer.answer = Encoding.UTF8.GetString(base_enc);
    response = await client.PostAsync("ministery-of-magic", 
        new StringContent(
            JsonSerializer.Serialize(answer), 
            Encoding.UTF8, 
            "application/json"
            ));
    response.EnsureSuccessStatusCode();

    response.Content.ReadAsStringAsync();
}

public class Spell
{
    public string key { get; set; }
    public string iv { get; set; }
    public string spell { get; set; }
}

public class Answer
{
    public string answer { get; set; }
}