using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;
    public int playerHealth = 100;
    public int enemyHealth = 100;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void PlayerAttack()
    {
        int damage = Random.Range(5, 15);
        enemyHealth -= damage;
        Debug.Log("El jugador ataca e inflige " + damage + " de daño.");

        CombatUIManager uiManager = FindObjectOfType<CombatUIManager>();
        if (uiManager != null)
            uiManager.UpdateHealthBars();

        CheckCombatState();

        if (enemyHealth > 0)
        {
            // Esperar un momento antes de que el enemigo ataque (para animaciones o efectos)
            Invoke("EnemyTurn", 1.5f);
        }
    }

    public void Defend()
    {
        Debug.Log("El jugador se defiende...");
    }

    public void UseItem()
    {
        Debug.Log("El jugador usa un objeto...");
    }

    public void Huir()
    {
        Debug.Log("El jugador intenta huir... (Pendiente)");
    }


    public void EnemyTurn()
    {
        int damage = Random.Range(5, 15);
        playerHealth -= damage;
        Debug.Log("El enemigo ataca e inflige " + damage + " de daño.");

        // Actualizar la UI de la barra de vida
        CombatUIManager uiManager = FindObjectOfType<CombatUIManager>();
        if (uiManager != null)
            uiManager.UpdateHealthBars();

        CheckCombatState();
    }


    void CheckCombatState()
    {
        if (enemyHealth <= 0)
        {
            Debug.Log("¡El enemigo ha sido derrotado!");
        }
    }
}
