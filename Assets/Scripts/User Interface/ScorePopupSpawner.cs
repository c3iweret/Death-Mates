using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScorePopupSpawner : MonoBehaviour {

    [SerializeField] private GameObject scorePopupPrefab;
    [SerializeField] private float destroyTimer = 1f;
    [SerializeField] private Color positiveScoreColor;
    [SerializeField] private Color negativeScoreColor;

    public IEnumerator SpawnPopup(int scoreDelta) {
        GameObject scorePopup = Instantiate(scorePopupPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);
        TextMeshPro tmp = scorePopup.GetComponentInChildren<TextMeshPro>();
        // Always diaplays the sign
        tmp.text = scoreDelta.ToString("+0;-#");
        tmp.color = scoreDelta >= 0 ? positiveScoreColor : negativeScoreColor;
        yield return new WaitForSeconds(destroyTimer);
        Destroy(scorePopup);
    }
}
