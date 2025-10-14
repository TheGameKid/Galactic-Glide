using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHealthBar : MonoBehaviour
{
    public Image HealthBar;
    public Image Fill;
    public Image Background;
    public Player player;
    public float currentAmmo = 100;
    public int maximumAmmo = 100;
    /// <summary>
    /// Sets the enemy active on the field and shows the enemy HP;
    /// </summary>
    void Start()
    {
        
        currentAmmo = maximumAmmo;

    }

    // Update is called once per frame
    /// <summary>
    /// The health bar will always look at the camera
    /// When the enemy health is 0, the enemy will disappear
    /// </summary>
    void Update()
    {

        UpdateHealth((float)currentAmmo / (float)maximumAmmo);



    }


    public void DecreaseBar(int damage)
    {
        currentAmmo -= damage;
        UpdateHealth((float)currentAmmo / (float)maximumAmmo);
    }
   
    /// <summary>
    /// This will update the health bar to match the current health
    /// </summary>
    public void UpdateHealth(float fraction)
    {
        HealthBar.fillAmount = fraction;
    }
}
