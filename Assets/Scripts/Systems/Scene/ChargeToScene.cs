using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChargeToScene : MonoBehaviour
{
    [Header("Settings")]
    [Range(0.1f, 5f)] public float fillSpeed = 1f;
    [SerializeField] private bool resetOnRelease = true;

    [SerializeField] private Image progressImage;

    [SerializeField] private bool FillHorizontal = false;

    [Header("重複防止")]
    [SerializeField] private bool isChangingScene = false;

    private float currentFill;

    [Header("参照")]
    public SceneChanger sceneChanger_List;

    [Header("移動先")]
    public int sceneIndexToLoad;

    private void Awake()
    {
        SetupImageComponent();
    }

    private void SetupImageComponent()
    {
        progressImage.type = Image.Type.Filled;

        if (FillHorizontal)
        {
            progressImage.fillMethod = Image.FillMethod.Horizontal;
            progressImage.fillOrigin = (int)Image.OriginHorizontal.Left;
        }
        else
        {
            progressImage.fillMethod = Image.FillMethod.Radial360;
            progressImage.fillOrigin = (int)Image.Origin360.Top;
            progressImage.fillClockwise = true;
            progressImage.type = Image.Type.Filled;
        }
    }

    private void Update()
    {
        if (isChangingScene) return;

        HandleInput();
        UpdateVisuals();

        if (currentFill >= 1.0f)
        {
            ChangeScene();
        }
    }

    public void ChangeScene()
    {
        isChangingScene = true;

        SceneFader.Instance.StartFadeOut(() => {
            sceneChanger_List.StartChangeSceneByIndex(sceneIndexToLoad);
        });
    }

    private void HandleInput()
    {
        if (Input.GetMouseButton(0))
        {
            currentFill += fillSpeed * Time.deltaTime;
        }
        else if (resetOnRelease)
        {
            currentFill -= fillSpeed * Time.deltaTime * 2;
        }

        currentFill = Mathf.Clamp01(currentFill);
    }

    private void UpdateVisuals()
    {
        progressImage.fillAmount = currentFill;
    }
}