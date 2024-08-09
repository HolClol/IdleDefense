using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UISnapCenter : MonoBehaviour
{
    public ScrollRect ScrollRect;
    public RectTransform ContentPanel;
    public RectTransform StageList;

    public HorizontalLayoutGroup HLG;

    public float SnapForce;
    private bool isSnapped = false;
    private float snapSpeed;

    private void Update()
    {
        int currentStage = Mathf.RoundToInt(0 - ContentPanel.localPosition.x / (StageList.rect.width + HLG.spacing));

        if (ScrollRect.velocity.magnitude < 400 && !isSnapped)
        {
            ScrollRect.velocity = Vector2.zero;
            snapSpeed += SnapForce * Time.deltaTime;

            ContentPanel.localPosition = new Vector3(
                Mathf.MoveTowards(ContentPanel.localPosition.x, 0 - (currentStage * (StageList.rect.width + HLG.spacing)), snapSpeed),
                ContentPanel.localPosition.y,
                ContentPanel.localPosition.z);
            if (ContentPanel.localPosition.x == 0 - (currentStage * (StageList.rect.width + HLG.spacing))) {
                 isSnapped = true;
            }
        }
        if (ScrollRect.velocity.magnitude > 400)
        {
            isSnapped = false;
            snapSpeed = 0;
        }
    }

}
