using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using static UnityModManagerNet.UnityModManager;

namespace SmallTricks.Config
{

    public interface IUpdatableSettings
    {
        void OverrideSettings(IUpdatableSettings userSettings);
        void Init();
    }




    class ModSettings
    {
        public static ModEntry ModEntry;

        public static string modpath;

        public static Blueprints Blueprints;
        private static string UserConfigFolder => ModEntry.Path + "UserSettings";

        private static JsonSerializerSettings cachedSettings;
        private static JsonSerializerSettings SerializerSettings
        {
            get
            {
                if (cachedSettings == null)
                {
                    cachedSettings = new JsonSerializerSettings
                    {
                        CheckAdditionalContent = false,
                        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                        DefaultValueHandling = DefaultValueHandling.Include,
                        FloatParseHandling = FloatParseHandling.Double,
                        Formatting = Formatting.Indented,
                        MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        NullValueHandling = NullValueHandling.Include,
                        ObjectCreationHandling = ObjectCreationHandling.Replace,
                        StringEscapeHandling = StringEscapeHandling.Default,
                    };
                }
                return cachedSettings;
            }
        }

        public static void LoadAllSettings()
        {
            modpath = ModEntry.Path;
            // LoadSettings("AddedContent.json", ref AddedContent);
            LoadSettings("Blueprints.json", ref Blueprints);
        }
        private static void LoadSettings<T>(string fileName, ref T setting) where T : IUpdatableSettings
        {
            Main.Log("Entering Load Settings.");
            JsonSerializer serializer = JsonSerializer.Create(SerializerSettings);

            var assembly = Assembly.GetExecutingAssembly();

            var resourcePath = $"SmallTricks.Config.{fileName}";

            var userPath = $"{UserConfigFolder}{Path.DirectorySeparatorChar}{fileName}";

            Directory.CreateDirectory(UserConfigFolder);
            using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
            using (StreamReader streamReader = new StreamReader(stream))
            using (JsonReader jsonReader = new JsonTextReader(streamReader))
            {
                setting = serializer.Deserialize<T>(jsonReader);
                setting.Init();
            }

            if (File.Exists(userPath))
            {
                using (StreamReader streamReader = File.OpenText(userPath))
                using (JsonReader jsonReader = new JsonTextReader(streamReader))
                {
                    try
                    {
                        T userSettings = serializer.Deserialize<T>(jsonReader);
                        setting.OverrideSettings(userSettings);
                    }
                    catch
                    {
                        Main.Error("Failed to load user settings. Settings will be rebuilt.");
                        try { File.Copy(userPath, UserConfigFolder + $"{Path.DirectorySeparatorChar}BROKEN_{fileName}", true); } catch { Main.Error("Failed to archive broken settings."); }
                    }
                }
            }
            SaveSettings(fileName, setting);
        }

        public static void SaveSettings(string fileName, object setting)
        {
            Directory.CreateDirectory(UserConfigFolder);
            var userPath = $"{UserConfigFolder}{Path.DirectorySeparatorChar}{fileName}";

            JsonSerializer serializer = JsonSerializer.Create(SerializerSettings);
            using (StreamWriter streamWriter = new StreamWriter(userPath))
            using (JsonWriter jsonWriter = new JsonTextWriter(streamWriter))
            {
                serializer.Serialize(jsonWriter, setting);
            }


        }





    }



}
