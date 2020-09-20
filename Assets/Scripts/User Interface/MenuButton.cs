using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class MenuButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private TextMeshProUGUI text;
    private string defaultText;

	private void Start() {
        // fetch default text value for selection effect
        defaultText = text.text;

        if (EventSystem.current.currentSelectedGameObject == this.gameObject)
			this.OnSelect(null);
		else
			this.OnDeselect(null);
	}

	public void OnSelect(BaseEventData data)
    {
        text.color = Color.yellow;
        text.text = ">" + defaultText + "<";
    }

    public void OnDeselect(BaseEventData data)
    {
        text.color = HexToColor("#ffffff");
        text.text = defaultText;
    }

	public static Color32 HexToColor(string hex) {
		hex = hex.Replace("#", string.Empty);
		byte r = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
		byte g = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
		byte b = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
		return new Color32(r, g, b, 255);
	}
}
