using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public abstract class FireAction : NetworkBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int startAmmunition = 20;
    protected string bulletCount = string.Empty;

    protected Queue<GameObject> bullets = new Queue<GameObject>();
    protected Queue<GameObject> ammunition = new Queue<GameObject>();
    protected bool reloading = false;

    public string BulletCount => bulletCount;
    protected virtual void Start()
    {
        if (!isServer)
        {
            return;
        }

        for (var i = 0; i < startAmmunition; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            NetworkServer.Spawn(bullet);
            //bullet.SetActive(false);
            RpcEnqueueAmmunition(bullet);
        }
    }

    [ClientRpc]
    private void RpcEnqueueAmmunition(GameObject bullet)
    {
        if (hasAuthority)
        {
            ammunition.Enqueue(bullet);
        }
       
    }

    public virtual async void Reloading()
    {
        bullets = await Reload();
    }
    protected virtual void Shooting()
    {
        if (bullets.Count == 0)
        {
            Reloading();
        }
    }

    private async Task<Queue<GameObject>> Reload()
    {
        if (!reloading)
        {
            reloading = true;
            StartCoroutine(ReloadingAnim());
            return await Task.Run(delegate
            {
                var cage = 10;
                if (bullets.Count < cage)
                {
                    Thread.Sleep(3000);
                    var bullets = this.bullets;
                    while (bullets.Count > 0)
                    {
                        ammunition.Enqueue(bullets.Dequeue());
                    }
                    cage = Mathf.Min(cage, ammunition.Count);
                    if (cage > 0)
                    {
                        for (var i = 0; i < cage; i++)
                        {
                            var sphere = ammunition.Dequeue();
                            bullets.Enqueue(sphere);
                        }
                    }
                }
                reloading = false;
                return bullets;
            });
        }
        else
        {
            return bullets;
        }
    }
    private IEnumerator ReloadingAnim()
    {
        while (reloading)
        {
            bulletCount = " | ";
            yield return new WaitForSeconds(0.01f);
            bulletCount = @" \ ";
            yield return new WaitForSeconds(0.01f);
            bulletCount = "---";
            yield return new WaitForSeconds(0.01f);
            bulletCount = " / ";
            yield return new WaitForSeconds(0.01f);
        }
        bulletCount = bullets.Count.ToString();
        yield return null;
    }
}
