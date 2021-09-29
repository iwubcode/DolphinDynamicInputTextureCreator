using DolphinDynamicInputTextureCreator.Models.Suggestions;
using DolphinDynamicInputTextureCreator.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace DolphinDynamicInputTextureCreator.Models
{
    public static class DefaultData
    {
        public static DevicesSuggestions Suggestions = OpenSettings<DevicesSuggestions>(DefaultSettingsFilePath);

        public static DynamicInputPackViewModel NewInputPack() => OpenSettings<DynamicInputPackViewModel>(DefaultPackFilePath);

        #region Path

        private static string DefaultSettingsFilePath => GetSettingsFilePath("Dit_StartupSettings.json");

        private static string DefaultPackFilePath => GetSettingsFilePath("Dit_StartupPack.dit");

        private static string GetSettingsFilePath(string filename)
        {
            string settings_path = Assembly.GetExecutingAssembly().Location;
            settings_path = Path.GetDirectoryName(settings_path);
            settings_path = Path.Combine(settings_path, filename);
            return settings_path;
        }

        #endregion


        public static void SaveInputPack(DynamicInputPackViewModel InputPack)
        {
            File.WriteAllText(DefaultPackFilePath, JsonConvert.SerializeObject(InputPack, Formatting.Indented));
        }

        public static void SaveSettings()
        {
            File.WriteAllText(DefaultSettingsFilePath, JsonConvert.SerializeObject(Suggestions, Formatting.Indented));
        }

        private static T OpenSettings<T>(string filepath) where T : new()
        {
            if (!File.Exists(filepath))
                return new T();

            string input = File.ReadAllText(filepath);
            var settings = new JsonSerializerSettings() { ObjectCreationHandling = ObjectCreationHandling.Replace };
            return JsonConvert.DeserializeObject<T>(input, settings) ?? new T();
        }

    }
}
