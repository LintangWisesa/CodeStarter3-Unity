using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TapController : MonoBehaviour {

	public delegate void PlayerDelegate ();
	public static event PlayerDelegate OnPlayerDied;//oyuncu öldüğünde gamemanager'daki gameover sayfasının aktif olması için
	public static event PlayerDelegate OnPlayerScored;//skor yapıldığında gamemanager'daki scoreText'in değişmesi için


	public float tapForce=10;
	public float tiltSmooth=5;
	public Vector3 startPos;

	public AudioSource tapAudio;
	public AudioSource scoreAudio;
	public AudioSource dieAudio;



	Rigidbody2D rigidbody;
	Quaternion downrotation;
	Quaternion forwardrotation;

	GameManager game;


	void Start(){
		rigidbody = GetComponent<Rigidbody2D> ();
		downrotation = Quaternion.Euler (0, 0, -90);
		forwardrotation = Quaternion.Euler (0, 0, 35);
		game = GameManager.Instance;
		rigidbody.simulated = false;
	}

	void OnEnable(){
		GameManager.OnGameStarted += OnGameStarted;
		GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
	}

	void OnDisable(){
		GameManager.OnGameStarted -= OnGameStarted;
		GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
	
	}

	void OnGameStarted(){
		//Geri sayım bittiğinde çağırılır velocity(çok da gerekli değil) ve simulated(Öldüğünde false olduğu için) yeniden ayarlanır.
		rigidbody.velocity = Vector3.zero;
		rigidbody.simulated = true;

	}
	void OnGameOverConfirmed(){
		//replay butonuna tıklandığında pozisyon ve rotasyon ilk haline getirilir.
		transform.localPosition = startPos;
		transform.rotation = Quaternion.identity;
	}
	void Update(){
		if (game.GameOver) {
			rigidbody.simulated = false;
			return;
		}
			
		if(Input.GetMouseButtonDown(0))
			{
			tapAudio.Play ();
			transform.rotation = forwardrotation;
			rigidbody.velocity = Vector3.zero;
			rigidbody.AddForce (Vector2.up * tapForce, ForceMode2D.Force);

			}

		transform.rotation = Quaternion.Lerp (transform.rotation, downrotation, tiltSmooth * Time.deltaTime);
	
	
	}

	void OnTriggerEnter2D(Collider2D col){

		if (col.gameObject.tag == "ScoreZone") {
		
			OnPlayerScored ();//event GameManagera gönderilir.
			scoreAudio.Play();
		}

		if (col.gameObject.tag == "DeadZone") {
		
			rigidbody.simulated = false;
			OnPlayerDied ();//event Gamemanagera gönderilir.
			dieAudio.Play();
		}
	
	
	
	}

}
