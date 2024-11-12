using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RobotManager.Classes
{
    public class Game
    {
        private int id;
        private DateTime date;
        private string against = String.Empty;
        private Field field;
        private bool home;

        [JsonPropertyName("id")]
        public int Id { get => id; set => id = value; }
        [JsonPropertyName("date")]
        public DateTime Date { get => date; set => date = value; }
        [JsonPropertyName("against")]
        public string Against { get => against; set => against = value; }
        [JsonPropertyName("field")]
        public Field Field { get => field; set => field = value; }
        [JsonPropertyName("home")]
        public bool Home { get => home; set => home = value; }


        public async Task<bool> Post()
        {
            Game game = this;
            using HttpClient client = new();
            string apiUrl = "https://skakominor.de/api/RobotManager/CreateEdit/game";
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(apiUrl, game);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Get()
        {
            using HttpClient client = new();
            string apiUrl = $"https://skakominor.de/api/RobotManager/GetSingle/game/{id}";
            try
            {
                Game game = await client.GetFromJsonAsync<Game>(apiUrl);
                if (game != null)
                {
                    this.date = game.date;
                    this.against = game.against;
                    this.field = game.field;
                    this.home = game.home;
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Delete()
        {
            using HttpClient client = new();
            string apiUrl = $"https://skakominor.de/api/RobotManager/Delete/game/{id}";
            try
            {
                HttpResponseMessage response = await client.DeleteAsync(apiUrl);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
    public enum Field
    {
        A,
        B,
        C,
        D
    }
}
