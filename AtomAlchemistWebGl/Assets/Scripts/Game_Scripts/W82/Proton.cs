using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Witmina_AtomAlchemist
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Proton : MonoBehaviour
    {
        [SerializeField] private GameObject _body;
        [SerializeField] private ParticleSystem _popParticle;
        [SerializeField] private Rigidbody2D rb;
        public bool Popped { get; private set; }
        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.collider.CompareTag("Finish"))
            {
                Destroy(gameObject);
            }
        }

        public void Shoot(Vector3 moveVector)
        {
            rb.velocity = moveVector;
        }

        public void Pop()
        {
            Popped = true;
            var rb = GetComponent<Rigidbody2D>();
            GetComponent<Collider2D>().enabled = false;

            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
            _body.SetActive(false);

            if (!_popParticle)
            {
                Destroy(gameObject);
                return;
            }

            _popParticle.Play();
            Destroy(gameObject, _popParticle.main.duration);
        }
    }
}