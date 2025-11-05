using UnityEngine;

[System.Serializable]
public class ParticleSetting
{
    public enum StatusSource
    {
        Player,
        Scene
    }

    public StatusSource source = StatusSource.Player;
    public string statusKey;
    public Transform spawnPosition;
    public GameObject[] particlePrefabs;
    public bool playWhenTrue = true;
}

public class ParticleManager : MonoBehaviour
{
    [Header("パーティクル制御リスト")]
    [SerializeField] private ParticleSetting[] settings;

    private bool[] prevStates;

    private void Awake()
    {
        prevStates = new bool[settings.Length];
    }

    private void Update()
    {
        for (int i = 0; i < settings.Length; i++)
        {
            var setting = settings[i];

            bool status = false;

            // 状態取得
            switch (setting.source)
            {
                case ParticleSetting.StatusSource.Player:
                    if (System.Enum.TryParse(setting.statusKey, out PlayerStatusType playerStatus))
                        status = PlayerStatus.Instance.GetStatus(playerStatus);
                    break;

                case ParticleSetting.StatusSource.Scene:
                    if (System.Enum.TryParse(setting.statusKey, out SceneStatusType sceneStatus))
                        status = SceneStatus.Instance.GetStatus(sceneStatus);
                    break;
            }

            bool shouldSpawn = setting.playWhenTrue ? status : !status;

            // 指定の状態に変化したときだけ
            if (shouldSpawn && !prevStates[i])
                SpawnParticles(setting);

            prevStates[i] = shouldSpawn;
        }
    }

    private void SpawnParticles(ParticleSetting setting)
    {
        if (setting.particlePrefabs == null) return;

        foreach (var prefab in setting.particlePrefabs)
        {
            if (prefab == null) continue;

            Vector3 pos = setting.spawnPosition != null
                ? setting.spawnPosition.position
                : Vector3.zero;

            Instantiate(prefab, pos, Quaternion.identity);
        }
    }
}