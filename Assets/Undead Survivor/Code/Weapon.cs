using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id; // ���� ID
    public int prefabId; // ������ ID
    public float damage; // ������
    public int count; // ����, ��ź�� ���Ϳ� �ε����� ������� �ʰ� ��� �հ� ������ ��ġ
    public float speed; // ȸ�� �ӵ�

    float timer; // ���� �������� �߻��ϱ� ����
    Player player;

    void Awake()
    {
        player = GameManager.instance.player;
    }

    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        switch (id)
        {
            case 0:
                // forward : (0,0,1), back : (0,0,-1) ������ ������ ���
                // Update���� �̵��̳� ȸ���� �Ѵٸ� deltaTime �����ֱ�
                transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;
            default:
                timer += Time.deltaTime; // �������� �Һ��� �ð� == ���� �ð�
                if (timer > speed)
                {
                    timer = 0f;
                    Fire(); // �߻�
                }
                break;
        }

        // .. Test Code..
        if (Input.GetButtonDown("Jump"))
        {
            LevelUp(10, 1);
        }
    }

    public void LevelUp(float damage, int count)
    {
        this.damage = damage * Character.Damage;
        this.count += count;

        if(id == 0)
            Batch();

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver); // ������/������ �ø��� ��� ���� ��� ����
    }

    public void Init(ItemData data)
    {
        // Basic Set
        name = "Weapon" + data.itemId;
        transform.parent = player.transform; // �θ� ������Ʈ�� �÷��̾�� ����
        transform.localPosition = Vector3.zero; // �÷��̾� �ȿ��� ��ġ�� (0,0,0)���� (������ġ)

        // Property Set (���� ���� �Ӽ� �������� ��ũ��Ʈ�� ������Ʈ �����ͷ� �ʱ�ȭ)
        id = data.itemId;
        damage = data.baseDamage * Character.Damage;
        count = data.baseCount + Character.Count;

        // prefabId�� Ǯ�� �Ŵ����� �������� ã�Ƽ� �ʱ�ȭ
        // ��ũ��Ʈ�� ������Ʈ�� �������� ���ؼ� �ε����� �ƴ� ���������� ����
        for(int index = 0; index < GameManager.instance.pool.prefabs.Length; index++)
        {
            if(data.projectile == GameManager.instance.pool.prefabs[index])
            {
                prefabId = index;
                break;
            }
        }


        // ���� ID�� ���� �ʱ�ȭ
        switch (id)
        {
            case 0:
                speed = 150 * Character.WeaponSpeed;
                // count ������ �������� ��ġ
                Batch();
                break;
            default:
                speed = 0.5f * Character.WeaponRate; // ���� �ӵ�
                break;
        }

        // Hand Set
        // enum ������ �����ʹ� ���� ���·ε� ��� ����
        Hand hand = player.hands[(int)data.itemType]; // int�� ���� ����ȯ
        hand.spriter.sprite = data.hand;
        hand.gameObject.SetActive(true);

        // BroadcastMessage : Ư�� �Լ� ȣ���� ��� �ڽĿ��� ����ϴ� �Լ�
        // �� ��° ���� : �� �޴� ���ڰ� �������� �ʾƵ� �ȴ�
        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver); // ��� �����Ǿ��� ��

    }

    void Batch()
    {
        for (int index = 0; index < count; index++)
        {
            // for������ count��ŭ Ǯ������ ��������
            // Bullet�� �θ� �ٲٱ� ����
            Transform bullet;
            
            if(index < transform.childCount)
            {
                bullet = transform.GetChild(index); // ������ �ִ� child ��Ȱ��
            }
            else
            {
                bullet = GameManager.instance.pool.Get(prefabId).transform; // ���ڶ� ���� ��������
                bullet.parent = transform; // �� �ڽ����� �θ� �ٲ�
            }

            // ��ġ�ϸ鼭 ���� ��ġ, ȸ�� �ʱ�ȭ �ϱ�
            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            Vector3 rotVec = Vector3.forward * 360 * index / count;
            bullet.Rotate(rotVec);
            //��bullet�� ������Ʈ�� ���� �������� ���� �������� 1.5��ŭ �̵�
            // Translate �Լ��� �ڽ��� �������� �̵�
            bullet.Translate(bullet.up * 1.5f, Space.World);
            bullet.GetComponent<Bullet>().Init(damage, -100, Vector3.zero); // -1 is Infinity Per. --> ����� -100 ����

        }
    }

    void Fire()
    {
        if (!player.scanner.nearestTarget) // ��ó�� ����� ���ٸ�
            return;

        // .. �Ѿ��� ���ư����� �ϴ� ���� ���
        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = targetPos - transform.position; // ũ�Ⱑ ���Ե� ����
        // normalized : ���� ������ ������ �����ϰ� ũ�⸦ 1�� ��ȯ�� �Ӽ�(ũ��� �׻� �����ϰ� �ϱ� ����)
        dir = dir.normalized;

        Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
        // .. ��ġ�� ȸ�� ����
        bullet.position = transform.position;
        // FromToRotation : ������ ���� �߽����� ��ǥ�� ���� ȸ���ϴ� �Լ�
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        // .. bullet�� ����
        bullet.GetComponent<Bullet>().Init(damage, count, dir);

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
    }

}
