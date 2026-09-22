using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject previousTile;
    public GameObject currentTile;
    public GameObject tilePrefab;
    public int score = 1;
    public TextMeshProUGUI scoreText;
    public GameObject restarButton;

    public float speed = 0.01f;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            DealWithPlayerClick();
        }
        if (currentTile != null)
        {
            currentTile.transform.localPosition += new Vector3(1, 0, 1) * speed;
        }
    }
    public void RestartLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    public void DealWithPlayerClick()
    {
        if (5>4)
        {
            currentTile = Instantiate(tilePrefab, previousTile.transform.position + Vector3.up, previousTile.transform.rotation);
        }
        else
        {
            if (previousTile.transform.localScale.x > currentTile.transform.localScale.x)
            {
                Debug.Log("Stopped just in time");
                previousTile = currentTile;
                currentTile = Instantiate(tilePrefab, previousTile.transform.position + Vector3.up, previousTile.transform.rotation);
                Camera.main.transform.position += Vector3.up;
                score++;
                scoreText.text = score.ToString();
            }
            else
            {
                Debug.Log("Not accurate enough");
                restarButton.SetActive(true);
            }
        }
    }
}
