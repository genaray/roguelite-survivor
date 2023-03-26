using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueliteSurvivor.Containers
{
    public class SettingsContainer
    {
        public float MasterVolume { get; set; }
        public float MenuMusicVolume { get; set; }
        public float GameMusicVolume { get; set; }
        public float SoundEffectsVolume { get; set; }

        public SettingsContainer()
        {
            MasterVolume = 0.5f;
            MenuMusicVolume = 1f;
            GameMusicVolume = 1f;
            SoundEffectsVolume = 1f;
        }

        public void Save() 
        {
            var jObject = JObject.FromObject(this);

            File.WriteAllText("settings.json", jObject.ToString());
        }

        public static SettingsContainer ToSettingsContainer(JToken settings)
        {
            var container = new SettingsContainer();
            container.MasterVolume = (float)settings["MasterVolume"];
            container.MenuMusicVolume = (float)settings["MenuMusicVolume"];
            container.GameMusicVolume = (float)settings["GameMusicVolume"];
            container.SoundEffectsVolume = (float)settings["SoundEffectsVolume"];
            return container;
        }
    }
}
