using System.Collections.Generic;
using UnityEngine;

public enum BuildingTypes {
    FARM,
    SHOP,
    HOUSE
}

public static class BuildingSensor {
    private static HashSet<Farm> farms = new HashSet<Farm>();
    private static HashSet<Shop> shops = new HashSet<Shop>();
    private static HashSet<House> houses = new HashSet<House>();

    public static void AddBuilding(Building building) {
        if (building is Farm) {
            farms.Add(building as Farm);
        } else if (building is Shop) {
            shops.Add(building as Shop);
        } else if (building is House) {
            houses.Add(building as House);
        }
    }
    
    public static Building FindNearest(BuildingTypes buildingType, Transform transform) {
        if (buildingType == BuildingTypes.FARM) {
            Farm closestFarm = null;
            foreach (Farm farm in farms) {
                if (closestFarm == null) {
                    closestFarm = farm;
                } else {
                    Vector3 vectorToClosestFarm = transform.position - closestFarm.transform.position;
                    Vector3 vectorToFarm = transform.position - farm.transform.position;
                    if (vectorToFarm.magnitude < vectorToClosestFarm.magnitude) {
                        closestFarm = farm;
                    }
                }
            }
            return closestFarm;
        } else if (buildingType == BuildingTypes.SHOP) {
            Shop closestShop = null;
            foreach (Shop shop in shops) {
                if (closestShop == null) {
                    closestShop = shop;
                } else {
                    Vector3 vectorToClosestShop = transform.position - closestShop.transform.position;
                    Vector3 vectorToShop = transform.position - shop.transform.position;
                    if (vectorToShop.magnitude < vectorToClosestShop.magnitude) {
                        closestShop = shop;
                    }
                }
            }
            return closestShop;
        }
        return null;
    }
}