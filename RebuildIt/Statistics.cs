namespace RebuildIt
{
    public class Statistics
    {
        public int BurnedDownBuildingsRebuilt { get; set; }
        public int CollapsedBuildingsRebuilt { get; set; }

        public int BuildingsRebuilt => BurnedDownBuildingsRebuilt + CollapsedBuildingsRebuilt;

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
