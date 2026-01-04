using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverOnHit : MonoBehaviour
{
    private bool gameOver = false;

    void OnCollisionEnter(Collision collision)
    {
        if (gameOver) return;

        gameOver = true;
        Debug.Log("GAME OVER");

        // 1 saniye sonra oyunu baþtan baþlat
        Invoke(nameof(RestartGame), 1f);
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
