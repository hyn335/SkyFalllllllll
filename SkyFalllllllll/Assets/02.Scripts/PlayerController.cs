using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 5f;
    public float boundaryX = 4.5f;

    [Header("점프 설정")]
    public float jumpForce = 5f;
    public float gravity = -15f;
    public float minY = -3f;
    public float maxY = 3f;

    [Header("대쉬 설정")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 1.5f;

    [Header("무적 설정")]
    public float invincibleDuration = 3f;
    public float blinkInterval = 0.1f;

    [Header("UI 연결")]
    public Image dashIcon;

    [Header("대쉬 아이콘 색상")]
    public Color readyColor = new Color(1f, 1f, 1f, 1f);
    public Color cooldownColor = new Color(0.3f, 0.3f, 0.3f, 0.4f);

    [Header("캐릭터 설정")]
    public Transform characterContainer;  // Player 오브젝트 연결

    private bool isInvincible = false;
    private bool isGameOver = false;
    private bool isDashing = false;
    private bool isJumping = false;
    private float dashCooldownTimer = 0f;
    private float verticalVelocity = 0f;

    void Start()
    {
        ApplySelectedCharacter();
        UpdateDashIcon();
    }

    void ApplySelectedCharacter()
    {
        if (CharacterManager.Instance == null) return;
        if (characterContainer == null) return;

        int selectedIndex = CharacterManager.Instance.GetSelectedCharacterIndex();

        // 모든 캐릭터 비활성화
        for (int i = 0; i < characterContainer.childCount; i++)
            characterContainer.GetChild(i).gameObject.SetActive(false);

        // 선택된 캐릭터 활성화
        if (selectedIndex < characterContainer.childCount)
            characterContainer.GetChild(selectedIndex).gameObject.SetActive(true);
    }

    void Update()
    {
        if (isGameOver) return;

        float inputX = 0f;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            inputX = -1f;
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            inputX = 1f;

        // 대쉬 쿨타임 감소
        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
            if (dashCooldownTimer <= 0f)
            {
                dashCooldownTimer = 0f;
                UpdateDashIcon();
            }
        }

        // 점프
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
            StartCoroutine(JumpCoroutine());

        // 대쉬
        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            && dashCooldownTimer <= 0f && inputX != 0f && !isDashing)
            StartCoroutine(DashCoroutine(inputX));

        // 좌우 이동
        if (!isDashing)
        {
            Vector3 pos = transform.position;
            pos.x += inputX * moveSpeed * Time.deltaTime;
            pos.x = Mathf.Clamp(pos.x, -boundaryX, boundaryX);
            transform.position = pos;
        }
    }

    System.Collections.IEnumerator JumpCoroutine()
    {
        isJumping = true;
        verticalVelocity = jumpForce;

        while (isJumping)
        {
            verticalVelocity += gravity * Time.deltaTime;

            Vector3 pos = transform.position;
            pos.y += verticalVelocity * Time.deltaTime;

            if (pos.y <= minY)
            {
                pos.y = minY;
                isJumping = false;
            }

            pos.y = Mathf.Clamp(pos.y, minY, maxY);
            transform.position = pos;

            yield return null;
        }
    }

    System.Collections.IEnumerator DashCoroutine(float direction)
    {
        isDashing = true;
        dashCooldownTimer = dashCooldown;
        UpdateDashIcon();

        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            Vector3 pos = transform.position;
            pos.x += direction * dashSpeed * Time.deltaTime;
            pos.x = Mathf.Clamp(pos.x, -boundaryX, boundaryX);
            transform.position = pos;

            elapsed += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
    }

    void UpdateDashIcon()
    {
        if (dashIcon == null) return;
        dashIcon.color = dashCooldownTimer <= 0f ? readyColor : cooldownColor;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isGameOver) return;

        if (other.CompareTag("Obstacle") && !isInvincible)
        {
            isGameOver = true;
            GameManager.Instance.GameOver();
        }

        if (other.CompareTag("InvincibleItem"))
        {
            Destroy(other.gameObject);
            StartCoroutine(InvincibleCoroutine());
        }

        if (other.CompareTag("SlowItem"))
        {
            Destroy(other.gameObject);
            GameManager.Instance.SlowObstacles();
        }
    }

    System.Collections.IEnumerator InvincibleCoroutine()
    {
        isInvincible = true;

        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();

        float elapsed = 0f;
        while (elapsed < invincibleDuration)
        {
            bool active = !renderers[0].enabled;
            foreach (SpriteRenderer sr in renderers)
                sr.enabled = active;

            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        foreach (SpriteRenderer sr in renderers)
            sr.enabled = true;

        isInvincible = false;
    }

    public void ResetPlayer()
    {
        isGameOver = false;
        isInvincible = false;
        isDashing = false;
        isJumping = false;
        dashCooldownTimer = 0f;
        verticalVelocity = 0f;

        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in renderers)
            sr.enabled = true;

        transform.position = new Vector3(0, minY, 0);
        UpdateDashIcon();
    }
}