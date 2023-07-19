using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RayShooter : FireAction
{
    private Camera camera;
    protected override void Start()
    {
        base.Start();
        camera = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
            if (Input.GetMouseButtonDown(0))
            {
                Shooting();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                Reloading();
            }
            if (Input.anyKey && !Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
    }

    protected override void Shooting()
    {
            base.Shooting();
            if (bullets.Count > 0)
            {
                StartCoroutine(Shoot());
            }

    }


    private IEnumerator Shoot()
    {

        if (reloading)
        {
            yield break;
        }
        if (hasAuthority)
        {
            var point = new Vector3(camera.pixelWidth / 2, camera.pixelHeight / 2, 0);
            var ray = camera.ScreenPointToRay(point);
            //if (!Physics.Raycast(ray, out var hit))
            //{
            //    yield break;
            //}
            var shoot = bullets.Dequeue();
            bulletCount = bullets.Count.ToString();
            ammunition.Enqueue(shoot);
            //shoot.SetActive(true);
            CmdShoot(shoot, ray.origin, ray.direction);
            //shoot.transform.position = hit.point;
            //shoot.transform.parent = hit.transform;
            yield return new WaitForSeconds(2.0f);
            //shoot.SetActive(false);
        }


    }

    [Command]
    private void CmdShoot(GameObject bullet, Vector3 origin, Vector3 direction)
    {
        var ray = new Ray(origin, direction);
        if (!Physics.Raycast(ray, out var hit))
        {
            return;
        }
        bullet.transform.position = hit.point;
        bullet.transform.parent = hit.transform;
    }
}
