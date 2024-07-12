using System.Collections;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    [FoldoutGroup("Weapon Stats")]
    public int damage = 10;
    [FoldoutGroup("Weapon Stats")]
    public float fireRate = 0.5f;
    [FoldoutGroup("Weapon Stats")]
    public float range = 100f;

    [FoldoutGroup("Ammo Stats")]
    public int ammo = 15;
    [FoldoutGroup("Ammo Stats")]
    public int maxAmmo = 15;
    [FoldoutGroup("Ammo Stats")]
    public float reloadTime = 1.5f;
    [FoldoutGroup("State"), ReadOnly]
    public bool isReloading = false;

    [FoldoutGroup("State"), ReadOnly]
    public bool isShooting = false;

    [SerializeField] private WeaponData mainWeap;
    [SerializeField] private WeaponData pistolWeap;
    [SerializeField] private WeaponData meleeWeap;
    [SerializeField] private WeaponData Bomb;
    public Animator anim;
    public Recoil recoil;
    public Camera cam;
    public ParticleSystem shootVFX;

    private void Update()
    {
        if (Input.GetButtonDown("Fire1") && !isReloading && ammo > 0)
        {
            recoil.ApplyRecoil();
            Shoot();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            anim.SetTrigger("Inspect");
        }

        if (Input.GetMouseButtonDown(1))
        {
            anim.SetBool("Aiming", true);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            anim.SetBool("Aiming", false);
        }
    }

    public void Shoot()
    {
        if (ammo > 0)
        {
            ammo--;
            isShooting = true;
            StartCoroutine(Cor_Shoot());
        }
        else
        {
            NoticeReload();
        }
    }

    private void NoticeReload()
    {
        Debug.Log("Reload");
    }

    public void Reload()
    {
        if (ammo < maxAmmo)
        {
            isReloading = true;
            anim.SetTrigger("Reload");
            StartCoroutine(Cor_Reload());
        }
    }

    private IEnumerator Cor_Reload()
    {
        yield return new WaitForSeconds(reloadTime);
        isReloading = false;
        ammo = maxAmmo;
    }

    private IEnumerator Cor_Shoot()
    {
        anim.SetTrigger("Shoot");
        PlayVFX(shootVFX);
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        RaycastHit hit;
        Debug.DrawRay(cam.transform.position, cam.transform.forward * 10, Color.red, 0.5f);

        if (Physics.Raycast(ray.origin, ray.direction, out hit, range))
        {
            HandleHit(hit);
        }

        yield return new WaitForSeconds(fireRate);
        isShooting = false;
    }

    private void HandleHit(RaycastHit hit)
    {
        var target = hit.transform.gameObject.GetComponent<StatusManager>();
        var view = hit.transform.gameObject.GetComponent<PhotonView>();
        if (target != null && !view.IsMine)
        {
            target.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage);

            VFXManager.Instance.PlayHitVFX(hit.point);
        }
        else if (target != null && view.IsMine)
        {
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            RaycastHit[] hitPoints = Physics.RaycastAll(ray.origin, ray.direction, range);
            Vector3 lastHit = Vector3.zero;
            lastHit = hitPoints[hitPoints.Length - 1].point;
            VFXManager.Instance.PlayHitOtherVFX(lastHit);
        }
        else
        {
            VFXManager.Instance.PlayHitOtherVFX(hit.point);
        }
    }

    private void PlayVFX(ParticleSystem particle)
    {
        particle.Stop();
        particle.Play();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 pointForward = cam.transform.position;
        pointForward = new Vector3(pointForward.x, pointForward.y - 0.1f, pointForward.z);
        Gizmos.DrawRay(pointForward, cam.transform.forward * range);
    }
}
