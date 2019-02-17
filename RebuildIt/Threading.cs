﻿using ColossalFramework;
using ICities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RebuildIt
{
    public class Threading : ThreadingExtensionBase
    {
        private ModConfig _modConfig;
        private Statistics _statistics;
        private SimulationManager _simulationManager;
        private EconomyManager _economyManager;
        private BuildingManager _buildingManager;
        private Building _building;
        private List<ushort> _buildingIds;
        private bool _running;
        private int _cachedInterval;
        private bool _intervalPassed;

        public override void OnCreated(IThreading threading)
        {
            try
            {
                _modConfig = ModConfig.Instance;
                _statistics = Statistics.Instance;
                _simulationManager = Singleton<SimulationManager>.instance;
                _economyManager = Singleton<EconomyManager>.instance;
                _buildingManager = Singleton<BuildingManager>.instance;
                _buildingIds = new List<ushort>();
            }
            catch (Exception e)
            {
                Debug.Log("[Rebuild It!] Threading:OnCreated -> Exception: " + e.Message);
            }
        }

        public override void OnReleased()
        {
            try
            {

            }
            catch (Exception e)
            {
                Debug.Log("[Rebuild It!] Threading:OnReleased -> Exception: " + e.Message);
            }
        }

        public override void OnAfterSimulationTick()
        {
            try
            {
                if (!_running)
                {
                    switch (_modConfig.Interval)
                    {
                        case 1:
                            _intervalPassed = _simulationManager.m_currentGameTime.Day != _cachedInterval ? true : false;
                            _cachedInterval = _simulationManager.m_currentGameTime.Day;
                            break;
                        case 2:
                            _intervalPassed = _simulationManager.m_currentGameTime.Month != _cachedInterval ? true : false;
                            _cachedInterval = _simulationManager.m_currentGameTime.Month;
                            break;
                        case 3:
                            _intervalPassed = _simulationManager.m_currentGameTime.Year != _cachedInterval ? true : false;
                            _cachedInterval = _simulationManager.m_currentGameTime.Year;
                            break;
                        default:
                            break;
                    }
                }

                if (_modConfig.RebuildBuildings && _intervalPassed)
                {
                    _running = true;

                    _intervalPassed = false;

                    _buildingIds.Clear();

                    for (ushort i = 0; i < _buildingManager.m_buildings.m_buffer.Length; i++)
                    {
                        _building = _buildingManager.m_buildings.m_buffer[i];

                        if (!IsRICOBuilding(_building))
                        {
                            if ((_building.m_flags & Building.Flags.BurnedDown) != Building.Flags.None || (_building.m_flags & Building.Flags.Collapsed) != Building.Flags.None)
                            {
                                if (!IsDisasterServiceRequired(_building) && IsRebuildingCostAcceptable(_building))
                                {
                                    if ((_building.m_problems & Notification.Problem.Fire) != Notification.Problem.None)
                                    {
                                        _buildingIds.Add(i);
                                        _statistics.BurnedDownBuildingsRebuilt++;
                                    }
                                    else if ((_building.m_problems & Notification.Problem.StructureDamaged) != Notification.Problem.None || (_building.m_problems & Notification.Problem.StructureVisited) != Notification.Problem.None || (_building.m_problems & Notification.Problem.StructureVisitedService) != Notification.Problem.None)
                                    {
                                        _buildingIds.Add(i);
                                        _statistics.CollapsedBuildingsRebuilt++;
                                    }
                                }
                            }
                        }

                        if (_buildingIds.Count >= _modConfig.MaxBuildingsPerInterval)
                        {
                            break;
                        }
                    }

                    RebuildUtils.RebuildBuildings(_buildingIds);

                    _running = false;
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Rebuild It!] Threading:OnAfterSimulationTick -> Exception: " + e.Message);
                _running = false;
            }
        }

        private bool IsRICOBuilding(Building building)
        {
            bool isRICO = false;

            switch (building.Info.m_class.GetZone())
            {
                case ItemClass.Zone.ResidentialHigh:
                case ItemClass.Zone.ResidentialLow:
                case ItemClass.Zone.Industrial:
                case ItemClass.Zone.CommercialHigh:
                case ItemClass.Zone.CommercialLow:
                case ItemClass.Zone.Office:
                    isRICO = true;
                    break;
                default:
                    isRICO = false;
                    break;
            }

            return isRICO;
        }

        private bool IsDisasterServiceRequired(Building building)
        {
            bool isDisasterServiceRequired = false;

            if (!_modConfig.IgnoreSearchingForSurvivors)
            {
                isDisasterServiceRequired = building.m_levelUpProgress != 255 ? true : false;
            }

            return isDisasterServiceRequired;
        }

        private bool IsRebuildingCostAcceptable(Building building)
        {
            bool isRebuildingCostAcceptable = false;

            if (!_modConfig.IgnoreRebuildingCost)
            {
                int relocationCost = building.Info.m_buildingAI.GetRelocationCost();
                if (_economyManager.PeekResource(EconomyManager.Resource.Construction, relocationCost) == relocationCost)
                {
                    isRebuildingCostAcceptable = true;
                }
            }
            else
            {
                isRebuildingCostAcceptable = true;
            }

            return isRebuildingCostAcceptable;
        }
    }
}