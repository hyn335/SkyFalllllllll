using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectUI : MonoBehaviour
{
    [Header("패널 연결")]
    public GameObject characterSelectPanel;

    [Header("슬롯 연결 (3개)")]
    public Image[] slotImages;          // 각 슬롯 배경 이미지
    public Image[] characterImages;     // 각 슬롯 캐릭터 이미지
    public Button[] selectButtons;      // 각 슬롯 선택 버튼

    [Header("색상 설정")]
    public Color normalColor = new Color(0.2f, 0.2f, 0.2f, 1f);    // 기본 슬롯 색상
    public Color selectedColor = new Color(0.8f, 0.7f, 0f, 1f);    // 선택된 슬롯 색상

    [Header("캐릭터 이미지")]
    public Sprite[] characterSprites;   // 각 캐릭터 이미지 3개

    void Start()
    {
        if (characterSelectPanel != null)
            characterSelectPanel.SetActive(false);
    }

    public void OpenPanel()
    {
        if (characterSelectPanel != null)
            characterSelectPanel.SetActive(true);
        RefreshSlots();
    }

    public void ClosePanel()
    {
        if (characterSelectPanel != null)
            characterSelectPanel.SetActive(false);
    }

    void RefreshSlots()
    {
        if (CharacterManager.Instance == null) return;

        int selectedIndex = CharacterManager.Instance.GetSelectedCharacterIndex();

        for (int i = 0; i < slotImages.Length; i++)
        {
            // 선택된 슬롯 노란색 테두리
            if (slotImages[i] != null)
                slotImages[i].color = (i == selectedIndex) ? selectedColor : normalColor;

            // 캐릭터 이미지 설정
            if (characterImages[i] != null && characterSprites != null && i < characterSprites.Length)
                characterImages[i].sprite = characterSprites[i];
        }
    }

    public void OnSelectCharacter(int index)
    {
        if (CharacterManager.Instance == null) return;
        CharacterManager.Instance.SelectCharacter(index);
        RefreshSlots();
    }
}