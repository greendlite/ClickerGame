using UnityEngine;
using UnityEngine.UI;

public class ScoreView : MonoBehaviour
{
    public Text ScoreText;
    public Game Game; // ссылка на Game.cs

    private void Update()
    {
        if (Game != null)
            ScoreText.text = Game.Score + "$";
    }
}
