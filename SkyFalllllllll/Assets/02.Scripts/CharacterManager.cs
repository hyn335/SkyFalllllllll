using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;

    [Header("캐릭터 프리팹 목록")]
    public GameObject[] characterPrefabs;  // 3개 캐릭터 프리팹

    private int selectedCharacterIndex = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // 저장된 캐릭터 불러오기
        selectedCharacterIndex = PlayerPrefs.GetInt("SkyFallSelectedCharacter", 0);
    }

    // 캐릭터 선택
    public void SelectCharacter(int index)
    {
        if (index < 0 || index >= characterPrefabs.Length) return;
        selectedCharacterIndex = index;
        PlayerPrefs.SetInt("SkyFallSelectedCharacter", index);
        PlayerPrefs.Save();
    }

    // 현재 선택된 캐릭터 인덱스
    public int GetSelectedCharacterIndex()
    {
        return selectedCharacterIndex;
    }
}