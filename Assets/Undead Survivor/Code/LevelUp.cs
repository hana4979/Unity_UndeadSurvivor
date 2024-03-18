using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUp : MonoBehaviour
{
    RectTransform rect;
    Item[] items; // �⺻ ���� ������ ���� ������ �ҷ�����

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true); // ��Ȱ��ȭ�� ������Ʈ�� ������
    }

    // �г� ����
    public void Show()
    {
        Next();
        rect.localScale = Vector3.one;
        GameManager.instance.Stop();
    }

    // �г� ����
    public void Hide()
    {
        rect.localScale = Vector3.zero;
        GameManager.instance.Resume();
    }

    // ��ư ��� �����ִ� �Լ�
    public void Select(int index)
    {
        items[index].OnClick();
    }

    void Next()
    {
        // 1. ��� ������ ��Ȱ��ȭ
        // foreach�� Ȱ���Ͽ� ��� ������ ��Ȱ��ȭ
        foreach (Item item in items)
        {
            item.gameObject.SetActive(false);
        }

        // 2. �� �߿��� ���� 3�� ������ Ȱ��ȭ
        int[] ran = new int[3];
        while (true)
        {
            ran[0] = Random.Range(0, items.Length); // 0 ~ 4
            ran[1] = Random.Range(0, items.Length);
            ran[2] = Random.Range(0, items.Length);

            // while���� �������� if��
            // ���� ���Ͽ� ��� ���� ������ �ݺ����� ������������ �ۼ�
            if (ran[0] != ran[1] && ran[1] != ran[2] && ran[0] != ran[2])
                break;
        }

        // �������� ���� ���ڿ� ������ ��ġ
        for (int index = 0; index < ran.Length; index++)
        {
            Item ranItem = items[ran[index]];

            // 3. ���� �������� ���� �Һ���������� ��ü
            if (ranItem.level == ranItem.data.damages.Length)
            {
                // item4�� ����� Ȱ��ȭ
                items[4].gameObject.SetActive(true);
            }
            else
            {
                ranItem.gameObject.SetActive(true);
            }
        }
    }
}
