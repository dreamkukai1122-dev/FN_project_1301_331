using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject previousTile;
    public GameObject currentTile;
    public GameObject tilePrefab;
    public int score = 0;
    public TextMeshProUGUI scoreText;
    public GameObject restarButton;

    [Header("Speed Settings")]
    public float initialSpeed = 0.02f; // ความเร็วเริ่มต้น
    private float currentSpeed;       // ความเร็วปัจจุบันที่จะเพิ่มขึ้นเรื่อยๆ
    private int direction = 1;

    [Header("Color Gradient Settings")]
    public Color startColor = Color.cyan;
    public Color endColor = Color.magenta;

    void Start()
    {
        if (restarButton != null)
            restarButton.SetActive(false);

        currentSpeed = initialSpeed; // กำหนดความเร็วเริ่มต้นตอนเริ่มเกม
        SpawnNewTile();
    }

    void Update()
    {
        if (currentTile == null) return;

        // ให้บล็อกขยับไป-มาตามความเร็วปัจจุบัน
        currentTile.transform.position += new Vector3(direction, 0, 0) * currentSpeed;

        // ชนขอบแล้วเด้งกลับ
        if (currentTile.transform.position.x > 3f || currentTile.transform.position.x < -3f)
        {
            direction *= -1;
        }

        // คลิกเมาส์ซ้ายเพื่อวางบล็อก
        if (Input.GetMouseButtonDown(0))
        {
            DealWithPlayerClick();
        }
    }

    void SpawnNewTile()
    {
        if (previousTile == null) return;

        Vector3 spawnPos = new Vector3(previousTile.transform.position.x, previousTile.transform.position.y + 1f, previousTile.transform.position.z);
        currentTile = Instantiate(tilePrefab, spawnPos, previousTile.transform.rotation);
        currentTile.transform.localScale = previousTile.transform.localScale;

        // ระบบไล่สีอัตโนมัติ
        Renderer renderer = currentTile.GetComponent<Renderer>();
        if (renderer != null)
        {
            float t = (score % 15) / 15f; // เปลี่ยนสีครบรอบทุกๆ 15 แต้ม
            renderer.material.color = Color.Lerp(startColor, endColor, t);
        }
    }

    public void DealWithPlayerClick()
    {
        if (currentTile == null) return;

        float hangover = currentTile.transform.position.x - previousTile.transform.position.x;
        float maxScale = previousTile.transform.localScale.x;
        float absHangover = Mathf.Abs(hangover);

        // ถ้าวางพลาดตกขอบ
        if (absHangover >= maxScale)
        {
            Debug.Log("Not accurate enough - Game Over");
            if (restarButton != null) restarButton.SetActive(true);
            Destroy(currentTile);
            currentTile = null;
            return;
        }

        // ตัดขนาดและคำนวณตำแหน่งกึ่งกลางใหม่
        float newSize = maxScale - absHangover;
        float midpoint = (currentTile.transform.position.x + previousTile.transform.position.x) / 2f;

        currentTile.transform.localScale = new Vector3(newSize, currentTile.transform.localScale.y, currentTile.transform.localScale.z);
        currentTile.transform.position = new Vector3(midpoint, currentTile.transform.position.y, currentTile.transform.position.z);

        // เพิ่มคะแนน
        score++;
        if (scoreText != null)
            scoreText.text = score.ToString();

        // --- ระบบเพิ่มความเร็วทุกๆ 15 แต้ม ---
        // ทุกครั้งที่คะแนนหารด้วย 15 ลงตัว จะบวกความเร็วเพิ่มขึ้น 0.01 (หรือปรับค่าเพิ่มลดได้ตามความเหมาะสม)
        if (score % 15 == 0)
        {
            currentSpeed += 0.01f;
            Debug.Log("Speed increased! Current Speed: " + currentSpeed);
        }

        // ขยับกล้องหนีขึ้นด้านบน
        if (Camera.main != null)
            Camera.main.transform.position += Vector3.up;

        previousTile = currentTile;
        SpawnNewTile();
    }

    public void RestartLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}