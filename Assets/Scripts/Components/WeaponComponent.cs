using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct WeaponComponent : IComponentData
{
    [SerializeField] public Weapon currentWeapon;
    public bool shooting;
}

[Serializable]
public struct Weapon : IComponentData
{
    public bool friendlyFire;
    public float fireDelay;
    public float realoadingTime;
    public float bulletSpeed;
    public int shots;

    public Weapon(int shots, bool friendlyFire, float fireRate, float bulletSpeed, float realoadingTime = 0)
    {
        this.friendlyFire = friendlyFire;
        this.shots = shots;
        this.fireDelay = fireRate;
        this.realoadingTime = 0;
        this.bulletSpeed = bulletSpeed;
    }
}