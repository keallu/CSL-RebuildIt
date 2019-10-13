using ColossalFramework.UI;
using System;
using UnityEngine;

namespace RebuildIt
{
    class ModManager : MonoBehaviour
    {
        private bool _initialized;

        private UISlicedSprite _tsBar;
        private UIMultiStateButton _bulldozerUndergroundToggle;
        private UISprite _happiness;
        private UITextureAtlas _textureAtlas;
        private UICheckBox _rebuildButton;
        private UILabel _rebuildCounter;
        private UIButton _rebuildStatistics;

        public void Start()
        {
            try
            {
                if (_tsBar == null)
                {
                    _tsBar = GameObject.Find("TSBar").GetComponent<UISlicedSprite>();
                }

                if (_bulldozerUndergroundToggle == null)
                {
                    _bulldozerUndergroundToggle = GameObject.Find("BulldozerUndergroundToggle").GetComponent<UIMultiStateButton>();
                }

                if (_happiness == null)
                {
                    _happiness = GameObject.Find("Happiness").GetComponent<UISprite>();
                }

                _textureAtlas = LoadResources();

                CreateUI();
            }
            catch (Exception e)
            {
                Debug.Log("[Rebuild It!] ModManager:Start -> Exception: " + e.Message);
            }
        }

        public void Update()
        {
            try
            {
                if (!_initialized || ModConfig.Instance.ConfigUpdated)
                {
                    UpdateUI();

                    _initialized = true;
                    ModConfig.Instance.ConfigUpdated = false;
                }

                if (_bulldozerUndergroundToggle.isVisible)
                {
                    _rebuildButton.isVisible = true;

                    if (ModConfig.Instance.ShowCounters)
                    {
                        UpdateCounters();
                    }
                }
                else
                {
                    _rebuildButton.isVisible = false;
                }

                if (ModConfig.Instance.ShowStatistics)
                {
                    UpdateStatistics();
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Rebuild It!] ModManager:Update -> Exception: " + e.Message);
            }
        }

        public void OnDestroy()
        {
            try
            {
                if (_rebuildStatistics != null)
                {
                    Destroy(_rebuildStatistics);
                }
                if (_rebuildCounter != null)
                {
                    Destroy(_rebuildCounter);
                }
                if (_rebuildButton != null)
                {
                    Destroy(_rebuildButton);
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Rebuild It!] ModManager:OnDestroy -> Exception: " + e.Message);
            }
        }

        private UITextureAtlas LoadResources()
        {
            try
            {
                if (_textureAtlas == null)
                {
                    string[] spriteNames = new string[]
                    {
                        "Rebuild",
                        "RebuildFocused",
                        "RebuildCounter"
                    };

                    _textureAtlas = ResourceLoader.CreateTextureAtlas("RebuildItAtlas", spriteNames, "RebuildIt.Icons.");

                    UITextureAtlas defaultAtlas = ResourceLoader.GetAtlas("Ingame");
                    Texture2D[] textures = new Texture2D[]
                    {
                        defaultAtlas["OptionBase"].texture,
                        defaultAtlas["OptionBaseFocused"].texture,
                        defaultAtlas["OptionBaseHovered"].texture,
                        defaultAtlas["OptionBasePressed"].texture,
                        defaultAtlas["OptionBaseDisabled"].texture
                    };

                    ResourceLoader.AddTexturesInAtlas(_textureAtlas, textures);
                }

                return _textureAtlas;
            }
            catch (Exception e)
            {
                Debug.Log("[Rebuild It!] ModManager:LoadResources -> Exception: " + e.Message);
                return null;
            }
        }

        private void CreateUI()
        {
            try
            {
                _rebuildButton = UIUtils.CreateButtonCheckBox(_tsBar, "RebuildItRebuild", _textureAtlas, "Rebuild", "Toggle Automatic Rebuilding of Buildings", ModConfig.Instance.RebuildBuildings);
                _rebuildButton.relativePosition = new Vector3(_bulldozerUndergroundToggle.relativePosition.x - 40f, _bulldozerUndergroundToggle.relativePosition.y);
                _rebuildButton.eventCheckChanged += (component, value) =>
                {
                    ModConfig.Instance.RebuildBuildings = value;
                    ModConfig.Instance.Save();
                };

                _rebuildCounter = UIUtils.CreateCounterLabel(_rebuildButton, "RebuildItRebuildCounter", "0", _textureAtlas, "RebuildCounter");
                _rebuildCounter.relativePosition = new Vector3(0f, -12f);

                _rebuildStatistics = UIUtils.CreateInfoButton(_happiness, "RebuildItStatistics", "ToolbarIconBulldozerPipes", "Automatic rebuilt buildings");
                _rebuildStatistics.size = new Vector2(100f, 26f);
                _rebuildStatistics.relativePosition = new Vector3(31f, 0f);
            }
            catch (Exception e)
            {
                Debug.Log("[Rebuild It!] ModManager:CreateUI -> Exception: " + e.Message);
            }
        }

        private void UpdateUI()
        {
            try
            {
                if (ModUtils.IsModEnabled("bulldozeit"))
                {
                    _rebuildButton.relativePosition = new Vector3(_bulldozerUndergroundToggle.relativePosition.x - 200f, _bulldozerUndergroundToggle.relativePosition.y);
                    _rebuildStatistics.relativePosition = new Vector3(31f + 105f, 0f);
                }

                UpdateAllButtonCheckBoxes();

                if (ModConfig.Instance.ShowCounters)
                {
                    _rebuildCounter.isVisible = true;
                }
                else
                {
                    _rebuildCounter.isVisible = false;
                }

                _rebuildStatistics.isVisible = ModConfig.Instance.ShowStatistics ? true : false;
            }
            catch (Exception e)
            {
                Debug.Log("[Rebuild It!] ModManager:UpdateUI -> Exception: " + e.Message);
            }
        }

        private void UpdateAllButtonCheckBoxes()
        {
            try
            {
                UpdateButtonCheckBox(_rebuildButton, "Rebuild", ModConfig.Instance.RebuildBuildings);
            }
            catch (Exception e)
            {
                Debug.Log("[Rebuild It!] ModManager:UpdateAllButtonCheckBoxes -> Exception: " + e.Message);
            }
        }

        private void UpdateButtonCheckBox(UICheckBox checkBox, string spriteName, bool state)
        {
            try
            {
                UIButton button;

                if (state)
                {
                    checkBox.isChecked = true;
                    button = checkBox.GetComponentInChildren<UIButton>();
                    button.normalBgSprite = "OptionBaseFocused";
                    button.normalFgSprite = spriteName + "Focused";
                }
                else
                {
                    checkBox.isChecked = false;
                    button = checkBox.GetComponentInChildren<UIButton>();
                    button.normalBgSprite = "OptionBase";
                    button.normalFgSprite = spriteName;
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Rebuild It!] ModManager:UpdateButtonCheckBox -> Exception: " + e.Message);
            }
        }

        private void UpdateCounters()
        {
            try
            {
                _rebuildCounter.text = Statistics.Instance.BuildingsRebuilt.ToString();
            }
            catch (Exception e)
            {
                Debug.Log("[Rebuild It!] ModManager:UpdateCounters -> Exception: " + e.Message);
            }
        }

        private void UpdateStatistics()
        {
            try
            {
                _rebuildStatistics.text = Statistics.Instance.BuildingsRebuilt.ToString();
            }
            catch (Exception e)
            {
                Debug.Log("[Rebuild It!] ModManager:UpdateStatistics -> Exception: " + e.Message);
            }
        }
    }
}