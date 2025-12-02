using UnityEngine;
using UnityEngine.UI;

public class ShopView : MonoBehaviour
{
    public Game Game; // ссылка на твой Game.cs
    public Text[] CostTexts; // тексты стоимости для апгрейдов
    public Button BuyLevelButton;

    private void Start()
    {
        // Привязываем кнопки к методам Game
        BuyLevelButton.onClick.AddListener(() => Game.OnClickBuyLevel());

        // Обновляем UI при старте
        UpdateCosts();
    }

    private void UpdateCosts()
    {
        // для будущих объектов в магазине
        /*for (int i = 0; i < CostTexts.Length; i++)
        {
            if (i < Game.CostInt.Length) 
                CostTexts[i].text = Game.CostInt[i] + "$";
        }*/

        //Обновляем цену после покупки
        CostTexts[0].text = Game.CostInt[0] + "$";
    }

    public void RefreshUI()
    {
        UpdateCosts();
    }
}
