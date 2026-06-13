using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("장애물 설정")]
    public float fallSpeed = 3f;        // 낙하 속도
    public bool isTracking = false;     // 추적 여부
    public float trackingSpeed = 2f;    // 추적 속도

    private Transform playerTransform;
    private bool isSlow = false;
    private float slowMultiplier = 0.5f;

    void Start()
    {
        // 플레이어 찾기
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
    }

    void Update()
    {
        float currentSpeed = isSlow ? fallSpeed * slowMultiplier : fallSpeed;

        if (isTracking && playerTransform != null)
        {
            // 추적 장애물 : 플레이어 방향으로 이동
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            transform.position += new Vector3(direction.x * trackingSpeed * Time.deltaTime,
                                              -currentSpeed * Time.deltaTime, 0);
        }
        else
        {
            // 일반 장애물 : 아래로 낙하
            transform.position += Vector3.down * currentSpeed * Time.deltaTime;
        }

        // 화면 아래로 벗어나면 삭제
        if (transform.position.y < -7f)
            Destroy(gameObject);
    }

    // 속도 감소 적용
    public void ApplySlow()
    {
        isSlow = true;
    }

    // 속도 감소 해제
    public void RemoveSlow()
    {
        isSlow = false;
    }
}