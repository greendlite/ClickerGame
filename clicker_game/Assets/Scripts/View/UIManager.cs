using UnityEngine;

/// <summary>
/// Класс отвечает только за работу с UI-панелями.
/// Это часть слоя View в архитектуре MVVM.
/// Здесь находится логика открытия/закрытия экранов, 
/// чтобы вынести это из класса Game и не мешать логику игры с логикой интерфейса.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Панели")]

    /// <summary>
    /// Панель магазина.
    /// Здесь игрок может покупать уровни или бонусы.
    /// </summary>
    public GameObject ShopPan;

    /// <summary>
    /// Панель бонусов.
    /// Здесь покупаются бонусы, которые дают деньги каждую секунду.
    /// </summary>
    public GameObject BonusPan;

    /// <summary>
    /// Панель настроек (громкость, выход, сброс и т.д.).
    /// </summary>
    public GameObject SettingsPan;

    /// <summary>
    /// Панель достижений.
    /// В ней показываются выполненные задания и награды.
    /// </summary>
    public GameObject AchievementsPan;

    /// <summary>
    /// Панель для ресурсов
    /// </summary>
    public GameObject ResourcesPan;

    /// <summary>
    /// Переключает видимость панели магазина.
    /// Если панель открыта — закроет.
    /// Если закрыта — откроет.
    /// </summary>
    public void ToggleShop()
    {
        ShopPan.SetActive(!ShopPan.activeSelf);
    }

    /// <summary>
    /// Переключает панель бонусов точно так же, как магазин.
    /// </summary>
    public void ToggleBonus()
    {
        BonusPan.SetActive(!BonusPan.activeSelf);
    }

    /// <summary>
    /// Показывает или скрывает панель настроек.
    /// </summary>
    public void ToggleSettings()
    {
        SettingsPan.SetActive(!SettingsPan.activeSelf);
    }

    /// <summary>
    /// Показывает или скрывает панель достижений.
    /// </summary>
    public void ToggleAchievements()
    {
        AchievementsPan.SetActive(!AchievementsPan.activeSelf);
    }

    /// <summary>
    /// Показывает или скрывает панель ресурсов
    /// </summary>
    public void ToggleResources()
    {
        ResourcesPan.SetActive(!ResourcesPan.activeSelf);
    }
}
