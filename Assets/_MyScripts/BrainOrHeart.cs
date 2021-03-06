﻿using UnityEngine;

namespace _MyScripts {
    public class BrainOrHeart : MonoBehaviour {
        public static BrainOrHeart Brain;
        public static BrainOrHeart Heart;

        [SerializeField] private bool           _isBrain;
        [SerializeField] private float          _speed     = 10f;
        [SerializeField] private float          _sizeDelta = 0.1f;
        [SerializeField] private GameController _gameController;
        [SerializeField] private AudioClip      _hurtClip;
        [SerializeField] private AudioClip      _deathClip;


        private Rigidbody2D _rb2D;
        private float       _angle;
        private AudioSource _audioSource;
        private float       _size = 1f;
        private Vector3     _startSize;


        private void Awake() {
            if (_isBrain) Brain = this;
            else Heart          = this;
            _rb2D        = GetComponent<Rigidbody2D>();
            _audioSource = GetComponent<AudioSource>();
            if (_gameController == null) Debug.LogError("GameController is not set", gameObject);
            if (_speed          == 0) Debug.LogError("_speed is ZERO",               gameObject);
            _startSize = transform.localScale;
        }

        private void OnEnable() { _rb2D.AddForce(Vector2.one * Random.value * _speed, ForceMode2D.Impulse); }

        private void Update() { transform.Rotate(Vector3.forward, _angle); }

        private void OnCollisionEnter2D(Collision2D other) {
            if (!other.collider.CompareTag("Player")) return;
            PlayAudio();
            _gameController.UpdateScore();
            var velocity = Vector2.zero;
            switch (other.transform.parent.GetComponent<BodyParts>().Type) {
                case BodyPartsType.Horizontal:
                    var xPosRelative = (transform.position.x - other.transform.position.x) / other.collider.bounds.size.x;
                    velocity.x = xPosRelative;
                    velocity.y = -other.contacts[0].point.normalized.y;
                    break;
                case BodyPartsType.Vertical:
                    var yPosRelative = (transform.position.y - other.transform.position.y) / other.collider.bounds.size.y;
                    velocity.y = yPosRelative;
                    velocity.x = -other.contacts[0].point.normalized.x;
                    break;
                default:
                    Debug.LogError("wazzat?", gameObject);
                    break;
            }

//            print(velocity);
//            Debug.DrawRay(transform.position, velocity * _speed, Color.cyan,  10f);
//            Debug.DrawRay(transform.position, velocity,          Color.green, 10f);
            _rb2D.AddForce(velocity * _speed, ForceMode2D.Impulse);
            _angle = velocity.magnitude;
        }

        private void PlayAudio(bool isDead = false) {
            _audioSource.clip = isDead ? _deathClip : _hurtClip;
            _audioSource.Play();
        }

        public void Shit() {
            if (_size <= _sizeDelta*5) return;
            _size -= _sizeDelta;
            if (_isBrain) { Heart._size += _sizeDelta; } else { Brain._size += _sizeDelta; }

            Heart.transform.localScale = _startSize * Heart._size;
            Brain.transform.localScale = _startSize * Brain._size;
            Debug.Log("Heart size: " + Heart._size + " Brain size: " + Brain._size + " Total size:" + (Heart._size + Brain._size));
        }
    }
}