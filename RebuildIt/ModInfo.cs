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
            "End of Year"
        };

        private static readonly int[] IntervalValues =
        {
            1,
            2,
            3
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
            group.AddDropdown("Interval (in game time)", IntervalLabels, selectedIndex, sel =>
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