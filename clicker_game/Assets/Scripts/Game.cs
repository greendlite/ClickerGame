using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;


public class Game : MonoBehaviour
{
    public int Score;
    public int[] CostInt;
    private int ClickScore = 1;
    public int[] CostBonus;
    private int TotalBonus;

    public ShopView shopView;

    public Text CopperText;
    public Text IronText;
    public Text DiamondText;

    public int[] OresCount = new int[3];


    [Header("Floating Ore")]
    [SerializeField] public FloatingOreView floatingOrePrefab;
    [SerializeField] public Transform floatingOreParent;

    [SerializeField] private Sprite copperSprite;
    [SerializeField] private Sprite ironSprite;
    [SerializeField] private Sprite diamondSprite;

    private void ShowFloatingOre(int oreType, int amount)
    {
        FloatingOreView view = Instantiate(
            floatingOrePrefab,
            floatingOreParent
        );

        Sprite icon = oreType switch
        {
            0 => copperSprite,
            1 => ironSprite,
            2 => diamondSprite,
            _ => null
        };

        view.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        view.Init(icon, amount);
    }

    [SerializeField] private int copperPrice = 1;
    [SerializeField] private int ironPrice = 5;
    [SerializeField] private int diamondPrice = 50;

    // Метод продажи всей руды
    public void SellAllOres()
    {
        Score += OresCount[0] * copperPrice;
        Score += OresCount[1] * ironPrice;
        Score += OresCount[2] * diamondPrice;

        // Обнуляем руду после продажи
        OresCount[0] = 0;
        OresCount[1] = 0;
        OresCount[2] = 0;

        UpdateOreTexts();
    }
    //public GameObject ShopPan;
    //public GameObject BonusPan;
    //public GameObject SettingsPan;
    //public GameObject AchievementsPan;

    //это убрать(ShopView)
    //public Text[] CostText;

    //public Text ScoreText;

    private Save sv = new();

    [Header("Достижения")] private int Achievement1Max;
    private bool isAchievement1 = false;
    private bool isAchievement2 = false;
    private bool isAchievement2Get = false;
    private bool isAchievement3 = false;
    private bool isAchievement3Get = false;

    [SerializeField] private Text[] AchievementsText;
    [SerializeField] private Text[] AchievementsCost;

    public Text Achievement1NameText;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("SV"))
        {
            sv = JsonUtility.FromJson<Save>(PlayerPrefs.GetString("SV"));
            Score = sv.Score;
            ClickScore = sv.ClickScore;
            Achievement1Max = sv.Achievement1Max;
            isAchievement1 = sv.isAchievement1;
            
            isAchievement2 = sv.isAchievement2;
            isAchievement2Get =  sv.isAchievement2Get;
            
            isAchievement3 = sv.isAchievement3;
            isAchievement3Get =  sv.isAchievement3Get;
            
            for (int i = 0; i < 1; i++)
            {
                CostBonus[i] = sv.CostBonus[i];
                TotalBonus += sv.CostBonus[i];
            }

            for (int i = 0; i < 2; i++)
            {
                CostInt[i] = sv.CostInt[i];
            }

            // Загружаем руду
            for (int i = 0; i < 3; i++)
                OresCount[i] = sv.OresCount[i];

            UpdateOreTexts();
        }
    }

    private void Start()
    {
        StartCoroutine(BonusShop());

        DateTime dt = new DateTime(sv.Date[0], sv.Date[1], sv.Date[2], sv.Date[3], sv.Date[4], sv.Date[5]);
        TimeSpan ts = DateTime.Now - dt;
        Score += (int)ts.TotalSeconds * TotalBonus;
        print($"�� ����������: {(int)ts.TotalSeconds * TotalBonus} $");
    }

    public void OnClickButton()
    {
        if (Achievement1Max < 100)
        {
            Achievement1Max++;
        }

        // Начисление очков
        //Score += ClickScore;

        // Выпадение руды
        int chance = UnityEngine.Random.Range(0, 100); // 0..99

        if (chance < 5)
        {
            // Алмаз — 5%
            OresCount[2]++;
            ShowFloatingOre(2, 1);
        }
        else if (chance < 35)
        {
            // Железо — 30% (5–34)
            OresCount[1]++;
            ShowFloatingOre(1, 1);
        }
        else
        {
            // Медь — 65% (35–99)
            OresCount[0]++;
            ShowFloatingOre(0, 1);
        }

        //Для обновления
        UpdateOreTexts();

        if (Achievement1Max >= 100 && !isAchievement1)
        {
            isAchievement1 = true;
            AchievementsText[0].text = "Выполнено";
        }
    }


    private void UpdateOreTexts()
    {
        CopperText.text = "Медь: " + OresCount[0].ToString();
        IronText.text = "Железо: " + OresCount[1].ToString();
        DiamondText.text = "Алмазы: " + OresCount[2].ToString();
    }


    private void Update()
    {
        //ScoreText.text = Score + "$";

        Achievement1NameText.text = "Нажмите " + Achievement1Max + "/100 раз";

        if (isAchievement1)
        {
            AchievementsCost[0].text = "Получено";
        }

        if (Achievement1Max == 100)
        {
            AchievementsText[0].text = "Выполнено";
        }

        if (isAchievement2)
            AchievementsText[1].text = "Выполнено";
        if (isAchievement2Get)
            AchievementsCost[1].text = "Получено";

        if (isAchievement3)
            AchievementsText[2].text = "Выполнено";
        if (isAchievement3Get)
            AchievementsCost[2].text = "Получено";
    }

    //public void OnClickBuyLevel()
    //{
    //    if (Score >= CostInt[0])
    //    {
    //        Score -= CostInt[0];
    //        CostInt[0] *= 2;
    //        ClickScore *= 2;
    //        //CostText[0].text = CostInt[0] + "$";

    //        shopView.RefreshUI();

    //        isAchievement2 = true;
    //    }
    //}

    public void OnClickBuyBonusShop()
    {
        if (Score >= CostInt[1])
        {
            Score -= CostInt[1];
            CostInt[1] *= 2;
            CostBonus[0] += 2;
            //CostText[1].text = CostInt[1] + "$";
            
            isAchievement3 = true;
        }
    }

    IEnumerator BonusShop()
    {
        while (true)
        {
            Score += CostBonus[0];
            yield return new WaitForSeconds(1);
        }
    }
#if UNITY_ANDROID && !UNITY_EDITOR
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            sv.Score = Score;
            sv.ClickScore = ClickScore;
            sv.CostBonus = new int[1];
            sv.CostInt = new int[2];
            sv.Achievement1Max = Achievement1Max;
            sv.isAchievement1 = isAchievement1;
            sv.isAchievement2 = isAchievement2;
            sv.isAchievementGet =  isAchievementGet;
            sv.isAchievement3 = isAchievement3;
            sv.isAchievement3Get =  isAchievement3Get;

            for (int i = 0; i < 1; i++)
            {
                sv.CostBonus[i] = CostBonus[i];
            }

            for (int i = 0; i < 2; i++)
            {
                sv.CostInt[i] = CostInt[i];
            }

            // Сохраняем руду
            for (int i = 0; i < 3; i++)
                sv.OresCount[i] = OresCount[i];

            sv.Date[0] = DateTime.Now.Year;
            sv.Date[1] = DateTime.Now.Month;
            sv.Date[2] = DateTime.Now.Day;
            sv.Date[3] = DateTime.Now.Hour;
            sv.Date[4] = DateTime.Now.Minute;
            sv.Date[5] = DateTime.Now.Second;


            PlayerPrefs.SetString("SV", JsonUtility.ToJson(sv));
        }
    }
#else
    private void OnApplicationQuit()
    {
        sv.Score = Score;
        sv.ClickScore = ClickScore;
        sv.CostBonus = new int[1];
        sv.CostInt = new int[2];
        sv.Achievement1Max = Achievement1Max;
        sv.isAchievement1 = isAchievement1;
        sv.isAchievement2 = isAchievement2;
        sv.isAchievement2Get =  isAchievement2Get;
        sv.isAchievement3 = isAchievement3;
        sv.isAchievement3Get =  isAchievement3Get;

        for (int i = 0; i < 1; i++)
        {
            sv.CostBonus[i] = CostBonus[i];
        }

        for (int i = 0; i < 2; i++)
        {
            sv.CostInt[i] = CostInt[i];
        }

        // Сохраняем руду
        for (int i = 0; i < 3; i++)
            sv.OresCount[i] = OresCount[i];

        sv.Date[0] = DateTime.Now.Year;
        sv.Date[1] = DateTime.Now.Month;
        sv.Date[2] = DateTime.Now.Day;
        sv.Date[3] = DateTime.Now.Hour;
        sv.Date[4] = DateTime.Now.Minute;
        sv.Date[5] = DateTime.Now.Second;

        PlayerPrefs.SetString("SV", JsonUtility.ToJson(sv));
    }
#endif
    public void OnClickAchievement1Button()
    {
        if (isAchievement1 && Achievement1Max == 100)
        {
            Score += 5000;
            isAchievement1 = false;
            isAchievement2Get = true;
        }
    }
    
    public void OnClickAchievement2Button()
    {
        if (isAchievement2)
        {
            Score += 3000;
            isAchievement2Get = true;
        }
    }
    
    public void OnClickAchievement3Button()
    {
        if (isAchievement3)
        {
            Score += 6000;
            isAchievement3Get = true;
        }
    }
}

[Serializable]
public class Save
{
    public int Score;
    public int ClickScore;
    public int[] CostInt;
    public int[] CostBonus;
    public int[] Date = new int[6];

    public bool isAchievement1;
    public bool isAchievement2;
    public bool isAchievement2Get;
    public bool isAchievement3;
    public bool isAchievement3Get;
    
    public int Achievement1Max;

    // Добавляем сохранение руды
    public int[] OresCount = new int[3];
}