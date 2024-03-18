using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void FixedUpdate()
    {
        // ���� ��ǥ�� ��ũ�� ��ǥ�� �ٸ��� ������ �ش� ����� X
        // rect.position = GameManager.instance.player.transform.position;
        // WorldToScreenPoint() : ���� ���� ������Ʈ ��ġ�� ��ũ�� ��ǥ�� ��ȯ
        rect.position = Camera.main.WorldToScreenPoint(GameManager.instance.player.transform.position);
    }
}
