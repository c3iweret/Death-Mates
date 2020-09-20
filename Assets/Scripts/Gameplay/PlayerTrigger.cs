using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PlayerTriggerEvent : UnityEvent<Player> { }

[System.Serializable]
public class PlayerInvincibleTriggerEvent : UnityEvent<Player> { }

[RequireComponent(typeof(Collider))]
public class PlayerTrigger : MonoBehaviour
{
    [SerializeField] private PlayerTriggerEvent triggerEvent, triggerStayEvent;
    [SerializeField] private PlayerInvincibleTriggerEvent invincibleTriggerEvent;

    private void Start()
    {
        if (triggerEvent == null)
        {
            triggerEvent = new PlayerTriggerEvent();
        }
    }

    void OnTriggerEnter(Collider collisionInfo)
    {
        Player player = collisionInfo.gameObject.GetComponent<Player>();

        if (player != null)
        {
            if (player.invincible)
            {
                invincibleTriggerEvent.Invoke(player);
            }
            else
            {
                triggerEvent.Invoke(player);
            }
        }
    }

    void OnTriggerStay(Collider collider)
    {
        // No point doing anyhing if there are no listeners
        if (triggerStayEvent.GetPersistentEventCount() == 0)
        {
            return;
        }

        Player player = collider.gameObject.GetComponent<Player>();

        if (player != null)
        {
            triggerStayEvent.Invoke(player);
        }
    }
}
