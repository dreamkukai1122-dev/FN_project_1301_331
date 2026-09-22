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

    public float speed = 0.02f;
    private int direction = 1; // ใช้สลับทิศทางการวิ่งของบล็อก

    void Start()
    {
        // ซ่อนปุ่ม Restart ตอนเริ่มเกม
        if (restarButton != null)
            restarButton.SetActive(false);

        SpawnNewTile();
    }

    void Update()
    {
        if (currentTile == null) return;

        // ให้บล็อกขยับไปมาซ้าย-ขวา (หรือตามแกน X และ Z)
        currentTile.transform.position += new Vector3(direction, 0, 0) * speed;

        // ถ้าชนขอบเขต ให้สลับทิศทางวิ่งกลับ
        if (currentTile.transform.position.x > 3f || currentTile.transform.position.x < -3f)
        {
            direction *= -1;
        }

        // เมื่อคลิกเมาส์ซ้าย
        if (Input.GetMouseButtonDown(0))
        {
            DealWithPlayerClick();
        }
    }

    void SpawnNewTile()
    {
        if (previousTile == null) return;

        // สร้างบล็อกใหม่เหนือบล็อกก่อนหน้า
        Vector3 spawnPos = new Vector3(previousTile.transform.position.x, previousTile.transform.position.y + 1f, previousTile.transform.position.z);
        currentTile = Instantiate(tilePrefab, spawnPos, previousTile.transform.rotation);

        // กำหนดขนาดให้เท่ากับบล็อกก่อนหน้า
        currentTile.transform.localScale = previousTile.transform.localScale;
    }

    public void DealWithPlayerClick()
    {
        if (currentTile == null) return;

        // คำนวณระยะห่างระหว่างบล็อกปัจจุบันกับบล็อกก่อนหน้า
        float hangover = currentTile.transform.position.x - previousTile.transform.position.x;
        float maxScale = previousTile.transform.localScale.x;
        float absHangover = Mathf.Abs(hangover);

        // ถ้าวางเหลื่อมเกินขนาดบล็อก แปลว่าตกขอบ (แพ้)
        if (absHangover >= maxScale)
        {
            Debug.Log("Not accurate enough - Game Over");
            if (restarButton != null) restarButton.SetActive(true);
            Destroy(currentTile);
            currentTile = null;
            return;
        }

        // คำนวณขนาดใหม่ที่เหลืออยู่หลังถูกตัด
        float newSize = maxScale - absHangover;

        // คำนวณตำแหน่งกึ่งกลางใหม่
        float midpoint = (currentTile.transform.position.x + previousTile.transform.position.x) / 2f;

        // ปรับขนาดและตำแหน่งของบล็อกปัจจุบันให้พอดีกับการซ้อนทับ
        currentTile.transform.localScale = new Vector3(newSize, currentTile.transform.localScale.y, currentTile.transform.localScale.z);
        currentTile.transform.position = new Vector3(midpoint, currentTile.transform.position.y, currentTile.transform.position.z);

        // อัปเดตคะแนน
        score++;
        if (scoreText != null)
            scoreText.text = score.ToString();

        // เลื่อนกล้องขึ้น
        if (Camera.main != null)
            Camera.main.transform.position += Vector3.up;

        // ตั้งค่าบล็อกปัจจุบันเป็นบล็อกเก่า เพื่อเตรียมสร้างชั้นต่อไป
        previousTile = currentTile;
        SpawnNewTile();
    }

    public void RestartLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}