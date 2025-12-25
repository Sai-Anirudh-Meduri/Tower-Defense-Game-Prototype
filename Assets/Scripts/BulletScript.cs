using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float lifeTime = 3f;

    private void Start()
    {
        Destroy(gameObject, lifeTime); // auto cleanup
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Alien"))
        {
            Destroy(collision.gameObject);
        }

        Destroy(gameObject);
    }
}
