using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int highScore = 0;
    
    public CardData attackCard;
    public CardData powerStrikeCard;
    public CardData blockCard;
    public CardData healCard;
    [Range(0f, 1f)] public float powerStrikeChance = 0.15f;
    public TMP_Text attackButtonLabel;

    public TMP_Text restartButtonLabel; 
    public Button mainMenuBtn;
    public Button surrenderBtn;

    public Image enemyCharacterImage; // drag your single consolidated Image here

    public List<EnemyData> enemies;
    private int currentEnemyIndex = 0;
    private CardData currentAttackCard;
    public Text enemyNameText; // drag EnemyName object here in Inspector

    public GameObject defeatPanel;
    public TMP_Text defeatText;
    public GameObject victoryPanel;
    public TMP_Text victoryText;
    public TMP_Text scoreText; // optional, can leave unassigned if skipping
    public Button continueButton;
    public Button endButton;

    // References
    public Slider playerHealthBar;
    public Slider enemyHealthBar;
    public Text playerHealthText;
    public Text enemyHealthText;
    public Text messageText;
    public Button attackBtn;
    public Button blockBtn;
    public Button healBtn;
    public Button restartBtn;

    public Image playerPanelImage;
    public Image enemyPanelImage;

    public GameObject floatingTextPrefab;
    public Transform playerTextSpawn; // e.g. PlayerHealthBar position
    public Transform enemyTextSpawn;  // e.g. EnemyHealthBar position


    public AudioSource sfxSource; // drag the new AudioSource here
    public AudioClip attackSFX;
    public AudioClip enemyAttackSFX;
    public AudioClip healSFX;
    public AudioClip blockSFX;
    public AudioClip blockedImpactSFX; // plays when an enemy attack is actually absorbed by shield
    public AudioClip victorySFX;
    public AudioClip defeatSFX;
    public AudioClip powerStrikeSFX;

    // Game state
    private int playerMaxHP = 100;
    private int playerHP;
    private int enemyMaxHP = 100;
    private int enemyHP;
    private int enemyAttack = 12;
    private int playerShield = 0;
    private bool isPlayerTurn = true;
    private bool gameOver = false;

    private string currentEnemyName;

    private int winCount = 0;
    private int loopCount = 0;
    private bool endlessMode = false;

    void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        StartGame();

    }

    void StartGame()
    {
        playerHP = playerMaxHP;
        currentEnemyIndex = 0;
        winCount = 0;
        loopCount = 0;
        endlessMode = false;
        LoadEnemy(enemies[currentEnemyIndex]);
        // UpdateEnemyObjects();
        playerShield = 0;
        gameOver = false;
        isPlayerTurn = true;
        
        RollAttackCard();
        UpdateUI();
        messageText.text = $"A {currentEnemyName} appears! {enemies[currentEnemyIndex].introLine}";
        // messageText.text = "Choose your action!";

        attackBtn.interactable = true;
        blockBtn.interactable = true;
        healBtn.interactable = true;
        defeatPanel.SetActive(false);
        surrenderBtn.gameObject.SetActive(true);
        victoryPanel.SetActive(false);
    }

    void RollAttackCard()
    {
        currentAttackCard = (UnityEngine.Random.value < powerStrikeChance) ? powerStrikeCard : attackCard;
        attackButtonLabel.text = currentAttackCard.cardName;
    }
    // Called when player clicks a button
    public void PlayerAttack()
    {
        if (!isPlayerTurn || gameOver) return;
        PerformPlayerAction("attack");
    }

    public void PlayerBlock()
    {
        if (!isPlayerTurn || gameOver) return;
        PerformPlayerAction("block");
    }

    public void PlayerHeal()
    {
        if (!isPlayerTurn || gameOver) return;
        PerformPlayerAction("heal");
    }

    void PlaySFX(AudioClip clip)
        {
            if (clip != null) sfxSource.PlayOneShot(clip);
        }

    void PerformPlayerAction(string action)
    {
        attackBtn.interactable = false;
        blockBtn.interactable = false;
        healBtn.interactable = false;

        if (action == "attack")
        {
            int dmg = currentAttackCard.value;
            enemyHP -= dmg;
            if (enemyHP < 0) enemyHP = 0;
            messageText.text = $"{currentAttackCard.cardName} dealt {dmg} damage!";
            SpawnFloatingText(enemyTextSpawn, $"-{dmg}", Color.red);
            StartCoroutine(FlashPanel(enemyPanelImage, Color.white));
            PlaySFX(currentAttackCard == powerStrikeCard ? powerStrikeSFX : attackSFX);
        }
        else if (action == "block")
        {
            playerShield = blockCard.value;
            PlaySFX(blockSFX);
            messageText.text = "You raise your shield!";
        }
        else if (action == "heal")
        {
            playerHP += healCard.value;
            PlaySFX(healSFX);
            if (playerHP > playerMaxHP) playerHP = playerMaxHP;
            messageText.text = $"You healed {healCard.value} HP!";
            SpawnFloatingText(playerTextSpawn, $"+{healCard.value}", Color.green);
            StartCoroutine(FlashPanel(playerPanelImage, Color.green));
        }

        UpdateUI();

        // Check if enemy died
        if (enemyHP <= 0)
        {
            enemyHP = 0;
            UpdateUI();
            StartCoroutine(NextEnemyRoutine());   
            return;
        }

        // Enemy's turn after a short delay
        isPlayerTurn = false;
        StartCoroutine(EnemyTurn());
    }
    void LoadEnemy(EnemyData data)
        {
            enemyMaxHP = data.maxHP;
            enemyHP = data.maxHP;
            enemyAttack = data.attackDamage;
            currentEnemyName = data.enemyName;
            enemyNameText.text = data.enemyName;

            enemyCharacterImage.sprite = data.enemySprite;
            enemyCharacterImage.rectTransform.sizeDelta = data.displaySize;
        }

    IEnumerator NextEnemyRoutine()
    {
        messageText.text = $"🎉 {currentEnemyName} defeated!";
        winCount++;
        UpdateScoreDisplay();   // <- update immediately, before any branching
        yield return new WaitForSeconds(1.5f);

        currentEnemyIndex++;

        if (currentEnemyIndex >= enemies.Count)
        {
            if (!endlessMode)
            {
                CheckHighScore();
                ShowVictoryPanel();
                yield break;
            }
            currentEnemyIndex = 0;
            loopCount++;
        }

        LoadEnemy(enemies[currentEnemyIndex]);
        if (endlessMode)
        {
            enemyMaxHP += loopCount * 5;
            enemyHP = enemyMaxHP;
            enemyAttack += loopCount * 1;
        }

        UpdateUI();
        RollAttackCard();
        messageText.text = $"A {currentEnemyName} appears! {enemies[currentEnemyIndex].introLine}";
        attackBtn.interactable = true;
        blockBtn.interactable = true;
        healBtn.interactable = true;
        isPlayerTurn = true;
    }


   void ShowVictoryPanel()
    {
        isPlayerTurn = false;
        victoryText.text = $"You have saved the kingdom from the beast horde!\n\nWins: {winCount} (Best: {highScore})\n\nRumors speak of darker foes beyond the border...";
        victoryPanel.SetActive(true);
        PlaySFX(victorySFX);
    }

// Hook to ContinueButton's OnClick in Inspector
public void OnContinueForHighScore()
{
    endlessMode = true;
    victoryPanel.SetActive(false);
    loopCount++;
    currentEnemyIndex = 0;
    LoadEnemy(enemies[currentEnemyIndex]);
    enemyMaxHP += loopCount * 5;
    enemyHP = enemyMaxHP;
    enemyAttack += loopCount * 1;
    UpdateUI();
    RollAttackCard();
    messageText.text = $"A stronger {currentEnemyName} appears! {enemies[currentEnemyIndex].introLine}";
    if (scoreText != null) scoreText.text = $"Wins: {winCount} (Best: {highScore})";
    attackBtn.interactable = true;
    blockBtn.interactable = true;
    healBtn.interactable = true;
    isPlayerTurn = true;
}

// Hook to EndButton's OnClick in Inspector
public void OnEndGameChoice()
{
    victoryPanel.SetActive(false);
    EndGame(true, $"🏆 Thanks for playing! Final wins: {winCount} (Best: {highScore})");
}

    IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(1.0f); // let player read their own action first

        messageText.text = "Enemy is preparing to attack...";
        yield return new WaitForSeconds(1.0f);

        int damage = enemyAttack - playerShield;
        
        SpawnFloatingText(playerTextSpawn, damage > 0 ? $"-{damage}" : "Blocked!", damage > 0 ? Color.red : Color.cyan);
        StartCoroutine(FlashPanel(playerPanelImage, Color.white));
        PlaySFX(damage > 0 ? enemyAttackSFX : blockedImpactSFX); // see #2 below for blockedImpactSFX

        if (damage < 0) damage = 0;
        playerHP -= damage;
        if (playerHP < 0) playerHP = 0;
        playerShield = 0;

        messageText.text = damage > 0 ? $"Enemy attacks for {damage} damage!" : "🛡️ Shield blocked the attack!";
        UpdateUI();

        yield return new WaitForSeconds(1.2f);

        if (playerHP <= 0)
        {
            playerHP = 0;
            UpdateUI();
            EndGame(false, $"GAME OVER! You have fallen in battle.\nFinal Score: {winCount} wins (Best: {highScore})");
            yield break;
        }

        isPlayerTurn = true;   
        RollAttackCard();
        messageText.text = "Your turn! Choose an action.";
        attackBtn.interactable = true;
        blockBtn.interactable = true;
        healBtn.interactable = true;
    }

    void UpdateUI()
    {
        playerHealthBar.maxValue = playerMaxHP;
        enemyHealthBar.maxValue = enemyMaxHP;
        playerHealthText.text = $"HP: {playerHP} / {playerMaxHP}";
        enemyHealthText.text = $"HP: {enemyHP} / {enemyMaxHP}";

        StartCoroutine(SmoothBar(playerHealthBar, playerHP));
        StartCoroutine(SmoothBar(enemyHealthBar, enemyHP));
    }

    IEnumerator SmoothBar(Slider bar, float target)
    {
        float start = bar.value;
        float elapsed = 0f;
        float duration = 0.4f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            bar.value = Mathf.Lerp(start, target, elapsed / duration);
            yield return null;
        }
        bar.value = target;
    }

    void EndGame(bool won, string message)
    {
        gameOver = true;
        isPlayerTurn = false;
        StopAllCoroutines();
        CheckHighScore();

        if (!won) PlaySFX(defeatSFX);

        attackBtn.interactable = false;
        blockBtn.interactable = false;
        healBtn.interactable = false;
        surrenderBtn.gameObject.SetActive(false);

        restartButtonLabel.text = won ? "Play Again" : "Try Again";
        defeatText.text = message;
        defeatPanel.SetActive(true);
    }

    // Called by Restart button
    public void RestartGame()
    {
        StartGame();
    }

 // Add a red flash to whichever panel got hit, and a green flash on heal
    IEnumerator FlashPanel(Image panelImage, Color flashColor)
    {
        Color original = panelImage.color;
        panelImage.color = flashColor;
        yield return new WaitForSeconds(0.15f);
        panelImage.color = original;
    }

    void SpawnFloatingText(Transform spawnPoint, string message, Color color)
    {
        GameObject go = Instantiate(floatingTextPrefab, spawnPoint.position, Quaternion.identity, spawnPoint.root);
        go.GetComponent<FloatingText>().Setup(message, color);
    }

    public void OnSurrender()
    {
        if (gameOver) return;
        EndGame(false, $"You surrendered.\nFinal Score: {winCount} wins (Best: {highScore})");
    }

    public void OnMainMenu()
    {
        // TODO: once a Main Menu scene exists, replace this with:
        SceneManager.LoadScene("MainMenu");
        
    }

    void UpdateScoreDisplay()
    {
        if (scoreText != null) scoreText.text = $"Wins: {winCount} (Best: {highScore})";
    }

    void CheckHighScore()
    {
        if (winCount > highScore)
        {
            highScore = winCount;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
    }

}

