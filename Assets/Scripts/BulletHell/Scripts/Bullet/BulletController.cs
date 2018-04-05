using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour 
{
    public List<BulletManager.Bullet> bulletList = new List<BulletManager.Bullet>();   

    public void AddBulletToList() { bulletList.Add(new BulletManager.Bullet()); }
    public void DelBulletToList() 
    {
        if (bulletList.Count == 0) return;
        bulletList.RemoveAt(bulletList.Count - 1); 
    }
}
