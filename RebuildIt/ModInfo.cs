using ICities;
using System;

namespace RebuildIt
{
    public class ModInfo : IUserMod
    {
        public string Name => "Rebuild It!";
        public string Description => "Allows to automate the rebuilding of buildings.";

        private static readonly string[] IntervalLabels =
        {
            "End of Day",
            "End of Month",
            "End of Year",
            "Every 5 seconds",
            "Every 10 seconds",
            "Every 30 seconds"
        };

        private static readonly int[] IntervalValues =
        {
            1,
            2,
            3,
            4,
            5,
            6
        };

        public void OnSettingsUI(UIHelperBase helper)
        {
            UIHelperBase group;
            bool selected;
            int selectedIndex;
            int selectedValue;
            int result;

            group = helper.AddGroup(Name);

            selectedIndex = GetSelectedOptionIndex(IntervalValues, ModConfig.Instance.Interval);
            group.AddDropdown("Interval", IntervalLabels, selectedIndex, sel =>
            {
                ModConfig.Instance.Interval = IntervalValues[sel];
                ModConfig.Instance.Save();
            });

            selectedValue = ModConfig.Instance.MaxBuildingsPerInterval;
            group.AddTextfield("Max Buildings (per interval)", selectedValue.ToString(), sel =>
            {
                int.TryParse(sel, out result);
                ModConfig.Instance.MaxBuildingsPerInterval = result;
                ModConfig.Instance.Save();
            });

            selected = ModConfig.Instance.IncludeServiceBuildings;
            group.AddCheckbox("Include Service Buildings", selected, sel =>
            {
                ModConfig.Instance.IncludeServiceBuildings = sel;
                ModConfig.Instance.Save();
            });

            selected = ModConfig.Instance.IncludeZonedBuildings;
            group.AddCheckbox("Include Zoned Buildings", selected, sel =>
            {
                ModConfig.Instance.IncludeZonedBuildings = sel;
                ModConfig.Instance.Save();
            });

            selected = ModConfig.Instance.IncludeAbandonedBuildings;
            group.AddCheckbox("Include Abandoned Buildings", selected, sel =>
            {
                ModConfig.Instance.IncludeAbandonedBuildings = sel;
                ModConfig.Instance.Save();
            });

            selected = ModConfig.Instance.IncludeBurnedDownBuildings;
            group.AddCheckbox("Include Burned Down Buildings", selected, sel =>
            {
                ModConfig.Instance.IncludeBurnedDownBuildings = sel;
                ModConfig.Instance.Save();
            });

            selected = ModConfig.Instance.IncludeCollapsedBuildings;
            group.AddCheckbox("Include Collapsed Buildings", selected, sel =>
            {
                ModConfig.Instance.IncludeCollapsedBuildings = sel;
                ModConfig.Instance.Save();
            });

            selected = ModConfig.Instance.IncludeFloodedBuildings;
            group.AddCheckbox("Include Flooded Buildings", selected, sel =>
            {
                ModConfig.Instance.IncludeFloodedBuildings = sel;
                ModConfig.Instance.Save();
            });

            selected = ModConfig.Instance.IgnoreSearchingForSurvivors;
            group.AddCheckbox("Ignore Searching For Survivors", selected, sel =>
            {
                ModConfig.Instance.IgnoreSearchingForSurvivors = sel;
                ModConfig.Instance.Save();
            });

            selected = ModConfig.Instance.IgnoreRebuildingCost;
            group.AddCheckbox("Ignore Rebuilding Cost", selected, sel =>
            {
                ModConfig.Instance.IgnoreRebuildingCost = sel;
                ModConfig.Instance.Save();
            });

            selected = ModConfig.Instance.ShowCounters;
            group.AddCheckbox("Show Counters in Bulldozer Bar", selected, sel =>
            {
                ModConfig.Instance.ShowCounters = sel;
                ModConfig.Instance.Save();
            });

            selected = ModConfig.Instance.ShowStatistics;
            group.AddCheckbox("Show Statistics in Info Panel", selected, sel =>
            {
                ModConfig.Instance.ShowStatistics = sel;
                ModConfig.Instance.Save();
            });
        }

        private int GetSelectedOptionIndex(int[] option, int value)
        {
            int index = Array.IndexOf(option, value);
            if (index < 0) index = 0;

            return index;
        }
    }
}