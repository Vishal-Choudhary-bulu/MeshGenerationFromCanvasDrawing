using UnityEngine;
using UnityEngine.UI;

public class ResolutionGridSetter : MonoBehaviour
{

    public RectTransform grid;

    public LayoutElement gridElement;

    public RectTransform drawBoard;

    void Start()
    {
        if (Screen.width > Screen.height / 2)
        {
            gridElement.ignoreLayout = true;
            grid.anchorMin = new Vector2(0.5f, 0.5f);
            grid.anchorMax = new Vector2(0.5f, 0.5f);
            grid.pivot = new Vector2(0.5f, 0.5f);
            float s = drawBoard.rect.height;
            grid.sizeDelta = new Vector2(s, s);
        }
        else
        {
            //gridElement.ignoreLayout = false;
            gridElement.ignoreLayout = true;
            grid.anchorMin = new Vector2(0.5f, 0.5f);
            grid.anchorMax = new Vector2(0.5f, 0.5f);
            grid.pivot = new Vector2(0.5f, 0.5f);
            float s = drawBoard.rect.width;
            grid.sizeDelta = new Vector2(s, s);
        }
    }

}
