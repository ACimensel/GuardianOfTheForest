using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
  public Slider slider;

  public void InitializeHealthBar(int maxHealth, int currentHealth) {
    slider.maxValue = maxHealth;
    slider.value = currentHealth;
  } 

  public void SetHealth(int health) {
    slider.value = health;
  }
}
