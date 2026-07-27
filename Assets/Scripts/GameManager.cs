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
    public int maxHealsPerFight = 2;
    private int healsRemaining;
    [Range(0f, 1f)] public float potionDropChance = 0.25f;
    public int potionHealAmount = 10;
    private int potionCount = 0;
    public TMP_Text potionCountText; // small label near the Heal card, e.g. "Potions: 0"
    
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
    private bool currentEnemyIsBoss = false;
    private bool inBossFight = false;
    public List<EnemyData> bosses;

    public GameObject defeatPanel;
    public TMP_Text defeatText;
    public GameObject victoryPanel;
    public TMP_Text victoryText;
    public TMP_Text scoreText; // optional, can leave unassigned if skipping
    public Button continueButton;
    public Button endButton;

    public Button potionBtn; // new small button, separate from the 3 main cards

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

// Audio sfx
    public AudioSource sfxSource; // drag the new AudioSource here
    public AudioClip attackSFX;
    public AudioClip enemyAttackSFX;
    public AudioClip healSFX;
    public AudioClip blockSFX;
    public AudioClip blockedImpactSFX; // plays when an enemy attack is actually absorbed by shield
    public AudioClip victorySFX;
    public AudioClip defeatSFX;
    public AudioClip powerStrikeSFX;
    
    //fallback SFX for enemies, in case they don't have their own
    public AudioClip enemyAppearSFX;
    public AudioClip enemyDeathSFX;

    //telegraphing eneny attacks
    [Range(0f, 1f)] public float telegraphChance = 0.25f;
    public float telegraphDamageMultiplier = 1.8f;
    private bool isTelegraphing = false;
    public AudioClip telegraphSFX;


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
        healsRemaining = maxHealsPerFight;
        LoadEnemy(enemies[currentEnemyIndex]);
        // UpdateEnemyObjects();
        playerShield = 0;
        gameOver = false;
        isPlayerTurn = true;
        potionCount = 0;
        UpdatePotionDisplay();
        
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
        if (scoreText != null) scoreText.gameObject.SetActive(true);
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
    if (healsRemaining <= 0)
    {
        messageText.text = "No heals remaining this fight!";
        return;
    }
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
            messageText.text = $"⚔️ {currentAttackCard.cardName} dealt {dmg} damage!";
            SpawnFloatingText(enemyTextSpawn, $"-{dmg}", Color.red);
            StartCoroutine(FlashPanel(enemyPanelImage, Color.white));
            PlaySFX(currentAttackCard == powerStrikeCard ? powerStrikeSFX : attackSFX);

            if (UnityEngine.Random.value < potionDropChance)
            {
                potionCount++;
                UpdatePotionDisplay();
                SpawnFloatingText(playerTextSpawn, "+1 Potion!", Color.cyan);
            }

            RollAttackCard(); // roll the NEXT attack card only now that this one was spent
        }
        else if (action == "block")
        {
            playerShield = blockCard.value;
            PlaySFX(blockSFX);
            messageText.text = "You raise your shield!";
        }
        else if (action == "heal")
        {
            healsRemaining--;
            playerHP += healCard.value;
            if (playerHP > playerMaxHP) playerHP = playerMaxHP;
            messageText.text = $"You healed {healCard.value} HP! ({healsRemaining} heals left)";
            SpawnFloatingText(playerTextSpawn, $"+{healCard.value}", Color.green);
            StartCoroutine(FlashPanel(playerPanelImage, Color.green));
            PlaySFX(healSFX);
        }
        else if (action == "potion")
        {
            playerHP += potionHealAmount;
            if (playerHP > playerMaxHP) playerHP = playerMaxHP;
            messageText.text = $"Potion restored {potionHealAmount} HP!";
            SpawnFloatingText(playerTextSpawn, $"+{potionHealAmount}", Color.cyan);
            StartCoroutine(FlashPanel(playerPanelImage, Color.cyan));
            PlaySFX(healSFX); // reuse for now, distinct clip later
        }

        UpdateUI();

        // Check if enemy died
        if (enemyHP <= 0)
        {
            enemyHP = 0;
            UpdateUI();
            StartCoroutine(NextEnemyRoutine(enemies[currentEnemyIndex])); // pass the enemy that was just defeated
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
        currentEnemyIsBoss = data.isBoss;
        enemyNameText.text = data.isBoss ? $"{data.enemyName}" : data.enemyName;

        enemyCharacterImage.sprite = data.enemySprite;
        enemyCharacterImage.rectTransform.sizeDelta = data.displaySize;

        //play enemy appear SFX, using the enemy's own if available, otherwise fallback to default
       PlaySFX(data.appearSFX != null ? data.appearSFX : enemyAppearSFX);
    }

    IEnumerator NextEnemyRoutine(EnemyData data)
    {
        winCount++;
        if (currentEnemyIsBoss)
        {
            potionCount++;
            UpdatePotionDisplay();
            messageText.text = $"{currentEnemyName} defeated! Bonus potion earned!";
        }
        else
        {
            messageText.text = $"{currentEnemyName} defeated!";
        }
        UpdateScoreDisplay();
        PlaySFX(data.deathSFX != null ? data.deathSFX : enemyDeathSFX);
        yield return new WaitForSeconds(1.5f);

        currentEnemyIndex++;

        if (currentEnemyIndex >= enemies.Count)
        {
            currentEnemyIndex = 0;

            if (!inBossFight)
            {
                // Lap of 3 regular enemies complete — face this lap's boss
                inBossFight = true;
                EnemyData boss = bosses[loopCount % bosses.Count];
                LoadEnemy(boss);
                currentEnemyIsBoss = true;
                isTelegraphing = false;
                healsRemaining = maxHealsPerFight;
                UpdateUI();
                messageText.text = $"A {currentEnemyName} appears! {boss.introLine}";
                attackBtn.interactable = true;
                blockBtn.interactable = true;
                healBtn.interactable = true;
                isPlayerTurn = true;
                yield break;
            }
            else
            {
                // Boss just defeated
                inBossFight = false;
                currentEnemyIsBoss = false;

                if (!endlessMode)
                {
                    CheckHighScore();
                    ShowVictoryPanel();
                    yield break;
                }
                loopCount++;
            }
        }

        EnemyData nextEnemy = enemies[currentEnemyIndex];
        LoadEnemy(nextEnemy);
        isTelegraphing = false;
        healsRemaining = maxHealsPerFight;
        UpdateUI();
        messageText.text = $"A {currentEnemyName} appears! {nextEnemy.introLine}";
        attackBtn.interactable = true;
        blockBtn.interactable = true;
        healBtn.interactable = true;
        isPlayerTurn = true;
    }


    void ShowVictoryPanel()
    {
        isPlayerTurn = false;
        victoryText.text = $"You have saved the kingdom from the beast horde!\n\nWins: {winCount}\n\nRumors speak of darker foes beyond the border...";
        victoryPanel.SetActive(true);
        if (scoreText != null) scoreText.gameObject.SetActive(false);
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
    healsRemaining = maxHealsPerFight;
    enemyMaxHP += loopCount * 5;
    enemyHP = enemyMaxHP;
    enemyAttack += loopCount * 1;
    UpdateUI();
    messageText.text = $"A stronger {currentEnemyName} appears! {enemies[currentEnemyIndex].introLine}";
    if (scoreText != null) scoreText.gameObject.SetActive(true);
    attackBtn.interactable = true;
    blockBtn.interactable = true;
    healBtn.interactable = true;
    isPlayerTurn = true;
    
}

// Hook to EndButton's OnClick in Inspector
public void OnEndGameChoice()
{
    victoryPanel.SetActive(false);
    EndGame(true, $"Thanks for playing! Final wins: {winCount} (Best: {highScore})");
}

    IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(1.0f); // let player read their own action first

        // Decide: normal attack, or wind up a heavy one? 
        float currentTelegraphChance = currentEnemyIsBoss ? enemies[currentEnemyIndex].bossTelegraphChance : telegraphChance;
        bool shouldTelegraph = !isTelegraphing && UnityEngine.Random.value < currentTelegraphChance;
        

        if (shouldTelegraph)
        {
            isTelegraphing = true;
            messageText.text = $"{currentEnemyName} is winding up a heavy attack!";
            PlaySFX(telegraphSFX);
            yield return new WaitForSeconds(1.3f);

            isPlayerTurn = true;
            messageText.text = "Brace yourself! Choose your action.";
            attackBtn.interactable = true;
            blockBtn.interactable = true;
            healBtn.interactable = true;
            yield break; // hand control back — the heavy hit resolves on the NEXT enemy turn
        }

        messageText.text = isTelegraphing ? $"{currentEnemyName} unleashes the heavy attack!" : $"{currentEnemyName} attacks!";
        yield return new WaitForSeconds(1.0f);

        int baseDamage = enemyAttack;
        if (isTelegraphing)
        {
            baseDamage = Mathf.RoundToInt(enemyAttack * telegraphDamageMultiplier);
            isTelegraphing = false;
        }

        int damage = baseDamage - playerShield;
        if (damage < 0) damage = 0;

        SpawnFloatingText(playerTextSpawn, damage > 0 ? $"-{damage}" : "Blocked!", damage > 0 ? Color.red : Color.cyan);
        StartCoroutine(FlashPanel(playerPanelImage, Color.white));
        PlaySFX(damage > 0 ? enemyAttackSFX : blockedImpactSFX);

        playerHP -= damage;
        if (playerHP < 0) playerHP = 0;
        playerShield = 0;

        messageText.text = damage > 0 ? $"Enemy attacks for {damage} damage!" : "Shield blocked the attack!";
        UpdateUI();

        yield return new WaitForSeconds(1.2f);

        if (playerHP <= 0)
        {
            playerHP = 0;
            UpdateUI();
            EndGame(false, $"GAME OVER! You have fallen in battle.\nFinal Score: {winCount} wins");
            yield break;
        }

        isPlayerTurn = true;
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

    void UpdatePotionDisplay()
    {
        if (potionCountText != null) potionCountText.text = $"Potions: {potionCount}";
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
    public void UsePotion()
    {
        if (!isPlayerTurn || gameOver) return;
        if (potionCount <= 0)
        {
            messageText.text = "No potions available!";
            return;
        }
        potionCount--;
        UpdatePotionDisplay();
        PerformPlayerAction("potion");
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

