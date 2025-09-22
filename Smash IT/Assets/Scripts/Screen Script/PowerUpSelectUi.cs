using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PowerUpSelectUI : MonoBehaviour
{
    [System.Serializable]
    public class PowerUpEntry
    {
        public PowerUpPickup.PowerUpType powerUpType; // enum instead of string
        public Button button;                         // assign your UI button
        public Outline outline;                       // assign the Outline component
    }

    public List<PowerUpEntry> powerUpButtons;
    public MatchSettings matchSettings;

    private void Start()
    {
        // Reset allowed powerups each time you open the menu
        matchSettings.allowedPowerUps.Clear();

        foreach (var entry in powerUpButtons)
        {
            // make sure the outline starts disabled
            if (entry.outline != null)
                entry.outline.enabled = false;

            // add click listener
            entry.button.onClick.AddListener(() => OnPowerUpClicked(entry));
        }
    }

    private void OnPowerUpClicked(PowerUpEntry entry)
    {
        if (matchSettings.allowedPowerUps.Contains(entry.powerUpType))
        {
            // deselect
            matchSettings.allowedPowerUps.Remove(entry.powerUpType);
            if (entry.outline != null)
                entry.outline.enabled = false;
        }
        else
        {
            // enforce limit
            if (matchSettings.allowedPowerUps.Count >= matchSettings.maxPowerUps)
            {
                Debug.Log("Reached max power-up limit!");
                return;
            }

            // select
            matchSettings.allowedPowerUps.Add(entry.powerUpType);
            if (entry.outline != null)
                entry.outline.enabled = true;
        }
    }
}
