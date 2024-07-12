using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public int damage;
    public float fireRate;
    public float range;
    public int maxAmmo;
    public float reloadTime;
    public GameObject weaponPrefab;
    public GameObject ammoPrefab;
    public Animator weaponAnim;

}
public enum WeaponType
{
    MAIN,
    PISTOL,
    MELEE,
    BOMB,
}