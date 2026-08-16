using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

[System.Serializable]
public class MapLocation
{
    public string locationKey;      // unique save key, e.g. "Forest"
    public Button locationButton;
    public Image bannerImage;       // "conquered" banner, child of button
    public Image lockImage;         // padlock icon, child of button
    public Vector2 playerStandPosition; // where the player sprite moves to when this is selected
    public bool isImplemented;      // false = "Coming Soon" even if selectable
    public string battleSceneName = "1_ForestBattle"; // scene to load when starting this location
}

public class CampaignMap : MonoBehaviour
{
    public MapLocation[] locations;
    public RectTransform playerIcon;
    public Button startButton;
    public GameObject comingSoonPanel;
    public TMP_Text comingSoonText;

    private MapLocation selectedLocation;

    void Start()
    {
        RefreshLocationStates();
        startButton.interactable = false;
        if (comingSoonPanel != null) comingSoonPanel.SetActive(false);
        
    }

    void RefreshLocationStates()
    {
        for (int i = 0; i < locations.Length; i++)
        {
            var loc = locations[i];
            bool completed = PlayerPrefs.GetInt($"Location_{loc.locationKey}_Completed", 0) == 1;

            // First location is always unlocked; every other unlocks once the PREVIOUS one is completed
            bool unlockedByProgress = (i == 0) || PlayerPrefs.GetInt($"Location_{locations[i - 1].locationKey}_Completed", 0) == 1;

            loc.locationButton.interactable = unlockedByProgress;
            if (loc.lockImage != null) loc.lockImage.gameObject.SetActive(!unlockedByProgress);
            if (loc.bannerImage != null) loc.bannerImage.gameObject.SetActive(completed);
        }
    }

    // Hook each location button's OnClick to this, passing its own key —
    // easiest done via a small per-button wrapper method OR by using the button's index.
    // Simplest: one method per real location for now, matching your existing pattern.
    public void SelectLocation(string locationKey)
    {
       MapLocation loc = System.Array.Find(locations, l => l.locationKey == locationKey);
        Debug.Log($"Clicked: {locationKey} → Found: {loc?.locationKey}, Target Position: {loc?.playerStandPosition}");
        if (loc == null || !loc.locationButton.interactable) return;

        selectedLocation = loc;
        StartCoroutine(MovePlayerTo(loc.playerStandPosition));
        startButton.interactable = true;
    }

    IEnumerator MovePlayerTo(Vector2 targetPos)
    {
        Vector2 start = playerIcon.anchoredPosition;
        float duration = 0.5f;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            playerIcon.anchoredPosition = Vector2.Lerp(start, targetPos, t / duration);
            yield return null;
        }
        playerIcon.anchoredPosition = targetPos;
    }

        public void OnStartClicked()
    {
        if (selectedLocation == null) return;

        bool hasValidScene = selectedLocation.isImplemented && !string.IsNullOrEmpty(selectedLocation.battleSceneName);

        if (!hasValidScene)
        {
            if (comingSoonPanel != null && comingSoonText != null)
            {
                comingSoonText.text = $"{selectedLocation.locationKey} is coming in a future update!";
                comingSoonPanel.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"{selectedLocation.locationKey} has no battle scene assigned, and comingSoonPanel/Text isn't wired in the Inspector.");
            }
            return;
        }

        GameData.Instance.currentLocationKey = selectedLocation.locationKey;
        SceneManager.LoadScene(selectedLocation.battleSceneName);
    }

    public void OnBack()
    {
        SceneManager.LoadScene("1_CharacterSelect");
    }
}