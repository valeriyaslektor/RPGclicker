// Assets/Scripts/EnemyClickHandler.cs
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyClickHandler : MonoBehaviour, IPointerClickHandler
{
    [Tooltip("Ссылка на GameManager")]
    public GameManager gameManager;
    [Tooltip("Аниматор персонажа")]
    public Animator    playerAnimator;
    [Tooltip("Имя trigger-параметра в Animator")]
    public string      attackTrigger = "attack";

    public void OnPointerClick(PointerEventData eventData)
    {
        // эмулируем реальный клик по врагу
        gameManager.OnEnemyClick();

        // запускаем анимацию атаки
        if (playerAnimator != null)
            playerAnimator.SetTrigger(attackTrigger);
    }
}









