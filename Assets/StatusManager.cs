using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusManager : MonoBehaviour
{
    [TitleGroup("UI Elements")]
    [FoldoutGroup("UI Elements/Health", false)]
    [LabelText("Health Display")]
    public TextMeshProUGUI healthDisplay;

    [FoldoutGroup("UI Elements/Health")]
    [LabelText("Health Slider")]
    public Slider sliderHealth;

    [FoldoutGroup("UI Elements/Armor", false)]
    [LabelText("Armor Display")]
    public TextMeshProUGUI armorDisplay;

    [FoldoutGroup("UI Elements/Armor")]
    [LabelText("Armor Slider")]
    public Slider sliderArmor;

    [TitleGroup("Status Values")]
    public int maxStatus = 100;
    [BoxGroup("Status Values/Health")]
    [LabelText("Max Health")]
    public int maxHealth = 100;

    [BoxGroup("Status Values/Health")]
    [LabelText("Current Health")]
    public int health;

    [BoxGroup("Status Values/Armor")]
    [LabelText("Max Armor")]
    public int maxArmor = 100;

    [BoxGroup("Status Values/Armor")]
    [LabelText("Current Armor")]
    public int armor;

    public void Start()
    {
        health = 100;
        armor = 100;

        UpdateGUI();
    }

    public void Healing(int heal)
    {
        this.health += heal;
    }

    public void SuitArmor(int armor)
    {
        this.armor += armor;
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        if (armor > 0)
        {
            armor -= damage;
            if (armor < 0)
            {
                health += armor;
            }
        }
        else
        {
            health -= damage;
            if (health < 0)
            {
                Destroy(gameObject);
                return;
            }
        }
        UpdateGUI();
    }

    private void UpdateGUI()
    {
        healthDisplay.text = $"{health}/{maxStatus}";
        armorDisplay.text = $"{armor}/{maxStatus}";

        sliderHealth.maxValue = maxStatus;
        sliderArmor.maxValue = maxStatus;

        sliderHealth.value = health;
        sliderArmor.value = armor;
    }
}
