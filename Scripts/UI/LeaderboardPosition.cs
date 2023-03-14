using TMPro;
using UnityEngine;
using UnityEngine.UI;

public struct Score
{
    public int level;
    public int score;

    public Score(IActor fromActor)
    {
        level = fromActor.CurrentLevel;
        score = fromActor.CurrentScore;
    }

    public static bool operator > (Score l, Score r)
    {
        if (l.level > r.level)
            return true;
        else if (l.level == r.level && l.score > r.score)
            return true;
        else return false;
    }

    public static bool operator < (Score l, Score r)
    {
        if (l.level < r.level)
            return true;
        else if (l.level == r.level && l.score < r.score)
            return true;
        else return false;
    }

    public static bool operator == (Score l, Score r)
    {
        return l.GetHashCode() == r.GetHashCode();
    }

    public static bool operator !=(Score l, Score r)
    {
        return l.GetHashCode() != r.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return obj.GetHashCode() == this.GetHashCode();
    }

    public override int GetHashCode()
    {
        return level ^ score;
    }
}
public class LeaderboardPosition : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI printer;
    [SerializeField] private Image img;
    [SerializeField] private float secondColorStrengthDonw = 0.75f, secondColorStrengthUp = 1.25f;

    public string actorName;
    public Score actorScore = new Score();
    public int actorPosition;

    private Vector3? defautPosition;

    public void SetColor(Color color)
    {
        printer.color = Color.white;
        if(img!=null)
        {
            //img.color = SetStrength(color);
            img.color = color;
        }
    }

    public void UpdateText()
    {
        gameObject.SetActive(true);
        printer.text = $"{actorPosition}) {actorName} {actorScore.level} lvl ({actorScore.score})";
        if (img != null && !defautPosition.HasValue)
            defautPosition = img.transform.localPosition;
    }

    public void SetStrikethrough()
    {
        printer.fontStyle |= FontStyles.Strikethrough;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private Color SetStrength(Color sourceColor)
    {
        Color secondColor;
        if (sourceColor.maxColorComponent >= 0.5f)
            secondColor = sourceColor * secondColorStrengthDonw;
        else
            secondColor = sourceColor * secondColorStrengthUp;
        secondColor.a = sourceColor.a;
        return secondColor;
    }

    public void ShiftImageElement(Vector3 amount)
    {
        if(img!=null)
        {
            img.transform.localPosition = defautPosition.Value + amount;
        }
    }
}
