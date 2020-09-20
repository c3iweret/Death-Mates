using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class PlayerIndicator : MonoBehaviour {

    [SerializeField] private Player targetPlayer;
    [SerializeField] private Image pickupIconImage;

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        UpdatePlaceableIcon();
    }

    public void UpdatePlaceableIcon()
    {
        pickupIconImage.enabled = targetPlayer.placer.HasPlacable;
        pickupIconImage.sprite = targetPlayer.placer.GetPlaceableIcon();
    }
}
