using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoyoteHealthUI : MonoBehaviour
{
    private PlayerStates states;
    public Image healthFill;
    public int maxHealth = 100;
    public int currentHealth;

    public Sprite portraitFull;
    public Sprite portraitInjured;
    public Sprite portraitCritical;

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        UpdateHealthUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
            TakeDamage(10);
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        UpdateHealthUI();
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        float fillAmount = (float)currentHealth / maxHealth;
        healthFill.fillAmount = fillAmount;

/* This comes in later.
        if (currentHealth > 66)
            portraitImage.sprite = portraitFull;
        else if (currentHealth > 33)
            portraitImage.sprite = portraitInjured;
        else
            portraitImage.sprite = portraitCritical;
*/
    }

    void Die()
    {
        anim.SetBool("isDead", true);
        // Additional logic: disable controls, trigger flea exodus, etc.
        states.SetState(PlayerState.Dead);
    }
}
