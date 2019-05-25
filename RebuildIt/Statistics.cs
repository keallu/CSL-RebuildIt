namespace RebuildIt
{
    public class Statistics
    {
        public int AbandonedBuildingsRebuilt { get; set; }
        public int BurnedDownBuildingsRebuilt { get; set; }
        public int CollapsedBuildingsRebuilt { get; set; }
        public int FloodedBuildingsRebuilt { get; set; }

        public int BuildingsRebuilt => AbandonedBuildingsRebuilt + BurnedDownBuildingsRebuilt + CollapsedBuildingsRebuilt + FloodedBuildingsRebuilt;

        private static Statistics instance;

        public static Statistics Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Statistics();
                }

                return instance;
            }
        }
    }
}
