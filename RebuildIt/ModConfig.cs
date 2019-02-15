namespace RebuildIt
{
    [ConfigurationPath("RebuildItConfig.xml")]
    public class ModConfig
    {
        public bool ConfigUpdated { get; set; }
        public int Interval { get; set; } = 1;
        public int MaxBuildingsPerInterval { get; set; } = 32;
        public bool IgnoreSearchingForSurvivors { get; set; } = true;
        public bool IgnoreRebuildingCost { get; set; } = true;

        private static ModConfig instance;

        public static ModConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Configuration<ModConfig>.Load();
                }

                return instance;
            }
        }

        public void Save()
        {
            Configuration<ModConfig>.Save();
            ConfigUpdated = true;
        }
    }
}