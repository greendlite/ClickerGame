using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

public class Game : MonoBehaviour
{
    public int Score;
    public int[] CostInt;
    private int ClickScore = 1;
    private int TotalBonus;

    public Text CoalName;
    public Text IronName;
    public Text GoldName;
    public Text DiamondName;

    public int[] OresCount = new int[4];
    //koment
    [Header("Кирки")]
    [SerializeField] private Pickaxe[] pickaxes; // Массив кирок
    [SerializeField] private Button[] pickaxeButtons; // Кнопки для покупки кирок
    [SerializeField] private Text[] pickaxeCostTexts; // Тексты стоимости кирок
    [SerializeField] private Text[] pickaxeStatusTexts; // Тексты статуса кирок
    [SerializeField] private Text currentPickaxeText; // Текст текущей кирки

    private int currentPickaxeIndex = 0; // Начинаем с 0
    private int oreMultiplier = 1; // Множитель добычи

    [System.Serializable]
    public class Pickaxe
    {
        public string name;
        public int baseCost;
        public bool isPurchased = false;
        public Sprite icon;
    }

    [Header("Достижения")]
    [SerializeField] private Achievement[] achievements; // Массив достижений
    [SerializeField] private Button[] achievementButtons; // Кнопки для получения наград
    [SerializeField] private Text[] achievementTitles; // Названия достижений
    [SerializeField] private Text[] achievementDescriptions; // Описания достижений
    [SerializeField] private Text[] achievementProgress; // Прогресс достижений
    [SerializeField] private Text[] achievementRewards; // Награды достижений
    [SerializeField] private Text[] achievementStatus; // Статус достижений

    [Header("Сброс игры")]
    [SerializeField] private Button resetButton; // Кнопка сброса
    [SerializeField] private GameObject resetConfirmPanel; // Панель подтверждения
    [SerializeField] private Text resetConfirmText; // Текст подтверждения

    // Начальные значения (для сброса)
    private int[] initialCostInt = new int[] { 10, 100 };
    private int[] initialCostBonus = new int[] { 1 };

    [System.Serializable]
    public class Achievement
    {
        public string id; // Уникальный ID достижения
        public string title; // Название достижения
        public string description; // Описание
        public AchievementType type; // Тип достижения
        public int targetValue; // Целевое значение
        public int currentValue; // Текущее значение
        public int reward; // Награда в деньгах
        public bool isCompleted; // Выполнено ли
        public bool isClaimed; // Получена ли награда

        public enum AchievementType
        {
            ClickCount,     // Количество кликов
            TotalScore,     // Всего заработано денег
            OresMined,      // Всего добыто руды
            CoalMined,      // Добыто угля
            IronMined,      // Добыто железа
            GoldMined,      // Добыто золота
            DiamondMined,   // Добыто алмазов
            PickaxeOwned,   // Приобретено кирок
            TotalSells,     // Всего продано
            TimePlayed      // Время игры
        }
    }

    // Счетчики для достижений
    private int totalClicks = 0;
    private int totalOresMined = 0;
    private int totalSells = 0;
    private float timePlayed = 0f;

    [Header("Floating Ore")]
    [SerializeField] public FloatingOreView floatingOrePrefab;
    [SerializeField] public Transform floatingOreParent;

    [SerializeField] private Sprite CoalSprite;
    [SerializeField] private Sprite IronSprite;
    [SerializeField] private Sprite GoldSprite;
    [SerializeField] private Sprite DiamondSprite;

    private void ShowFloatingOre(int oreType, int amount)
    {
        FloatingOreView view = Instantiate(
            floatingOrePrefab,
            floatingOreParent
        );

        Sprite icon = oreType switch
        {
            0 => CoalSprite,
            1 => IronSprite,
            2 => GoldSprite,
            3 => DiamondSprite,
            _ => null
        };

        view.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        view.Init(icon, amount);
    }

    [SerializeField] public int CoalPrice = 1;
    [SerializeField] public int IronPrice = 5;
    [SerializeField] public int GoldPrice = 10;
    [SerializeField] public int DiamondPrice = 30;

    // Инициализация системы кирок
    private void InitializePickaxes()
    {
        if (pickaxes.Length < 4)
        {
            Debug.LogError("Нужно 4 кирки в массиве pickaxes!");
            return;
        }

        pickaxes[0].isPurchased = true;
        UpdatePickaxeUI();
    }

    // Инициализация системы достижений
    private void InitializeAchievements()
    {
        if (achievements == null || achievements.Length == 0)
        {
            CreateDefaultAchievements();
        }

        UpdateAchievementsUI();
    }

    // Создание достижений по умолчанию
    private void CreateDefaultAchievements()
    {
        achievements = new Achievement[]
        {
            // Начальные достижения
            new Achievement
            {
                id = "click_10",
                title = "Начинающий шахтер",
                description = "Сделать 10 кликов",
                type = Achievement.AchievementType.ClickCount,
                targetValue = 10,
                currentValue = totalClicks,
                reward = 100,
                isCompleted = false,
                isClaimed = false
            },          
            // Достижения по добыче
            new Achievement
            {
                id = "ore_100",
                title = "Первая сотня",
                description = "Добыть 100 единиц руды",
                type = Achievement.AchievementType.OresMined,
                targetValue = 100,
                currentValue = totalOresMined,
                reward = 200,
                isCompleted = false,
                isClaimed = false
            },
            // Достижения по продажам
            new Achievement
            {
                id = "sell_10",
                title = "Торговец",
                description = "Продать руду 10 раз",
                type = Achievement.AchievementType.TotalSells,
                targetValue = 10,
                currentValue = totalSells,
                reward = 500,
                isCompleted = false,
                isClaimed = false
            },
            new Achievement
            {
                id = "pickaxe_4",
                title = "Магнат инструментов",
                description = "Иметь все 4 кирки",
                type = Achievement.AchievementType.PickaxeOwned,
                targetValue = 4,
                currentValue = GetOwnedPickaxesCount(),
                reward = 5000,
                isCompleted = false,
                isClaimed = false
            },
        };
    }

    // Получить количество купленных кирок
    private int GetOwnedPickaxesCount()
    {
        int count = 0;
        foreach (var pickaxe in pickaxes)
        {
            if (pickaxe.isPurchased) count++;
        }
        return count;
    }

    // Обновление всех достижений
    private void UpdateAllAchievements()
    {
        if (achievements == null) return;

        foreach (var achievement in achievements)
        {
            // Если достижение уже получено, пропускаем обновление прогресса
            if (achievement.isClaimed) continue;

            // Обновляем текущие значения в зависимости от типа
            switch (achievement.type)
            {
                case Achievement.AchievementType.ClickCount:
                    achievement.currentValue = totalClicks;
                    break;
                case Achievement.AchievementType.TotalScore:
                    achievement.currentValue = Score;
                    break;
                case Achievement.AchievementType.OresMined:
                    achievement.currentValue = totalOresMined;
                    break;
                case Achievement.AchievementType.CoalMined:
                    achievement.currentValue = OresCount[0];
                    break;
                case Achievement.AchievementType.IronMined:
                    achievement.currentValue = OresCount[1];
                    break;
                case Achievement.AchievementType.GoldMined:
                    achievement.currentValue = OresCount[2];
                    break;
                case Achievement.AchievementType.DiamondMined:
                    achievement.currentValue = OresCount[3];
                    break;
                case Achievement.AchievementType.PickaxeOwned:
                    achievement.currentValue = GetOwnedPickaxesCount();
                    break;
                case Achievement.AchievementType.TotalSells:
                    achievement.currentValue = totalSells;
                    break;
                case Achievement.AchievementType.TimePlayed:
                    achievement.currentValue = (int)timePlayed;
                    break;
            }

            // Проверяем выполнение
            if (!achievement.isCompleted && achievement.currentValue >= achievement.targetValue)
            {
                achievement.isCompleted = true;
                Debug.Log($"Достижение выполнено: {achievement.title}");
            }
        }

        UpdateAchievementsUI();
    }

    // Обновление UI достижений - ИСПРАВЛЕННАЯ ВЕРСИЯ
    private void UpdateAchievementsUI()
    {
        if (achievements == null) return;

        for (int i = 0; i < Mathf.Min(achievements.Length, achievementButtons.Length); i++)
        {
            var achievement = achievements[i];

            // Обновляем тексты
            if (achievementTitles.Length > i && achievementTitles[i] != null)
                achievementTitles[i].text = achievement.title;

            if (achievementDescriptions.Length > i && achievementDescriptions[i] != null)
                achievementDescriptions[i].text = achievement.description;

            // Обновляем прогресс
            if (achievementProgress.Length > i && achievementProgress[i] != null)
            {
                if (achievement.isClaimed)
                {
                    // Для полученных достижений показываем финальный результат
                    achievementProgress[i].text = $"{achievement.targetValue}/{achievement.targetValue}";
                }
                else if (achievement.isCompleted)
                {
                    // Для выполненных, но не полученных
                    achievementProgress[i].text = $"{achievement.targetValue}/{achievement.targetValue}";
                }
                else
                {
                    // Для незавершенных
                    achievementProgress[i].text = $"{achievement.currentValue}/{achievement.targetValue}";
                }
            }

            if (achievementRewards.Length > i && achievementRewards[i] != null)
                achievementRewards[i].text = $"+{achievement.reward}$";

            // Обновляем статус и кнопку
            if (achievementStatus.Length > i && achievementStatus[i] != null)
            {
                if (achievement.isClaimed)
                {
                    achievementStatus[i].text = "Получено";
                    achievementStatus[i].color = Color.gray;
                }
                else if (achievement.isCompleted)
                {
                    achievementStatus[i].text = "Готово к получению";
                    achievementStatus[i].color = Color.green;
                }
                else
                {
                    achievementStatus[i].text = "В процессе";
                    achievementStatus[i].color = Color.yellow;
                }
            }

            // Настраиваем кнопку
            if (achievementButtons.Length > i && achievementButtons[i] != null)
            {
                achievementButtons[i].interactable = achievement.isCompleted && !achievement.isClaimed;

                // Обновляем текст на кнопке
                Text buttonText = achievementButtons[i].GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    if (achievement.isClaimed)
                        buttonText.text = "Получено";
                    else if (achievement.isCompleted)
                        buttonText.text = "Получить награду";
                    else
                        buttonText.text = "Заблокировано";
                }
            }
        }
    }

    // Получение награды за достижение - ИСПРАВЛЕННАЯ ВЕРСИЯ
    public void ClaimAchievement(int index)
    {
        if (index < 0 || index >= achievements.Length)
        {
            Debug.LogError($"Неверный индекс достижения: {index}");
            return;
        }

        var achievement = achievements[index];

        if (!achievement.isCompleted)
        {
            Debug.Log($"Достижение {achievement.title} еще не выполнено!");
            return;
        }

        if (achievement.isClaimed)
        {
            Debug.Log($"Награда за достижение {achievement.title} уже получена!");
            return;
        }

        // Выдаем награду
        Score += achievement.reward;
        achievement.isClaimed = true;

        // Фиксируем текущее значение на момент получения
        // (чтобы не менялось дальше)
        achievement.currentValue = achievement.targetValue;

        Debug.Log($"Получена награда за достижение {achievement.title}: +{achievement.reward}$");

        // Обновляем UI
        UpdateAchievementsUI();
        UpdatePickaxeUI();
    }

    // Покупка кирки
    public void BuyPickaxe(int index)
    {
        Debug.Log($"Попытка купить кирку с индексом: {index}, Название: {pickaxes[index].name}");

        if (pickaxes[index].isPurchased)
        {
            Debug.Log($"Кирка {pickaxes[index].name} уже куплена!");
            return;
        }

        if (index == 0)
        {
            pickaxes[0].isPurchased = true;
            currentPickaxeIndex = 0;
            oreMultiplier = 1;

            Debug.Log($"Получена стартовая кирка! Добыча: x{oreMultiplier}");
            UpdatePickaxeUI();
            UpdateAllAchievements(); // Обновляем достижения
            return;
        }

        if (!pickaxes[index - 1].isPurchased)
        {
            Debug.Log($"Сначала купите {pickaxes[index - 1].name} кирку!");
            return;
        }

        if (Score >= pickaxes[index].baseCost)
        {
            Score -= pickaxes[index].baseCost;
            pickaxes[index].isPurchased = true;
            currentPickaxeIndex = index;
            oreMultiplier = (int)Mathf.Pow(2, index);

            Debug.Log($"Куплена {pickaxes[index].name} кирка! Добыча теперь x{oreMultiplier}");

            UpdatePickaxeUI();
            UpdateAllAchievements(); // Обновляем достижения
        }
        else
        {
            Debug.Log($"Недостаточно денег для покупки {pickaxes[index].name} кирки!");
        }
    }

    // Обновление UI кирок
    private void UpdatePickaxeUI()
    {
        for (int i = 0; i < pickaxes.Length; i++)
        {
            if (pickaxeButtons.Length > i && pickaxeButtons[i] != null &&
                pickaxeCostTexts.Length > i && pickaxeStatusTexts.Length > i)
            {
                Image buttonImage = pickaxeButtons[i].GetComponent<Image>();
                if (pickaxes[i].isPurchased)
                {
                    pickaxeButtons[i].interactable = false;
                    pickaxeStatusTexts[i].text = "Куплено";
                    pickaxeCostTexts[i].text = "Куплено";

                    // Купленные кирки - белый цвет
                    if (buttonImage != null)
                        buttonImage.color = Color.white;

                    if (i == currentPickaxeIndex)
                    {
                        Text buttonText = pickaxeButtons[i].GetComponentInChildren<Text>();
                        if (buttonText != null)
                            buttonText.text = $"Текущая\n{pickaxes[i].name}";
                    }
                }
                else
                {
                    bool isAvailable = (i == 0) || (i > 0 && pickaxes[i - 1].isPurchased);

                    if (isAvailable)
                    {
                        pickaxeButtons[i].interactable = true;
                        pickaxeStatusTexts[i].text = "Доступно";
                        pickaxeCostTexts[i].text = (i == 0) ? "Бесплатно" : $"{pickaxes[i].baseCost}$";

                        // Проверяем достаточно ли денег (для платных кирок)
                        if (i > 0 && Score < pickaxes[i].baseCost)
                        {
                            // Недостаточно денег - красный
                            if (buttonImage != null)
                                buttonImage.color = new Color(1f, 0.8f, 0.8f); // Светло-красный
                        }
                        else
                        {
                            // Бесплатная кирка или достаточно денег - белый
                            if (buttonImage != null)
                                buttonImage.color = Color.white;
                        }
                    }
                    else
                    {
                        pickaxeButtons[i].interactable = false;
                        pickaxeStatusTexts[i].text = "Заблокировано";
                        pickaxeCostTexts[i].text = (i == 0) ? "Бесплатно" : $"{pickaxes[i].baseCost}$";

                        if (buttonImage != null)
                            buttonImage.color = Color.gray;
                    }
                }
            }
        }

        if (currentPickaxeText != null && currentPickaxeIndex < pickaxes.Length)
        {
            currentPickaxeText.text = $"Текущая кирка: {pickaxes[currentPickaxeIndex].name}\n" +
                                      $"Множитель добычи: x{oreMultiplier}";
        }
    }

    // Метод продажи всей руды
    public void SellAllOres()
    {
        Score += OresCount[0] * CoalPrice;
        Score += OresCount[1] * IronPrice;
        Score += OresCount[2] * GoldPrice;
        Score += OresCount[3] * DiamondPrice;

        // Увеличиваем счетчик продаж
        totalSells++;

        // Обнуляем руду после продажи
        OresCount[0] = 0;
        OresCount[1] = 0;
        OresCount[2] = 0;
        OresCount[3] = 0;

        UpdateOreTexts();
        UpdatePickaxeUI();
        UpdateAllAchievements(); // Обновляем достижения
    }

    // Для продажи угля
    public void SellCoal()
    {
        if (OresCount[0] > 0)
        {
            Score += OresCount[0] * CoalPrice;
            OresCount[0] = 0;
            totalSells++;

            UpdateOreTexts();
            UpdatePickaxeUI();
            UpdateAllAchievements();
        }
    }

    // Для продажи железа
    public void SellIron()
    {
        if (OresCount[1] > 0)
        {
            Score += OresCount[1] * IronPrice;
            OresCount[1] = 0;
            totalSells++;

            UpdateOreTexts();
            UpdatePickaxeUI();
            UpdateAllAchievements();
        }
    }

    // Для продажи золота
    public void SellGold()
    {
        if (OresCount[2] > 0)
        {
            Score += OresCount[2] * GoldPrice;
            OresCount[2] = 0;
            totalSells++;

            UpdateOreTexts();
            UpdatePickaxeUI();
            UpdateAllAchievements();
        }
    }

    // Для продажи алмазов
    public void SellDiamond()
    {
        if (OresCount[3] > 0)
        {
            Score += OresCount[3] * DiamondPrice;
            OresCount[3] = 0;
            totalSells++;

            UpdateOreTexts();
            UpdatePickaxeUI();
            UpdateAllAchievements();
        }
    }

    private Save sv = new();

    private void Awake()
    {
        if (PlayerPrefs.HasKey("SV"))
        {
            sv = JsonUtility.FromJson<Save>(PlayerPrefs.GetString("SV"));
            Score = sv.Score;
            ClickScore = sv.ClickScore;
            totalClicks = sv.totalClicks;
            totalOresMined = sv.totalOresMined;
            totalSells = sv.totalSells;
            timePlayed = sv.timePlayed;

            // Загружаем состояние кирок
            if (sv.pickaxeStates != null && sv.pickaxeStates.Length >= 4)
            {
                for (int i = 0; i < Mathf.Min(4, sv.pickaxeStates.Length); i++)
                {
                    pickaxes[i].isPurchased = sv.pickaxeStates[i];
                }
            }

            currentPickaxeIndex = sv.currentPickaxeIndex;
            oreMultiplier = sv.oreMultiplier;

            // Загружаем достижения
            if (sv.achievements != null && sv.achievements.Length > 0)
            {
                achievements = sv.achievements;
            }

            for (int i = 0; i < 1; i++)
            {
                TotalBonus += sv.CostBonus[i];
            }

            for (int i = 0; i < 2; i++)
            {
                CostInt[i] = sv.CostInt[i];
            }

            // Загружаем руду
            for (int i = 0; i < 4; i++)
                OresCount[i] = sv.OresCount[i];

            UpdateOreTexts();
        }
        else
        {
            pickaxes[0].isPurchased = true;
        }

        InitializePickaxes();
        InitializeAchievements();
    }

    private void Start()
    {
        StartCoroutine(BonusShop());

        DateTime dt = new DateTime(sv.Date[0], sv.Date[1], sv.Date[2], sv.Date[3], sv.Date[4], sv.Date[5]);
        TimeSpan ts = DateTime.Now - dt;
        Score += (int)ts.TotalSeconds * TotalBonus;
        print($"Вы получили: {(int)ts.TotalSeconds * TotalBonus} $");

        UpdatePickaxeUI();
        UpdateAllAchievements();

        // Инициализация кнопки сброса
        InitializeResetButton();
    }

    // Инициализация кнопки сброса
    private void InitializeResetButton()
    {
        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ShowResetConfirmation);
        }

        // Скрываем панель подтверждения при старте
        if (resetConfirmPanel != null)
        {
            resetConfirmPanel.SetActive(false);
        }
    }

    // Показать окно подтверждения сброса
    public void ShowResetConfirmation()
    {
        if (resetConfirmPanel != null)
        {
            resetConfirmPanel.SetActive(true);

            // Обновляем текст подтверждения
            if (resetConfirmText != null)
            {
                resetConfirmText.text = "Вы уверены, что хотите сбросить игру?\n" +
                                       "Все ваши достижения, деньги и прогресс будут удалены.\n" +
                                       "Это действие нельзя отменить!";
            }
        }
    }

    // Скрыть окно подтверждения
    public void HideResetConfirmation()
    {
        if (resetConfirmPanel != null)
        {
            resetConfirmPanel.SetActive(false);
        }
    }

    // Полный сброс игры
    public void ResetGame()
    {
        Debug.Log("Начинаем сброс игры...");

        // 1. Удаляем сохранение
        PlayerPrefs.DeleteKey("SV");
        PlayerPrefs.Save();
        Debug.Log("Сохранение удалено из PlayerPrefs");

        // 2. Сбрасываем все переменные к начальным значениям

        // Основные счетчики
        Score = 0;
        totalClicks = 0;
        totalOresMined = 0;
        totalSells = 0;
        timePlayed = 0f;
        ClickScore = 1;
        TotalBonus = 1;

        // Сбрасываем кирки
        if (pickaxes != null && pickaxes.Length > 0)
        {
            for (int i = 0; i < pickaxes.Length; i++)
            {
                pickaxes[i].isPurchased = (i == 0); // Только первая кирка куплена
            }
        }
        currentPickaxeIndex = 0;
        oreMultiplier = 1;

        // Сбрасываем руду
        for (int i = 0; i < OresCount.Length; i++)
        {
            OresCount[i] = 0;
        }

        // Сбрасываем стоимость улучшений
        if (CostInt != null && CostInt.Length >= 2)
        {
            // Если у вас были другие начальные значения, замените их
            CostInt[0] = initialCostInt[0];
            CostInt[1] = initialCostInt[1];
        }

        // Сбрасываем достижения
        if (achievements != null)
        {
            // Пересоздаем достижения с нулевыми значениями
            CreateDefaultAchievements();
        }

        // 3. Сбрасываем объект сохранения
        sv = new Save();

        // 4. Обновляем весь UI
        UpdateOreTexts();
        UpdatePickaxeUI();
        UpdateAllAchievements();

        // 5. Скрываем панель подтверждения
        HideResetConfirmation();

        Debug.Log("Игра полностью сброшена!");
    }

    public void OnClickButton()
    {
        totalClicks++;
        totalOresMined += oreMultiplier;

        int amount = 1 * oreMultiplier;

        // Выпадение руды
        int chance = UnityEngine.Random.Range(0, 100);

        if (chance == 0)
        {
            OresCount[3] += amount;
            ShowFloatingOre(3, amount);
        }
        else if (chance <= 5)
        {
            OresCount[2] += amount;
            ShowFloatingOre(2, amount);
        }
        else if (chance <= 34)
        {
            OresCount[1] += amount;
            ShowFloatingOre(1, amount);
        }
        else
        {
            OresCount[0] += amount;
            ShowFloatingOre(0, amount);
        }

        UpdateOreTexts();
        UpdateAllAchievements(); // Обновляем достижения после каждого клика
    }

    private void UpdateOreTexts()
    {
        CoalName.text = "Уголь: " + OresCount[0].ToString();
        IronName.text = "Железо: " + OresCount[1].ToString();
        GoldName.text = "Золото: " + OresCount[2].ToString();
        DiamondName.text = "Алмазы: " + OresCount[3].ToString();
    }

    private void Update()
    {
        // Обновляем время игры
        timePlayed += Time.deltaTime;

        // Периодически обновляем достижения (раз в секунду)
        if (Time.frameCount % 60 == 0) // Примерно раз в секунду при 60 FPS
        {
            UpdateAllAchievements();
        }

        UpdatePickaxeUI();
    }

    public void OnClickBuyBonusShop()
    {
        if (Score >= CostInt[1])
        {
            Score -= CostInt[1];
            CostInt[1] *= 2;
            UpdatePickaxeUI();
            UpdateAllAchievements();
        }
    }

    IEnumerator BonusShop()
    {
        while (true)
        {
            UpdateAllAchievements(); // Обновляем достижения при получении бонуса
            yield return new WaitForSeconds(1);
        }
    }

    private void SaveGame()
    {
        sv.Score = Score;
        sv.ClickScore = ClickScore;
        sv.totalClicks = totalClicks;
        sv.totalOresMined = totalOresMined;
        sv.totalSells = totalSells;
        sv.timePlayed = timePlayed;

        sv.CostBonus = new int[1];
        sv.CostInt = new int[2];

        // Сохраняем достижения
        sv.achievements = achievements;

        // Сохраняем состояние кирок
        sv.pickaxeStates = new bool[4];
        for (int i = 0; i < 4; i++)
        {
            sv.pickaxeStates[i] = pickaxes[i].isPurchased;
        }
        sv.currentPickaxeIndex = currentPickaxeIndex;
        sv.oreMultiplier = oreMultiplier;


        for (int i = 0; i < 2; i++)
        {
            sv.CostInt[i] = CostInt[i];
        }

        // Сохраняем руду
        for (int i = 0; i < 4; i++)
            sv.OresCount[i] = OresCount[i];

        sv.Date[0] = DateTime.Now.Year;
        sv.Date[1] = DateTime.Now.Month;
        sv.Date[2] = DateTime.Now.Day;
        sv.Date[3] = DateTime.Now.Hour;
        sv.Date[4] = DateTime.Now.Minute;
        sv.Date[5] = DateTime.Now.Second;

        PlayerPrefs.SetString("SV", JsonUtility.ToJson(sv));
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private void OnApplicationPause(bool pause)
    {
        if (pause) SaveGame();
    }
#else
    private void OnApplicationQuit()
    {
        SaveGame();
    }
#endif
}

[Serializable]
public class Save
{
    public int Score;
    public int ClickScore;
    public int[] CostInt;
    public int[] CostBonus;
    public int[] Date = new int[6];

    // Статистика для достижений
    public int totalClicks;
    public int totalOresMined;
    public int totalSells;
    public float timePlayed;

    // Сохраняем достижения
    public Game.Achievement[] achievements;

    // Добавляем сохранение руды
    public int[] OresCount = new int[4];

    // Добавляем сохранение состояния кирок
    public bool[] pickaxeStates = new bool[4];
    public int currentPickaxeIndex;
    public int oreMultiplier;
}