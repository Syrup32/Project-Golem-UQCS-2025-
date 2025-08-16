using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public PlayerHealth playerHealth; // Reference to player health script
    public Image healthFillImage;     // Reference to the fill image

    private void Update()
    {
        if (playerHealth != null && healthFillImage != null)
        {
            float fillAmount = (float)playerHealth.CurrentHealth / playerHealth.maxHealth;
            healthFillImage.fillAmount = fillAmount;
        }
    }
}
