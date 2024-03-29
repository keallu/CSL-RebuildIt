﻿using ColossalFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RebuildIt
{
    public static class RebuildUtils
    {
        public static void RebuildBuildings(List<ushort> list)
        {
            try
            {
                SimulationManager simulationManager = Singleton<SimulationManager>.instance;

                foreach (ushort buildingId in list)
                {
                    simulationManager.AddAction(RebuildBuilding(buildingId));
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Rebuild It!] RebuildUtils:RebuildBuildings -> Exception: " + e.Message);
            }
        }

        private static IEnumerator RebuildBuilding(ushort buildingId)
        {
            try
            {
                SimulationManager simulationManager = Singleton<SimulationManager>.instance;
                BuildingManager buildingManager = Singleton<BuildingManager>.instance;
                Building building = buildingManager.m_buildings.m_buffer[buildingId];
                BuildingInfo buildingInfo = building.Info;

                if (!IsRICOBuilding(building))
                {
                    int relocationCost = buildingInfo.m_buildingAI.GetRelocationCost();
                    Singleton<EconomyManager>.instance.FetchResource(EconomyManager.Resource.Construction, relocationCost, buildingInfo.m_class);

                    buildingManager.RelocateBuilding(buildingId, building.m_position, building.m_angle);
                    UpdatePublicServiceIndex(buildingInfo);

                    if (buildingInfo.m_subBuildings != null && buildingInfo.m_subBuildings.Length != 0)
                    {
                        Matrix4x4 matrix4x = default(Matrix4x4);
                        matrix4x.SetTRS(building.m_position, Quaternion.AngleAxis(building.m_angle * 57.29578f, Vector3.down), Vector3.one);

                        BuildingInfo subBuildingInfo;
                        Vector3 position;
                        float angle;
                        bool fixedHeight;

                        for (int i = 0; i < buildingInfo.m_subBuildings.Length; i++)
                        {
                            subBuildingInfo = buildingInfo.m_subBuildings[i].m_buildingInfo;
                            position = matrix4x.MultiplyPoint(buildingInfo.m_subBuildings[i].m_position);
                            angle = buildingInfo.m_subBuildings[i].m_angle * ((float)Math.PI / 180f) + building.m_angle;
                            fixedHeight = buildingInfo.m_subBuildings[i].m_fixedHeight;

                            if (buildingManager.CreateBuilding(out ushort subBuildingId, ref simulationManager.m_randomizer, subBuildingInfo, position, angle, 0, simulationManager.m_currentBuildIndex))
                            {
                                if (fixedHeight)
                                {
                                    buildingManager.m_buildings.m_buffer[subBuildingId].m_flags |= Building.Flags.FixedHeight;
                                }
                                simulationManager.m_currentBuildIndex++;
                            }

                            UpdatePublicServiceIndex(subBuildingInfo);

                            if (buildingId != 0 && subBuildingId != 0)
                            {
                                buildingManager.m_buildings.m_buffer[buildingId].m_subBuilding = subBuildingId;
                                buildingManager.m_buildings.m_buffer[subBuildingId].m_parentBuilding = buildingId;
                                buildingManager.m_buildings.m_buffer[subBuildingId].m_flags |= Building.Flags.Untouchable;
                                buildingId = subBuildingId;
                            }
                        }
                    }
                }
                else
                {
                    buildingManager.ReleaseBuilding(buildingId);

                    if (buildingManager.CreateBuilding(out var _, ref simulationManager.m_randomizer, buildingInfo, building.m_position, building.m_angle, building.Width, simulationManager.m_currentBuildIndex))
                    {
                        ZoneManager zoneManager = Singleton<ZoneManager>.instance;

                        simulationManager.m_currentBuildIndex++;

                        switch (buildingInfo.m_class.m_service)
                        {
                            case ItemClass.Service.Residential:
                                zoneManager.m_actualResidentialDemand = Mathf.Max(0, zoneManager.m_actualResidentialDemand - 5);
                                break;
                            case ItemClass.Service.Commercial:
                                zoneManager.m_actualCommercialDemand = Mathf.Max(0, zoneManager.m_actualCommercialDemand - 5);
                                break;
                            case ItemClass.Service.Industrial:
                                zoneManager.m_actualWorkplaceDemand = Mathf.Max(0, zoneManager.m_actualWorkplaceDemand - 5);
                                break;
                            case ItemClass.Service.Office:
                                zoneManager.m_actualWorkplaceDemand = Mathf.Max(0, zoneManager.m_actualWorkplaceDemand - 5);
                                break;
                        }
                    }

                    simulationManager.m_currentBuildIndex++;
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Rebuild It!] RebuildUtils:RebuildBuilding -> Exception: " + e.Message);
            }

            yield return null;
        }

        private static void UpdatePublicServiceIndex(BuildingInfo buildingInfo)
        {
            int publicServiceIndex = ItemClass.GetPublicServiceIndex(buildingInfo.m_class.m_service);
            if (publicServiceIndex != -1)
            {
                Singleton<BuildingManager>.instance.m_buildingDestroyed2.Disable();
                Singleton<GuideManager>.instance.m_serviceNotUsed[publicServiceIndex].Disable();
                Singleton<GuideManager>.instance.m_serviceNeeded[publicServiceIndex].Deactivate();
                Singleton<CoverageManager>.instance.CoverageUpdated(buildingInfo.m_class.m_service, buildingInfo.m_class.m_subService, buildingInfo.m_class.m_level);
            }
        }

        private static bool IsRICOBuilding(Building building)
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
    }
}
