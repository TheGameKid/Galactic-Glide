using UnityEngine;

public class Rainbow : MonoBehaviour
{
    public RectTransform panel;   // Assign your UI panel here
    public Player player;
    public float moveDuration = 10f;

    // Start and End X positions (fixed Y)
    float startX = -4431f;
    float endX = -1980f;
    float timer = 0f;
    bool moving = false;

    private void Awake()
    {
        if (panel == null) panel = GetComponent<RectTransform>();
        startX = 1790f;
        endX = -4400f;
    }
    public void StartMoving()
    {
        // Start position
        player.TimesTwo.SetActive(true);
        moving = true;
        timer = 0;
       // Vector2 pos = panel.anchoredPosition;
        //pos.x = startX;
        //panel.anchoredPosition = pos;
    }

    void Update()
    {
        if (!moving || panel == null || !player.TimesTwo.activeInHierarchy) return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / moveDuration);

        // Move horizontally from startX to endX
        //Vector2 pos = panel.anchoredPosition;
        //pos.x = Mathf.Lerp(startX, endX, t);
        //panel.anchoredPosition = pos;

        // When done, reset and stop
        if (t >= 1f)
        {
          //  pos.x = startX;
            //panel.anchoredPosition = pos;
            moving = false;
            timer = 0f;
            player.TimesTwo.SetActive(false);
            player.mult = false;
            player.MultiplyGone.Play();
        }
    }
}
